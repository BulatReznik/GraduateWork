using Microsoft.AspNetCore.Mvc;
using System.Text;
using BPMNWorkFlow;
using System.Xml.Linq;
using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;
using Microsoft.IdentityModel.Tokens;

namespace BPMNTest.Controllers
{

    public class HomeController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> SaveDiagramAsync(IFormCollection form)
        {
            KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> diagramData = form.First(); // Предполагается, что данные диаграммы в поле "diagramData"

            if (diagramData.Value.IsNullOrEmpty())
            {
                return BadRequest("diagramData был пуст");
            }

            // Преобразуйте вашу строку в StringReader
            var stringReader = new StringReader(diagramData.Value);

            // Затем используйте конструктор XDocument, который принимает TextReader
            var xmlDoc = XDocument.Load(stringReader);

            var p = new Process(xmlDoc);
            var processInstance = p.NewProcessInstance();
            processInstance.SetDefaultHandlers();

            var processVar = new Dictionary<string, object>();
            await processInstance.StartAsync(processVar);

            var diagramDataString = diagramData.Value.ToString();

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