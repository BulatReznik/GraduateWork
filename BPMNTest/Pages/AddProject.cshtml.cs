using BPMN.Models.Project;
using BPMN.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BPMN.Pages
{
    public class AddProjectModel(ApiService apiService) : PageModel
    {
        private readonly ApiService _apiService = apiService;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(ProjectCreateRequest projectRequest)
        {
            var response = await _apiService.PostStringAsync("v1/projects/create", projectRequest);

            if (response.IsSuccess)
            {
                return RedirectToPage("/Projects");
            }

            // В случае ошибки отображаем сообщение об ошибке
            TempData["ErrorMessage"] = response.ErrorMessage;
            return Page();
        }
    }
}
