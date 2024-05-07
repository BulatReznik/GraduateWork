using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace YandexTrackerApi.BusinessLogic.Models.ProjectModels
{
    /// <summary>
    /// Команда для заполнений данных нужных для подключения к таск-трекеру
    /// </summary>
    public class ProjectYandexTrackerCreateCommand : IRequest<ResponseModel<string>>
    {
        /// <summary>
        /// Id пользователя пытающегося изменить данные
        /// </summary>
        [Required(ErrorMessage = "Поле UserId не может быть пустым")]
        [JsonIgnore]
        public Guid UserId { get; set; }

        /// <summary>
        /// Id проекта в который мы добавляем информацию о таск-трекере
        /// </summary>
        [Required(ErrorMessage = "Поле ProjectId не может быть пустым")]
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Токен аутентификации приложения
        /// </summary>
        [Required(ErrorMessage = "Поле OAuthToken не может быть пустым")]
        public string OauthToken { get; set; } = null!;

        /// <summary>
        /// Токен аутентификации организации
        /// </summary>
        [Required(ErrorMessage = "Поле CloudOrgId не может быть пустым")]
        public string CloudOrgId { get; set; } = null!;
    }
}
