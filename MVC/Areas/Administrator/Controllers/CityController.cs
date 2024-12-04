using LmsModels.Administrator;
using LmsServices.Admin.Implmentations;
using LmsServices.Admin.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lms.Areas.Administrator.Controllers
{
	[Area("Administrator")]
	public class CityController : Controller
	{
		private readonly ICountryService _countryService;
		private readonly IStateService _stateService;
		private readonly ICityService _cityService;

        public CityController()
        {
			_countryService = new CountryService();
			_stateService = new StateService();
			_cityService = new CityService();            
        }


		[Authorize(Roles ="SuperAdmin")]
		public IActionResult Index()
		{
			ViewBag.Countries = _countryService.GetAll();
			ViewBag.Cities = _cityService.GetAll();
			return View();
		}

		[HttpPost]
		[Authorize(Roles = "SuperAdmin")]
		public IActionResult Create(CityModel city)
		{
			_cityService.Create(city);
			TempData["success"] = "Record Added successfully!";

			return RedirectToAction("Index");
		}


		[HttpPost]
		[Authorize(Roles = "SuperAdmin")]
		public IActionResult Update(CityModel city)
		{
			_cityService.Update(city);
			TempData["success"] = "Record updated successfully!";

			return RedirectToAction("Index");
		}


		[HttpPost]
		[Authorize(Roles = "SuperAdmin")]
		public IActionResult Delete(int CityId)
		{
			_cityService.Delete(CityId);
			TempData["success"] = "Record deleted successfully!";

			return RedirectToAction("Index");
		}


	}
}
