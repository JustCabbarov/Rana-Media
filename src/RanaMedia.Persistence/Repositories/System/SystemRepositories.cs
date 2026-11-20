using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RanaMedia.Domain.Entities;
using RanaMedia.Domain.Repositories.System;
using RanaMedia.Persistence.Data;

namespace RanaMedia.Persistence.Repositories.System;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity, new()
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public IQueryable<T> Query() => _dbSet.AsQueryable();

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual async Task<T> DeleteAsync(Guid id)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException($"{typeof(T).Name} tapılmadı (ID: {id}).");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        return entity;
    }

    public virtual async Task<List<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();
        if (include != null)
            query = include(query);

        return await query.ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
        IQueryable<T> query = _dbSet;
        if (include != null)
            query = include(query);

        return await query.FirstOrDefaultAsync(x => x.Id == id);
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        var existingEntity = await _dbSet.FindAsync(entity.Id)
            ?? throw new KeyNotFoundException($"{typeof(T).Name} tapılmadı (ID: {entity.Id}).");

        _context.Entry(existingEntity).CurrentValues.SetValues(entity);
        existingEntity.UpdatedAt = DateTime.UtcNow;
        return existingEntity;
    }
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}

public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    private readonly ILogger<RefreshTokenRepository> _logger;

    public RefreshTokenRepository(AppDbContext context, ILogger<RefreshTokenRepository> logger) : base(context)
    {
        _logger = logger;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _dbSet
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task RevokeTokenAsync(string token)
    {
        var storedToken = await _dbSet.FirstOrDefaultAsync(rt => rt.Token == token);
        if (storedToken is null || storedToken.IsRevoked)
        {
            _logger.LogWarning("Token tapılmadı və ya artıq ləğv edilib.");
            throw new UnauthorizedAccessException("Token tapılmadı və ya artıq ləğv edilib.");
        }

        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllUserTokensAsync(Guid userId)
    {
        var tokens = await _dbSet.Where(rt => rt.UserId == userId && !rt.IsRevoked).ToListAsync();
        foreach (var token in tokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();
    }
}

public class PasswordResetRepository : GenericRepository<PasswordResetOTP>, IPasswordResetRepository
{
    public PasswordResetRepository(AppDbContext context) : base(context) { }

    public async Task<PasswordResetOTP?> GetLatestActiveOtpAsync(string email)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return await _dbSet
            .Where(o => o.Email.ToLower() == normalized && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();
    }
}

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(AppDbContext context) : base(context) { }

    public async Task<List<Notification>> GetUserNotificationsAsync(Guid userId, int limit = 30)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _dbSet.CountAsync(n => n.UserId == userId && !n.IsRead);
    }
}

