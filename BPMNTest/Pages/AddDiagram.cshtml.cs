using BPMN.Models.Diagram;
using BPMN.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BPMN.Pages
{
    public class AddDiagramModel : PageModel
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ApiService _apiService;
        private readonly string _diagramName;

        public string _diagram;
        public string _projectId;

        public AddDiagramModel(IWebHostEnvironment hostEnvironment, ApiService apiService)
        {
            _hostEnvironment = hostEnvironment;
            _diagramName = "diagram.bpmn";
            _apiService = apiService;
        }

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
            var filePath = Path.Combine(_hostEnvironment.WebRootPath, _diagramName);
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
