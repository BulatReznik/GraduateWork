using MediatR;
using Microsoft.EntityFrameworkCore;
using YandexTrackerApi.BusinessLogic.Models;
using YandexTrackerApi.BusinessLogic.Models.ProjectModels;
using YandexTrackerApi.DbModels;

namespace YandexTrackerApi.BusinessLogic.Commands.ProjectCommands
{
    public class ProjectUserDeleteCommandHandler : IRequestHandler<ProjectUserDeleteCommand, ResponseModel<string>>
    {
        private readonly IGraduateWorkContext _context;
        private readonly ILogger _logger;

        public ProjectUserDeleteCommandHandler(IGraduateWorkContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ResponseModel<string>> Handle(ProjectUserDeleteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var access = await _context
                    .UsersProjects
                    .AnyAsync(up => up.UserId == request.UserId 
                                    && up.ProjectId == request.ProjectId
                        , cancellationToken);

                if (!access)
                {
                    return new ResponseModel<string> { ErrorMessage = "У пользователя нет доступа к этому проекту" };
                }

                var usersProject = await _context.UsersProjects.FirstOrDefaultAsync(up => up.UserId == request.UserDelId
                    , cancellationToken: cancellationToken);

                if (usersProject == null)
                {
                    return new ResponseModel<string>() { ErrorMessage = "Проект уже был удален" };
                }

                _context.UsersProjects.Remove(usersProject);
                await _context.SaveChangesAsync(cancellationToken);

                return new ResponseModel<string>() { Data = "Пользователь был успешно исключен из проекта" };
            }
            catch (Exception ex)
            {
                var errorMessage = "Не удалось удалить пользователя из проекта";
                _logger.LogError(ex, errorMessage);
                throw;
            }
        }
    }
}
