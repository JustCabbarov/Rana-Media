using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RanaMedia.Application.Options;
using RanaMedia.Contract.DTOs.Auth;
using RanaMedia.Contract.Services.System;
using RanaMedia.Domain.Entities;
using RanaMedia.Domain.Repositories.System;

namespace RanaMedia.Application.Services.System;

public class TokenHandler : ITokenHandler
{
    private readonly JwtOption _jwtOptions;
    private readonly UserManager<AppUser> _userManager;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ILogger<TokenHandler> _logger;

    public TokenHandler(
        IOptions<JwtOption> jwtOptions,
        UserManager<AppUser> userManager,
        IRefreshTokenRepository refreshTokenRepository,
        ILogger<TokenHandler> logger)
    {
        _jwtOptions = jwtOptions.Value;
        _userManager = userManager;
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
    }

    public async Task<string> CreateAccessTokenAsync(AppUser user)
    {
        _logger.LogInformation("Access token yaradılır. UserId: {UserId}", user.Id);

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.FullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Task<string> CreateRefreshTokenAsync(AppUser user)
    {
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        return Task.FromResult(refreshToken);
    }

    public async Task<LoginResponseDTO> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        if (storedToken is null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow || storedToken.User is null)
        {
            _logger.LogWarning("Etibarsız refresh token.");
            throw new UnauthorizedAccessException("Refresh token yanlış və ya müddəti bitib.");
        }

        await _refreshTokenRepository.RevokeTokenAsync(refreshToken);

        var newAccessToken = await CreateAccessTokenAsync(storedToken.User);
        var newRefreshToken = await CreateRefreshTokenAsync(storedToken.User);

        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            Token = newRefreshToken,
            UserId = storedToken.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays),
            IsRevoked = false
        });

        return new LoginResponseDTO
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpireAt = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes)
        };
    }

    public Task<bool> ValidateTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return Task.FromResult(false);
        var handler = new JwtSecurityTokenHandler();
        try
        {
            handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey))
            }, out _);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public async Task RevokeTokenAsync(string refreshToken)
    {
        await _refreshTokenRepository.RevokeTokenAsync(refreshToken);
    }
}

