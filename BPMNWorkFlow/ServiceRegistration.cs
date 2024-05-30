using BPMNWorkFlow.BusinessLogic.Commands;
using BPMNWorkFlow.BusinessLogic.Commands.NodeNameCommands;
using BPMNWorkFlow.BusinessLogic.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BPMNWorkFlow
{
    public static class ServiceRegistration
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<ITaskHandlerFactory, TaskHandlerFactory>();
            services.AddTransient<LoadDataTaskHandler>();
            services.AddTransient<LoginTaskHandler>();
        }
    }
}
