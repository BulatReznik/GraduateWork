using MediatR;
using System.ComponentModel.DataAnnotations;
using YandexTrackerApi.BusinessLogic.Models.DiagramModels;

namespace YandexTrackerApi.BusinessLogic.Models.DiagramQueries
{
    /// <summary>
    /// Запрос на получение диаграммы
    /// </summary>
    public class DiagramQuery : IRequest<ResponseModel<DiagramResponse>>
    {
        [Required(ErrorMessage = "Поле Id должно быть заполнено")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Поле userId должно быть заполненено")]
        public Guid UserId { get; set; }
    }
}
