using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Areas.Administrator.Controllers
{

	[Area("Administrator")]
	[Authorize(Roles = "SuperAdmin")]
	public class RoleController : Controller
	{

		private readonly RoleManager<IdentityRole> _roleManager;

		public RoleController(RoleManager<IdentityRole> roleManager)
		{
			_roleManager = roleManager;
		}

		[Authorize(Roles = "SuperAdmin")]
		public  IActionResult Index()
		{
			var roles = _roleManager.Roles.ToList();
			return View(roles);
		}

		[Authorize(Roles = "SuperAdmin")]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[Authorize(Roles = "SuperAdmin")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(string roleName)
		{
			if (string.IsNullOrWhiteSpace(roleName))
			{
				ModelState.AddModelError(string.Empty, "Role name cannot be empty.");
				return View();
			}

			var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
			if (result.Succeeded)
			{
				return RedirectToAction(nameof(Index));
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return View();
		}


		[Authorize(Roles = "SuperAdmin")]
		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrWhiteSpace(id)) return NotFound();

			var role = await _roleManager.FindByIdAsync(id);
			if (role == null) return NotFound();

			return View(role);
		}

		[HttpPost]
		[Authorize(Roles = "SuperAdmin")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(IdentityRole identityRole)
		{
			var role = await _roleManager.FindByIdAsync(identityRole.Id);
			if (role == null) return NotFound();

			if (string.IsNullOrWhiteSpace(identityRole.Name))
			{
				ModelState.AddModelError(string.Empty, "Role name cannot be empty.");
				return View(role);
			}

			role.Name = identityRole.Name;
			var result = await _roleManager.UpdateAsync(role);

			if (result.Succeeded)
			{
				return RedirectToAction(nameof(Index));
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return View(role);
		}


		[Authorize(Roles = "SuperAdmin")]
		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrWhiteSpace(id)) return NotFound();

			var role = await _roleManager.FindByIdAsync(id);
			if (role == null) return NotFound();

			return View(role);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "SuperAdmin")]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			var role = await _roleManager.FindByIdAsync(id);
			if (role == null) return NotFound();

			var result = await _roleManager.DeleteAsync(role);
			if (result.Succeeded)
			{
				return RedirectToAction(nameof(Index));
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return View(role);
		}

	}
}
