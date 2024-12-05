using LmsModels.Student;
using LmsServices.Admin.Implmentations;
using LmsServices.Admin.Interfaces;
using LmsServices.Course.Implementations;
using LmsServices.Course.Interfaces;
using LmsServices.Student.Implementations;
using LmsServices.Student.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using MVC.Areas.Identity.Data;
using MVC.Models;
using Microsoft.EntityFrameworkCore;



namespace lms.Areas.Student.Controllers
{
	[Area("Student")]
	public class LeadController : Controller
	{
		private readonly ILeadService _leadService;
		private readonly ILeadSourceService _leadSourceService;
		private readonly IQualificationService _qualificationService;
		private readonly ICourseService _courseService;
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly UserManager<AppUser> _userManager;

		private int currentUserId;
		private int firmId;
		private int branchId;
		private string firmShortName;


		public LeadController(
			IHttpContextAccessor contextAccessor,
			UserManager<AppUser> userManager
		)
		{
			_leadService = new LeadService();
			_leadSourceService = new LeadSourceService();
			_qualificationService = new QualificationService();
			_courseService = new CourseService();
			_contextAccessor = contextAccessor;
			_userManager = userManager;

			var currentUser = _contextAccessor.HttpContext.Items["CurrentUser"] as CurrentUser;

			firmId = currentUser?.FirmId ?? 0;
			branchId = currentUser?.BranchId ?? 0;
			firmShortName = currentUser?.FirmShortName ?? "NA";
			string? currentUserId = currentUser?.Id;
		}



		public async Task<IActionResult> Index(int id=-1)
		{
			ViewBag.AssignedToId = id;  // Pass the id to the view


			ViewBag.Employees = _userManager.Users
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


			IEnumerable<LeadModel> Leads;

			if (id== 0)
			{
				Leads = _leadService.GetAll(firmId, 0, "AssignedTo", 0);	// get all active
			}
			else if(id > 0)
			{
				Leads = _leadService.GetAll(firmId, 0, "AssignedTo", id);   // get all by id
			}
			else
			{
				Leads = _leadService.GetAll(firmId, 0, "AssignedTo", id);	// get all Inactive

			}


			return View(Leads);
		}
		
		public IActionResult Create()
		{
			ViewBag.LeadSources = _leadSourceService.GetAll();
			ViewBag.Courses = _courseService.GetAll();
			ViewBag.Qualifications = _qualificationService.GetAll();

			return View();
		}

		[HttpPost]
		public IActionResult Create(LeadModel lead)
		{
			lead.FirmId = firmId;
			_leadService.Create(lead);
			TempData["success"] = "Record Added successfully!";
			return RedirectToAction("Index");
		}

		public IActionResult Edit(int id)
		{
			ViewBag.LeadSources = _leadSourceService.GetAll();
			ViewBag.Courses = _courseService.GetAll();
			ViewBag.Qualifications = _qualificationService.GetAll();
			var Leads = _leadService.GetById(id, firmId);

			return View(Leads);
		}

		[HttpPost]
		public IActionResult AssignTo(int EmployeeId, string LeadIds)
		{

			int[] intArray = LeadIds.Split(',').Select(int.Parse).ToArray();

			foreach (int leadId in intArray)
			{
				_leadService.AssignTo(EmployeeId, leadId, currentUserId, firmId);	// 3 is loggedIn User
			}
			TempData["success"] = "Leads Assigned successfully!";
			return RedirectToAction("Index");

		}

		[HttpPost]
		public IActionResult UpdateRemark(string Remark, string Status, int LeadId)
		{
			_leadService.UpdateRemark(LeadId, Status, Remark, 3);

			TempData["success"] = "Leads Status / Remark updated successfully!";

			return RedirectToAction("Index");
		}


		[HttpPost]
		public IActionResult Edit(LeadModel lead)
		{
			_leadService.Update(lead);
			TempData["success"] = "Record updated successfully!";
			return RedirectToAction("Index");
		}

		[HttpPost]
		public IActionResult Delete(int id)
		{
			_leadService.Delete(id, firmId);

			TempData["success"] = "Record Delete successfully!";
			return RedirectToAction("Index");
		}


	}
}
