using Microsoft.EntityFrameworkCore;
using MesClient.Core.Models;
using MesClient.Infrastructure.Data;

namespace MesClient.Infrastructure.Repositories;

/// <summary>
/// 기본 리포지토리
/// </summary>
public abstract class BaseRepository<T> where T : BaseEntity
{
    protected readonly MesDbContext _context;
    protected readonly DbSet<T> _dbSet;

    protected BaseRepository(MesDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(long id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.Where(x => x.IsActive).ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        entity.CreatedAt = DateTime.Now;
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        entity.ModifiedAt = DateTime.Now;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return false;
        
        entity.IsActive = false;
        entity.ModifiedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }
}
