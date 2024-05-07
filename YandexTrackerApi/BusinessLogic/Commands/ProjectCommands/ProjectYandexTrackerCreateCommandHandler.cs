using MediatR;
using Microsoft.EntityFrameworkCore;
using YandexTrackerApi.BusinessLogic.Models;
using YandexTrackerApi.BusinessLogic.Models.ProjectModels;
using YandexTrackerApi.DbModels;

namespace YandexTrackerApi.BusinessLogic.Commands.ProjectCommands
{
    public class ProjectYandexTrackerCreateCommandHandler : IRequestHandler<ProjectYandexTrackerCreateCommand, ResponseModel<string>>
    {
        private readonly IGraduateWorkContext _context;
        private readonly ILogger _logger;

        public ProjectYandexTrackerCreateCommandHandler(IGraduateWorkContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ResponseModel<string>> Handle(ProjectYandexTrackerCreateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var access = await _context.UsersProjects
                    .AnyAsync(usersProject => usersProject.ProjectId == request.ProjectId && usersProject.UserId == request.UserId
                        , cancellationToken: cancellationToken);

                if (!access)
                {
                    return new ResponseModel<string> { ErrorMessage = "Нет доступа к проекту" };
                }

                var yandexTrackerDbModel = new YandexTracker()
                {
                    Id = request.ProjectId,
                    CloudOrgId = request.CloudOrgId,
                    OauthToken = request.OauthToken
                };

                await _context.YandexTrackers.AddAsync(yandexTrackerDbModel, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return new ResponseModel<string> { Data = "Данные для подключения к таск-трекру сохранены" };
            }
            catch (Exception ex)
            {
                var errorMessage = "Не удалось сохранить данные для подключения к таск-трекеру";
                _logger.LogError(errorMessage, ex);
                return new ResponseModel<string> { ErrorMessage = errorMessage };
            }
        }
    }
}
