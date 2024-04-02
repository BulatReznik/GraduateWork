namespace YandexTrackerApi.Models.YandexModels
{
    public class User
    {
        public string Self { get; set; } = null!;
        public long Uid { get; set; }
        public string Login { get; set; } = null!;
        public long TrackerUid { get; set; }
        public long PassportUid { get; set; }
        public string CloudUid { get; set; } = null!;
        public string FirstName { get; set; } = null!;  
        public string LastName { get; set; } = null!;
        public string Display { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool External { get; set; }
        public bool HasLicense { get; set; }
        public bool Dismissed { get; set; }
        public bool UseNewFilters { get; set; }
        public bool DisableNotifications { get; set; }
        public DateTime FirstLoginDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public bool WelcomeMailSent { get; set; }
    }
}
