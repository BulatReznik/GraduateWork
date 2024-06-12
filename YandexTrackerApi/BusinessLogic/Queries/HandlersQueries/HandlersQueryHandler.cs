using MediatR;
using Microsoft.EntityFrameworkCore;
using YandexTrackerApi.BusinessLogic.Models.HandlersQueries;
using YandexTrackerApi.DbModels;
using YandexTrackerApi.BusinessLogic.Models.HandlersModels;

namespace YandexTrackerApi.BusinessLogic.Queries.HandlersQueries
{
    public class HandlersQueryHandler : IRequestHandler<HandlersQuery, Models.ResponseModel<HandlersResponse>>
    {
        private readonly IGraduateWorkContext _context;
        private readonly ILogger<HandlersQueryHandler> _logger;

        public HandlersQueryHandler(IGraduateWorkContext context, ILogger<HandlersQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Models.ResponseModel<HandlersResponse>> Handle(HandlersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var handlers = await _context.TaskHandlerMappings
                    .Select(th => new Handler
                    {
                        HandlerClassName = th.HandlerClassName,
                        NodeName = th.NodeName,
                        Description = th.Description
                    })
                    .ToListAsync(cancellationToken);

                var response = new HandlersResponse
                {
                    Handlers = handlers
                };

                return new Models.ResponseModel<HandlersResponse> { Data = response };
            }
            catch (Exception ex)
            {
                var errorMessage = "Не удалось получить список узлов";
                _logger.LogError(errorMessage, ex);
                return new Models.ResponseModel<HandlersResponse> { ErrorMessage = errorMessage };
            }
        }
    }
}
