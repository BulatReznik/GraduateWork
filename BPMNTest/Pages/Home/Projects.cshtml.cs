using BPMNTest.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BPMNTest.Pages.Home
{
    public class ProjectsModel : PageModel
    {
        public List<Project> Projects { get; set; }

        public void OnGet()
        {
            Projects = new List<Project>
            {
                new() { Id = Guid.NewGuid(), Name = "Разработка CRM-системы", Description = "Создание индивидуальной CRM-системы для управления клиентской базой", StartDate = new DateTime(2023, 1, 15), EndDate = new DateTime(2023, 12, 31), Status = "Активен" },
                new() { Id = Guid.NewGuid(), Name = "Автоматизация логистики", Description = "Разработка системы для оптимизации логистических процессов в крупной торговой сети", StartDate = new DateTime(2023, 2, 1), EndDate = new DateTime(2023, 6, 30), Status = "Завершен" },
                new() { Id = Guid.NewGuid(), Name = "Обновление корпоративного портала", Description = "Проект по обновлению дизайна и функционала корпоративного портала компании", StartDate = new DateTime(2023, 3, 20), EndDate = new DateTime(2023, 11, 20), Status = "В разработке" },
                new() { Id = Guid.NewGuid(), Name = "Интеграция системы бухгалтерского учета", Description = "Проект по интеграции новой системы бухгалтерского учета с существующими IT-системами компании", StartDate = new DateTime(2023, 4, 10), EndDate = new DateTime(2023, 9, 30), Status = "В ожидании" },
                new() { Id = Guid.NewGuid(), Name = "Разработка мобильного приложения", Description = "Создание мобильного приложения для предоставления услуг онлайн-консультаций", StartDate = new DateTime(2023, 5, 5), EndDate = new DateTime(2023, 10, 5), Status = "Инициирован" }
            };
        }

    }
}
