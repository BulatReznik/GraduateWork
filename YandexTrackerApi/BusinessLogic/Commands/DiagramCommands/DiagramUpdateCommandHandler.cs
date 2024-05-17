using MediatR;
using Microsoft.EntityFrameworkCore;
using YandexTrackerApi.BusinessLogic.Models;
using YandexTrackerApi.BusinessLogic.Models.DiagramModels;
using YandexTrackerApi.DbModels;

namespace YandexTrackerApi.BusinessLogic.Commands.DiagramCommands
{
    public class DiagramUpdateCommandHandler : IRequestHandler<DiagramUpdateCommand, ResponseModel<string>>
    {
        private readonly ILogger _logger;
        private readonly IGraduateWorkContext _context;

        public DiagramUpdateCommandHandler(ILogger logger, IGraduateWorkContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ResponseModel<string>> Handle(DiagramUpdateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Найти диаграмму по ее Id
                var diagram = await _context.Diagrams
                    .FirstOrDefaultAsync(d => d.Id == request.Id
                , cancellationToken);

                // Проверить, найдена ли диаграмма
                if (diagram == null)
                {
                    return new ResponseModel<string> { ErrorMessage = "Диаграмма не найдена" };
                }

                // Обновить свойства диаграммы на основе данных из запроса
                diagram.Name = request.Name;
                diagram.Xml = request.XMLDiagram;
                diagram.Date = DateTime.UtcNow;

                // Сохранить изменения в базе данных
                await _context.SaveChangesAsync(cancellationToken);

                return new ResponseModel<string> { Data = "Диаграмма успешно обновлена" };
            }
            catch (Exception ex)
            {
                var errorMessage = "Не удалось обновить диаграмму";
                _logger.LogError(errorMessage, ex);
                return new ResponseModel<string> { ErrorMessage = errorMessage };
            }
        }
    }
}
