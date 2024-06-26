﻿using MediatR;

namespace YandexTrackerApi.BusinessLogic.Models.UserQueries
{
    /// <summary>
    /// Модель проверки списка email на валидность
    /// </summary>
    public record UserCheckMailQuery : IRequest<bool>
    {
        public List<string> MailList { get; init; } = null!;
    }
}
