using MediatR;
using BPMNWorkFlow.BusinessLogic.Models;

namespace YandexTrackerApi.BusinessLogic.Models.HandlersQueries
{
    public class HandlersQuery : IRequest<ResponseModel<HandlersResponse>>
    {
    }
}
