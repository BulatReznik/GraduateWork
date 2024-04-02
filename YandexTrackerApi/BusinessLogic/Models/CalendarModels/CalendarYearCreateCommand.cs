using MediatR;
using System.ComponentModel.DataAnnotations;

namespace YandexTrackerApi.BusinessLogic.Models.CalendarModels
{
    public class CalendarYearCreateCommand : IRequest<ResponseModel<string>>
    {
        [Required(ErrorMessage = "Поле Year не может быть пустым")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Длина поля Year должна быть равна 4")]
        public string Year { get; set; } = null!;
    }
}
