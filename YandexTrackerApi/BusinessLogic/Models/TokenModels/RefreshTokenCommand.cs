using MediatR;
using System.ComponentModel.DataAnnotations;
using YandexTrackerApi.BusinessLogic.Models;

namespace YandexTrackerApi.BusinessLogic.Models.TokenModels
{
    public class RefreshTokenCommand : IRequest<ResponseModel<RefreshTokenResponseDTO>>
    {
        [Required(ErrorMessage = "Поле TokenHash должно быть заполнено")]
        public string TokenHash { get; init; } = null!;
    }
}
