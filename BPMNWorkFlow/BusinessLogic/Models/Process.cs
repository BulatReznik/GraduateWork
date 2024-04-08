using System.Collections.Immutable;
using System.Xml.Linq;

namespace BPMNWorkFlow.BusinessLogic.Models
{
    public class Process
    {
        /// <summary>
        /// Свойства процесса
        /// </summary>
        internal IEnumerable<Property> Properties { get; set; }

        /// <summary>
        /// Xml документ для процесса
        /// </summary>
        public XElement ProcessXML { get; set; }
        /// <summary>
        /// Пространство имен для процесса
        /// </summary>
        public XNamespace ProcessNamespace { get; set; }

        public Process(XDocument xDocument)
        {
            // Загрузка XML-документа и определение пространства имен
            XDocument doc = xDocument;
            ProcessNamespace = @"http://www.omg.org/spec/BPMN/20100524/MODEL";
            var processXML = doc.Root?.Element(ProcessNamespace + "process");

            if (processXML != null)
            {
                ProcessXML = processXML;
            }
            else
            {
                throw new Exception("Не удалось получить processXML");
            }
            Properties = PropertyInitializer(ProcessXML, ProcessNamespace);
        }

        /// <summary>
        /// Метод для создания нового экземпляра процесса
        /// </summary>
        public ProcessInstance NewProcessInstance()
        {
            // Получение элемента startEvent из XML-документа
            var current = ProcessXML.Element(ProcessNamespace + "startEvent") ?? throw new Exception("current было null");

            // Проверка наличия атрибута "id" у элемента startEvent
            var id = current.Attribute("id")?.Value;
            if (id != null)
            {
                // Создание нового узла процесса
                var node = new ProcessNode(id, current.Name.LocalName, "");
                var nodes = BuildNodes(ProcessXML);
                var processInstance = new ProcessInstance(this);
                BuildLinkedNodes(current, ref node, nodes, processInstance);
                processInstance.Id = Guid.NewGuid().ToString();
                processInstance.StartNode = node;
                processInstance.Nodes = nodes.ToImmutableDictionary();

                return processInstance;
            }
            else
            {
                // В случае отсутствия атрибута "id" у startEvent выбрасывается исключение
                throw new Exception("Не удалось получить Id startEvent");
            }
        }

        /// <summary>
        /// Метод для построения словаря узлов процесса
        /// </summary>
        private Dictionary<string, ProcessNode> BuildNodes(XElement processXML)
        {
            // Создание словаря узлов процесса
            var nodes = processXML
                .Elements()
                .ToDictionary(e => e.Attribute("id").Value,
                e => new ProcessNode(e.Attribute("id").Value,
                e.Name.LocalName,
                e.Attribute("name")?.Value));

            // Удаление узлов с NodeType "property" из словаря
            nodes.Where(e => e.Value.NodeType == "property")
                .Select(e => e.Key).ToList()
                .ForEach(k => nodes.Remove(k));

            // Добавление выражений из элементов script в узлы процесса
            var scripts = processXML.Elements()
                .Elements(ProcessNamespace + "script")
                .Select(s => new
                {
                    id = s.Parent.Attribute("id").Value,
                    expression = s.Value
                });

            foreach (var s in scripts)
            {
                nodes[s.id].Expression = s.expression;
            }

            // Добавление выражений из элементов conditionExpression в узлы процесса
            var conditionExpressions = processXML.Elements()
                .Elements(ProcessNamespace + "conditionExpression")
                .Select(c => new
                {
                    id = c.Parent.Attribute("id").Value,
                    expression = c.Value
                });

            foreach (var c in conditionExpressions)
            {
                nodes[c.id].Expression = c.expression;
            }

            // Добавление выражений из элементов task в узлы процесса
            var taskExpressions = processXML.Elements(ProcessNamespace + "task")
                .Elements(ProcessNamespace + "dataInputAssociation")
                .Elements(ProcessNamespace + "assignment")
                .Elements(ProcessNamespace + "from")
                .Select(e => new { id = e.Parent.Parent.Parent.Attribute("id").Value, expression = e.Value });

            foreach (var e in taskExpressions)
            {
                nodes[e.id].Expression = e.expression;
            }

            return nodes;
        }

