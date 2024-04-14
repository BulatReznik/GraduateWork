using BPMN.Models.Diagram;
using BPMN.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BPMN.Pages
{
    public class UpdateDiagramModel : PageModel
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ApiService _apiService;

        public string _diagram;
        public string _projectId;
        public Guid _diagramId;

        public UpdateDiagramModel(IWebHostEnvironment hostEnvironment, ApiService apiService)
        {
            _hostEnvironment = hostEnvironment;
            _apiService = apiService;
        }

        public void OnGet(Guid diagramId)
        {
            _diagramId = diagramId;
        }

        public async Task<IActionResult> OnGetDiagram(Guid id)
        {
            // �������� ���������, ��������, �� ������ �������
            var diagramData = await _apiService.GetAsync<DiagramResponse>($"v1/diagram/{id}");

            // ���������, �������� �� ���������
            if (!diagramData.IsSuccess)
            {
                return NotFound(); // ���� ��������� �� �������, ���������� ������ 404
            }

            // ������� ���� ��� ���������� ����� ���������
            var tempFilePath = Path.Combine(_hostEnvironment.WebRootPath, "temp", $"{id}.bpmn");

            // ��������� ��������� �� ��������� ����
            await System.IO.File.WriteAllTextAsync(tempFilePath, diagramData.Data.Document);

            // ���������� ��������� ���� ��� ���������� ����
            return PhysicalFile(tempFilePath, "application/xml");
        }

        /// <summary>
        /// ������� ��������� �� ��������� ������
        /// </summary>
        public void OnGetDeleteTempFile(Guid id)
        {
            var tempFilePath = Path.Combine(_hostEnvironment.WebRootPath, "temp", $"{id}.bpmn");

            if (System.IO.File.Exists(tempFilePath))
            {
                System.IO.File.Delete(tempFilePath);
            }
        }

        public async Task<IActionResult> OnPost(DiagramUpdateModel diagramUpdateModel)
        {
            var response = await _apiService.PostStringAsync("v1/diagrams/update/", diagramUpdateModel);

            if (!response.IsSuccess)
            {
                TempData["ErrorMessage"] = "������������ �� ������";
                return Page();
            }

            return RedirectToPage("/Project", new { diagramUpdateModel.ProjectId });
        }
    }
}
