namespace BPMNWorkFlow.BusinessLogic.Models
{
    public class Globals(IDictionary<string, object> parameters)
    {
        public IDictionary<string, object> globals = parameters;
    }
}