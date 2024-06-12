using MediatR;
using YandexTrackerApi.BusinessLogic.Models.HandlersQueries;
using YandexTrackerApi.DbModels;

namespace YandexTrackerApi.BusinessLogic.Queries.HandlersQueries
{
    public class HandlersQueryHandler : IRequestHandler<HandlersQuery, Models.ResponseModel<HandlersResponse>>
    {
        private readonly IGraduateWorkContext _context;
        private readonly ILogger _logger;

        public HandlersQueryHandler(IGraduateWorkContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Models.ResponseModel<HandlersResponse>> Handle(HandlersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _context.
            }
            catch (Exception ex)
            {
                var errorMessage = "Не удалось получить список узлов";
                _logger.LogError(errorMessage, ex);
                return new Models.ResponseModel<HandlersResponse> { ErrorMessage = errorMessage };
            }
        }
    }
