using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using QuestionAnswer.DAL.Interfaces;

namespace QuestionAnswer.DAL.Repositories;

public class GenericRepository<T>(QuestionAnswerDbContext dbContext) : IGenericRepository<T> where T : class
{
    protected readonly QuestionAnswerDbContext DbContext = dbContext;
    protected readonly DbSet<T> DbSet = dbContext.Set<T>();

    public async Task<T?> GetByIdAsync(int id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public IQueryable<T> Query()
    {
        return DbSet.AsQueryable();
    }

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await DbSet.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public void Remove(T entity)
    {
        DbSet.Remove(entity);
    }
}

