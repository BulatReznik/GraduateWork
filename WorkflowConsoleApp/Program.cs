
using BPMNWorkFlow;
using BPMNWorkFlow.BusinessLogic.Interfaces;
using BPMNWorkFlow.BusinessLogic.Models;
using System.IO;
using System.Xml.Linq;

namespace WorkflowConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var filepath = "diagram (26).bpmn";
            XDocument xdock = XDocument.Load(File.OpenRead(filepath));

            var p = new Process(xdock);
            var processInstance = p.NewProcessInstance();

            processInstance.SetDefaultHandlers();

            processInstance.SetHandler("task", new MyTaskHandler());

            var processVar = new Dictionary<string, object>() { };
            processInstance.Start(processVar);
            Console.ReadLine();
        }

        private class MyStartHandler : INodeHandler
        {
            public void Execute(ProcessNode currentNode, ProcessNode previousNode)
            {
                Console.WriteLine("Custom Start Handler");
                Console.WriteLine(currentNode.NodeId);
                currentNode.Done();
            }
        }

        private class MyTaskHandler : INodeHandler
        {
            public void Execute(ProcessNode currentNode, ProcessNode previousNode)
            {

                Console.WriteLine("Custom Task Handler");
                Console.WriteLine(currentNode.NodeId);
                Console.WriteLine(currentNode.NodeName);
                currentNode.Done();
            }
        }
    }
}
