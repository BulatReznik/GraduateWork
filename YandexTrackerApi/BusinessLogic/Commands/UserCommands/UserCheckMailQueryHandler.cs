using MediatR;
using YandexTrackerApi.BusinessLogic.Models.UserModels;

namespace YandexTrackerApi.BusinessLogic.Commands.UserCommands
{
    /// <summary>
    /// Проверка списка email на валидность
    /// </summary>
    public class UserCheckMailQueryHandler : IRequestHandler<UserCheckMailQuery, bool>
    {
        private readonly ILogger _logger;

        public UserCheckMailQueryHandler(ILogger logger)
        {
            _logger = logger;
        }

        public Task<bool> Handle(UserCheckMailQuery query, CancellationToken cancellationToken)
        {
            bool isValid = true;

            foreach (var mail in query.MailList)
            {
                var trimmedEmail = mail.Trim();

                if (trimmedEmail.EndsWith("."))
                    return Task.FromResult(false);

                try
                {
                    var addr = new System.Net.Mail.MailAddress(mail);
                    isValid &= addr.Address == trimmedEmail;
                }
                catch (Exception ex)
                {
                    _logger.LogError (ex, $"Ошибка проверки email: {mail}");
                    isValid &= false;
                }

                if (!isValid) break;
            }

            return Task.FromResult(isValid);
        }
    }
}
