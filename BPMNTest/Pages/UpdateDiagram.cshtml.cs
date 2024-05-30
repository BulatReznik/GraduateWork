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
            // Используйте diagramId переданный в URL
            DiagramId = diagramId;

            // Получаем диаграмму, например, из вашего сервиса
            var diagramData = await _apiService.GetAsync<DiagramResponse>($"v1/diagram/{DiagramId}");

            // Проверяем, получена ли диаграмма
            if (!diagramData.IsSuccess)
            {
                return NotFound(); // Если диаграмма не найдена, возвращаем ошибку 404
            }

            DiagramName = diagramData.Data?.Name;

            // Создаем путь для временного файла диаграммы
            var tempFilePath = Path.Combine(_hostEnvironment.WebRootPath, "temp", $"{DiagramId}.bpmn");

            // Сохраняем диаграмму во временный файл
            await System.IO.File.WriteAllTextAsync(tempFilePath, diagramData.Data.Document);

            // Возвращаем временный файл как физический файл
            return PhysicalFile(tempFilePath, "application/xml");
        }

        /// <summary>
        /// Удаляем диаграмму из временных файлов
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

            TempData["ErrorMessage"] = "Пользователь не найден";
            return Page();
        }

        public async Task<IActionResult> OnPostExecuteDiagram()
        {
            try
            {
                var executeDiagramRequest = new DiagramExecuteModel { Id = DiagramId };

                // Отправляем запрос на выполнение диаграммы
                var executeDiagramResponse = await _apiService.PostAsync<DiagramExecuteModel, DiagramExecuteResponse>("v1/diagrams/execute", executeDiagramRequest);

                if (!executeDiagramResponse.IsSuccess)
                {
                    // Обработка ошибки, если запрос на выполнение диаграммы не удался
                    TempData["ErrorMessage"] = executeDiagramResponse.ErrorMessage;
                    return RedirectToPage("/Error"); // Перенаправляем на страницу с ошибкой
                }

                // Успешное выполнение диаграммы
                var successMessage = $"Выполненные узлы: \n{executeDiagramResponse?.Data?.ExecutePath}\n\n";

                // Получение важных выходных параметров
                var importantOutputParameters = executeDiagramResponse?.Data?.ImportantOutputParameters;

                // Формирование сообщения с важными выходными параметрами
                var outputParametersMessage = "Важные выходные параметры:\n";
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

                // Получаем диаграмму, например, из вашего сервиса
                var diagramData = await _apiService.GetAsync<DiagramResponse>($"v1/diagram/{DiagramId}");
                DiagramName = diagramData.Data?.Name;

                return Page();
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Во время выполнения диаграммы произошла ошибка";
                return Page();
            }
        }
    }
}
