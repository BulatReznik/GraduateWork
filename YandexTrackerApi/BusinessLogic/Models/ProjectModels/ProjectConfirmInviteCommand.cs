using MediatR;
using Newtonsoft.Json;

namespace YandexTrackerApi.BusinessLogic.Models.ProjectModels
{
    /// <summary>
    /// Команда для подтверждения приглашения в проект
    /// </summary>
    public class ProjectConfirmInviteCommand : IRequest<ResponseModel<string>>
    {
        /// <summary>
        /// Код для подтверждения принятия приглашения на проект
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Id пользователя который принимает код
        /// </summary>
        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}
