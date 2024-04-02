using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace YandexTrackerApi.BusinessLogic.Models.ProjectModels
{
    /// <summary>
    /// Команда для содания проекта
    /// </summary>
    public class ProjectCreateCommand : IRequest<ResponseModel<string>>
    {
        /// <summary>
        /// Имя проекта
        /// </summary>
        [Required(ErrorMessage = "Поле Name не может быть пустым")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Описание проекта
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Id пользователя создающего проект
        /// </summary>
        [Required(ErrorMessage = "Поле CreatorId не может быть пустым")]
        [JsonIgnore]
        public Guid CreatorId { get; set; }
    }
}
