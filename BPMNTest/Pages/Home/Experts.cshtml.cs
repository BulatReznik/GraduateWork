using BPMNTest.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BPMNTest.Pages.Home
{
    public class ExpertsModel : PageModel
    {
        public List<Expert> Experts { get; set; }

        public void OnGet()
        {
            // Инициализация фиктивных данных
            Experts = new List<Expert>
            {
                new() { Id = Guid.NewGuid(), Name = "Иван Иванов", Email = "ivanov@example.com", Phone = "123-456-7890", Department = "Отдел разработки" },
                new() { Id = Guid.NewGuid(), Name = "Мария Петрова", Email = "petrova@example.com", Phone = "987-654-3210", Department = "Отдел маркетинга" }
                // Добавьте больше экспертов по аналогии
            };
        }
    }
}
