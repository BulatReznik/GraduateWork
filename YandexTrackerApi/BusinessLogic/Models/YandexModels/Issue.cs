namespace YandexTrackerApi.Models.YandexModels
{
    public class Issue
    {
        public string Self { get; set; } = null!;
        public string Key { get; set; } = null!;
        public int Version { get; set; }
        public string Summary { get; set; } = null!;
        public DateTime StatusStartTime { get; set; }
        public User UpdatedBy { get; set; } = null!;
        public StatusType StatusType { get; set; } = null!;

        public Project Project { get; set; } = null!;
        public string Description { get; set; } = null!;
        public List<Board> Boards { get; set; } = null!;
        public Type Type { get; set; } = null!;
        public Priority Priority { get; set; } = null!;
        public PreviousStatusLastAssignee PreviousStatusLastAssignee { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public List<User> Followers { get; set; } = null!;
        public User CreatedBy { get; set; } = null!;
        public int CommentWithoutExternalMessageCount { get; set; }
        public int Votes { get; set; }
        public int CommentWithExternalMessageCount { get; set; }
        public Assignee Assignee { get; set; } = null!;
        public DateTime Deadline { get; set; }
        public Queue Queue { get; set; } = null!;
        public DateTime UpdatedAt { get; set; }
        public Status Status { get; set; } = null!;
        public PreviousStatus PreviousStatus { get; set; } = null!;
        public bool Favorite { get; set; }

        public string Id { get; set; } = null!;
        public string OriginalEstimation { get; set; }
        public string Spent { get; set; } = null!;

        public DateOnly Start { get; set; }
        public DateOnly End { get; set; }
    }
}
