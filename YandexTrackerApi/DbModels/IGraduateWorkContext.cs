using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace YandexTrackerApi.DbModels
{
    public interface IGraduateWorkContext
    {
        /// <summary>
        /// Сохранить изменения в контексте
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Начало транзакции
        /// </summary>
        /// <returns></returns>
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Проверка существования сущности по предикате
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<bool> AnyAsync<TModel>(
            Expression<Func<TModel, bool>> predicate)
            where TModel : class;

        /// <summary>
        /// Получение атомарной сущности
        /// nullable
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="clause"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        Task<TResponse?> FirstOrDefaultAsync<TModel, TResponse>(
            Expression<Func<TModel, bool>> clause,
            Expression<Func<TModel, TResponse>> selector)
            where TModel : class;

        DbSet<Project> Projects { get; set; }

        DbSet<User> Users { get; set; }

        DbSet<UsersProject> UsersProjects { get; set; }

        DbSet<CalendarDatum> CalendarData { get; set; }

        DbSet<YandexTracker> YandexTrackers { get; set; }

        DbSet<YandexTrackerUser> YandexTrackerUsers { get; set; }

        DbSet<YandexTrackerUserHoliday> YandexTrackerUserHolidays { get; set; }

        DbSet<YandexTrackerTask> YandexTrackerTasks { get; set; }

        DbSet<Diagram> Diagrams { get; set; }
    }
}
