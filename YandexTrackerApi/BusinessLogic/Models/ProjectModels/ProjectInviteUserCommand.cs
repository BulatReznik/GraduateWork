using System.Text.Json.Serialization;
using MediatR;

namespace YandexTrackerApi.BusinessLogic.Models.ProjectModels
{
    /// <summary>
    /// Команда для приглашения пользователя на проект
    /// </summary>
    public class ProjectInviteUserCommand : IRequest<ResponseModel<string>>
    {
        /// <summary>
        /// Id проекта
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Email пользователя которого мы приглашаем на проект
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Id пользователя добавляющего на проект другого пользователя
        /// </summary>
        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}
