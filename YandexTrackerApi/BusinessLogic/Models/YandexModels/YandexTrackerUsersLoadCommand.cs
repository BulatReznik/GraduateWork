using Azure;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace YandexTrackerApi.BusinessLogic.Models.YandexModels
{
    /// <summary>
    /// Команда на получение пользователей для таск трекера
    /// </summary>
    public class YandexTrackerUsersLoadCommand : IRequest<ResponseModel<string>>
    {
        /// <summary>
        /// Id проекта для которого получаем пользователя
        /// </summary>
        [Required(ErrorMessage = "Поле ProjectId должно быть заполнено")]
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Id пользователя запрашивающего пользователей из проекта
        /// </summary>
        [Required(ErrorMessage = "Поле UserId должно быть заполнено")]
        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}
