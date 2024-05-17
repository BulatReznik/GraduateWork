using MediatR;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace YandexTrackerApi.BusinessLogic.Models.DiagramModels
{
    public class DiagramDeleteCommand : IRequest<ResponseModel<string>>
    {
        [Required(ErrorMessage = "Поле DiagramId должно быть заполнено")]
        public Guid DiagramId { get; set; }

        [JsonIgnore]
        [Required(ErrorMessage = "Поле UserId должно быть заполнено")]
        public Guid UserId { get; set; }
    }
}
