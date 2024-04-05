using MediatR;
using System.ComponentModel.DataAnnotations;

namespace YandexTrackerApi.BusinessLogic.Models.CalendarModels
{
    public class CalendarDayCreateCommand : IRequest<ResponseModel<string>>
    {
        [Required(ErrorMessage = "Поле DateStart не может быть пустым")]
        public DateOnly DateStart { get; set; }

        [Required(ErrorMessage = "Поле DateFinish не может быть пустым")]
        public DateOnly DateFinish { get; set; }

        [Required(ErrorMessage = "Поле UserId не может быть пустым")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Поле EmployeeId не может быть пустым")]
        public string EmployeeId { get; set; } = null!;

        [Required(ErrorMessage = "Поле ProjectId не может быть пустым")]
        public Guid ProjectId { get; set; }
    }
}
