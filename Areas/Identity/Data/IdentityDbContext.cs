using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectManager.Areas.Identity.Data;

public class IdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var hasher = new PasswordHasher<ApplicationUser>();

        builder.Entity<ApplicationUser>().HasData(
            new ApplicationUser
            {
                Id = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                FirstName = "John",
                LastName = "Smith",
                UserName = "JohnSmith",
                NormalizedUserName = "JOHNSMITH",
                Email = "JohnSmith@generated.com",
                NormalizedEmail = "JOHNSMITH@GENERATED.COM",
                PasswordHash = hasher.HashPassword(null, "JOHNSMITH647")
            },
            new ApplicationUser
            {
                Id = "9489fe22-5f52-446e-a5e7-c62fe2467ce8",
                FirstName = "Vass",
                LastName = "Norbert",
                UserName = "VassNorbert",
                NormalizedUserName = "VASSNORBERT",
                Email = "vassnorbert22@gmail.com",
                NormalizedEmail = "VASSNORBERT22@GMAIL.COM",
                PasswordHash = hasher.HashPassword(null, "VASSNORBERT8564")
            },
            new ApplicationUser
            {
                Id = "320486f4-031b-4e90-a0d1-e17c14ddeed6",
                FirstName = "Ralph",
                LastName = "Hensley",
                UserName = "RalphHensley",
                NormalizedUserName = "RALPHHENSLEY",
                Email = "RalphHensley@generated.com",
                NormalizedEmail = "RALPHHENSLEY@GENERATED.COM",
                PasswordHash = hasher.HashPassword(null, "RALPHHENSLEY796")
            }
        );

        builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
    }
}

public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.FirstName).HasMaxLength(255);
        builder.Property(x => x.LastName).HasMaxLength(255);
    }
}