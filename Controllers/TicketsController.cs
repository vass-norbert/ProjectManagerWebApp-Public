using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Areas.Identity.Data;
using ProjectManager.Data;
using ProjectManager.Logic;
using ProjectManager.Models;
using ProjectManager.Models.ViewModels;

namespace ProjectManager.Controllers;

public class TicketsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IdentityDbContext _identity;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly DataAccess _dataAccess;

    public TicketsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, DataAccess DataAccess, IdentityDbContext identity)
    {
        _context = context;
        _identity = identity;
        _userManager = userManager;
        _dataAccess = DataAccess;
    }

    // GET: Create new ticket view
    [Authorize]
    public IActionResult Create(int? projectId)
    {
        if (projectId == null) { return NotFound(); }

        Ticket ticket = new()
        {
            ProjectId = (int)projectId,
            OwnerId = _userManager.GetUserId(User)
        };

        return View(ticket);
    }

    // POST: Create new ticket page POST ACTION
    [Authorize]
    [HttpPost]
    public IActionResult Create(Ticket ticket)
    {
        if (ticket.Description != null)
        {
            ticket.Description = ticket.Description.Replace("\r", "");
        }

        _context.Add(ticket);
        _context.SaveChanges();

        return RedirectToAction("Details", "Projects", new { projectId = ticket.ProjectId });
    }

    // GET: Ticket details page
    [Authorize]
    public IActionResult Details(int? ticketId, int? projectId)
    {
        if (ticketId == null) { return NotFound(); }

        if (_dataAccess.ProjectAccessRole(projectId) == null) { return Forbid(); }

        var ticket = _context.Tickets
            .Include(x => x.Project)
            .FirstOrDefault(x => x.Id == ticketId);

        if (ticket == null) { return NotFound(); }

        var currentLoggedInUser = _userManager.Users.FirstOrDefault(x => x.Id == _userManager.GetUserId(User));
        var ticketAuthor = _userManager.Users.FirstOrDefault(x => x.Id == ticket.OwnerId);

        if (currentLoggedInUser == null) { return NotFound(); }

        TicketDetailsViewModel viewmodel = new()
        {
            Ticket = ticket,
            CurrentLoggedInUser = new UserInfo()
            {
                Id = currentLoggedInUser.Id,
                FirstName = currentLoggedInUser.FirstName,
                LastName = currentLoggedInUser.LastName,
                Email = currentLoggedInUser.Email,
                Role = (Role)_dataAccess.ProjectAccessRole(projectId)
            }
        };

        if (ticketAuthor != null)
        {
            viewmodel.TicketAuthor = new UserInfo()
            {
                Id = ticketAuthor.Id,
                FirstName = ticketAuthor.FirstName,
                LastName = ticketAuthor.LastName,
                Email = ticketAuthor.Email,
                Role = _context.ProjectRoles.FirstOrDefault(x => x.UserId == ticketAuthor.Id).Role
            };
        }

        var assignedUser = _userManager.Users.FirstOrDefault(x => x.Id == ticket.AssignedUserId);
        if (assignedUser != null && _context.ProjectRoles.FirstOrDefault(x => x.UserId == assignedUser.Id) != null)
        {
            viewmodel.AssignedUser = new UserInfo()
            {
                Id = assignedUser.Id,
                FirstName = assignedUser.FirstName,
                LastName = assignedUser.LastName,
                Email = assignedUser.Email,
                Role = _context.ProjectRoles.FirstOrDefault(x => x.UserId == assignedUser.Id).Role
            };
        }

        var projectRoles = _context.ProjectRoles.Where(x => x.ProjectId == projectId);
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

        viewmodel.ProjectUsers = users;

        return View(viewmodel);
    }

    // GET: Ticket edit page
    [Authorize]
    public IActionResult Edit(int? ticketId, int? projectId)
    {
        if (ticketId == null) { return NotFound(); }

        // Check if user has access to project
        if (_dataAccess.ProjectAccessRole(projectId) is Role.ProjectOwner or Role.Administrator)
        {
            var ticket = _context.Tickets.Find(ticketId);

            return View(ticket);
        }
        else
        {
            return NotFound();
        }

    }

    // POST: Ticket edit page POST ACTION
    [Authorize]
    [HttpPost]
    public IActionResult Edit(Ticket ticket)
    {
        if (ticket.Description != null)
        {
            ticket.Description = ticket.Description.Replace("\r", "");
        }

        _context.Update(ticket);
        _context.SaveChanges();

        return RedirectToAction(nameof(Details), new { ticketId = ticket.Id, projectId = ticket.ProjectId });
    }

    // GET: Projects delete page
    [Authorize]
    public IActionResult Delete(int? ticketId, int? projectId)
    {
        if (ticketId == null) { return NotFound(); }

        var accessRole = _dataAccess.ProjectAccessRole(projectId);

        // Check if user has access to project
        if (_dataAccess.ProjectAccessRole(projectId) is Role.Administrator or Role.ProjectOwner)
        {
            var ticket = _context.Tickets.Find(ticketId);

            return View(ticket);
        }
        else
        {
            return NotFound();
        }
    }

    // POST: Projects delete page DELETE POST ACTION
    [Authorize]
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var ticket = _context.Tickets.Find(id);

        if (ticket == null) { return NotFound(); }

        _context.Tickets.Remove(ticket);
        _context.SaveChanges();

        return RedirectToAction("Details", "Projects", new { projectId = ticket.ProjectId });
    }

    // POST: Update ticket status
    [HttpPost]
    public IActionResult UpdateTicketStatus(TicketDetailsViewModel viewModel)
    {
        if ((viewModel.CurrentLoggedInUser.Role is Role.ProjectOwner or Role.Administrator) || viewModel.CurrentLoggedInUser.Id == viewModel.Ticket.AssignedUserId)
        {
            var ticket = _context.Tickets.Find(viewModel.Ticket.Id);
            if (ticket == null) { return NotFound(); }

            if (ticket.Status != viewModel.Ticket.Status)
            {
                ticket.Status = viewModel.Ticket.Status;

                _context.Tickets.Update(ticket);
                _context.SaveChanges();

                TempData["TicketStatusChangeSuccess"] = "Successfully updated ticket status!";
            }
        }
        else
        {
            return Forbid();
        }

        return RedirectToAction(nameof(Details), new { ticketId = viewModel.Ticket.Id, projectId = viewModel.Ticket.ProjectId });
    }

    // POST: Update ticket severity
    [HttpPost]
    public IActionResult UpdateTicketSeverity(TicketDetailsViewModel viewModel)
    {
        if (viewModel.CurrentLoggedInUser.Role is Role.ProjectOwner or Role.Administrator)
        {
            var ticket = _context.Tickets.Find(viewModel.Ticket.Id);
            if (ticket == null) { return NotFound(); }

            if (ticket.Severity != viewModel.Ticket.Severity)
            {
                ticket.Severity = viewModel.Ticket.Severity;

                _context.Tickets.Update(ticket);
                _context.SaveChanges();

                TempData["TicketSeverityChangeSuccess"] = "Successfully updated ticket severity!";
            }
        }
        else
        {
            return Forbid();
        }

        return RedirectToAction(nameof(Details), new { ticketId = viewModel.Ticket.Id, projectId = viewModel.Ticket.ProjectId });
    }

    // POST: Update ticket's assigned user
    [HttpPost]
    public IActionResult AssignUserToTicket(TicketDetailsViewModel viewModel)
    {
        if (viewModel.CurrentLoggedInUser.Role is Role.ProjectOwner or Role.Administrator)
        {
            var ticket = _context.Tickets.Find(viewModel.Ticket.Id);
            if (ticket == null) { return NotFound(); }

            if (ticket.AssignedUserId != viewModel.Ticket.AssignedUserId)
            {
                ticket.AssignedUserId = viewModel.Ticket.AssignedUserId;

                _context.Tickets.Update(ticket);
                _context.SaveChanges();

                TempData["TicketAssignedUserChangeSuccess"] = "Successfully updated ticket's assigned user!";
            }
        }
        else
        {
            return Forbid();
        }

        return RedirectToAction(nameof(Details), new { ticketId = viewModel.Ticket.Id, projectId = viewModel.Ticket.ProjectId });
    }

    // GET: User's assigned tickets list page
    public IActionResult AssignedTickets()
    {
        var tickets = _context.Tickets
            .Where(x => x.AssignedUserId == _userManager.GetUserId(User))
            .OrderBy(x => x.Status)
            .ThenByDescending(x => x.Severity)
            .ThenBy(x => x.Name)
            .ToList();

        return View(tickets);
    }

    // GET: Page of user created tickets
    public IActionResult PersonalTickets()
    {
        var tickets = _context.Tickets
            .Where(x => x.OwnerId == _userManager.GetUserId(User))
            .OrderBy(x => x.Status)
            .ThenByDescending(x => x.Severity)
            .ThenBy(x => x.Name)
            .ToList();

        return View(tickets);
    }
}