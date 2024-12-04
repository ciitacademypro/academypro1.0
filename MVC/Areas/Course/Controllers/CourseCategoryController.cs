using LmsModels.Course;
using LmsServices.Course.Implementations;
using LmsServices.Course.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace lms.Areas.Course.Controllers
{
	[Area("course")]
	public class CourseCategoryController : Controller
	{

		ICourseCategoryService _courseCategoryService;

		public CourseCategoryController()
		{
			_courseCategoryService = new CourseCategoryService();
		}

		public IActionResult Index()
		{
			ViewBag.CourseCategories =  _courseCategoryService.GetAll();
			return View();
		}

		[HttpPost]
		public IActionResult Create([Bind("ParentId, CourseCategoryName, Status")] CourseCategoryModel coursesCategory)
		{
			coursesCategory.ParentId = coursesCategory.ParentId == 0 ? null : coursesCategory.ParentId;

			_courseCategoryService.Create(coursesCategory);

			TempData["success"] = "Record Added successfully!";
			return RedirectToAction("Index");
		}
				
		[HttpPost]
		public IActionResult Update(CourseCategoryModel coursesCategory)
		{
			coursesCategory.ParentId = coursesCategory.ParentId == 0 ? null : coursesCategory.ParentId;

			_courseCategoryService.Update(coursesCategory);

			TempData["success"] = "Record Updated successfully!";
			return RedirectToAction("Index");
		}

		[HttpPost]
		public IActionResult Delete(int category_id)
		{
			_courseCategoryService.Delete(category_id);

			TempData["success"] = "Record Delete successfully!";
			return RedirectToAction("Index");
		}


	}
}
