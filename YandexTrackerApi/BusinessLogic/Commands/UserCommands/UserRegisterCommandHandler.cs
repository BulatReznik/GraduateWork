using MediatR;
using YandexTrackerApi.BusinessLogic.Models.UserModels;
using YandexTrackerApi.BusinessLogic.Models;
using YandexTrackerApi.BusinessLogic.Managers.JWT;
using YandexTrackerApi.AppLogic.Core;
using Microsoft.Extensions.Options;
using YandexTrackerApi.DbModels;
using Microsoft.EntityFrameworkCore;
using YandexTrackerApi.BusinessLogic.Models.Enums;

namespace YandexTrackerApi.BusinessLogic.Commands.UserCommands
{
    public class UserRegisterCommandHandler : IRequestHandler<UserRegisterCommand, ResponseModel<UserRegisterResponseDTO>>
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IJWTAuthManager _jwtAuthManager;
        private readonly IGraduateWorkContext _context;
        private readonly AppConfig _appConfig;

        public UserRegisterCommandHandler(
            IMediator mediator,
            ILogger logger,
            IJWTAuthManager jwtAuthManager,
            IGraduateWorkContext context,
            IOptions<AppConfig> options
            )
        {
            _mediator = mediator;
            _logger = logger;
            _jwtAuthManager = jwtAuthManager;
            _context = context;
            _appConfig = options.Value;
        }
        public async Task<ResponseModel<UserRegisterResponseDTO>> Handle(UserRegisterCommand command, CancellationToken cancellationToken)
        {
            var emailValid = await _mediator.Send(new UserCheckMailQuery { MailList = [command.Login] }, cancellationToken);

            if (!emailValid)
                return new ResponseModel<UserRegisterResponseDTO> { ErrorMessage = "Ошибка валидации email" };

            try
            {
                var existingUser = await _context.User
                        .AnyAsync(u => u.Login == command.Login
                        , cancellationToken: cancellationToken);

                if (existingUser)
                    return new ResponseModel<UserRegisterResponseDTO> { ErrorMessage = "Данный логин уже занят" };

                else
                {
                    var userDbModel = new User
                    {
                        Id = Guid.NewGuid(),
                        Name = command.Name,
                        Login = command.Login,
                        Password = command.Password
                    };

                    await _context.User.AddAsync(userDbModel, cancellationToken);

                    await _context.SaveChangesAsync(cancellationToken);

                    // Генерация access и refresh токенов
                    var identity = await _mediator.Send(new UserIdentityQuery
                    {
                        Login = command.Login,
                        UserId = userDbModel.Id
                    }
                    , cancellationToken);
                    var accessToken = _jwtAuthManager.GenerateToken(identity.Claims, JwtTokenCommandType.Access);
                    var refreshToken = _jwtAuthManager.GenerateToken(identity.Claims, JwtTokenCommandType.Refresh);

                    return new ResponseModel<UserRegisterResponseDTO>
                    {
                        Data = new UserRegisterResponseDTO
                        {
                            AccessToken = accessToken,
                            RefreshToken = refreshToken,
                            Id = identity.Claims.Last().Value,
                            Login = identity.Name
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "Ошибка регистрации";
                _logger.LogError(ex, errorMessage);
                return new ResponseModel<UserRegisterResponseDTO> { ErrorMessage = errorMessage };
            }
        }
    }
}
