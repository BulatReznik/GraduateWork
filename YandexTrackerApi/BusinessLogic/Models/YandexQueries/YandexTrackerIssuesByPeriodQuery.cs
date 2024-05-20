using System.ComponentModel.DataAnnotations;
using MediatR;

namespace YandexTrackerApi.BusinessLogic.Models.YandexQueries
{
    /// <summary>
    /// Запрос на получение данных из Яндекс tracker за период
    /// </summary>
    public class YandexTrackerIssuesByPeriodQuery : IRequest<ResponseModel<YandexTrackerIssuesByPeriodResponse>>
    {
        /// <summary>
        /// Id пользователя, который запрашивает задачи по проекту
        /// </summary>
        [Required(ErrorMessage = "Поле UserId не должно быть пустым")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Id проекта из которого выгружаем задачи
        /// </summary>
        [Required(ErrorMessage = "Поле ProjectId не должно быть пустым")]
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Исполнитель
        /// </summary>
        [Required(ErrorMessage = "Полу UserName не должно быть пустым")]
        public string UserName { get; set; } = null!;

        /// <summary>
        /// Дата начала
        /// </summary>
        [Required(ErrorMessage = "Полу StartDate не должно быть пустым")]
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Дата завершения
        /// </summary>
        [Required(ErrorMessage = "Полу EndDate не должно быть пустым")]
        public DateOnly EndDate { get; set; }
    }
}