        /// <summary>
        /// Метод для построения связанных узлов процесса
        /// </summary>
        private void BuildLinkedNodes(XElement current, ref ProcessNode node, IDictionary<string, ProcessNode> nodes, ProcessInstance processInstance)
        {
            // Установка экземпляра процесса для узла
            node.ProcessInstance = processInstance;
            var seq = NextSequences(current, ProcessXML, ProcessNamespace);
            var next = seq.Any() ? seq : NextElement(current, ProcessXML);
            node.NextNodes = new List<ProcessNode>();

            foreach (var n in next)
            {
                var nextNode = nodes[n.Attribute("id").Value];
                nextNode.PreviousNodes ??= new List<ProcessNode>();

                if (!nextNode.PreviousNodes.Contains(node))
                {
                    nextNode.PreviousNodes.Add(node);
                }

                node.NextNodes.Add(nextNode);
                BuildLinkedNodes(n, ref nextNode, nodes, processInstance);
            }
        }

        /// <summary>
        /// Метод для получения ассоциации между узлом и переменной
        /// </summary>
        internal string GetAssociation(string nodeId, string nodeVariableName)
        {
            var node = ProcessXML.Elements().Where(e => e.Attribute("id").Value == nodeId);
            var inputId = node.Elements(ProcessNamespace + "ioSpecification").Elements(ProcessNamespace + "dataInput")
                .Where(e => e.Attribute("name").Value == nodeVariableName)
                .FirstOrDefault()
                .Attribute("id").Value;

            var propertyId = node.Elements(ProcessNamespace + "dataInputAssociation")
                .Where(d => d.Element(ProcessNamespace + "targetRef").Value == inputId).Elements(ProcessNamespace + "sourceRef").
                FirstOrDefault().Value;

            var propertyName = ProcessXML.Elements(ProcessNamespace + "property")
                .Where(e => e.Attribute("id").Value == propertyId).Attributes("name")
                .FirstOrDefault().Value;

            return propertyName;
        }

        /// <summary>
        /// Метод для инициализации свойств процесса
        /// </summary>
        private static List<Property> PropertyInitializer(XElement process, XNamespace ns)
        {
            var itemDefinitions = process.Parent.Elements(ns + "itemDefinition");
            var properties = process.Elements(ns + "property").ToList();
            var propertyList = new List<Property>();

            foreach (var property in properties)
            {
                string id = property.Attribute("id").Value;
                string name = property.Attribute("name").Value;
                string itemSubjectRef = property.Attribute("itemSubjectRef").Value;

                string structureRef = itemDefinitions
                    .Where(i => i.Attribute("id").Value == itemSubjectRef)
                    .FirstOrDefault()
                    .Attribute("structureRef")
                    .Value;

                bool isCollection = Convert.ToBoolean(itemDefinitions
                    .Where(i => i.Attribute("id").Value == itemSubjectRef)
                    .FirstOrDefault()
                    .Attribute("isCollection")
                    .Value);
                propertyList.Add(new Property(id, name, structureRef, isCollection));
            }

            return propertyList;
        }

        // Делегат для получения следующих последовательностей узлов
        private readonly Func<XElement, XElement, XNamespace, IEnumerable<XElement>> NextSequences =
            (e, ProcessXML, NS) => ProcessXML
            .Elements(NS + "sequenceFlow")?
            .Where(s => s.Attribute("sourceRef")?.Value == e.Attribute("id").Value);

        // Делегат для получения следующего элемента узла
        private readonly Func<XElement, XElement, IEnumerable<XElement>> NextElement =
            (s, ProcessXML) => ProcessXML.Elements()
            .Where(e => e.Attribute("id").Value == s.Attribute("targetRef")?.Value);
    }
}
