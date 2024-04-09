using System.Xml.Linq;

namespace YandexTrackerApi.BusinessLogic.Models.DiagramModels
{
    public class DiagramResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public XDocument Document { get; set; } = null!;
    }
}
