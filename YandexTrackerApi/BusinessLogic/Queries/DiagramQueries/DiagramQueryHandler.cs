using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using YandexTrackerApi.BusinessLogic.Models;
using YandexTrackerApi.BusinessLogic.Models.DiagramModels;
using YandexTrackerApi.BusinessLogic.Models.DiagramQueries;
using YandexTrackerApi.DbModels;

namespace YandexTrackerApi.BusinessLogic.Queries.DiagramQueries
{
    public class DiagramQueryHandler : IRequestHandler<DiagramQuery, ResponseModel<DiagramResponse>>
    {
        private readonly ILogger _logger;
        private readonly IGraduateWorkContext _context;

        public DiagramQueryHandler(ILogger logger, IGraduateWorkContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<ResponseModel<DiagramResponse>> Handle(DiagramQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var diagramDbModel = await (from diagrams in _context.Diagrams

                                            join projects in _context.Projects
                                            on diagrams.ProjectId equals projects.Id

                                            join usersProjects in _context.UsersProjects
                                            on projects.Id equals usersProjects.ProjectId

                                            where diagrams.Id == request.Id &&
                                                  usersProjects.UserId == request.UserId
                                            select new DiagramResponse
                                            {
                                                Id = diagrams.Id,
                                                Document = diagrams.Xml,
                                                Name = diagrams.Name,
                                                Date = diagrams.Date,
                                            }).FirstOrDefaultAsync(cancellationToken);

                if (diagramDbModel != null)
                {
                    return new ResponseModel<DiagramResponse>() { Data = diagramDbModel };
                }
                else
                {
                    return new ResponseModel<DiagramResponse> { ErrorMessage = "Не удалось получить диаграмму" };
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "Не удалось получить диаграмму";
                _logger.LogError(errorMessage, ex);
                return new ResponseModel<DiagramResponse> { ErrorMessage = errorMessage };
            }
        }
    }
}
