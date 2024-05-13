using MediatR;
using Microsoft.EntityFrameworkCore;
using YandexTrackerApi.BusinessLogic.Models;
using YandexTrackerApi.BusinessLogic.Models.ProjectModels;
using YandexTrackerApi.BusinessLogic.Models.ProjectQueries;
using YandexTrackerApi.DbModels;

namespace YandexTrackerApi.BusinessLogic.Queries.ProjectsQueries
{
    public class ProjectQueryHandler : IRequestHandler<ProjectQuery, ResponseModel<ProjectByIdResponse>>
    {
        private readonly IGraduateWorkContext _context;
        private readonly ILogger _logger;

        public ProjectQueryHandler(ILogger logger, IGraduateWorkContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<ResponseModel<ProjectByIdResponse>> Handle(ProjectQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var access = await _context.UsersProjects
                    .AnyAsync(up => up.ProjectId == request.ProjectId 
                                    && up.UserId == request.UserId 
                                    && up.Condirmed == true
                    , cancellationToken: cancellationToken);

                if (!access)
                {
                    return new ResponseModel<ProjectByIdResponse> { ErrorMessage = "У пользователя нет доступа к этому проекту" };
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == request.UserId
                    , cancellationToken);

                var project = await _context.Projects
                    .Select(p => new ProjectByIdResponse
                    {
                        CreatorId = p.CreatorId,
                        Description = p.Description,
                        Id = p.Id,
                        Name = p.Name,
                        CreatorName = user.Name
                    }
                    ).FirstOrDefaultAsync(p => p.Id == request.ProjectId
                        , cancellationToken: cancellationToken);

                return new ResponseModel<ProjectByIdResponse> { Data = project };
            }
            catch (Exception ex)
            {
                const string errorMessage = "Не удалось получить проект";
                _logger.LogError(errorMessage, ex);
                return new ResponseModel<ProjectByIdResponse> { ErrorMessage = errorMessage };
            }
        }
    }
}
