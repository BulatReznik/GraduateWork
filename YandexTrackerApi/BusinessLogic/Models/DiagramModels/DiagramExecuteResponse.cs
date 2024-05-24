using System.Collections.Immutable;

namespace YandexTrackerApi.BusinessLogic.Models.DiagramModels
{
    public class DiagramExecuteResponse
    {
        public string ExecutePath { get; set; }
        public IImmutableDictionary<string, IImmutableDictionary<string, object>> ImportantOutputParameters { get; set;}
    }
}
