using MediatR;
using Microsoft.EntityFrameworkCore;
using YandexTrackerApi.BusinessLogic.Models;
using YandexTrackerApi.BusinessLogic.Models.DiagramModels;
using YandexTrackerApi.DbModels;

namespace YandexTrackerApi.BusinessLogic.Commands.DiagramCommands
{
    public class DiagramCreateCommandHandler : IRequestHandler<DiagramCreateCommand, ResponseModel<string>>
    {
        private readonly ILogger _logger;
        private readonly IGraduateWorkContext _context;

        public DiagramCreateCommandHandler(ILogger logger, IGraduateWorkContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<ResponseModel<string>> Handle(DiagramCreateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var access = await _context.UsersProjects
                    .AnyAsync(up => up.UserId == request.UserId
                                    && up.ProjectId == request.ProjectId
                , cancellationToken: cancellationToken);

                if (!access)
                {
                    return new ResponseModel<string> { ErrorMessage = "У пользователя нет доступа к этому проекту" };
                }

                var diagramDbModel = new Diagram()
                {
                    Id = Guid.NewGuid(),
                    Xml = request.XMLDiagram,
                    Name = request.Name,
                    ProjectId = request.ProjectId,
                };

                await _context.Diagrams.AddAsync(diagramDbModel, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return new ResponseModel<string> { Data = "Сохранение модели прошло успешно" };
            }
            catch (Exception ex)
            {
                var errorMessage = "Не удалось получить BPMN диаграмму";
                _logger.LogError(errorMessage, ex);
                return new ResponseModel<string> { ErrorMessage = errorMessage };
            }
        }
    }
}
