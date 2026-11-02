using NewLifeHospital.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register MVC
builder.Services.AddControllersWithViews();
// ASP.NET Core Identity Configuration
builder.Services.AddIdentity<Microsoft.AspNetCore.Identity.IdentityUser,
    Microsoft.AspNetCore.Identity.IdentityRole>(options =>
    {
        // Password Security
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;

        // Account Lockout Security
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);
    })
.AddEntityFrameworkStores<NewLifeHospital.Data.ApplicationDbContext>()
.AddDefaultTokenProviders();
// Register SQL Server database
builder.Services.AddDbContext<NewLifeHospital.Data.ApplicationDbContext>(options =>
options.UseSqlServer(
builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Email Service
builder.Services.AddScoped<NewLifeHospital.Services.EmailService>();

var app = builder.Build();
await IdentitySeeder.SeedAsync(
    app.Services,
    app.Configuration
);
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}")
.WithStaticAssets();

app.Run();
