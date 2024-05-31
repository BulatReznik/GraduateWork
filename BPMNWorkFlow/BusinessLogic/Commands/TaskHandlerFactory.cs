using BPMNWorkFlow.BusinessLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMNWorkFlow.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BPMNWorkFlow.BusinessLogic.Commands
{
    public class TaskHandlerFactory : ITaskHandlerFactory
    {
        private readonly GraduateWorkContext _context;
        private readonly IServiceProvider _serviceProvider;

        public TaskHandlerFactory(GraduateWorkContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }

        public async Task<ITaskHandler> GetTaskHandlerAsync(string nodeName)
        {
            var mapping = await _context.TaskHandlerMappings
                .FirstOrDefaultAsync(m => m.NodeName == nodeName);

            if (mapping == null)
            {
                throw new InvalidOperationException($"Неизвестная задача: {nodeName}");
            }

            var handlerType = Type.GetType($"BPMNWorkFlow.BusinessLogic.Commands.NodeNameCommands.{mapping.HandlerClassName}, BPMNWorkFlow");

            if (handlerType == null)
            {
                throw new InvalidOperationException($"Не удалось найти класс обработчика: {mapping.HandlerClassName}");
            }

            var handler = _serviceProvider.GetRequiredService(handlerType) as ITaskHandler;
            if (handler == null)
            {
                throw new InvalidOperationException($"Не удалось создать экземпляр обработчика: {handlerType.Name}");
            }

            return handler;
        }
    }
}
