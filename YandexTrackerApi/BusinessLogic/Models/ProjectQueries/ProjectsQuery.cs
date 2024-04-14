using MediatR;
using System.ComponentModel.DataAnnotations;
using YandexTrackerApi.BusinessLogic.Models.ProjectModels;

namespace YandexTrackerApi.BusinessLogic.Models.ProjectQueries
{
    /// <summary>
    /// Запрос на получение проектов пользователя
    /// </summary>
    public class ProjectsQuery : IRequest<ResponseModel<List<ProjectByIdResponse>>>
    {
        /// <summary>
        /// Id пользователя, который запрашивает проекта
        /// </summary>
        [Required(ErrorMessage = "Поле UserId не может быть пустым")]
        public Guid UserId { get; set; }
    }
}
