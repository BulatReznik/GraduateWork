using BPMNWorkFlow.DbModels;
using Microsoft.EntityFrameworkCore;

namespace BPMNWorkFlow.BusinessLogic.Repository
{
    public class TaskHandlerRepository : ITaskHandlerRepository
    {
        private readonly GraduateWorkContext _context;

        public TaskHandlerRepository(GraduateWorkContext context)
        {
            _context = context;
        }

        public async Task<string> GetHandlerClassNameAsync(string nodeName)
        {
            var mapping = await _context.TaskHandlerMappings
                .FirstOrDefaultAsync(m => m.NodeName == nodeName);

            return mapping?.HandlerClassName;
        }
    }
}
