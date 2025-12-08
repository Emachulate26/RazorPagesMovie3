using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// --------------------------
// Add services to the container
// --------------------------

// Razor Pages
builder.Services.AddRazorPages();

// DbContext
builder.Services.AddDbContext<RazorPagesMovieContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RazorPagesMovieContext")
    ?? throw new InvalidOperationException("Connection string 'RazorPagesMovieContext' not found.")));

// Identity Configuration
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()              // <-- Enable roles
    .AddEntityFrameworkStores<RazorPagesMovieContext>();

// Session (optional)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// --------------------------
// Seed Admin and Roles
// --------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Call SeedData to create roles and default users
    RazorPagesMovie.Data.SeedData.Initialize(services);

}

// --------------------------
// Configure middleware pipeline
// --------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();   // <-- Must be before UseAuthorization
app.UseAuthorization();

app.UseSession();

app.MapStaticAssets();
app.MapRazorPages();

app.Run();
