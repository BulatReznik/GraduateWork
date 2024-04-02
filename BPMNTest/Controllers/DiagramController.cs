using Microsoft.AspNetCore.Mvc;
using System.Security.AccessControl;
using System.Text;
using System.Xml;

namespace BPMNTest.Controllers
{
    public class HomeController : Controller
    {
        [HttpPost]
        public IActionResult SaveDiagram(IFormCollection form)
        {
            KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> diagramData = form.First(); // Предполагается, что данные диаграммы в поле "diagramData"

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(diagramData.Value);



            var diagramDataString = diagramData.ToString();

            if (string.IsNullOrEmpty(diagramDataString))
            {
                // Обработка случая, когда данных нет
                return BadRequest("Данные диаграммы отсутствуют.");
            }

            var buffer = Encoding.UTF8.GetBytes(diagramDataString);
            var fileName = "diagram.bpmn"; // Вы можете выбрать другое имя и расширение файла

            File(buffer, "application/octet-stream", fileName);

            return File(buffer, "application/octet-stream", fileName);
        }

        [HttpGet]
        public IActionResult Projects()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Invitations()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Experts()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Project()
        {
            return View();
        }
    }
}