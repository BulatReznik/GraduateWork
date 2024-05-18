using MediatR;
using Microsoft.EntityFrameworkCore;
using YandexTrackerApi.BusinessLogic.Models;
using YandexTrackerApi.BusinessLogic.Models.ProjectModels;
using YandexTrackerApi.BusinessLogic.Models.ProjectQueries;
using YandexTrackerApi.DbModels;

namespace YandexTrackerApi.BusinessLogic.Queries.ProjectsQueries
{
    public class ProjectUsersQueryHandler : IRequestHandler<ProjectUsersQuery, ResponseModel<List<ProjectUserResponse>>>
    {
        private readonly IGraduateWorkContext _context;
        private readonly ILogger _logger;

        public ProjectUsersQueryHandler(IGraduateWorkContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ResponseModel<List<ProjectUserResponse>>> Handle(ProjectUsersQuery request, CancellationToken cancellationToken)
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
                    return new ResponseModel<List<ProjectUserResponse>> { ErrorMessage = "У пользователя нет доступа к этому проекту" };
                }

                var projectUsers = await _context.UsersProjects
                    .Where(up => up.ProjectId == request.ProjectId)
                    .Select(up => new ProjectUserResponse
                    {
                        UserId = up.UserId,
                        Confirmed = up.Condirmed,
                        Login = up.User.Login,
                        Name = up.User.Name
                    }).ToListAsync(cancellationToken);

                return new ResponseModel<List<ProjectUserResponse>>
                {
                    Data = projectUsers
                };
            }
            catch (Exception ex)
            {
                var errorMessage = "Не удалось получить список пользователей приглашенных в проект";
                _logger.LogError(ex, errorMessage);
                return new ResponseModel<List<ProjectUserResponse>>() { ErrorMessage = errorMessage };
            }
        }
    }
}
