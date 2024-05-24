using MediatR;
using System.Text.Json.Serialization;
using YandexTrackerApi.BusinessLogic.Models;

namespace YandexTrackerApi.BusinessLogic.Models.DiagramModels
{
    public class DiagramExecuteCommand : IRequest<ResponseModel<DiagramExecuteResponse>>
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}
