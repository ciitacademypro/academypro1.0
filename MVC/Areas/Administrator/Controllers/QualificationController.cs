using LmsModels.Administrator;
using LmsServices.Admin.Implmentations;
using LmsServices.Admin.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lms.Areas.Administrator.Controllers
{
	[Area("Administrator")]
	public class QualificationController : Controller
	{
		private readonly IQualificationService _qualificationService;

        public QualificationController()
        {
			_qualificationService = new QualificationService();
		}

		[Authorize(Roles = "SuperAdmin")]
		public IActionResult Index()
		{
			ViewBag.Qualifications = _qualificationService.GetAll();
			return View();
		}



		[HttpPost]
		[Authorize(Roles = "SuperAdmin")]
		public IActionResult Create(QualificationModel qualification)
		{
			_qualificationService.Create(qualification);
			TempData["success"] = "Record Added successfully!";

			return RedirectToAction("Index");
		}


		[HttpPost]
		[Authorize(Roles = "SuperAdmin")]
		public IActionResult Update(QualificationModel qualification)
		{
			_qualificationService.Update(qualification);
			TempData["success"] = "Record updated successfully!";

			return RedirectToAction("Index");
		}


		[HttpPost]
		[Authorize(Roles = "SuperAdmin")]
		public IActionResult Delete(int QualificationId)
		{
			_qualificationService.Delete(QualificationId);
			TempData["success"] = "Record deleted successfully!";

			return RedirectToAction("Index");
		}


	}
}
