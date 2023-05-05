using ProjectManager.Models;

namespace ProjectManager.Data
{
    public class SampleDataSeeder
    {
        private readonly ApplicationDbContext _dbContext;

        public SampleDataSeeder(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void EnsureSeedData(string userId)
        {
            if (userId != null)
            {
                var seedProjects = new List<Project>
                    {
                        new Project
                        {
                            Name = "Project Manager ASP.NET web application",
                            Description = "An ASP.NET Core, MVC CRUD web application for managing projects and assigning tickets to different users working on the same project.",
                            CreationDate = DateTime.Now
                        },
                        new Project
                        {
                            Name = "To Do list phone application",
                            Description = "A to-do list application for both android and ios devices.",
                            CreationDate = DateTime.Now
                        }
                    };

                _dbContext.Projects.AddRange(seedProjects);
                _dbContext.SaveChanges();

                var seedProjectRoles = new List<ProjectRole>
                    {
                        new ProjectRole {ProjectId = seedProjects[0].Id, Role = Role.ProjectOwner, UserId = "9489fe22-5f52-446e-a5e7-c62fe2467ce8"}, // Vass Norbert
                        new ProjectRole {ProjectId = seedProjects[0].Id, Role = Role.Developer, UserId = "8e445865-a24d-4543-a6c6-9443d048cdb9"}, // John Smith
                        new ProjectRole {ProjectId = seedProjects[0].Id, Role = Role.Developer, UserId = "320486f4-031b-4e90-a0d1-e17c14ddeed6"}, // Ralph Hensley
                        new ProjectRole {ProjectId = seedProjects[0].Id, Role = Role.Administrator, UserId = userId }, // User

                        new ProjectRole {ProjectId = seedProjects[1].Id, Role = Role.Administrator, UserId = "9489fe22-5f52-446e-a5e7-c62fe2467ce8"}, // Vass Norbert
                        new ProjectRole {ProjectId = seedProjects[1].Id, Role = Role.ProjectOwner, UserId = userId}, // User
                        new ProjectRole {ProjectId = seedProjects[1].Id, Role = Role.Developer, UserId = "8e445865-a24d-4543-a6c6-9443d048cdb9"}, // John Smith
                        new ProjectRole {ProjectId = seedProjects[1].Id, Role = Role.Developer, UserId = "320486f4-031b-4e90-a0d1-e17c14ddeed6"}, // Ralph Hensley
                    };

                var seedTickets = new List<Ticket>
                    {
                        new Ticket
                        {
                            ProjectId = seedProjects[0].Id,
                            Name = "Create models",
                            Description = "Create the models for Projects and Tickets",
                            CreationDate = DateTime.Now,
                            Status = Status.Closed,
                            Severity = Severity.High,
                            OwnerId = "9489fe22-5f52-446e-a5e7-c62fe2467ce8",
                            AssignedUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9"
                        },
                        new Ticket
                        {
                            ProjectId = seedProjects[0].Id,
                            Name = "Write the controllers for CRUD capabilities",
                            Description = "Write the controllers for Projects and Tickets to be able to Create, Read, Update and Delete tickets and projects to and form the database.",
                            CreationDate = DateTime.Now,
                            Status= Status.Closed,
                            Severity = Severity.High,
                            OwnerId = "320486f4-031b-4e90-a0d1-e17c14ddeed6",
                            AssignedUserId = "9489fe22-5f52-446e-a5e7-c62fe2467ce8"
                        },
                        new Ticket
                        {
                            ProjectId = seedProjects[0].Id,
                            Name = "Frontend for CRUD views",
                            Description = "With css and bootstrap, redesign the views responsible for CRUD operations so they're more usable and look better.",
                            CreationDate = DateTime.Now,
                            Status = Status.Closed,
                            Severity = Severity.Low,
                            OwnerId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                            AssignedUserId = "320486f4-031b-4e90-a0d1-e17c14ddeed6"
                        },
                        new Ticket
                        {
                            ProjectId = seedProjects[0].Id,
                            Name = "Views for non-CRUD operations",
                            Description = "Create views for pages that help the user with navigation and oversee data about all currently assigned tickets that are under their name.",
                            CreationDate = DateTime.Now,
                            Status = Status.Closed,
                            Severity = Severity.Medium,
                            OwnerId = "9489fe22-5f52-446e-a5e7-c62fe2467ce8",
                            AssignedUserId = "9489fe22-5f52-446e-a5e7-c62fe2467ce8"
                        },
                        new Ticket
                        {
                            ProjectId = seedProjects[0].Id,
                            Name = "Seed sample projects for newly registered users",
                            Description = "Seed the application database at every new register with pre-generated sample projects, so that new users can try out the website's features without having to make multiple accounts to add users to their projects.",
                            CreationDate = DateTime.Now,
                            Status = Status.Closed,
                            Severity = Severity.Medium,
                            OwnerId = "9489fe22-5f52-446e-a5e7-c62fe2467ce8",
                            AssignedUserId = "9489fe22-5f52-446e-a5e7-c62fe2467ce8"
                        },
                        new Ticket
                        {
                            ProjectId = seedProjects[0].Id,
                            Name = "Publish the finished website",
                            Description = "Publish the finished website with Microsoft Azure",
                            CreationDate = DateTime.Now,
                            Status = Status.InProgress,
                            Severity = Severity.High,
                            OwnerId = "9489fe22-5f52-446e-a5e7-c62fe2467ce8",
                            AssignedUserId = "9489fe22-5f52-446e-a5e7-c62fe2467ce8"
                        },
                        new Ticket
                        {
                            ProjectId = seedProjects[0].Id,
                            Name = "Try out the website and test it's features",
                            Description = "Test out the website and it's features, try creating projects, tickets, adding and removing users to and form them.",
                            CreationDate = DateTime.Now,
                            Status = Status.Open,
                            Severity = Severity.Medium,
                            OwnerId = "9489fe22-5f52-446e-a5e7-c62fe2467ce8",
                            AssignedUserId = userId
                        },

                        // Second project's tickets
                        new Ticket
                        {
                            ProjectId = seedProjects[1].Id,
                            Name = "Add more users to project",
                            Description = "Add more users to this project to test out website functionality",
                            CreationDate = DateTime.Now,
                            Status = Status.Closed,
                            Severity = Severity.Low,
                            OwnerId = userId,
                            AssignedUserId = "9489fe22-5f52-446e-a5e7-c62fe2467ce8"
                        },
                        new Ticket
                        {
                            ProjectId = seedProjects[1].Id,
                            Name = "Delete some users",
                            Description = "There were accidently added too many users to the project, try out removing some of them!",
                            CreationDate = DateTime.Now,
                            Status = Status.Open,
                            Severity = Severity.Low,
                            OwnerId = "9489fe22-5f52-446e-a5e7-c62fe2467ce8",
                            AssignedUserId = userId
                        }
                    };

                _dbContext.ProjectRoles.AddRange(seedProjectRoles);
                _dbContext.Tickets.AddRange(seedTickets);
                _dbContext.SaveChanges();

                //_dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT ProjectManagerApplicationDb.dbo.Tickets OFF;");
            }
        }
    }

}
