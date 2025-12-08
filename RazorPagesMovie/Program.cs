using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add Razor Pages and DbContext
builder.Services.AddRazorPages();
builder.Services.AddDbContext<RazorPagesMovieContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RazorPagesMovieContext") ?? throw new InvalidOperationException("Connection string 'RazorPagesMovieContext' not found.")));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Set to false

    // Simplify passwords for testing
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 4;
})
.AddEntityFrameworkStores<RazorPagesMovieContext>();
// **REMOVED CONFLICTING CUSTOM AUTHENTICATION SETUP**
// (Removed: builder.Services.AddAuthentication("MyCookieAuth").AddCookie(...) )
// (Removed: builder.Services.AddAuthorization() ) 

// Add session services (OK to keep for non-auth data)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthorization(options =>
{
    // Define the 'AdminPolicy'
    options.AddPolicy("AdminPolicy", policy =>
        // This policy requires the logged-in user to have the Role claim set to "Admin"
        policy.RequireRole("Admin")
    );
});


var app = builder.Build();

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}

// Configure middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// **CRITICAL: Authentication and Authorization MUST be here**
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapStaticAssets();
app.MapRazorPages();

app.Run();