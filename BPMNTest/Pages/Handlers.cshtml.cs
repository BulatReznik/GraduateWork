using BPMN.Models;
using BPMN.Models.Handler;
using BPMN.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BPMN.Pages
{
    public class HandlersModel : PageModel
    {
        private readonly ApiService _apiService;

        [BindProperty]
        public HandlersResponse Handlers { get; set; } 

        public HandlersModel(ApiService apiService) 
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGet()
        {
            var handlersResponse = await _apiService.GetAsync<HandlersResponse>("/api/v1/handlers/");

            if (handlersResponse.IsSuccess)
            {
                Handlers = handlersResponse.Data;
            }
            TempData["ErrorMessage"] = handlersResponse.ErrorMessage;

            return Page();
        }
    }
}
