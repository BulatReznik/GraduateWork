using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace YandexTrackerApi.BusinessLogic.Models.YandexModels
{
    /// <summary>
    /// Команда для получения данных из трекера о задачах одного из пользователей
    /// </summary>
    public class YandexTrackerIssueByUserByPeriodLoadCommand : IRequest<ResponseModel<string>>
    {
        /// <summary>
        /// Id пользователя делающего запрос
        /// </summary>
        [JsonIgnore]
        [Required(ErrorMessage = "Поле UserId должно быть заполнено")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Id проекта по которому получаются задачи
        /// </summary>
        [Required(ErrorMessage = "Поле ProjectId должно быть заполнено")]
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Id пользователя по которому получаем задачи
        /// </summary>
        [Required(ErrorMessage = "Поле EmployeeId должно быть заполнено")]
        public string EmployeeId { get; set; } = null!;

        /// <summary>
        /// Начальная дата загрузки
        /// </summary>
        [Required(ErrorMessage = "Поле StartDate должно быть заполнено")]
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Конечная дата загрузки
        /// </summary>
        [Required(ErrorMessage = "Поле EndDate должно быть заполнено")]
        public DateOnly EndDate { get; set; }
    }
}
