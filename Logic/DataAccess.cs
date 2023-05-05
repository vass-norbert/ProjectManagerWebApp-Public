using ProjectManager.Data;
using ProjectManager.Models;
using System.Security.Claims;

namespace ProjectManager.Logic;

public class DataAccess
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DataAccess(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets the current user's id.
    /// </summary>
    public string? GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
    }

    /// <summary>
    /// Gets the role that the current logged in user has in the project in question.
    /// </summary>
    /// <returns>
    /// Either a Role enum value, or null if the given projectId equals null or if the user doesn't have any access to the project.
    /// </returns>
    public Role? ProjectAccessRole(int? projectId)
    {
        if (projectId == null) { return null; }

        var projectRole = _context.ProjectRoles.FirstOrDefault(x => x.ProjectId == projectId && x.UserId == GetCurrentUserId());

        if (projectRole != null)
        {
            return projectRole.Role;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Returns the number of tickets the current logged in user has assigned to them that are not closed.
    /// </summary>
    public int AssignedActiveTickets()
    {
        return _context.Tickets.Count(x => x.AssignedUserId == GetCurrentUserId() && x.Status != Status.Closed);
    }

    /// <summary>
    /// Returns the number of tickets the current logged in user has assigned to them that are not closed and have the specified status.
    /// </summary>
    public int AssignedActiveTickets(Severity severity)
    {
        return _context.Tickets.Count(x => x.AssignedUserId == GetCurrentUserId() && x.Status != Status.Closed && x.Severity == severity);
    }
}
