using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace YandexTrackerApi.DbModels
{
    public partial class GraduateWorkContext : IGraduateWorkContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return await base.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task<bool> AnyAsync<TModel>(
            Expression<Func<TModel, bool>> predicate)
            where TModel : class
        {
            var db = Set<TModel>();

            return await db.AnyAsync(predicate);
        }

        public Task<List<TResponse>> AllAsync<TModel, TResponse>(
            Expression<Func<TModel, bool>> clause,
            Expression<Func<TModel, TResponse>> selector)
            where TModel : class
        {
            var query = Set<TModel>()
                .Where(clause)
                .Select(selector)
                .AsQueryable();

            return query.ToListAsync();
        }

        public Task<TResponse?> FirstOrDefaultAsync<TModel, TResponse>(
            Expression<Func<TModel, bool>> clause,
            Expression<Func<TModel, TResponse>> selector)
            where TModel : class
        {
            var query = Set<TModel>()
                .Where(clause)
                .Select(selector)
                .AsQueryable();

            return query.FirstOrDefaultAsync();
        }
    }
}
