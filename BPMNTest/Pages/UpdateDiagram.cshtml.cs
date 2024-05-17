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

        public string? DiagramName { get; set; }

        public UpdateDiagramModel(IWebHostEnvironment hostEnvironment, ApiService apiService)
        {
            _hostEnvironment = hostEnvironment;
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGet(Guid diagramId)
        {
            _diagramId = diagramId;

            var diagramData = await _apiService.GetAsync<DiagramResponse>($"v1/diagram/{diagramId}");
            DiagramName = diagramData.Data?.Name;
            return Page();
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

            DiagramName = diagramData.Data?.Name;

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

            if (response.IsSuccess)
                return RedirectToPage("/Project", new { diagramUpdateModel.ProjectId });

            TempData["ErrorMessage"] = "������������ �� ������";
            return Page();
        }
    }
}
