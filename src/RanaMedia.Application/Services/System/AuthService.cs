using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RanaMedia.Application.Exceptions;
using RanaMedia.Application.Options;
using RanaMedia.Contract.DTOs.Auth;
using RanaMedia.Contract.DTOs.Common;
using RanaMedia.Contract.Services.System;
using RanaMedia.Domain.Entities;
using RanaMedia.Domain.Repositories.Editorial;
using RanaMedia.Domain.Repositories.System;

namespace RanaMedia.Application.Services.System;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenHandler _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserSaveRepository _userSaveRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtOption _jwtOptions;
    private readonly IMapper _mapper;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        SignInManager<AppUser> signInManager,
        ITokenHandler tokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IUserSaveRepository userSaveRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork,
        IOptions<JwtOption> jwtOptions,
        IMapper mapper,
        IEmailSender emailSender,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _userSaveRepository = userSaveRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
        _jwtOptions = jwtOptions.Value;
        _mapper = mapper;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<LoginResponseDTO> LoginAsync(LoginDTO loginDto)
    {
        var email = loginDto.Email.Trim().ToLowerInvariant();
        _logger.LogInformation("Login cəhdi: {Email}", email);

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || user.IsDeleted || !user.IsActive)
        {
            throw new InvalidCredentialsException("Email və ya şifrə yanlışdır.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: true);
        if (result.IsLockedOut)
        {
            throw new ForbiddenException("Çoxsaylı yanlış cəhd səbəbindən hesab müvəqqəti bloklanıb.", "ACCOUNT_LOCKED");
        }
        if (!result.Succeeded)
        {
            throw new InvalidCredentialsException("Email və ya şifrə yanlışdır.");
        }

        var accessToken = await _tokenService.CreateAccessTokenAsync(user);
        var refreshToken = await _tokenService.CreateRefreshTokenAsync(user);

        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays),
            IsRevoked = false
        });
        await _unitOfWork.SaveChangesAsync();

        return new LoginResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpireAt = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes)
        };
    }

    public async Task RegisterAsync(RegisterDTO registerDto)
    {
        var email = registerDto.Email.Trim().ToLowerInvariant();
        _logger.LogInformation("Qeydiyyat cəhdi: {Email}", email);

        var existing = await _userManager.FindByEmailAsync(email);
        if (existing != null)
        {
            throw new ConflictException("Bu e-poçt ünvanı ilə artıq qeydiyyatdan keçilib.", "EMAIL_ALREADY_EXISTS");
        }

        var user = new AppUser
        {
            Email = email,
            UserName = email,
            PhoneNumber = registerDto.Phone,
            FirstName = registerDto.FirstName.Trim(),
            LastName = registerDto.LastName.Trim(),
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user, registerDto.Password);
        if (!createResult.Succeeded)
        {
            throw new BusinessRuleException(
                string.Join("; ", createResult.Errors.Select(e => e.Description)),
                "IDENTITY_REGISTRATION_FAILED");
        }

        if (await _roleManager.RoleExistsAsync("User"))
        {
            await _userManager.AddToRoleAsync(user, "User");
        }

        await _emailSender.SendEmailAsync(
            email,
            "Rana Media-ya xoş gəlmisiniz",
            $"<p>Salam {user.FirstName}, Rana Media hesabınız uğurla yaradıldı.</p>");

        _logger.LogInformation("Qeydiyyat uğurlu: {Email}, UserId: {UserId}", email, user.Id);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        await _tokenService.RevokeTokenAsync(refreshToken);
        await _signInManager.SignOutAsync();
    }

    public async Task AssignRoleAsync(Guid userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException($"İstifadəçi tapılmadı: {userId}");

        if (!await _roleManager.RoleExistsAsync(roleName))
            throw new NotFoundException($"Rol tapılmadı: {roleName}");

        var res = await _userManager.AddToRoleAsync(user, roleName);
        if (!res.Succeeded)
            throw new BusinessRuleException(string.Join("; ", res.Errors.Select(e => e.Description)));
    }

    public async Task<IdentityResult> CreateRoleAsync(CreateRoleDTO appRole)
    {
        return await _roleManager.CreateAsync(new AppRole
        {
            Name = appRole.Name,
            Description = appRole.Description,
            CreatedAt = DateTime.UtcNow
        });
    }

    public async Task<IList<AppUser>> GetUserByRoleIdAsync(Guid roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString())
            ?? throw new NotFoundException($"Rol tapılmadı: {roleId}");

        return await _userManager.GetUsersInRoleAsync(role.Name!);
    }

    public async Task<IdentityResult> RemoveRoleAsync(Guid userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException($"İstifadəçi tapılmadı: {userId}");

        return await _userManager.RemoveFromRoleAsync(user, roleName);
    }

    public async Task<UserDTO> GetUserByJwtTokenAsync(string jwtToken)
    {
        if (jwtToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            jwtToken = jwtToken.Substring(7);

        var tokenHandler = new JwtSecurityTokenHandler();
        if (!tokenHandler.CanReadToken(jwtToken))
            throw new UnauthorizedException("Token oxunmadı.");

        var jwtSecurityToken = tokenHandler.ReadJwtToken(jwtToken);
        var userId = jwtSecurityToken.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier
                              || c.Type == "nameid"
                              || c.Type == JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Token-də istifadəçi identifikatoru tapılmadı.");

        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("İstifadəçi tapılmadı.");

        var dto = _mapper.Map<UserDTO>(user);
        dto.RoleName = (await _userManager.GetRolesAsync(user)).ToList();
        dto.UnseenSavedArticleCount = await _userSaveRepository.GetUnseenSavedCountAsync(user.Id);
        dto.UnreadNotificationsCount = await _notificationRepository.GetUnreadCountAsync(user.Id);
        return dto;
    }

    public async Task<PaginatedResult<UserDTO>> GetAllUsersAsync(UserFilterDTO filter)
    {
        var query = _userManager.Users.AsNoTracking().AsQueryable();

        if (filter.UserId.HasValue)
            query = query.Where(u => u.Id == filter.UserId.Value);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.Trim().ToLowerInvariant();
            query = query.Where(u =>
                (u.Email != null && u.Email.ToLower().Contains(s)) ||
                u.FirstName.ToLower().Contains(s) ||
                u.LastName.ToLower().Contains(s));
        }

        if (!string.IsNullOrWhiteSpace(filter.Role))
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(filter.Role);
            var ids = usersInRole.Select(u => u.Id).ToList();
            query = query.Where(u => ids.Contains(u.Id));
        }

        var total = await query.CountAsync();
        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var items = new List<UserDTO>();
        foreach (var u in users)
        {
            var dto = _mapper.Map<UserDTO>(u);
            dto.RoleName = (await _userManager.GetRolesAsync(u)).ToList();
            items.Add(dto);
        }

        return new PaginatedResult<UserDTO>
        {
            Items = items,
            TotalCount = total,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<List<CreateRoleDTO>> GetAllRolesAsync()
    {
        var roles = await _roleManager.Roles.AsNoTracking().ToListAsync();
        return _mapper.Map<List<CreateRoleDTO>>(roles);
    }

    public async Task<UserDTO> EditUserAsync(Guid userId, EditUserDTO editUserDto)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException($"İstifadəçi tapılmadı: {userId}");

        if (!string.IsNullOrWhiteSpace(editUserDto.FirstName)) user.FirstName = editUserDto.FirstName.Trim();
        if (!string.IsNullOrWhiteSpace(editUserDto.LastName)) user.LastName = editUserDto.LastName.Trim();
        if (editUserDto.Phone != null) user.PhoneNumber = editUserDto.Phone;
        if (editUserDto.Bio != null) user.Bio = editUserDto.Bio;
        if (editUserDto.AvatarUrl != null) user.AvatarUrl = editUserDto.AvatarUrl;
        if (editUserDto.IsActive.HasValue) user.IsActive = editUserDto.IsActive.Value;

        var res = await _userManager.UpdateAsync(user);
        if (!res.Succeeded)
            throw new BusinessRuleException(string.Join("; ", res.Errors.Select(e => e.Description)));

        var dto = _mapper.Map<UserDTO>(user);
        dto.RoleName = (await _userManager.GetRolesAsync(user)).ToList();
        return dto;
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException($"İstifadəçi tapılmadı: {userId}");

        user.IsDeleted = true;
        user.IsActive = false;
        await _userManager.UpdateAsync(user);
    }

    public async Task<AcceptGuidelinesResponseDTO> AcceptCommunityGuidelinesAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException($"İstifadəçi tapılmadı: {userId}");

        var now = DateTime.UtcNow;
        user.CommunityGuidelinesAcceptedAt ??= now;
        await _userManager.UpdateAsync(user);

        return new AcceptGuidelinesResponseDTO
        {
            Accepted = true,
            AcceptedAt = user.CommunityGuidelinesAcceptedAt.Value
        };
    }
}

