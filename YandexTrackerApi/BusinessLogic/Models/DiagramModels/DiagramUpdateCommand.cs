using MediatR;
using System.Text.Json.Serialization;

namespace YandexTrackerApi.BusinessLogic.Models.DiagramModels
{
    public class DiagramUpdateCommand : IRequest<ResponseModel<string>>
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string XMLDiagram { get; set; } = null!;

        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}
