namespace BPMN.Models.Register
{
    public class RegisterResponseModel
    {
        public string AccessToken { get; init; } = null!;
        public string RefreshToken { get; init; } = null!;
        public string Login { get; init; } = null!;
        public string Id { get; init; } = null!;
    }
}
