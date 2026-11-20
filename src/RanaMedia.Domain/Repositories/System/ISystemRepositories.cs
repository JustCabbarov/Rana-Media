using RanaMedia.Domain.Entities;

namespace RanaMedia.Domain.Repositories.System;

public interface IGenericRepository<T> where T : BaseEntity
{
    IQueryable<T> Query();
    Task<T?> GetByIdAsync(Guid id, Func<IQueryable<T>, IQueryable<T>>? include = null);
    Task<List<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? include = null);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<T> DeleteAsync(Guid id);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task RevokeTokenAsync(string token);
    Task RevokeAllUserTokensAsync(Guid userId);
}

public interface IPasswordResetRepository : IGenericRepository<PasswordResetOTP>
{
    Task<PasswordResetOTP?> GetLatestActiveOtpAsync(string email);
}

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<List<Notification>> GetUserNotificationsAsync(Guid userId, int limit = 30);
    Task<int> GetUnreadCountAsync(Guid userId);
}

