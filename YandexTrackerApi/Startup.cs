using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using YandexTrackerApi.AppLogic.Core;

namespace YandexTrackerApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add our Config object so it can be injected
            services.Configure<AppConfig>(Configuration.GetSection("AppConfig"));

            var appConfig = Configuration.GetSection("AppConfig").Get<AppConfig>()
                            ?? throw new ApplicationException("AppConfig loading error");

            services.AddLogging(builder =>
            {
                builder.AddConsole();
            });

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Startup>());

            services.AddHttpContextAccessor();

            services.AddAllSingletones();
            services.AddAllScoped(appConfig);
            services.AddAllTransients();

            services.AddDistributedMemoryCache();

            services.AddSession();

            services
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearerConfiguration(
                    appConfig.JWTIssuer,
                    appConfig.JWTAudience,
                    appConfig.JWTAccessKey);

            services.AddAuthorization();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MOIORestAPI",
                    Version = "v1"
                });

                // Создание и добавление описания схемы безопасности "Bearer"
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "",                    // Описание схемы безопасности (пустая строка)
                    Name = "Authorization",              // Имя параметра заголовка, который будет использоваться для передачи токена
                    In = ParameterLocation.Header,       // Местоположение параметра - в заголовке запроса
                    Type = SecuritySchemeType.ApiKey,    // Тип схемы безопасности - API-ключ (API Key)
                    Scheme = "Bearer"                    // Название схемы безопасности - "Bearer" (часто используется для токенов)
                });

                // Создание и добавление требования безопасности
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",          // Название схемы безопасности - "oauth2"
                            Name = "Bearer",           // Имя параметра заголовка, который будет использоваться для передачи токена
                            In = ParameterLocation.Header,  // Местоположение параметра - в заголовке запроса
                        },
                        new List<string>()   // Перечисление разрешений (в данном случае пустой список строк)
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the
        //HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            //if (env.IsDevelopment())
            //{
            IdentityModelEventSource.ShowPII = true;
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "GraduateWork"));
            //}
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseWebSockets();

            app.UseSession();
            app.UseCors(builder => builder
               .AllowAnyHeader()
               .AllowAnyMethod()
               .SetIsOriginAllowed((host) => true)
               .AllowCredentials()
             );

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
