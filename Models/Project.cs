using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Models;

public class Project
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Project name")]
    [MaxLength(64)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Creation date")]
    public DateTime CreationDate { get; init; } = DateTime.Now;

    public bool IsPublic { get; set; }

    // Navigation Properties
    public ICollection<Ticket>? Tickets { get; set; }
    public ICollection<ProjectRole>? ProjectRoles { get; set; }
}