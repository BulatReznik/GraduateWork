namespace BPMN.Models.Project
{
    public class ProjectInviteUserRequest
    {
        /// <summary>
        /// Id проекта
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// Email пользователя которого мы приглашаем на проект
        /// </summary>
        public string Email { get; set; } = null!;
    }
}
