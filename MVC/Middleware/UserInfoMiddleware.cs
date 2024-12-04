using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MVC.Areas.Identity.Data;
using MVC.Models;

namespace MVC.Middleware
{
	public class UserInfoMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IMemoryCache _memoryCache;
		private readonly UserManager<AppUser> _userManager;
		private readonly IServiceScopeFactory _serviceScopeFactory;

		//public UserInfoMiddleware(RequestDelegate next, IMemoryCache memoryCache)
		//{
		//	_next = next;
		//	_memoryCache = memoryCache;
		//}



		// Constructor to inject dependencies
		public UserInfoMiddleware(RequestDelegate next, IMemoryCache memoryCache, IServiceScopeFactory serviceScopeFactory)
		{
			_next = next;
			_memoryCache = memoryCache;
			_serviceScopeFactory = serviceScopeFactory;
		}

		// InvokeAsync method processes the HTTP request

		// InvokeAsync method processes the HTTP request
		public async Task InvokeAsync(HttpContext httpContext)
		{
			if (httpContext.User.Identity.IsAuthenticated)
			{
				var userName = httpContext.User.Identity.Name;

				// Try to get user info from memory cache
				var currentUser = _memoryCache.Get<CurrentUser>($"User_{userName}");

				if (currentUser == null)
				{
					// Create a scope to resolve scoped services
					using (var scope = _serviceScopeFactory.CreateScope())
					{
						var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();


						var user = await userManager.Users
							.Include(u => u.Firm) // Include the Firm entity
							.Include(u => u.Branch) // Include the Branch entity
							.FirstOrDefaultAsync(u => u.UserName == userName); // Query user by UserName

						//var user = await userManager.FindByNameAsync(userName);

						if (user != null)
						{
							var roles = await userManager.GetRolesAsync(user);

							currentUser = new CurrentUser
							{
								Id = user.Id,
								Name = user.Name,
								Email = user.Email,
								Roles = string.Join(",", roles),
								FirmId = user.FirmId,
								FirmName = user.Firm?.FirmName ?? "No Firm",
								FirmShortName = user.Firm?.FirmShortName ?? "NA",
								BranchId = user.BranchId,
								BranchName = user.Branch?.BranchName ?? "No Branch"
							};

							// Cache the user info for future requests
							_memoryCache.Set($"User_{userName}", currentUser, TimeSpan.FromMinutes(30));
						}
					}
				}

				// Store user information in HttpContext.Items for easy access in views
				if (currentUser != null)
				{
					httpContext.Items["CurrentUser"] = currentUser;
				}
			}

			// Call the next middleware in the pipeline
			await _next(httpContext);
		}
	}


}

