using Microsoft.EntityFrameworkCore;
using ProjectManager.Data;
using ProjectManager.Areas.Identity.Data;
using ProjectManager.Logic;

var builder = WebApplication.CreateBuilder(args);

var identityConnectionString = builder.Configuration.GetConnectionString("IdentityDbContextConnection") ?? throw new InvalidOperationException("Connection string 'IdentityDbContextConnection' not found.");
var applicationConnectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

builder.Services.AddDbContext<IdentityDbContext>(options => options.UseSqlite(identityConnectionString));
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(applicationConnectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<IdentityDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<DataAccess>();
builder.Services.AddScoped<SampleDataSeeder>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
