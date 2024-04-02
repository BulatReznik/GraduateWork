using System.ComponentModel.DataAnnotations;
using YandexTrackerApi.Models.YandexModels;

namespace YandexTrackerApi.BusinessLogic.Queries.YandexQueries
{
    /// <summary>
    /// Запрос на получение данных из Яндекс трекера за период
    /// </summary>
    public class IssuesByPeriodQuery
    {
        /// <summary>
        /// Исполнитель
        /// </summary>
        [Required(ErrorMessage = "Полу Assignee не должно быть пустым")]
        public string Assignee { get; set; } = null!;

        /// <summary>
        /// Дата начала
        /// </summary>
        [Required(ErrorMessage = "Полу StartDate не должно быть пустым")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата завершения
        /// </summary>
        [Required(ErrorMessage = "Полу EndDate не должно быть пустым")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Конструктор по умолчанию, устанавливающий базовые значения
        /// </summary>
        public IssuesByPeriodQuery()
        {
            // Устанавливаем базовые значения для свойств
            Assignee = "bulat.almuchammetov-smartf";
            StartDate = new DateTime(2024, 3, 1);
            EndDate = new DateTime(2024, 4, 1);
        }
    }
}
