using MediatR;
using Newtonsoft.Json;

namespace YandexTrackerApi.BusinessLogic.Models.ProjectModels
{
    public class ProjectUserDeleteCommand : IRequest<ResponseModel<string>>
    {
        /// <summary>
        /// Id пользователя, который хочет удалить пользователя с проекта
        /// </summary>
        [JsonIgnore]
        public Guid UserId { get; set; }

        /// <summary>
        /// Id проекта из которого мы хотим удалить проект
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Id пользователя, которого мы хотим удалить
        /// </summary>
        public Guid UserDelId { get; set; }
    }
}
