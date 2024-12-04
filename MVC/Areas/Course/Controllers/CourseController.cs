using LmsModels.Course;
using LmsServices.Course.Implementations;
using LmsServices.Course.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC.Areas.Identity.Data;

namespace lms.Areas.Course.Controllers
{
	[Area("course")]
	[Authorize]
	public class CourseController : Controller
	{
		ICourseCategoryService _courseCategoryService;
		ICourseService _courseService;
		private readonly UserManager<AppUser> _userManager;

		public CourseController(UserManager<AppUser> userManager)
		{
			_courseCategoryService = new CourseCategoryService();
			_courseService = new CourseService();
			_userManager = userManager;
		}

		public IActionResult Index()
		{
			var courses =  _courseService.GetAll(); // CourseId, CategoryId
			return View(courses); // Pass the resolved list to the view
		}

		[Authorize(Roles = "Admin")]
		public IActionResult Create()
		{
			ViewBag.CourseCategories =  _courseCategoryService.GetAll();
			return View();
		}

		[HttpPost]
		[Authorize(Roles ="Admin")]
		public async Task<IActionResult> Create(CourseModel course, string feesJsonString)
		{

			// Get the current logged-in user's Id
			var userId = _userManager.GetUserId(User);

			// Fetch the user from the database
			var currentUser = await _userManager.FindByIdAsync(userId);

			// Access the FirmId and ensure it's not null
			if (currentUser?.FirmId == null)
			{
				TempData["error"] = "FirmId is not available for the logged-in user.";
				return RedirectToAction("Index");
			}

			course.FirmId = currentUser.FirmId.Value; // Explicitly convert nullable to non-nullable


			_courseService.CreateWithFees(course, feesJsonString);

			TempData["success"] = "Course with fees created successfully!";
			return RedirectToAction("Index");
		}

		public IActionResult Edit(int id)
		{
			ViewBag.CourseCategories = _courseCategoryService.GetAll();
			ViewBag.id = id;
			CourseModel course = _courseService.GetById(id);
			return View(course);
		}

		[HttpPost]
		public IActionResult Edit(CourseModel course)
		{
			_courseService.Update(course);

			TempData["success"] = "Course updated successfully!";
			return RedirectToAction("Index");
		}



		public IActionResult GetIdNameList(int id)
		{
			var courses =  _courseService.GetAll(0, id); // CourseId, CategoryId
			if (courses == null || !courses.Any())
			{
				return Json(new object[] { }); // Return an empty JSON array
			}

			// Select only the CourseId and CourseName
			var idNameList = courses.Select(c => new
			{
				c.CourseId,
				c.CourseName
			}).ToList();
			return Json(idNameList);

		}

		public IActionResult GetCourseFees(int id)
		{
			var coursesFees = _courseService.GetCourseFees(id); // CourseId, CategoryId
			if (coursesFees == null || !coursesFees.Any())
			{
				return Json(new object[] { }); // Return an empty JSON array
			}

			return Json(coursesFees);

		}



		[HttpPost]
		public IActionResult Delete(int id)
		{
			_courseService.Delete(id);
			TempData["success"] = "Course Deleted successfully!";
			return RedirectToAction ("Index");
		}


		public IActionResult CreateModule()
		{
			ViewBag.courses =  _courseService.GetAll(); // CourseId, CategoryId

			ViewBag.CourseCategories =  _courseCategoryService.GetAll();

			return View();
		}


	}
}
