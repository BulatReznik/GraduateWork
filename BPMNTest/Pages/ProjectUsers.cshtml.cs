using BPMN.Models.Project;
using BPMN.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BPMN.Pages
{
    public class ProjectUsersModel : PageModel
    {
        private readonly ApiService _apiService;
        public List<ProjectUserResponse> ProjectUsers { get; set; }

        [BindProperty]
        public string ProjectId { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string UserId { get; set; }

        public ProjectUsersModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGet(string projectId)
        {

            var response = await _apiService.GetAsync<List<ProjectUserResponse>>($"v1/projects/users/{projectId}");

            if (response.IsSuccess)
            {
                ProjectUsers = response.Data;
                ProjectId = projectId;
                return Page();
            }

            // � ������ ������ ���������� ��������� �� ������
            TempData["ErrorMessage"] = response.ErrorMessage;

            return Page();

        }

        public async Task<IActionResult> OnPost()
        {
            var request = new ProjectInviteUserRequest()
            {
                ProjectId = ProjectId,
                Email = Email,
            };

            // ���������� ������ �� ����������� ������������ �� ������
            var response = await _apiService.PostStringAsync("v1/projects/invite", request);

            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "������������ ������� ��������� �� ������.";
            }
            else
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
            }

            // �������������� ������������ �� ������� �������� � ������������ �������
            return RedirectToPage(new { projectId = request.ProjectId });
        }

        public async Task<IActionResult> OnPostDeleteUser()
        {
            var request = new ProjectUserDeleteRequest()
            {
                ProjectId = ProjectId,
                UserDelId = UserId,
            };

            // ���������� ������ �� �������� ������������ �� �������
            var response = await _apiService.PostStringAsync("v1/projects/delete", request);

            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "������������ ������� ������ �� �������.";
            }
            else
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
            }

            // �������������� ������������ �� ������� �������� � ������������ �������
            return RedirectToPage(new { projectId = request.ProjectId });
        }
    }
}
