// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MVC.Areas.Identity.Data;

namespace MVC.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly IMemoryCache _memoryCache;
		public LogoutModel(SignInManager<AppUser> signInManager, ILogger<LogoutModel> logger, IMemoryCache memoryCache)
        {
            _signInManager = signInManager;
            _logger = logger;
            _memoryCache = memoryCache;
		}

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();

			HttpContext.Session.Clear();
			//_memoryCache.Clear(); // Or you can clear specific cache items: _memoryCache.Remove("User_");

			var userName = User.Identity?.Name;
			if (!string.IsNullOrEmpty(userName))
			{
				_memoryCache.Remove($"User_{userName}");
			}



			_logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                return RedirectToPage();
            }
        }
    }
}
