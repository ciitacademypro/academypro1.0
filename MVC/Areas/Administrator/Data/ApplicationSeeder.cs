using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MVC.Areas.Identity.Data;
using System;
using System.Threading.Tasks;

namespace MVC.Areas.Administrator.Data
{
	public class ApplicationSeeder
	{

		public static async Task SeedRolesAndUsers(IServiceProvider serviceProvider)
		{
			// Get RoleManager and UserManager
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

			// Define roles
			string[] roleNames = { "SuperAdmin",  "Admin", "Staff", "Trainer","Coordinator", "Student" };
			
			// Create roles
			foreach (var roleName in roleNames)
			{
				if (!await roleManager.RoleExistsAsync(roleName))
				{
					await roleManager.CreateAsync(new IdentityRole(roleName));
				}
			}

			// Create SuperAdmin user
			var name = "Super Admin";
			var adminEmail = "superadmin@academypro.com";
			var adminPassword = "Admin@123";


			var adminUser = await userManager.FindByEmailAsync(adminEmail);
			if (adminUser == null)
			{
				adminUser = new AppUser
				{
					FirmId = null,
					BranchId = null,
					Name = name,
					Uid1 = "SA0001",
					UserName = adminEmail,
					Email = adminEmail,
					EmailConfirmed = true
				};

				var createAdminResult = await userManager.CreateAsync(adminUser, adminPassword);
				if (createAdminResult.Succeeded)
				{
					await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
				}
			}

		}

	}
}
