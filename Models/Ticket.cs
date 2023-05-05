using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Models;

public enum Status
{
    Open,
    [Display(Name = "In Progress")] InProgress,
    Closed
}

public enum Severity
{
    Low = 0,
    Medium = 1,
    High = 2,
}

public class Ticket
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Ticket title")]
    [MaxLength(124)]
    public string Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Creation date")]
    public DateTime CreationDate { get; init; } = DateTime.Now;

    [Required]
    public Status Status { get; set; }

    [Required]
    public Severity Severity { get; set; }

    public string? OwnerId { get; set; }

    public string? AssignedUserId { get; set; }

    // Navigation Properties
    public int ProjectId { get; set; }
    public Project Project { get; set; }
}