using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace MVC.Middleware
{
	public class CacheValidationMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IMemoryCache _cache;

		public CacheValidationMiddleware(RequestDelegate next, IMemoryCache cache)
		{
			_next = next;
			_cache = cache;
		}

		public async Task InvokeAsync(HttpContext context)
		{

			var path = context.Request.Path;

			// Exclude specific paths to prevent redirect loops
			//if (path.StartsWithSegments("/Account/Login", StringComparison.OrdinalIgnoreCase) ||
			//	path.StartsWithSegments("/Account/Logout", StringComparison.OrdinalIgnoreCase))
			//{
			//	await _next(context);
			//	return;
			//}

			if (path.StartsWithSegments("/Account/Login", StringComparison.OrdinalIgnoreCase))
			{
				// Do not process further for the login page itself
				await _next(context);
				return;
			}

			var user = context.User;


			if (user.Identity.IsAuthenticated)
			{
				var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get the user ID from claims
				if (userId != null)
				{
					// Check if the cache entry for this user exists
					if (!_cache.TryGetValue($"User_{userId}", out _))
					{
						// Logout the user if the cache is missing or expired
						await context.SignOutAsync();
						context.Response.Redirect("/Account/Login");
						return;
					}
				}
			}
			await _next(context);
		}

	}
}
