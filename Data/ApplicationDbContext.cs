using Microsoft.EntityFrameworkCore;
using ProjectManager.Models;

namespace ProjectManager.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<ProjectRole> ProjectRoles { get; set; }
}