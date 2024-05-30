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
        
        [BindProperty]
        public string ProjectId { get; set; }

        [BindProperty]
        public Guid DiagramId { get; set; }

        [BindProperty]
        public string? DiagramName { get; set; }

        public UpdateDiagramModel(IWebHostEnvironment hostEnvironment, ApiService apiService)
        {
            _hostEnvironment = hostEnvironment;
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGet(Guid diagramId)
        {
            DiagramId = diagramId;

            var diagramData = await _apiService.GetAsync<DiagramResponse>($"v1/diagram/{diagramId}");
            DiagramName = diagramData.Data?.Name;
            return Page();
        }

        public async Task<IActionResult> OnGetDiagram(Guid diagramId)
        {
            // ����������� diagramId ���������� � URL
            DiagramId = diagramId;

            // �������� ���������, ��������, �� ������ �������
            var diagramData = await _apiService.GetAsync<DiagramResponse>($"v1/diagram/{DiagramId}");

            // ���������, �������� �� ���������
            if (!diagramData.IsSuccess)
            {
                return NotFound(); // ���� ��������� �� �������, ���������� ������ 404
            }

            DiagramName = diagramData.Data?.Name;

            // ������� ���� ��� ���������� ����� ���������
            var tempFilePath = Path.Combine(_hostEnvironment.WebRootPath, "temp", $"{DiagramId}.bpmn");

            // ��������� ��������� �� ��������� ����
            await System.IO.File.WriteAllTextAsync(tempFilePath, diagramData.Data.Document);

            // ���������� ��������� ���� ��� ���������� ����
            return PhysicalFile(tempFilePath, "application/xml");
        }

        /// <summary>
        /// ������� ��������� �� ��������� ������
        /// </summary>
        public void OnGetDeleteTempFile()
        {
            var tempFilePath = Path.Combine(_hostEnvironment.WebRootPath, "temp", $"{DiagramId}.bpmn");

            if (System.IO.File.Exists(tempFilePath))
            {
                System.IO.File.Delete(tempFilePath);
            }
        }

        public async Task<IActionResult> OnPostSave(DiagramUpdateModel diagramUpdateModel)
        {
            var response = await _apiService.PostStringAsync("v1/diagrams/update/", diagramUpdateModel);

            if (response.IsSuccess)
                return RedirectToPage("/Project", new { diagramUpdateModel.ProjectId });

            TempData["ErrorMessage"] = "������������ �� ������";
            return Page();
        }

        public async Task<IActionResult> OnPostExecuteDiagram()
        {
            try
            {
                var executeDiagramRequest = new DiagramExecuteModel { Id = DiagramId };

                // ���������� ������ �� ���������� ���������
                var executeDiagramResponse = await _apiService.PostAsync<DiagramExecuteModel, DiagramExecuteResponse>("v1/diagrams/execute", executeDiagramRequest);

                if (!executeDiagramResponse.IsSuccess)
                {
                    // ��������� ������, ���� ������ �� ���������� ��������� �� ������
                    TempData["ErrorMessage"] = executeDiagramResponse.ErrorMessage;
                    return RedirectToPage("/Error"); // �������������� �� �������� � �������
                }

                // �������� ���������� ���������
                var successMessage = $"����������� ����: \n{executeDiagramResponse?.Data?.ExecutePath}\n\n";

                // ��������� ������ �������� ����������
                var importantOutputParameters = executeDiagramResponse?.Data?.ImportantOutputParameters;

                // ������������ ��������� � ������� ��������� �����������
                var outputParametersMessage = "������ �������� ���������:\n";
                if (importantOutputParameters != null)
                {
                    foreach (var parameter in importantOutputParameters)
                    {
                        outputParametersMessage += $"{parameter.Key}:\n";
                        foreach (var value in parameter.Value)
                        {
                            outputParametersMessage += $"{value.Key}: {value.Value}\n";
                        }
                    }
                }

                TempData["SuccessMessage"] = successMessage + outputParametersMessage;

                // �������� ���������, ��������, �� ������ �������
                var diagramData = await _apiService.GetAsync<DiagramResponse>($"v1/diagram/{DiagramId}");
                DiagramName = diagramData.Data?.Name;

                return Page();
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "�� ����� ���������� ��������� ��������� ������";
                return Page();
            }
        }
    }
}
