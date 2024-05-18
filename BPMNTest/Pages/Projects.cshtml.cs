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

        [BindProperty] public string Code { get; set; }

        public ProjectsModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGet()
        {
            await LoadProjects();
            return Page();
        }

        public IActionResult OnGetProject(string projectId)
        {
            return RedirectToPage("/Project", new { projectId });
        }

        public async Task<IActionResult> OnPostConfirmInvite()
        {
            var projectConfirmInviteRequest = new ProjectConfirmInviteRequest
            {
                Code = Code
            };

            var response = await _apiService.PostStringAsync("v1/projects/confirm/invite", projectConfirmInviteRequest);

            await LoadProjects();

            if (response.IsSuccess)
            {
                return Page();
            }

            TempData["ErrorMessage"] = response.ErrorMessage;
            return Page();
        }


        private async Task LoadProjects()
        {
            var projectsResponse = await _apiService.GetAsync<List<ProjectsResponseModel>>("v1/projects");

            if (projectsResponse.IsSuccess)
            {
                Projects = projectsResponse.Data;
            }

            TempData["ErrorMessage"] = projectsResponse.ErrorMessage;
        }
    }
}
