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
        public string _projectId { get; set; }

        public ProjectModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGet(string projectId)
        {
            // �������� ���������� � �������
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

            // �������� ���������� � ���������� �� �������
            var diagramsResponse = await _apiService.PostAsync<DiagramsRequest, List<DiagramsResponse>>("v1/diagrams", diagramsRequest);

            if (!diagramsResponse.IsSuccess)
            {
                TempData["ErrorMessage"] = diagramsResponse.ErrorMessage;
                return Page();
            }

            // ���������� ProjectViewModel ������� �� ������� �� �������
            Project = new ProjectViewModel
            {
                Id = projectResponse.Data.Id,
                Name = projectResponse.Data.Name,
                Description = projectResponse.Data.Description,
                CreatorId = projectResponse.Data.CreatorId,
                CreatorName = projectResponse.Data.CreatorName,
                Diagrams = diagramsResponse.Data.Select(d => new DiagramViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    Date = d.Date
                }).ToList()
            };

            return Page();
        }

        public async Task<IActionResult> OnPostExecuteDiagram(string projectId, Guid diagramId)
        {
            try
            {
                var executeDiagramRequest = new DiagramExecuteModel { Id = diagramId };

                // ���������� ������ �� ���������� ���������
                var executeDiagramResponse = await _apiService.PostStringAsync("v1/diagrams/execute", executeDiagramRequest);

                if (!executeDiagramResponse.IsSuccess)
                {
                    // ��������� ������, ���� ������ �� ���������� ��������� �� ������
                    TempData["ErrorMessage"] = executeDiagramResponse.ErrorMessage;
                    return RedirectToPage("/Error"); // �������������� �� �������� � �������
                }

                // �������� ���������� ���������
                TempData["SuccessMessage"] = $"��������� ������� ���������, ����������� ����: \n{executeDiagramResponse.Data}";
                return RedirectToPage("/Project", new { projectId });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "�� ����� ���������� ��������� ��������� ������";
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostDeleteDiagramAsync(string projectId, Guid diagramId)
        {
            try
            {
                var deleteDiagramRequest = new DiagramDeleteModel { DiagramId = diagramId };

                // ���������� ������ �� ���������� ���������
                var deleteDiagramResponse = await _apiService.PostStringAsync("v1/diagrams/delete", deleteDiagramRequest);

                if (!deleteDiagramResponse.IsSuccess)
                {
                    // ��������� ������, ���� ������ �� ���������� ��������� �� ������
                    TempData["ErrorMessage"] = deleteDiagramResponse.ErrorMessage;
                    return RedirectToPage("/Error"); // �������������� �� �������� � �������
                }

                // �������� ���������� ���������
                TempData["SuccessMessage"] = $"{deleteDiagramResponse.Data}";
                return RedirectToPage("/Project", new { projectId });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "�� �������� ��������� ��������� ������";
                return RedirectToPage("/Error");
            }
        }

        public IActionResult OnGetUsers(string projectId)
        {
            // ��������������� �� �������� ������� � ���������� ���������������
            return RedirectToPage("/ProjectUsers", new { projectId });
        }
    }
}
