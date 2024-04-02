namespace YandexTrackerApi.BusinessLogic.Models.TokenModels
{
    public record RefreshTokenResponseDTO
    {
        public string AccessToken { get; init; } = null!;

        public string? Login { get; init; }

        public string Id { get; init; } = null!;
    }
}
