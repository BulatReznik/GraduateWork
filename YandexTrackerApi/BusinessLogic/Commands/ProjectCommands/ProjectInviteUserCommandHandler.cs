using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using YandexTrackerApi.BusinessLogic.Models;
using YandexTrackerApi.BusinessLogic.Models.ProjectModels;
using YandexTrackerApi.DbModels;

namespace YandexTrackerApi.BusinessLogic.Commands.ProjectCommands
{
    public class ProjectInviteUserCommandHandler : IRequestHandler<ProjectInviteUserCommand, ResponseModel<string>>
    {
        private readonly IGraduateWorkContext _context;
        private readonly ILogger _logger;

        public ProjectInviteUserCommandHandler(ILogger logger, IGraduateWorkContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<ResponseModel<string>> Handle(ProjectInviteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Random random = new();
                var inviteCode = random.Next(1000, 9999);

                var access = await _context
                    .UsersProjects
                    .AnyAsync(up => up.UserId == request.UserId 
                                    && up.ProjectId == request.ProjectId
                                    && up.Condirmed == true
                        , cancellationToken);

                if (access == false)
                {
                    return new ResponseModel<string>() { ErrorMessage = "У пользователя нет доступа к этому проекту" };
                }

                var project = await _context
                    .Projects
                    .FirstOrDefaultAsync(p => p.Id == request.ProjectId
                        , cancellationToken);

                var user = await _context
                    .Users
                    .FirstOrDefaultAsync(u => u.Login == request.Email
                        , cancellationToken);

                if (user == null)
                {
                    return new ResponseModel<string>() { ErrorMessage = "Не удалось найти пользователя с таким email" };
                }

                var userProjectExist = await _context.UsersProjects.AnyAsync(up =>
                    up.UserId == user.Id && up.ProjectId == request.ProjectId
                    , cancellationToken);

                if (userProjectExist)
                {
                    return new ResponseModel<string>() { ErrorMessage = "Пользователь уже был добавлен в проект" };
                }

                var userProject = new UsersProject()
                {
                    UserId = user.Id,
                    ProjectId = request.ProjectId,
                    Condirmed = false,
                    InviteCode = inviteCode
                };

                await _context.UsersProjects
                    .AddAsync(userProject
                        , cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // Генерируем текст сообщения для отправки на почту
                var messageText = GenerateMessageEmailText(inviteCode, project.Name);

                // Отправляем приглашение на почту
                var isSent = await SendInviteToEmail(request.Email, messageText);

                return isSent ? new ResponseModel<string>() { Data = "Пользователю выслано приглашение" }
                    : new ResponseModel<string>() { ErrorMessage = "Не удалось отправить приглашение на почту" };
            }
            catch (Exception e)
            {
                var errorMessage = "Не удалось пригласить пользователя на проект";
                _logger.LogError(errorMessage);
                return new ResponseModel<string>() { ErrorMessage = errorMessage };
            }
        }

        /// <summary>
        /// Метод для генерации текста сообщения, который отправляется на почту для приглашения пользователей
        /// </summary>
        private string GenerateMessageEmailText(int inviteCode, string projectName) =>
            // Генерация HTML-сообщения для отправки на почту
            $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Приглашение в проект</title>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #f8f9fa;
                            color: #333;
                        }}
                        .container {{
                            padding: 20px;
                            border-radius: 5px;
                            background-color: #fff;
                            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                        }}
                        h1 {{
                            color: #007bff;
                        }}
                        p {{
                            margin-bottom: 20px;
                        }}
                        .invite-code {{
                            font-size: 18px;
                            font-weight: bold;
                            color: #007bff;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h1>Приглашение в проект</h1>
                        <p>Вы получили приглашение присоединиться к проекту <strong>'{projectName}'</strong>.</p>
                        <p>Используйте следующий код приглашения для вступления в проект: <span class='invite-code'>{inviteCode}</span></p>
                    </div>
                </body>
                </html>
            ";


        /// <summary>
        /// Отправляет сообщение на почту пользователя
        /// </summary>
        private async Task<bool> SendInviteToEmail(string userEmail, string messageText)
        {

            // Адрес электронной почты и пароль от учетной записи отправителя
            string senderEmail = "73bulat73@gmail.com";
            string password = "kkke qsyx pmnp jbuf";

            // Адрес вашего SMTP-сервера и порт
            const string smtpServer = "smtp.gmail.com";
            const int port = 587; // порт для отправки почты (обычно 587 для SMTP с TLS)
            try
            {
                // Создание сообщения
                MailMessage mailMessage = new(new MailAddress(senderEmail), new MailAddress(userEmail))
                {
                    Body = messageText,
                    IsBodyHtml = true,
                    Sender = new MailAddress(senderEmail),
                    Subject = "Приглашение в проект"
                };

                // Создание клиента SMTP и настройка безопасного подключения
                SmtpClient smtpClient = new(smtpServer, port)
                {
                    Credentials = new NetworkCredential(senderEmail, password),
                    EnableSsl = true
                };

                // Отправка сообщения
                await smtpClient.SendMailAsync(mailMessage);

                // Сообщение успешно отправлено
                return true;
            }
            catch (Exception ex)
            {
                // Если произошла ошибка при отправке сообщения, логируем её и возвращаем false
                _logger.LogError($"Ошибка при отправке приглашения на почту {userEmail}: {ex.Message}");
                return false;
            }
        }
    }
}
