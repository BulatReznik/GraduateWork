using MediatR;
using Microsoft.EntityFrameworkCore;
using YandexTrackerApi.BusinessLogic.Models;
using YandexTrackerApi.BusinessLogic.Models.ProjectModels;
using YandexTrackerApi.DbModels;

namespace YandexTrackerApi.BusinessLogic.Commands.ProjectCommands
{
    public class ProjectConfirmInviteCommandHandler : IRequestHandler<ProjectConfirmInviteCommand, ResponseModel<string>>
    {
        private readonly ILogger _logger;
        private readonly IGraduateWorkContext _context;

        public ProjectConfirmInviteCommandHandler(ILogger logger, IGraduateWorkContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<ResponseModel<string>> Handle(ProjectConfirmInviteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userProject = await _context.UsersProjects
                    .FirstOrDefaultAsync(up => up.InviteCode == request.Code && up.UserId == request.UserId
                        , cancellationToken);

                if (userProject == null)
                    return new ResponseModel<string>() { ErrorMessage = "Не удалось найти проект по этому коду" };

                userProject.InviteCode = null;
                userProject.Condirmed = true;

                await _context.SaveChangesAsync(cancellationToken);

                return new ResponseModel<string>() { Data = "Пользователь получил доступ к проекту" };
            }
            catch (Exception ex)
            {
                var errorMessage = "Не удалось добавить пользователя в проект";
                _logger.LogError(ex, errorMessage);
                return new ResponseModel<string>() { ErrorMessage = errorMessage };
            }
        }
    }
}
