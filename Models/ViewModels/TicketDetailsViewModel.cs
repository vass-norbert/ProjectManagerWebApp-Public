namespace ProjectManager.Models.ViewModels
{
    public class TicketDetailsViewModel
    {
        public Ticket Ticket { get; set; }
        public UserInfo? AssignedUser { get; set; }
        public UserInfo TicketAuthor { get; set; }
        public UserInfo CurrentLoggedInUser { get; set; }
        public List<UserInfo> ProjectUsers { get; set; }
    }
}
