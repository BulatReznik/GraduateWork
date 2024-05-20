using MediatR;
using YandexTrackerApi.BusinessLogic.Models;
using YandexTrackerApi.BusinessLogic.Models.ProjectModels;
using YandexTrackerApi.DbModels;

namespace YandexTrackerApi.BusinessLogic.Commands.ProjectCommands
{
    public class ProjectCreateCommandHandler : IRequestHandler<ProjectCreateCommand, ResponseModel<string>>
    {
        private readonly ILogger _logger;
        private readonly IGraduateWorkContext _context;

        public ProjectCreateCommandHandler(ILogger logger, IGraduateWorkContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<ResponseModel<string>> Handle(ProjectCreateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var projectDbModel = new Project
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    CreatorId = request.CreatorId
                };

                var usersProjectsDbModel = new UsersProject
                {
                    ProjectId = projectDbModel.Id,
                    UserId = request.CreatorId,
                    Condirmed = true
                };

                await _context.Projects.AddAsync(projectDbModel, cancellationToken);

                await _context.UsersProjects.AddAsync(usersProjectsDbModel, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return new ResponseModel<string> { Data = "Проект сохранен" };
            }
            catch (Exception ex)
            {
                var errorMessage = "Не удалось сохранить проект";
                _logger.LogError(errorMessage, ex);
                return new ResponseModel<string> { ErrorMessage = errorMessage };
            }
        }
    }
}
