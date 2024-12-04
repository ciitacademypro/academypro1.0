using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LmsModels.Administrator;
using MVC.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using LmsModels.Admin;
using LmsServices.Admin.Implmentations;
using Newtonsoft.Json;
using MVC.Areas.Admin.Models;
using MVC.Areas.Administrator.Models;

namespace MVC.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    [Authorize(Roles ="SuperAdmin")]
    public class FirmController : Controller
    {
        private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public FirmController(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_context = context;
			_userManager = userManager;
			_roleManager = roleManager;
		}


        // GET: Administrator/Firm
        public async Task<IActionResult> Index()
        {
			var usersWithRoles = new List<UserWithRolesViewModel>();
			var users = _userManager.Users.ToList();
			ViewBag.roles = HttpContext.Session.GetString("roles");

			foreach (var user in users)
			{
				var roles = await _userManager.GetRolesAsync(user);
				usersWithRoles.Add(new UserWithRolesViewModel
				{
					UserName = user.UserName, // Or any property like Email
					Email = user.Email, // Or any property like Email
					PhoneNumber = user.PhoneNumber, // Or any property like Email
					Roles = roles.ToList()
				});
			}
			//var firms = new List<Firm>(); // Replace this with the actual logic to fetch firms
			var firms = await _context.Firms.ToListAsync();

			return View(firms);
        }

        // GET: Administrator/Firm/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var firmModel = await _context.Firms
                .FirstOrDefaultAsync(m => m.FirmId == id);
            if (firmModel == null)
            {
                return NotFound();
            }

            return View(firmModel);
        }

        // GET: Administrator/Firm/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Administrator/Firm/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirmId,FirmShortName,FirmName,FirmLogo,FirmEmail,FirmContact,FirmWeb,FirmDescription,Status")] Firm firmModel)
        {

			if (await _context.Firms.AnyAsync(f => f.FirmShortName == firmModel.FirmShortName))
			{
				ModelState.AddModelError("FirmShortName", "Firm short name must be unique.");
				return View(firmModel); // Return the view with validation error
			}

			if (ModelState.IsValid)
            {
                _context.Add(firmModel);
                await _context.SaveChangesAsync();

                // Create Admin user
                var name = "Firm Admin";
				var adminEmail = firmModel.FirmEmail;
				var adminPassword = "Admin@123";

				var adminUser = await _userManager.FindByEmailAsync(adminEmail);

				if (adminUser == null)
				{
					var branchModel = new BranchModel
					{
						FirmId = firmModel.FirmId,
						BranchName = firmModel.FirmShortName + " 1",
						Status = true,
					};

					var branchService = new BranchService();
					var branchId = branchService.Create(branchModel);


					adminUser = new AppUser
					{
						FirmId = firmModel.FirmId,
						BranchId = branchId,
						Name = name,
						Uid1 = firmModel.FirmShortName+"0001",
						UserName = adminEmail,
						Email = adminEmail,
                        PhoneNumber = firmModel.FirmContact,
						EmailConfirmed = true
					};

					var createAdminResult = await _userManager.CreateAsync(adminUser, adminPassword);

                    // Log the adminUser object to verify properties
                    //Console.WriteLine(JsonConvert.SerializeObject(adminUser));

					if (createAdminResult.Succeeded)
					{
						await _userManager.AddToRoleAsync(adminUser, "Admin");
					}

				}



				return RedirectToAction(nameof(Index));
            }
            return View(firmModel);
        }

        // GET: Administrator/Firm/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var firmModel = await _context.Firms.FindAsync(id);
            if (firmModel == null)
            {
                return NotFound();
            }
            return View(firmModel);
        }

        // POST: Administrator/Firm/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FirmId,FirmShortName,FirmName,FirmLogo,FirmEmail,FirmContact,FirmWeb,FirmDescription,Status,DeletedAt")] Firm firmModel)
        {
            if (id != firmModel.FirmId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(firmModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FirmModelExists(firmModel.FirmId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(firmModel);
        }

        // GET: Administrator/Firm/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var firmModel = await _context.Firms
                .FirstOrDefaultAsync(m => m.FirmId == id);
            if (firmModel == null)
            {
                return NotFound();
            }

            return View(firmModel);
        }

        // POST: Administrator/Firm/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var firmModel = await _context.Firms.FindAsync(id);
            if (firmModel != null)
            {
                _context.Firms.Remove(firmModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FirmModelExists(int id)
        {
            return _context.Firms.Any(e => e.FirmId == id);
        }
    }
}
