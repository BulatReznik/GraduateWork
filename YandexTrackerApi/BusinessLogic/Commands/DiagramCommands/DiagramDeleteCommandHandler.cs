using MediatR;
using Microsoft.EntityFrameworkCore;
using YandexTrackerApi.BusinessLogic.Models;
using YandexTrackerApi.BusinessLogic.Models.DiagramModels;
using YandexTrackerApi.DbModels;

namespace YandexTrackerApi.BusinessLogic.Commands.DiagramCommands
{
    public class DiagramDeleteCommandHandler : IRequestHandler<DiagramDeleteCommand, ResponseModel<string>>
    {
        private readonly IGraduateWorkContext _context;
        private readonly ILogger _logger;

        public DiagramDeleteCommandHandler(IGraduateWorkContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ResponseModel<string>> Handle(DiagramDeleteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var diagram = await _context.Diagrams.FirstOrDefaultAsync(d => d.Id == request.DiagramId
                    , cancellationToken: cancellationToken);

                if (diagram == null)
                {
                    return new ResponseModel<string>() { ErrorMessage = "Не удалось найти эту диаграмму" };
                }

                var access = await _context.UsersProjects
                    .AnyAsync(up => up.UserId == request.UserId
                                    && up.ProjectId == diagram.ProjectId
                        , cancellationToken: cancellationToken);

                if (!access)
                {
                    return new ResponseModel<string> { ErrorMessage = "У пользователя нет доступа к этому проекту" };
                }

                _context.Diagrams.Remove(diagram);
                await _context.SaveChangesAsync(cancellationToken);

                return new ResponseModel<string> { Data = "Диаграмма была удалена" };
            }
            catch (Exception ex)
            {
                const string errorMessage = "Не удалось удалить диаграмму";
                _logger.LogError(ex, errorMessage);
                return new ResponseModel<string>() { ErrorMessage = errorMessage };
            }
        }
    }
}
