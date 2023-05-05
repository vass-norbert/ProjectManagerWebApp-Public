namespace ProjectManager.Models.ViewModels;

public class ProjectDetailsViewModel
{
    public Project Project { get; set; }
    public List<UserInfo> Users { get; set; }
    public Role? CurrentLoggedInUsersRole { get; set; }
}
