using LmsModels.Administrator;
using LmsServices.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Areas.Administrator.Models;
using MVC.Areas.Identity.Data;

namespace MVC.Areas.Administrator.Controllers
{

	[Area("Administrator")]
	[Authorize(Roles = "SuperAdmin")]
	public class UserController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public async Task<IActionResult> Index()
		{

			var users = _userManager.Users
				.Include(user => user.Firm) // Eagerly load the Firm navigation property
				.ToList() // Materialize the query
			 .Select(user => new
			 {
				 user.Id,
				 user.FirmId,
				 user.BranchId,
				 user.UserName,
				 user.Email,
				 user.PhoneNumber,
				 FirmName = user.Firm != null ? user.Firm.FirmName : "No Firm"
			 })
			.ToList();


			var userRoles = new List<UserWithRolesViewModel>();

			foreach (var user in users)
			{
				var roles = await _userManager.GetRolesAsync(new AppUser { Id = user.Id });
				if(roles.Contains("SuperAdmin") || roles.Contains("Admin"))
				userRoles.Add(new UserWithRolesViewModel
				{
					Id = user.Id,
					FirmId = user.FirmId,
					FirmName = user.FirmName,
					BranchId = user.BranchId,
					UserName = user.UserName,
					Email = user.Email,
					PhoneNumber = user.PhoneNumber,
					Roles = roles.ToList()
				});
			}

			return View(userRoles);
		}
	}
}
