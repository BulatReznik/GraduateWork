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
                // � ������ ������ ���������� ��������� �� ������
                TempData["ErrorMessage"] = response.ErrorMessage;
            }

            return Page();
        }

        // ��������� �������� �� �������� �������
        public IActionResult OnGetProject(string projectId)
        {
            // ��������������� �� �������� ������� � ���������� ���������������
            return RedirectToPage("/Project", new { projectId });
        }
    }
}
