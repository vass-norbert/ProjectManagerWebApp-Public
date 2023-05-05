using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Models;

public enum Role
{
    [Display(Name = "Project Owner")] ProjectOwner,
    Administrator,
    Developer
}

public class ProjectRole
{
    [Key]
    public int Id { get; set; }

    // User info
    [Required]
    public string UserId { get; set; }

    // Role
    public Role Role { get; set; }

    // Navigation Properties
    public int ProjectId { get; set; }
    public Project Project { get; set; }
    public List<Ticket> Tickets { get; set; }
}