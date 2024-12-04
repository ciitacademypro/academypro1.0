using LmsModels.Administrator;
using LmsServices.Admin.Implmentations;
using LmsServices.Admin.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lms.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class StateController : Controller
    {
        private readonly IStateService _stateService;
        private readonly ICountryService _countryService;


		public StateController()
        {
            _stateService = new StateService();
			_countryService = new CountryService();
        }

        public IActionResult Index()
        {
            ViewBag.States = _stateService.GetAll();
            ViewBag.Countries = _countryService.GetAll();
            return View();
        }

        [HttpPost]
        public IActionResult Create(StateModel state)
        {
            _stateService.Create(state);
            TempData["success"] = "Record Added successfully!";

            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult Update(StateModel state)
        {
            _stateService.Update(state);
            TempData["success"] = "Record updated successfully!";

            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult Delete(int stateId)
        {
            _stateService.Delete(stateId);
            TempData["success"] = "Record deleted successfully!";

            return RedirectToAction("Index");
        }

		public IActionResult GetIdNameList(int id)
		{
			var states = _stateService.GetByCountryId(id); // StateId, CountryId
			if (states == null || !states.Any())
			{
				return Json(new object[] { }); // Return an empty JSON array
			}

			// Select only the StateId and StateName
			var idNameList = states.Select(c => new
			{
				c.StateId,
				c.StateName
			}).ToList();
			return Json(idNameList);
		}

	}
}
