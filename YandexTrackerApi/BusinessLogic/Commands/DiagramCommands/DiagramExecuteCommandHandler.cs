using BPMNWorkFlow.BusinessLogic.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Xml.Linq;
using YandexTrackerApi.BusinessLogic.Models;
using YandexTrackerApi.BusinessLogic.Models.DiagramModels;
using YandexTrackerApi.DbModels;

namespace YandexTrackerApi.BusinessLogic.Commands.DiagramCommands
{
    public class DiagramExecuteCommandHandler : IRequestHandler<DiagramExecuteCommand, ResponseModel<string>>
    {
        private readonly ILogger _logger;
        private readonly IGraduateWorkContext _context;

        public DiagramExecuteCommandHandler(ILogger logger, IGraduateWorkContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ResponseModel<string>> Handle(DiagramExecuteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var diagramDbModel = await _context.Diagrams
                    .FirstOrDefaultAsync(d => d.Id == request.Id
                    , cancellationToken);

                if (diagramDbModel?.Xml == null)
                {
                    return new ResponseModel<string> { ErrorMessage = "Не удалось получить диаграмму из базы данных" };
                }

                // Преобразуйте вашу строку в StringReader
                var stringReader = new StringReader(diagramDbModel.Xml);

                // Затем используйте конструктор XDocument, который принимает TextReader
                var xmlDoc = XDocument.Load(stringReader);

                var p = new Process(xmlDoc);
                var processInstance = p.NewProcessInstance();
                processInstance.SetDefaultHandlers();

                var processVar = new Dictionary<string, object>();

                // Создаем список для хранения пути выполнения
                var executionPath = new List<string>();

                await processInstance.StartAsync(processVar);

                foreach (var node in processInstance.Nodes.Values)
                {
                    if (node.TaskCompletionSource.Task.IsCompletedSuccessfully)
                    {
                        executionPath.Add($"Завершение узла: Id: {node.NodeId} Имя узла: {node.NodeName}");
                    }
                }

                // Преобразуем список в строку для отправки пользователю
                var executionPathString = string.Join("\n", executionPath);

                return new ResponseModel<string> { Data = executionPathString };

            }
            catch (Exception ex)
            {
                var errorMessage = "Во время выполнения задачи произошла ошибка";
                _logger.LogError(ex, errorMessage);
                return new ResponseModel<string> { ErrorMessage = errorMessage };
            }
        }
    }
}
