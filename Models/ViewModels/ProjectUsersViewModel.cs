namespace ProjectManager.Models.ViewModels
{
    public class ProjectUsersViewModel
    {
        public List<UserInfo> Users { get; set; }
        public int ProjectId { get; set; }
        public Role? CurrentLoggedInUsersRole { get; set; }
        public string? ProjectName{ get; set; }
        public string? NewUserEmail { get; set; }
    }
}
