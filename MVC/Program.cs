using LmsServices.Admin.Implmentations;
using LmsServices.Admin.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using MVC.Areas.Administrator.Data;
using MVC.Areas.Identity.Data;
using MVC.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Get the connection string
var connectionString = builder.Configuration.GetConnectionString("AppDbContextConnection")
						?? throw new InvalidOperationException("Connection string 'AppDbContextConnection' not found.");

// Add DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Register Identity services (with role support)
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>options.SignIn.RequireConfirmedAccount = false)
	.AddEntityFrameworkStores<AppDbContext>()
	.AddDefaultTokenProviders();


builder.Services.AddScoped<UserManager<AppUser>>();
builder.Services.AddScoped<RoleManager<IdentityRole>>();
//builder.Services.AddScoped<IHttpContextAccessor, HttpContextAccessor>(); // Required for accessing HttpContext
//builder.Services.AddScoped<UserInfoMiddleware>(); // Ensure middleware is added with scoped service

builder.Services.AddScoped<IBranchService, BranchService>();


builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(20); // Set session timeout
	options.Cookie.HttpOnly = true; // Make the cookie accessible only to the server
	options.Cookie.IsEssential = true; // Make the session cookie essential
});

builder.Services.AddHttpContextAccessor();


builder.Services.ConfigureApplicationCookie(options =>
{
	options.AccessDeniedPath = "/Identity/Account/AccessDenied";
	options.Cookie.Name = "AcademyPro";
	options.Cookie.HttpOnly = true;
	options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
	options.LoginPath = "/Identity/Account/Login";
	// ReturnUrlParameter requires 
	//using Microsoft.AspNetCore.Authentication.Cookies;
	options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
	options.SlidingExpiration = true;

});

builder.Services.AddMemoryCache();


builder.Services.AddStackExchangeRedisCache(options =>
{
	options.Configuration = "localhost:6379"; // Replace with your Redis server connection string
});

// Register Razor Pages (needed for Identity UI)
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

// Add services for MVC controllers and views
builder.Services.AddControllersWithViews();


var app = builder.Build();

// Configure HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();


// Seed roles and users
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		await ApplicationSeeder.SeedRolesAndUsers(services);
	}
	catch (Exception ex)
	{
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred while seeding roles and users.");
	}
}

app.UseStaticFiles(new StaticFileOptions
{
	OnPrepareResponse = ctx =>
	{
		ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=31536000, immutable");
	}
});


// Authentication and Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

// Register custom UserInfoMiddleware
app.UseMiddleware<UserInfoMiddleware>();

app.UseSession(); // Add this line to enable session middleware


// Configure static assets and Razor Pages
app.MapStaticAssets();

// Add this line to specify the path for Identity
app.MapRazorPages(); 


// Top-level route registration for areas
app.MapControllerRoute(
	name: "areas",
	pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);


// Top-level route registration for areas
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}"
);



app.Run();
