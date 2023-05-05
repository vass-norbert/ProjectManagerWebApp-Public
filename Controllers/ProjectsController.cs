using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Areas.Identity.Data;
using ProjectManager.Data;
using ProjectManager.Logic;
using ProjectManager.Models;
using ProjectManager.Models.ViewModels;

namespace ProjectManager.Controllers;

public class ProjectsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IdentityDbContext _identity;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly DataAccess _dataAccess;

    public ProjectsController(ApplicationDbContext context, IdentityDbContext identity, UserManager<ApplicationUser> userManager, DataAccess dataAccess)
    {
        _context = context;
        _identity = identity;
        _userManager = userManager;
        _dataAccess = dataAccess;
    }

    // GET: Projects overview view
    [Authorize]
    public IActionResult Overview()
    {
        return View();
    }

    // GET: Create new project view
    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Creates new project
    [HttpPost]
    [Authorize]
    public IActionResult Create(Models.Project project)
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null) { return BadRequest(); }

        project.ProjectRoles = new List<ProjectRole>
        {
            new ProjectRole {
                UserId = userId,
                Role = Role.ProjectOwner,
                ProjectId = project.Id
            }
        };

        if (project.Description != null)
        {
            project.Description = project.Description.Replace("\r", "");
        }

        _context.Add(project);
        _context.SaveChanges();

        return RedirectToAction(nameof(MyProjects));
    }

    // GET: Personal projects view
    [Authorize]
    public IActionResult MyProjects()
    {
        var myProjects = _context.ProjectRoles
            .Where(x => x.UserId == _userManager.GetUserId(User) && x.Role == Role.ProjectOwner)
            .Include(x => x.Project)
            .Select(x => x.Project)
            .ToList()
            .OrderByDescending(x => x.CreationDate);

        return View(myProjects);
    }

    // GET: Assigned projects view
    [Authorize]
    public IActionResult AssignedProjects()
    {
        var assignedProjects = _context.ProjectRoles
            .Where(x => x.UserId == _userManager.GetUserId(User) && (x.Role == Role.Administrator || x.Role == Role.Developer))
            .Include(x => x.Project)
            .Select(x => x.Project)
            .ToList()
            .OrderByDescending(x => x.CreationDate);

        return View(assignedProjects);
    }

    // GET: Project details view
    [Authorize]
    public IActionResult Details(int? projectId)
    {
        if (projectId == null) { return NotFound(); }

        var project = _context.Projects
            .Include(x => x.Tickets)
            .Include(x => x.ProjectRoles)
            .FirstOrDefault(x => x.Id == projectId);

        if (project == null || project.ProjectRoles == null) { return NotFound(); }

        if (!project.ProjectRoles.Any(x => x.UserId == _userManager.GetUserId(User)))
        {
            return Forbid();
        }

        List<UserInfo> users = new();
        foreach (var item in project.ProjectRoles)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == item.UserId);
            if (user == null) { continue; }

            users.Add(
                new UserInfo
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = item.Role,
                });
        }

        if (project.Tickets != null)
        {
            project.Tickets = project.Tickets
                .OrderBy(x => x.Status)
                .ThenByDescending(x => x.Severity)
                .ThenBy(x => x.Name)
                .ToList();
        }

        ProjectDetailsViewModel viewmodel = new()
        {
            Project = project,
            Users = users.OrderBy(x => x.Role).ThenBy(x => x.FullName).ToList(),
            CurrentLoggedInUsersRole = _dataAccess.ProjectAccessRole(projectId),
        };

        return View(viewmodel);
    }

    // GET: Project edit view
    [Authorize]
    public IActionResult Edit(int? projectId)
    {
        if (projectId == null) { return NotFound(); }

        var project = _context.Projects
            .Include(x => x.Tickets)
            .Include(x => x.ProjectRoles)
            .FirstOrDefault(x => x.Id == projectId);

        if (project == null || project.ProjectRoles == null) { return NotFound(); }

        if (project.ProjectRoles.Any(x => x.UserId == _userManager.GetUserId(User) && x.Role == Role.ProjectOwner))
        {
            return View(project);
        }
        else
        {
            return Forbid();
        }
    }

    // POST: Saves changes made to project name and description
    [Authorize]
    [HttpPost]
    public IActionResult Edit(Models.Project project)
    {
        if (project.Description != null)
        {
            project.Description = project.Description.Replace("\r", "");
        }

        _context.Update(project);
        _context.SaveChanges();

        return RedirectToAction(nameof(Details), new { projectId = project.Id });
    }

    // GET: Project delete page
    [Authorize]
    public IActionResult Delete(int? projectId)
    {
        if (projectId == null) { return NotFound(); }

        var project = _context.Projects
            .Include(x => x.ProjectRoles)
            .FirstOrDefault(x => x.Id == projectId);

        if (project == null || project.ProjectRoles == null) { return NotFound(); }

        if (project.ProjectRoles.Any(x => x.UserId == _userManager.GetUserId(User) && (x.Role == Role.ProjectOwner)))
        {
            return View(project);
        }
        else
        {
            return Forbid();
        }
    }

    // POST: Deletes project
    [Authorize]
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var project = _context.Projects
            .FirstOrDefault(x => x.Id == id);

        if (project == null) { return NotFound(); }

        _context.Projects.Remove(project);
        _context.SaveChanges();

        return RedirectToAction(nameof(MyProjects));
    }

    // GET: View to add new users to project and manage already added users
    [Authorize]
    public IActionResult Users(int? projectId)
    {
        if (projectId == null) { return NotFound(); }

        var projectRoles = _context.ProjectRoles.Where(x => x.ProjectId == projectId);

        if (projectRoles == null) { return NotFound(); }

        if (!projectRoles.Any(x => x.UserId == _userManager.GetUserId(User) && (x.Role == Role.ProjectOwner || x.Role == Role.Administrator)))
        {
            return Forbid();
        }

        List<UserInfo> users = new();
        foreach (var item in projectRoles)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == item.UserId);
            if (user == null) { continue; }

            users.Add(
                new UserInfo
                {
                    Id = item.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = item.Role,
                });
        }

        ProjectUsersViewModel viewModel = new()
        {
            ProjectId = (int)projectId,
            CurrentLoggedInUsersRole = _dataAccess.ProjectAccessRole(projectId),
            ProjectName = _context.Projects.Find(projectId).Name,
            Users = users.OrderBy(x => x.Role).ThenBy(x => x.FullName).ToList(),
        };

        return View(viewModel);
    }

    // POST: Add new user to project
    [Authorize]
    [HttpPost]
    public IActionResult AddUser(ProjectUsersViewModel viewModel)
    {
        var project = _context.Projects
            .Include(x => x.ProjectRoles)
            .FirstOrDefault(x => x.Id == viewModel.ProjectId);

        if (project == null || project.ProjectRoles == null) { return NotFound(); }

        if (!project.ProjectRoles.Any(x => x.UserId == _userManager.GetUserId(User) && (x.Role == Role.ProjectOwner || x.Role == Role.Administrator)))
        {
            return Forbid();
        }

        var newUser = _identity.Users.FirstOrDefault(x => x.Email == viewModel.NewUserEmail);

        if (newUser == null)
        {
            TempData["EmailError"] = "This email address does not exist.";
            return RedirectToAction(nameof(Users), new { projectId = viewModel.ProjectId });
        }

        if (project.ProjectRoles.Any(x => x.UserId == newUser.Id))
        {
            TempData["EmailError"] = "User is already added to this project.";
            return RedirectToAction(nameof(Users), new { projectId = viewModel.ProjectId });
        }

        project.ProjectRoles.Add(
            new ProjectRole
            {
                UserId = newUser.Id,
                Role = Role.Developer,
                ProjectId = viewModel.ProjectId
            });

        _context.Update(project);
        _context.SaveChanges();

        return RedirectToAction(nameof(Users), new { projectId = viewModel.ProjectId });
    }

    // GET: View where user role can be set, or user deleted from project
    [Authorize]
    public IActionResult ManageUser(int? projectId, string? userId)
    {
        if (projectId == null || userId == null) { return NotFound(); }

        if (userId == _userManager.GetUserId(User)) { return Forbid(); }

        var projectRole = _context.ProjectRoles
            .Include(x => x.Project)
            .FirstOrDefault(x => x.UserId == userId && x.ProjectId == projectId);

        if (projectRole == null || projectRole.Project == null) { return NotFound(); }

        if (_dataAccess.ProjectAccessRole(projectRole.ProjectId) is not Role.ProjectOwner and not Role.Administrator)
        {
            return Forbid();
        }

        var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);

        ProjectManageUserViewModel viewModel = new()
        {
            projectRole = projectRole,
            userInfo = new UserInfo
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            }
        };

        return View(viewModel);
    }

    // POST: Updates user role in project
    [Authorize]
    [HttpPost]
    public IActionResult UpdateRole(ProjectManageUserViewModel viewModel)
    {
        var projectRole = viewModel.projectRole;

        if (projectRole == null) { return NotFound(); }

        if (_dataAccess.ProjectAccessRole(projectRole.ProjectId) is not Role.ProjectOwner and not Role.Administrator)
        {
            return Forbid();
        }

        if (projectRole.UserId == _userManager.GetUserId(User)) { return Forbid(); }

        _context.ProjectRoles.Update(projectRole);
        _context.SaveChanges();

        return RedirectToAction(nameof(Users), new { projectId = viewModel.projectRole.ProjectId });
    }

    // POST: Deletes user form project
    [Authorize]
    [HttpPost]
    public IActionResult DeleteUserFromProject(ProjectManageUserViewModel viewModel)
    {
        var projectRole = viewModel.projectRole;

        if (projectRole == null || projectRole.UserId == null) { return NotFound(); }

        if (_dataAccess.ProjectAccessRole(projectRole.ProjectId) is not Role.ProjectOwner and not Role.Administrator)
        {
            return Forbid();
        }

        if (projectRole.UserId == _userManager.GetUserId(User)) { return Forbid(); }

        _context.Tickets
            .Where(x => x.ProjectId == viewModel.projectRole.ProjectId && x.AssignedUserId == viewModel.projectRole.UserId)
            .ToList()
            .ForEach(x => { x.AssignedUserId = null; });

        _context.Tickets
            .Where(x => x.ProjectId == viewModel.projectRole.ProjectId && x.OwnerId == viewModel.projectRole.UserId)
            .ToList()
            .ForEach(x => { x.OwnerId = null; });

        _context.ProjectRoles.Remove(projectRole);
        _context.SaveChanges();

        return RedirectToAction(nameof(Users), new
        {
            projectId = viewModel.projectRole.ProjectId
        });
    }

    // POST: Leave project
    [Authorize]
    [HttpPost]
    public IActionResult LeaveProjectFromDetails(ProjectDetailsViewModel viewModel)
    {
        int? projectId = viewModel.Project.Id;
        if (projectId == null) { return NotFound(); }

        var projectRole = _context.ProjectRoles.FirstOrDefault(x => x.ProjectId == projectId && x.UserId == _userManager.GetUserId(User));

        if (projectRole == null || projectRole.UserId == null) { return NotFound(); }

        if (_dataAccess.ProjectAccessRole(projectRole.ProjectId) is Role.ProjectOwner)
        {
            return Forbid();
        }

        _context.Tickets
            .Where(x => x.ProjectId == projectId && x.AssignedUserId == _userManager.GetUserId(User))
            .ToList()
            .ForEach(x => { x.AssignedUserId = null; });

        _context.Tickets
            .Where(x => x.ProjectId == projectId && x.OwnerId == _userManager.GetUserId(User))
            .ToList()
            .ForEach(x => { x.OwnerId = null; });

        _context.ProjectRoles.Remove(projectRole);
        _context.SaveChanges();

        return RedirectToAction(nameof(Overview));
    }

    // POST: Leave project
    [Authorize]
    [HttpPost]
    public IActionResult LeaveProjectFromUsers(ProjectUsersViewModel viewModel)
    {
        int? projectId = viewModel.ProjectId;
        if (projectId == null) { return NotFound(); }

        var projectRole = _context.ProjectRoles.FirstOrDefault(x => x.ProjectId == projectId && x.UserId == _userManager.GetUserId(User));

        if (projectRole == null || projectRole.UserId == null) { return NotFound(); }

        if (_dataAccess.ProjectAccessRole(projectRole.ProjectId) is Role.ProjectOwner)
        {
            return Forbid();
        }

        _context.Tickets
            .Where(x => x.ProjectId == projectId && x.AssignedUserId == _userManager.GetUserId(User))
            .ToList()
            .ForEach(x => { x.AssignedUserId = null; });

        _context.Tickets
            .Where(x => x.ProjectId == projectId && x.OwnerId == _userManager.GetUserId(User))
            .ToList()
            .ForEach(x => { x.OwnerId = null; });

        _context.ProjectRoles.Remove(projectRole);
        _context.SaveChanges();

        return RedirectToAction(nameof(Overview));
    }

}