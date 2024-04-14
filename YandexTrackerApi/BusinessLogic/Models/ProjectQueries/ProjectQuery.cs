using MediatR;
using YandexTrackerApi.BusinessLogic.Models.DiagramModels;
using YandexTrackerApi.BusinessLogic.Models.ProjectModels;

namespace YandexTrackerApi.BusinessLogic.Models.ProjectQueries
{
    public class ProjectQuery : IRequest<ResponseModel<ProjectByIdResponse>>
    {
        /// <summary>
        /// Id диаграммы
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Id пользователя
        /// </summary>
        public Guid UserId { get; set; }
    }
}
