using LmsModels.Course;
using LmsServices.Course.Implementations;
using LmsServices.Course.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MVC.Areas.Administrator.Models;
using MVC.Models;

namespace lms.Areas.Course.Controllers
{
	[Area("Course")]
	public class CourseModuleController : Controller
	{
		ICourseModuleService _courseModuleService;
		ICourseModuleContentService _contentService;
		ICourseService _courseService;
		ICourseCategoryService _courseCategoryService;

		private readonly IHttpContextAccessor _contextAccessor;

		private int currentUserId;
		private int firmId;
		private int branchId;
		private string firmShortName;


		public CourseModuleController(
					IHttpContextAccessor contextAccessor
		)
		{
			
			_courseModuleService = new CourseModuleService();
			_contentService = new CourseModuleContentService();
			_courseService = new CourseService();
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
			ViewBag.courseModules = _courseModuleService.GetAll(firmId, 0, 0);
			return View("~/Areas/Course/Views/CourseModule/Index.cshtml");
		}

		public IActionResult Create()
		{
           
            ViewBag.CourseCategories = _courseCategoryService.GetAll();
            return View("~/Areas/Course/Views/CourseModule/Create.cshtml");
		}


		[HttpPost]
		public IActionResult Create(CourseModuleModel courseModule)
		{
			_courseModuleService.Create(courseModule);

			TempData["success"] = "Course Module created successfully!";
			return RedirectToAction("Index");
		}

		public ActionResult Edit(int id)
		{
			
			var courseModule = _courseModuleService.GetById(id, firmId);
		    if (courseModule == null)
			{
				TempData["error"] = "Course Module not found!";
				return RedirectToAction("Index");
			}
			ViewBag.Course = _courseService.GetAll(firmId);
            ViewBag.CourseCategories = _courseCategoryService.GetAll();

			return View( courseModule);
		}

		[HttpPost]
		public ActionResult Edit(CourseModuleModel courseModule)
		{
	
			var existingModule = _courseModuleService.GetById(courseModule.CourseModuleId, firmId);

			if (existingModule == null)
			{
				TempData["error"] = "Course Module not found!";
				return RedirectToAction("Index");
			}

			
			_courseModuleService.Update(courseModule);

			TempData["success"] = "Course Module updated successfully!";
			return RedirectToAction("Index");
		}


		public IActionResult GetIdNameList(short id)
		{
			var modules =  _courseModuleService.GetAll(0, id); // CourseId, CategoryId
			if (modules == null || !modules.Any())
			{
				return Json(new object[] { }); // Return an empty JSON array
			}

			// Select only the CourseId and CourseName
			var idNameList = modules.Select(c => new
			{
				c.CourseModuleId,
				c.ModuleName
			}).ToList();
			return Json(idNameList);

		}


		public IActionResult GetIdNameDurationList(int id)
		{
			var Contents = _contentService.GetAll(0,id); 
			if (Contents == null || !Contents.Any())
			{
				return Json(new object[] { }); // Return an empty JSON array
			}

			// Select only the StateId and StateName
			var idNameList = Contents.Select(c => new
			{
				id = c.CourseModuleContentId,
				title = c.ContentName,
				duration = c.DurationInHrs
			}).ToList();
			return Json(idNameList);
		}




		public IActionResult Contents(int id)
		{
           	ViewBag.Id = id; 

			ViewBag.Contents = _contentService.GetAll(0, id); // Fetch data

			ViewBag.courseModule = _courseModuleService.GetById(id, firmId);


            return View("~/Areas/Course/Views/CourseModuleContent/index.cshtml");
		}

		[HttpPost]
		public IActionResult CreateContents(CourseModuleContentModel content)
		{
           _contentService.Create(content);
			TempData["success"] = "Course Module Content added successfully!";

			return RedirectToAction("Contents", new { id = content.CourseModuleId });

		}


		[HttpPost]
		public IActionResult UpdateContents(CourseModuleContentModel content)
		{
           _contentService.Update(content);
			TempData["success"] = "Course Module Content updated successfully!";

			return RedirectToAction("Contents", new { id = content.CourseModuleId });
		}

		[HttpPost]
		public IActionResult Delete(int id) 
		{
			_courseModuleService.Delete(id);
			TempData["success"] = "Course Module Deleted successfully!";
			return RedirectToAction("Index");
		}

		[HttpPost]
		public IActionResult DeleteContent(int id, int otherId) 
		{
			_contentService.Delete(id);
			TempData["success"] = "Course Module Content Deleted successfully!";
			return RedirectToAction("contents", new { id = otherId });
		}



	}
}
