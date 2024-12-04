using LmsModels.Administrator;
using LmsServices.Admin.Implmentations;
using LmsServices.Admin.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lms.Areas.Administrator.Controllers
{
	[Area("Administrator")]
	public class LeadSourceController : Controller
	{
		private readonly ILeadSourceService _leadSourceService;

        public LeadSourceController()
        {
			_leadSourceService = new LeadSourceService();
		}

		[Authorize(Roles = "SuperAdmin")]
		public IActionResult Index()
		{
			ViewBag.Branches = _leadSourceService.GetAll();
			return View();
		}

		[HttpPost]
		[Authorize(Roles = "SuperAdmin")]
		public IActionResult Create(LeadSourceModel leadSource)
		{
			_leadSourceService.Create(leadSource);
			TempData["success"] = "Record Added successfully!";

			return RedirectToAction("Index");
		}


		[HttpPost]
		[Authorize(Roles = "SuperAdmin")]
		public IActionResult Update(LeadSourceModel leadSource)
		{
			_leadSourceService.Update(leadSource);
			TempData["success"] = "Record updated successfully!";

			return RedirectToAction("Index");
		}


		[HttpPost]
		[Authorize(Roles = "SuperAdmin")]
		public IActionResult Delete(int LeadSourceId)
		{
			_leadSourceService.Delete(LeadSourceId);
			TempData["success"] = "Record deleted successfully!";

			return RedirectToAction("Index");
		}
	}
}
