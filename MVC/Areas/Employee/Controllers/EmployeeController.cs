using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LmsModels.Employee;
using MVC.Areas.Identity.Data;
using LmsServices.Admin.Interfaces;
using MVC.Models;
using MVC.Areas.Administrator.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using LmsServices.Common;
using Microsoft.AspNetCore.Identity.UI.Services;
using LmsModels.Administrator;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MVC.Areas.Employee.Controllers
{
	[Area("Employee")]
	public class EmployeeController : Controller
	{
		private readonly IBranchService _branchService;
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly IUserStore<AppUser> _userStore;
		private readonly UserManager<AppUser> _userManager;
		private readonly IUserEmailStore<AppUser> _emailStore;
		private int firmId;
		private int branchId;
		private string firmShortName;

		public EmployeeController(
			IBranchService branchService,
			IHttpContextAccessor contextAccessor,
			IUserStore<AppUser> userStore,
			UserManager<AppUser> userManager
			)
		{
			_branchService = branchService;
			_contextAccessor = contextAccessor;
			_userStore = userStore;
			_userManager = userManager;
			_emailStore = GetEmailStore();

			var currentUser = _contextAccessor.HttpContext.Items["CurrentUser"] as CurrentUser;

			firmId = currentUser?.FirmId ?? 0;
			branchId = currentUser?.BranchId ?? 0;
			firmShortName = currentUser?.FirmShortName ?? "NA";

		}


		// GET: EmployeeController
		public async Task<ActionResult> Index()
		{
			var users = _userManager.Users
				.Include(user => user.Firm) // Eagerly load the Firm navigation property
				.Where(user => user.FirmId == firmId) // Apply the filter
				.ToList() // Materialize the query
			 .Select(user => new
			 {
				 user.Id,
				 user.FirmId,
				 user.Name,
				 user.Uid1,
				 user.BranchId,
				 user.UserName,
				 user.Email,
				 user.PhoneNumber,
				 FirmName = user.Firm != null ? user.Firm.FirmName : "No Firm"
			 })
			.ToList();

			//.ToList();
			var userRoles = new List<UserWithRolesViewModel>();

			foreach (var user in users)
			{
				var roles = await _userManager.GetRolesAsync(new AppUser { Id = user.Id });
				userRoles.Add(new UserWithRolesViewModel
				{
					Id = user.Id,
					FirmId = user.FirmId,
					Uid1 = user.Uid1,
					Name = user.Name,
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

		// GET: EmployeeController/Details/5
		public ActionResult Details(int id)
		{
			return View();
		}

		// GET: EmployeeController/Create
		public ActionResult Create()
		{
			var currentUser = _contextAccessor.HttpContext.Items["CurrentUser"] as CurrentUser;
			ViewBag.Branches = _branchService.GetAll(currentUser?.FirmId ?? 0);

			return View();
		}

		// POST: EmployeeController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(EmployeeModel model)
		{

			if (ModelState.IsValid)
			{
				TempData["success"] = "Pass";

				int totalUsers = CommonService.RowCountFirmUser(firmId);
				totalUsers++;
				string EmployeeCode = $"{firmShortName}{(totalUsers):000000}"; // Ensures leading zeros up to 6 digits


				var user = CreateUser();
				await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
				await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);

				// Set the Name property
				user.Name = model.Name;
				user.FirmId = firmId;
				user.BranchId = model.BranchId;
				user.Uid1 = EmployeeCode;
				user.PhoneNumber = model.PhoneNumber;
				user.Status= true;

				var result = await _userManager.CreateAsync(user, "Admin@123");

				if (result.Succeeded)
				{
					//await _userManager.AddToRoleAsync(user, model.Roles);
					if (model.Roles != null && model.Roles.Any())
					{
						var roleResult = await _userManager.AddToRolesAsync(user, model.Roles);

						if (roleResult.Succeeded)
						{
							TempData["success"] = "User created and roles assigned successfully!";
							return RedirectToAction("Index");
						}
						else
						{
							TempData["error"] = "User created, but assigning roles failed.";
							foreach (var error in roleResult.Errors)
							{
								ModelState.AddModelError("Roles", error.Description);
							}
							return View(model);
						}
					}

				}

				return View(model);
			}
			TempData["success"] = "fail";

			if (model.Roles == null || !model.Roles.Any())
			{
				ModelState.AddModelError("Roles", "At least one role is required.");
			}

			return View(model);


		}

		// GET: EmployeeController/Edit/5
		public ActionResult Edit(int id)
		{
			return View();
		}

		// POST: EmployeeController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int id, IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		// GET: EmployeeController/Delete/5
		public ActionResult Delete(int id)
		{
			return View();
		}

		// POST: EmployeeController/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int id, IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		private AppUser CreateUser()
		{
			try
			{
				return Activator.CreateInstance<AppUser>();
			}
			catch
			{
				throw new InvalidOperationException($"Can't create an instance of '{nameof(AppUser)}'. " +
					$"Ensure that '{nameof(AppUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
					$"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
			}
		}

		private IUserEmailStore<AppUser> GetEmailStore()
		{
			if (!_userManager.SupportsUserEmail)
			{
				throw new NotSupportedException("The default UI requires a user store with email support.");
			}
			return (IUserEmailStore<AppUser>)_userStore;
		}

	}
}
