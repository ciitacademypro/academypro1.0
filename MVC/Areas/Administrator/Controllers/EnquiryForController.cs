using LmsServices.Admin.Implmentations;
using LmsServices.Admin.Interfaces;
using Microsoft.AspNetCore.Mvc;
using LmsModels.Administrator;
using Microsoft.AspNetCore.Authorization;


namespace lms.Areas.Administrator.Controllers
{
	[Area("Administrator")]
	public class EnquiryForController : Controller
	{
		
		private readonly IEnquiryForService _enquiryForService;
        public EnquiryForController()
        {
            _enquiryForService = new EnquiryForService();
        }

		[Authorize(Roles = "SuperAdmin")]
		public IActionResult Index()
		{
			ViewBag.Enquiries=_enquiryForService.GetAll();
			return View();
		}

		[HttpPost]
		[Authorize(Roles = "SuperAdmin")]
		public IActionResult Create(EnquiryForModel enquiry)
		{
			_enquiryForService.Create(enquiry);
			TempData["success"] = "Record Added successfully!";

			return RedirectToAction("Index");
		}


		[HttpPost]
		[Authorize(Roles = "SuperAdmin")]
		public IActionResult Update(EnquiryForModel enquiry)
		{
			_enquiryForService.Update(enquiry);
			TempData["success"] = "Record updated successfully!";

			return RedirectToAction("Index");
		}


		[HttpPost]
		[Authorize(Roles = "SuperAdmin")]
		public IActionResult Delete(int EnquiryForId)
		{
			_enquiryForService.Delete(EnquiryForId);
			TempData["success"] = "Record deleted successfully!";

			return RedirectToAction("Index");
		}

	}
}
