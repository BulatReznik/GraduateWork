using MediatR;
using Microsoft.EntityFrameworkCore;
using YandexTrackerApi.BusinessLogic.Models;
using YandexTrackerApi.BusinessLogic.Models.DiagramModels;
using YandexTrackerApi.BusinessLogic.Models.DiagramQueries;
using YandexTrackerApi.DbModels;

namespace YandexTrackerApi.BusinessLogic.Queries.DiagramQueries
{
    public class DiagramsQueryHandler : IRequestHandler<DiagramsQuery, ResponseModel<List<DiagramsResponse>>>
    {
        private readonly ILogger _logger;
        private readonly IGraduateWorkContext _context;

        public DiagramsQueryHandler(ILogger logger, IGraduateWorkContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<ResponseModel<List<DiagramsResponse>>> Handle(DiagramsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var access = await _context.UsersProjects.AnyAsync(up => up.ProjectId == request.ProjectId
                                                            && up.UserId == request.UserId
                                                            , cancellationToken);
                if (!access)
                {
                    return new ResponseModel<List<DiagramsResponse>> { ErrorMessage = "У пользователя нет доступа к этому проекту" };
                }

                var diagrams = await _context.Diagrams
                    .Where(d => d.ProjectId == request.ProjectId)
                    .ToListAsync(cancellationToken);

                var response = diagrams.Select(diagram => new DiagramsResponse
                {
                    Id = diagram.Id,
                    Name = diagram.Name,
                    Date = diagram.Date,
                }).ToList();

                return new ResponseModel<List<DiagramsResponse>> { Data = response };

            }
            catch (Exception ex)
            {
                var errorMessage = "Не удалось получить список диаграмм";
                _logger.LogError(errorMessage, ex);
                return new ResponseModel<List<DiagramsResponse>> { ErrorMessage = errorMessage };
            }
        }
    }
}
