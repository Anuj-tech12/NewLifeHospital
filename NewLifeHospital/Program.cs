using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register MVC
builder.Services.AddControllersWithViews();

// Register SQL Server database
builder.Services.AddDbContext<NewLifeHospital.Data.ApplicationDbContext>(options =>
options.UseSqlServer(
builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Email Service
builder.Services.AddScoped<NewLifeHospital.Services.EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}")
.WithStaticAssets();

app.Run();
