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

            // В случае ошибки отображаем сообщение об ошибке
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

            // Отправляем запрос на приглашение пользователя на проект
            var response = await _apiService.PostStringAsync("v1/projects/invite", request);

            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Пользователь успешно приглашен на проект.";
            }
            else
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
            }

            // Перенаправляем пользователя на текущую страницу с обновленными данными
            return RedirectToPage(new { projectId = request.ProjectId });
        }

        public async Task<IActionResult> OnPostDeleteUser()
        {
            var request = new ProjectUserDeleteRequest()
            {
                ProjectId = ProjectId,
                UserDelId = UserId,
            };

            // Отправляем запрос на удаление пользователя из проекта
            var response = await _apiService.PostStringAsync("v1/projects/delete", request);

            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Пользователь успешно удален из проекта.";
            }
            else
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
            }

            // Перенаправляем пользователя на текущую страницу с обновленными данными
            return RedirectToPage(new { projectId = request.ProjectId });
        }
    }
}
