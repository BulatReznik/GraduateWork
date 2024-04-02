using MediatR;
using YandexTrackerApi.BusinessLogic.Managers.JWT;
using YandexTrackerApi.BusinessLogic.Models;
using YandexTrackerApi.BusinessLogic.Models.Enums;
using YandexTrackerApi.BusinessLogic.Models.UserModels;

namespace YandexTrackerApi.BusinessLogic.Queries.UserQueries
{
    public class UserLoginQueryHandler : IRequestHandler<UserLoginQuery, ResponseModel<UserLoginResponseModel>>
    {
        private readonly ILogger _logger;
        private readonly IJWTAuthManager _jwtAuthManager;
        private readonly IMediator _mediator;

        public UserLoginQueryHandler(
            IJWTAuthManager jwtAuthManager,
            IMediator mediator,
            ILogger logger)
        {
            _jwtAuthManager = jwtAuthManager;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ResponseModel<UserLoginResponseModel>> Handle(
          UserLoginQuery query,
          CancellationToken cancellationToken)
        {
            try
            {
                var user = await _mediator.Send(
                    new UserDataQuery
                    {
                        Login = query.Login,
                        Password = query.Password,
                    },
                    cancellationToken);

                if (user == null)
                    return new ResponseModel<UserLoginResponseModel> { ErrorMessage = "Неправильный логин / пароль" };

                var identity = await _mediator.Send(
                   new UserIdentityQuery
                   {
                       Login = user.Login,
                       UserId = user.Id
                   },
                   cancellationToken);

                var accessToken = _jwtAuthManager.GenerateToken(
                    identity.Claims,
                    JwtTokenCommandType.Access);

                var refreshToken = _jwtAuthManager.GenerateToken(
                    identity.Claims,
                    JwtTokenCommandType.Refresh);

                var response = new ResponseModel<UserLoginResponseModel>
                {
                    Data = new UserLoginResponseModel
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        Login = identity.Name,
                        Id = identity.Claims.Last().Value
                    }
                };

                return response;
            }
            catch (Exception ex)
            {
                var errorMessage = "Неправильный логин/пароль";
                _logger.LogError(errorMessage, ex);
                return new ResponseModel<UserLoginResponseModel> { ErrorMessage = errorMessage };
            }
        }
    }
}
