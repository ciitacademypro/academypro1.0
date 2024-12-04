﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "SuperAdmin,Admin")]
	public class BranchController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
