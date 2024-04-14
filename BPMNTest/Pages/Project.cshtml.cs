using BPMN.Models.Diagram;
using BPMN.Models.Project;
using BPMN.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BPMN.Pages
{
    public class ProjectModel : PageModel
    {
        private readonly ApiService _apiService;
        public ProjectViewModel Project { get; set; }

        public ProjectModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGet(string projectId)
        {
            // Получаем информацию о проекте
            var projectResponse = await _apiService.GetAsync<ProjectResponse>($"v1/projects/{projectId}");

            if (!projectResponse.IsSuccess)
            {
                TempData["ErrorMessage"] = projectResponse.ErrorMessage;
                return Page();
            }

            var diagramsRequest = new DiagramsRequest()
            {
                ProjectId = projectId,
            };

            // Получаем информацию о диаграммах на проекте
            var diagramsResponse = await _apiService.PostAsync<DiagramsRequest, List<DiagramsResponse>>("v1/diagrams", diagramsRequest);

            if (!diagramsResponse.IsSuccess)
            {
                TempData["ErrorMessage"] = diagramsResponse.ErrorMessage;
                return Page();
            }

            // Заполнение ProjectViewModel данными из ответов на запросы
            Project = new ProjectViewModel
            {
                Id = projectResponse.Data.Id,
                Name = projectResponse.Data.Name,
                Description = projectResponse.Data.Description,
                CreatorId = projectResponse.Data.CreatorId,
                Diagrams = diagramsResponse.Data.Select(d => new DiagramViewModel
                {
                    Id = d.Id,
                    Name = d.Name
                }).ToList()
            };

            return Page();
        }
    }
}
