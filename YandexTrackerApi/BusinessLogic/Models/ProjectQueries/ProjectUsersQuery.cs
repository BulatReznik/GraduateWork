using MediatR;
using YandexTrackerApi.BusinessLogic.Models.ProjectModels;

namespace YandexTrackerApi.BusinessLogic.Models.ProjectQueries
{
    public class ProjectUsersQuery : IRequest<ResponseModel<List<ProjectUserResponse>>>
    {
        /// <summary>
        /// Id пользователя запрашивающего информацию по другим пользователям на проекте
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Id проекта, у которого запрашиваются пользователи
        /// </summary>
        public Guid ProjectId { get; set; }
    }
}
