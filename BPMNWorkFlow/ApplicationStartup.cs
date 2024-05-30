using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNWorkFlow
{
    public class ApplicationStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Регистрируем сервисы
            ServiceRegistration.RegisterServices(services);

            // Создаем ServiceCollection и регистрируем дополнительные сервисы при необходимости
            var serviceCollection = new ServiceCollection();
            // Здесь может быть регистрация других сервисов

            // Создаем ServiceProvider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Регистрируем созданный ServiceProvider в качестве сервиса
            services.AddSingleton<IServiceProvider>(serviceProvider);
        }
    }
}
