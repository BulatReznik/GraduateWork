using MediatR;
using System.Text.Json.Serialization;
using YandexTrackerApi.BusinessLogic.Models.DiagramModels;

namespace YandexTrackerApi.BusinessLogic.Models.DiagramQueries
{
    public class DiagramsQuery : IRequest<ResponseModel<List<DiagramsResponse>>>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public Guid ProjectId { get; set; }
    }
}
