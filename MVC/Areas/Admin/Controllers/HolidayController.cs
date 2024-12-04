
using LmsModels.Administrator;
using LmsServices.Admin.Implmentations;
using LmsServices.Admin.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace lms.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HolidayController : Controller
    {

            private readonly IHolidayService _HolidayService;

            public HolidayController()
            {

                _HolidayService = new HolidayService();
            }

            public IActionResult Index()
            {
                ViewBag.Holidayes = _HolidayService.GetAll();

                return View();
            }

            public IActionResult Create()
            {
                return View();
            }

            [HttpPost]
            public IActionResult Create(HolidayModel Holiday)
            {
            _HolidayService.Create(Holiday);
                TempData["success"] = "Record Added successfully!";
                return RedirectToAction("Index");
            }

            public IActionResult Edit(int id)
            {
                var Holidays = _HolidayService.GetById(id);

                return View(Holidays);
            }

            [HttpPost]
            public IActionResult Edit(HolidayModel Holiday)
            {
            _HolidayService.Update(Holiday);
                TempData["success"] = "Record updated successfully!";
                return RedirectToAction("Index");
            }

            [HttpPost]
            public IActionResult Delete(int id)
            {
            _HolidayService.Delete(id);

                TempData["success"] = "Record Delete successfully!";
                return RedirectToAction("Index");
            }

		public IActionResult GetIdNameList()
		{
			var holidays = _HolidayService.GetAll(); // StateId, CountryId
			if (holidays == null || !holidays.Any())
			{
				return Json(new object[] { }); // Return an empty JSON array
			}

			// Select only the StateId and StateName
			var idNameList = holidays.Select(c => new
			{
				c.HolidayId,
				c.HolidayDate
			}).ToList();
			return Json(idNameList);
		}

        public IActionResult GetDateList()
        {
            var holidays = _HolidayService.GetAll(); // Get the list of holidays
            if (holidays == null || !holidays.Any())
            {
                return Json(new object[] { }); // Return an empty JSON array if no holidays
            }

            // Select only the HolidayDate and format it to "yyyy-MM-dd"
            var dateList = holidays.Select(c => c.HolidayDate.ToString("yyyy-MM-dd")).ToList();
            return Json(dateList);
        }

    }
}

