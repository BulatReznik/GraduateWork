using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using YandexTrackerApi.BusinessLogic.Managers.JWT;
using YandexTrackerApi.BusinessLogic.Managers.User;
using YandexTrackerApi.DbModels;

namespace YandexTrackerApi.AppLogic.Core
{
    public static class ServiceProviderExtension
    {
        public static void AddAllSingletones(this IServiceCollection services)
        {
            services.AddSingleton(typeof(ILogger), typeof(Logger<Startup>));
        }

        public static void AddAllScoped(this IServiceCollection services, AppConfig appConfig)
        {
            services.AddDbContextPool<IGraduateWorkContext, GraduateWorkContext>(
                dbContextOptions =>
                {
                    dbContextOptions.UseSqlServer(appConfig.DBConnectionString);
                    dbContextOptions.ConfigureWarnings(warnings =>
                    {

                    });
                }
            );

            // Менеджеры
            services.AddScoped<IUserManager, UserManager>();
            //services.AddScoped<IUserGroupManager, UserGroupManager>();

            services.AddScoped<IJWTAuthManager, JWTAuthManager>();
            services.AddScoped<IJWTUserManager, JWTUserManager>();
            services.AddScoped<ISecurityTokenValidator, JwtSecurityTokenHandler>();
        }

        public static void AddAllTransients(this IServiceCollection services)
        {
        }
    }
}
