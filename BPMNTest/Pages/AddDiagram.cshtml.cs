using BPMN.Models.Diagram;
using BPMN.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BPMN.Pages
{
    public class AddDiagramModel(IWebHostEnvironment hostEnvironment, ApiService apiService) : PageModel
    {
        private readonly ApiService _apiService = apiService;
        private readonly string _diagramName = "diagram.bpmn";

        public string _diagram;
        public string _projectId;

        public void OnGet(string projectId)
        {
            _projectId = projectId;
        }

        public async Task<IActionResult> OnPost(DiagramFormModel diagramFormModel)
        {
            var response = await _apiService.PostStringAsync("v1/diagram/", diagramFormModel);

            if (response.IsSuccess)
            {
                return RedirectToPage("/Project", new { diagramFormModel.ProjectId });
            }
            else
            {
                // В случае ошибки отображаем сообщение об ошибке
                TempData["ErrorMessage"] = response.ErrorMessage;
                return Page();
            }
        }

        public IActionResult OnGetDiagram()
        {
            var filePath = Path.Combine(hostEnvironment.WebRootPath, _diagramName);
            var fileInfo = new FileInfo(filePath);

            // Проверяем, существует ли файл
            if (!fileInfo.Exists)
            {
                return NotFound(); // Возвращаем ошибку 404, если файл не найден
            }

            // Возвращаем файл как результат запроса
            return PhysicalFile(filePath, "application/xml");
        }
    }
}
