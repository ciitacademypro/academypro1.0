// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MVC.Areas.Identity.Data;
using System.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using MVC.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;

namespace MVC.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMemoryCache _memoryCache;

		public LoginModel(SignInManager<AppUser> signInManager, ILogger<LoginModel> logger, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;

		}

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

		public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			if (User.Identity.IsAuthenticated)
			{

				// Fetch the current user and roles
				var user = await _userManager.GetUserAsync(User); // Get the logged-in user
				if (user != null)
				{

					var roles = await _userManager.GetRolesAsync(user);

					// Redirect based on roles
					if (roles.Contains("Admin"))
					{
						return RedirectToAction("Index", "Course", new { area = "Course" });
					}
					else if (roles.Contains("SuperAdmin"))
					{
						return RedirectToAction("Index", "User", new { area = "Administrator" });
					}
					else
					{
						return RedirectToAction("Index", "Home", new { area = "" });
					}
				}
			}

			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
			ReturnUrl = returnUrl;

			// Return the page if no redirection occurred
			return Page();

			//ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync0(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true

				//var result = await _signInManager.PasswordSignInAsync("superadmin@academypro.com", "Admin@123", false, lockoutOnFailure: false);
				var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
				if (result.Succeeded)
                {

					HttpContext.Session.Clear();

					//var user = await _userManager.FindByEmailAsync(Input.Email);
					var user = await _userManager.Users
						.Include(u => u.Branch)
						.Include(u => u.Firm)
						.FirstOrDefaultAsync(u => u.Email == Input.Email);

					if (user == null)
					{
						ModelState.AddModelError(string.Empty, "User not found.");
						return Page();
					}
    
					var roles = await _userManager.GetRolesAsync(user);

					if (roles == null || !roles.Any())
					{
						_logger.LogWarning("No roles found for user.");
						ModelState.AddModelError(string.Empty, "User does not have any roles assigned.");
						return Page();
					}

					HttpContext.Session.SetString("currentUserRoles", string.Join(",", roles)); // Join roles as a comma-separated string

					if (user != null)
                    {
                        var currentUser = new CurrentUser
                        {
                            Id = user.Id,
                            BranchId = user.BranchId,
                            Name = user.Name,
                            FirmId = user.FirmId,
                            Email = user.Email,
                            Uid1 = user.Uid1,
                            Status = user.Status,
                            // Use null-coalescing operator to handle null values
                            FirmName = user.Firm?.FirmName ?? "No Firm",  // Get FirmName if Firm is not null, otherwise "No Firm"
                            BranchName = user.Branch?.BranchName ?? "No Branch", // Get BranchName if Branch is not null, otherwise "No Branch"

                            Roles = string.Join(",", roles)
                            
                        };

						string userJson = JsonSerializer.Serialize(currentUser);
						HttpContext.Session.SetString("currentUser", userJson);

						// Add user details to cache
						//_memoryCache.Set($"User_{user.Id}", user, TimeSpan.FromMinutes(30));
						_memoryCache.Set($"User_{currentUser.Id}", currentUser, TimeSpan.FromMinutes(30));

					};

					_logger.LogInformation("User logged in.");
					//return LocalRedirect(returnUrl);
					if (roles.Contains("Admin"))
					{
						return RedirectToAction("Index", "Course", new { area = "Course" });
					}
					else if (roles.Contains("SuperAdmin"))
					{
						return RedirectToAction("Index", "User", new { area = "Administrator" });
					}
					else
					{
						return RedirectToAction("Index", "Home");
					}


				}
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }


		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");

			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			if (ModelState.IsValid)
			{
				// Perform password sign-in
				var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

				if (result.Succeeded)
				{
					// Clear any existing session
					HttpContext.Session.Clear();

					// Find the user by email
					var user = await _userManager.Users
						.Include(u => u.Branch)
						.Include(u => u.Firm)
						.FirstOrDefaultAsync(u => u.Email == Input.Email);

					if (user == null)
					{
						ModelState.AddModelError(string.Empty, "User not found.");
						return Page();
					}

					// Get roles for the user
					var roles = await _userManager.GetRolesAsync(user);

					if (roles == null || !roles.Any())
					{
						_logger.LogWarning("No roles found for user: {UserEmail}", user.Email);
						ModelState.AddModelError(string.Empty, "User does not have any roles assigned.");
						return Page();
					}

					// Store user roles in session as a comma-separated string
					HttpContext.Session.SetString("currentUserRoles", string.Join(",", roles));

					// Create current user object
					var currentUser = new CurrentUser
					{
						Id = user.Id,
						BranchId = user.BranchId,
						Name = user.Name,
						FirmId = user.FirmId,
						Email = user.Email,
						Uid1 = user.Uid1,
						Status = user.Status,
						FirmName = user.Firm?.FirmName ?? "No Firm",
						BranchName = user.Branch?.BranchName ?? "No Branch",
						Roles = string.Join(",", roles)
					};

					// Serialize and set user data in session
					string userJson = JsonSerializer.Serialize(currentUser);
					HttpContext.Session.SetString("currentUser", userJson);

					// Cache the current user (optional)
					if (_memoryCache != null)
					{
						_memoryCache.Set($"User_{currentUser.Id}", currentUser, TimeSpan.FromMinutes(30));
					}

					_logger.LogInformation("User {UserEmail} logged in.", user.Email);

					// Redirect based on roles
					if (roles.Contains("Admin"))
					{
						return RedirectToAction("Index", "Course", new { area = "Course" });
					}
					else if (roles.Contains("SuperAdmin"))
					{
						return RedirectToAction("Index", "User", new { area = "Administrator" });
					}
					else
					{
						return RedirectToAction("Index", "Home");
					}
				}

				if (result.RequiresTwoFactor)
				{
					return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
				}
				if (result.IsLockedOut)
				{
					_logger.LogWarning("User account locked out.");
					return RedirectToPage("./Lockout");
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					return Page();
				}
			}

			// If we got this far, something failed, redisplay form
			return Page();
		}



	}
}
