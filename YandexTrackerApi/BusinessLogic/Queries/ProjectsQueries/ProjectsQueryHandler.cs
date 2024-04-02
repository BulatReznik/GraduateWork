using MediatR;
using Microsoft.EntityFrameworkCore;
using YandexTrackerApi.BusinessLogic.Models;
using YandexTrackerApi.BusinessLogic.Models.ProjectModels;
using YandexTrackerApi.BusinessLogic.Models.ProjectQueries;
using YandexTrackerApi.DbModels;

namespace YandexTrackerApi.BusinessLogic.Queries.ProjectsQueries
{
    public class ProjectsQueryHandler : IRequestHandler<ProjectsQuery, ResponseModel<List<ProjectResponseModel>>>
    {
        private readonly ILogger _logger;
        private readonly IGraduateWorkContext _context;

        public ProjectsQueryHandler(ILogger logger, IGraduateWorkContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<ResponseModel<List<ProjectResponseModel>>> Handle(ProjectsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _context.Projects
                    .Where(p => p.UsersProjects.Any(up => up.UserId == request.UserId))
                    .Select(p => new ProjectResponseModel
                    {
                        CreatorId = p.CreatorId,
                        Description = p.Description,
                        Id = p.Id,
                        Name = p.Name
                    })
                    .ToListAsync(cancellationToken: cancellationToken);

                return new ResponseModel<List<ProjectResponseModel>> { Data = result };
            }
            catch (Exception ex)
            {
                var errorMessage = "Ошибка получения проектов";
                _logger.LogError(ex, errorMessage);
                return new ResponseModel<List<ProjectResponseModel>> { ErrorMessage = errorMessage };
            }
        }
    }
}
