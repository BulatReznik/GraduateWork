using BPMN.Models.Login;
using BPMN.Models.Project;
using BPMN.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BPMN.Pages
{
    public class ProjectsModel : PageModel
    {
        private readonly ApiService _apiService;
        public List<ProjectsResponseModel> Projects { get; set; }

        public ProjectsModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGet()
        {
            var response = await _apiService.GetAsync<List<ProjectsResponseModel>>("v1/projects");

            if (response.IsSuccess)
            {
                Projects = response.Data;
                return Page();
            }
            else
            {
                // В случае ошибки отображаем сообщение об ошибке
                TempData["ErrorMessage"] = response.ErrorMessage;
            }

            return Page();
        }

        // Обработка перехода на страницу проекта
        public IActionResult OnGetProject(string projectId)
        {
            // Перенаправление на страницу проекта с переданным идентификатором
            return RedirectToPage("/Project", new { projectId });
        }
    }
}
