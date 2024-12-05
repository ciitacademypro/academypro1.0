using LmsModels.Student;
using LmsServices.Admin.Implmentations;
using LmsServices.Admin.Interfaces;
using LmsServices.Course.Implementations;
using LmsServices.Course.Interfaces;
using LmsServices.Student.Implmentations;
using LmsServices.Student.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;

namespace lms.Areas.Student.Controllers
{
	[Area("Student")]
	public class EnquiryController : Controller
	{

		private readonly IEnquiryService _enquiryService;
		private readonly ILeadSourceService _leadSourceService;
		private readonly IQualificationService _qualificationService;
		private readonly IBranchService _branchService;
		private readonly ICityService _cityService;
		private readonly IEnquiryForService _enquiryForService;
		private readonly ICourseCategoryService _courseCategoryService;
		private readonly IHttpContextAccessor _contextAccessor;

		private int currentUserId;
		private int firmId;
		private int branchId;
		private string firmShortName;


		public EnquiryController(
			IHttpContextAccessor contextAccessor
			)
		{
			_enquiryService = new EnquiryService();
			_leadSourceService = new LeadSourceService();
			_qualificationService = new QualificationService();
			_branchService = new BranchService();
			_cityService = new CityService();
			_enquiryForService = new EnquiryForService();
			_courseCategoryService = new CourseCategoryService();
			_contextAccessor = contextAccessor;

			var currentUser = _contextAccessor.HttpContext.Items["CurrentUser"] as CurrentUser;

			firmId = currentUser?.FirmId ?? 0;
			branchId = currentUser?.BranchId ?? 0;
			firmShortName = currentUser?.FirmShortName ?? "NA";
			string? currentUserId = currentUser?.Id;


		}

		public IActionResult Index()
		{
            ViewBag.Enquiries = _enquiryService.GetAll(firmId, branchId);
			ViewBag.CourseCategories = _courseCategoryService.GetAll();

			return View();
		}

		public IActionResult Create()
		{
			ViewBag.LeadSources = _leadSourceService.GetAll();
			ViewBag.Cities = _cityService.GetAll();
			ViewBag.Qualifications = _qualificationService.GetAll();
			ViewBag.Branches = _branchService.GetAll(firmId);
			ViewBag.Enquiries = _enquiryForService.GetAll();

			return View();
		}

		[HttpPost]
		public IActionResult Create(EnquiryModel enquiry)
		{
			enquiry.EnquiryDate = DateTime.Now;
			enquiry.FirmId = firmId;
			enquiry.BranchId = branchId;
			_enquiryService.Create(enquiry);
			TempData["success"] = "Record Added successfully!";
			return RedirectToAction("Index");
		}

		public IActionResult Edit(int id)
		{
			ViewBag.LeadSources = _leadSourceService.GetAll();
			ViewBag.Cities = _cityService.GetAll();
			ViewBag.Qualifications = _qualificationService.GetAll();
			ViewBag.Branches = _branchService.GetAll(0);
			ViewBag.Enquiries = _enquiryForService.GetAll();

			var enquiries = _enquiryService.GetById(id, firmId, branchId);

			return View(enquiries);
		}

		[HttpPost]
		public IActionResult Edit(EnquiryModel enquiry)
		{
			_enquiryService.Update(enquiry);
			TempData["success"] = "Record updated successfully!";
			return RedirectToAction("Index");
		}

		[HttpPost]
		public IActionResult UpdateRemark(int EnquiryId, string Remark )
		{
			_enquiryService.UpdateRemark(EnquiryId, Remark);

			TempData["success"] = "Enquiry Remark updated successfully!";

			return RedirectToAction("Index");
		}


		[HttpPost]
		public IActionResult Delete(int id)
		{
			_enquiryService.Delete(id, firmId);

			TempData["success"] = "Record Delete successfully!";
			return RedirectToAction("Index");
		}



	}
}
