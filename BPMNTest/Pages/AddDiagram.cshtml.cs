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
                // � ������ ������ ���������� ��������� �� ������
                TempData["ErrorMessage"] = response.ErrorMessage;
                return Page();
            }
        }

        public IActionResult OnGetDiagram()
        {
            var filePath = Path.Combine(_hostEnvironment.WebRootPath, _diagramName);
            var fileInfo = new FileInfo(filePath);

            // ���������, ���������� �� ����
            if (!fileInfo.Exists)
            {
                return NotFound(); // ���������� ������ 404, ���� ���� �� ������
            }

            // ���������� ���� ��� ��������� �������
            return PhysicalFile(filePath, "application/xml");
        }
    }
}
