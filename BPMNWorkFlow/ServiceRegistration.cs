﻿using BPMNWorkFlow.BusinessLogic.Commands;
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

            services.AddTransient<PostRequestTaskHandler>();
            services.AddTransient<GetRequestTaskHandler>();
            services.AddTransient<PutRequestTaskHandler>();
            services.AddTransient<DeleteRequestTaskHandler>();
            services.AddTransient<ShowTextTaskHandler>();
            services.AddTransient<CompareTaskHandler>();
        }
    }
}
