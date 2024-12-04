
using LmsModels.Batch;
using LmsServices.Batch.Implmentations;
using LmsServices.Batch.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;

namespace lms.Areas.Batch.Controllers
{
    [Area("Batch")]
    public class ClassRoomController : Controller
    {

        private readonly IClassRoomService _ClassRoomService;
		private readonly IHttpContextAccessor _contextAccessor;

		private int firmId;
		private int branchId;
		private string firmShortName;


		public ClassRoomController(
			IHttpContextAccessor contextAccessor
			)
		{
			 _ClassRoomService = new ClassRoomService();
			_contextAccessor = contextAccessor;
			var currentUser = _contextAccessor.HttpContext.Items["CurrentUser"] as CurrentUser;

			firmId = currentUser?.FirmId ?? 0;
			branchId = currentUser?.BranchId ?? 0;
			firmShortName = currentUser?.FirmShortName ?? "NA";


		}

		public IActionResult Index()
            {
			    ViewBag.ClassRooms = _ClassRoomService.GetAll(firmId);

			    return View();
            }

            public IActionResult Create()
            {
                return View();
            }

            [HttpPost]
            public IActionResult Create(ClassRoomModel ClassRoom)
            {
            ClassRoom.BranchId = 1;

			_ClassRoomService.Create(ClassRoom, firmId);
                TempData["success"] = "Record Added successfully!";
                return RedirectToAction("Index");
            }

            public IActionResult Edit(int id)
            {
                var ClassRooms = _ClassRoomService.GetById(id);

                return View(ClassRooms);
            }

            [HttpPost]
            public IActionResult Edit(ClassRoomModel ClassRoom)
            {
                ClassRoom.FirmId = firmId;
                _ClassRoomService.Update(ClassRoom);
                TempData["success"] = "Record updated successfully!";
                return RedirectToAction("Index");
            }

            [HttpPost]
            public IActionResult Delete(int id)
            {
            _ClassRoomService.Delete(id);

                TempData["success"] = "Record Delete successfully!";
                return RedirectToAction("Index");
            }



    }
}

