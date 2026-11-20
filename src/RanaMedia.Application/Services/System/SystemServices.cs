using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RanaMedia.Application.Exceptions;
using RanaMedia.Application.Options;
using RanaMedia.Contract.DTOs.Auth;
using RanaMedia.Contract.DTOs.Forms;
using RanaMedia.Contract.Services.System;
using RanaMedia.Domain.Entities;
using RanaMedia.Domain.Repositories.System;

namespace RanaMedia.Application.Services.System;

public class EmailSender : IEmailSender
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IOptions<EmailSettings> settings, ILogger<EmailSender> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        if (string.IsNullOrWhiteSpace(_settings.Host) ||
            string.IsNullOrWhiteSpace(_settings.Username) ||
            _settings.Username.Contains("YOUR_SMTP", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("[SMTP Dev Fallback] To: {To} | Subject: {Subject}", toEmail, subject);
            return;
        }

        using var smtp = new SmtpClient(_settings.Host, _settings.Port)
        {
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            EnableSsl = _settings.EnableSsl
        };

        using var message = new MailMessage(
            new MailAddress(_settings.FromEmail, _settings.FromName),
            new MailAddress(toEmail))
        {
            Subject = subject,
            IsBodyHtml = true,
            Body = htmlBody
        };

        await smtp.SendMailAsync(message);
    }

    public async Task SendOtpEmailAsync(string toEmail, string otp)
    {
        var html = $"""
            <h3>Şifrə Sıfırlama</h3>
            <p>Sizin təsdiq kodunuz:</p>
            <h2 style="letter-spacing:3px">{otp}</h2>
            <p>Kod 5 dəqiqə ərzində etibarlıdır.</p>
            """;
        await SendEmailAsync(toEmail, "Rana Media — Password Reset Code", html);
    }
}

public class PasswordResetService : IPasswordResetService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IPasswordResetRepository _otpRepository;
    private readonly IEmailSender _emailSender;
    private readonly IUnitOfWork _unitOfWork;

    public PasswordResetService(
        UserManager<AppUser> userManager,
        IPasswordResetRepository otpRepository,
        IEmailSender emailSender,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _otpRepository = otpRepository;
        _emailSender = emailSender;
        _unitOfWork = unitOfWork;
    }

    public async Task SendOtpAsync(OTPrequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _userManager.FindByEmailAsync(email)
            ?? throw new NotFoundException("Bu e-poçt ünvanına uyğun istifadəçi tapılmadı.");

        var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        await _otpRepository.AddAsync(new PasswordResetOTP
        {
            UserId = user.Id,
            Email = email,
            OtpCode = otp,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false
        });
        await _unitOfWork.SaveChangesAsync();

        await _emailSender.SendOtpEmailAsync(email, otp);
    }

    public async Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var normalized = email.Trim().ToLowerInvariant();
        var user = await _userManager.FindByEmailAsync(normalized)
            ?? throw new NotFoundException("İstifadəçi tapılmadı.");

        var otpRecord = await _otpRepository.GetLatestActiveOtpAsync(normalized);
        if (otpRecord == null || otpRecord.OtpCode != token.Trim())
        {
            throw new UnauthorizedException("OTP kodu yanlışdır və ya müddəti bitib.");
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
        if (result.Succeeded)
        {
            otpRecord.IsUsed = true;
            await _unitOfWork.SaveChangesAsync();
        }
        return result;
    }
}

public class GenericService<TVM, TEntity> : IGenericService<TVM, TEntity>
    where TVM : class
    where TEntity : BaseEntity, new()
{
    private readonly IGenericRepository<TEntity> _repository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GenericService<TVM, TEntity>> _logger;

    public GenericService(
        IMapper mapper,
        IGenericRepository<TEntity> repository,
        IUnitOfWork unitOfWork,
        ILogger<GenericService<TVM, TEntity>> logger)
    {
        _mapper = mapper;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TVM> AddAsync(TVM entity)
    {
        if (entity == null) throw new NotNullExceptions();
        var entityToAdd = _mapper.Map<TEntity>(entity);
        var result = await _repository.AddAsync(entityToAdd);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Added {EntityType} with ID: {EntityId}", typeof(TEntity).Name, result.Id);
        return _mapper.Map<TVM>(result);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"{typeof(TEntity).Name} tapılmadı.");
        await _repository.DeleteAsync(existing.Id);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<TVM>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
    {
        var data = await _repository.GetAllAsync(include);
        return _mapper.Map<IEnumerable<TVM>>(data);
    }

    public async Task<TVM> GetByIdAsync(Guid id, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
    {
        var entity = await _repository.GetByIdAsync(id, include)
            ?? throw new NotFoundException($"{typeof(TEntity).Name} tapılmadı.");
        return _mapper.Map<TVM>(entity);
    }

    public async Task<TVM> UpdateAsync(TVM entity)
    {
        if (entity == null) throw new NotNullExceptions();
        var mapped = _mapper.Map<TEntity>(entity);
        await _repository.UpdateAsync(mapped);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<TVM>(mapped);
    }
}

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NotificationService(
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<NotificationDTO>> GetMyNotificationsAsync(Guid userId, int limit = 30)
    {
        var items = await _notificationRepository.GetUserNotificationsAsync(userId, limit);
        return _mapper.Map<List<NotificationDTO>>(items);
    }

    public async Task MarkAsReadAsync(Guid userId, Guid notificationId)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId)
            ?? throw new NotFoundException("Bildiriş tapılmadı.");

        if (notification.UserId != userId)
            throw new ForbiddenException("Bu bildirişə giriş icazəniz yoxdur.");

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var items = await _notificationRepository.GetUserNotificationsAsync(userId, 200);
        foreach (var item in items.Where(x => !x.IsRead))
        {
            var tracked = await _notificationRepository.GetByIdAsync(item.Id);
            if (tracked != null)
            {
                tracked.IsRead = true;
                tracked.ReadAt = DateTime.UtcNow;
            }
        }
        await _unitOfWork.SaveChangesAsync();
    }
}

