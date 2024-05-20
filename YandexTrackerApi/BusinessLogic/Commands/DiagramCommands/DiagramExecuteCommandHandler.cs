using BPMNWorkFlow.BusinessLogic.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using YandexTrackerApi.BusinessLogic.Models.DiagramModels;
using YandexTrackerApi.DbModels;

namespace YandexTrackerApi.BusinessLogic.Commands.DiagramCommands
{
    public class DiagramExecuteCommandHandler : IRequestHandler<DiagramExecuteCommand, Models.ResponseModel<string>>
    {
        private readonly ILogger _logger;
        private readonly IGraduateWorkContext _context;

        public DiagramExecuteCommandHandler(ILogger logger, IGraduateWorkContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Models.ResponseModel<string>> Handle(DiagramExecuteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var diagramDbModel = await _context.Diagrams
                    .FirstOrDefaultAsync(d => d.Id == request.Id
                    , cancellationToken);

                if (diagramDbModel?.Xml == null)
                {
                    return new Models.ResponseModel<string> { ErrorMessage = "Не удалось получить диаграмму из базы данных" };
                }

                // Преобразуйте вашу строку в StringReader
                var stringReader = new StringReader(diagramDbModel.Xml);

                // Затем используйте конструктор XDocument, который принимает TextReader
                var xmlDoc = XDocument.Load(stringReader);

                var process = new Process(xmlDoc);
                var processInstance = process.NewProcessInstance();
                processInstance.SetDefaultHandlers();

                var processVar = new Dictionary<string, object>();

                // Создаем список для хранения пути выполнения

                await processInstance.StartAsync(processVar);

                var outputParameters = processInstance.OutputParameters;

                // Создаем путь выполнения, сортируя узлы по nodeNumber
                var executionPath = (
                    from node in processInstance.Nodes.Values
                    where node.TaskCompletionSource.Task.IsCompletedSuccessfully
                          && node.OutputParameters.ContainsKey("nodeNumber")  // Проверяем наличие nodeNumber
                    let nodeNumber = (int)node.OutputParameters["nodeNumber"]  // Извлекаем nodeNumber
                    orderby nodeNumber  // Сортируем по nodeNumber
                    select $"Завершение узла: Id: {node.NodeId} Имя узла: {node.NodeName} Номер узла: {nodeNumber}"
                ).ToList();

                // Преобразуем список в строку для отправки пользователю
                var executionPathString = string.Join("\n", executionPath);

                return new Models.ResponseModel<string> { Data = executionPathString };

            }
            catch (Exception ex)
            {
                var errorMessage = "Во время выполнения задачи произошла ошибка";
                _logger.LogError(ex, errorMessage);
                return new Models.ResponseModel<string> { ErrorMessage = errorMessage };
            }
        }
    }
}
