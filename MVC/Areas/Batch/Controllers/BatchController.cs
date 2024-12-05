using LmsModels.Batch;
using LmsServices.Batch.Implementations;
using LmsServices.Batch.Implmentations;
using LmsServices.Batch.Interfaces;
using LmsServices.Course.Implementations;
using LmsServices.Course.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Areas.Identity.Data;
using MVC.Models;

namespace lms.Areas.Batch.Controllers
{
	[Area("Batch")]
	public class BatchController : Controller
	{
		ICourseCategoryService _courseCategoryService;
		ICourseModuleService _courseModuleService; 
		IBatchService _batchService;
		IClassRoomService _classRoomService;
		IBatchScheduleService _batchScheduleService;
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly UserManager<AppUser> _userManager;
		private readonly AppDbContext _appDbContext;

		private int firmId;
		private int branchId;
		private string firmShortName;


		public BatchController(
			IHttpContextAccessor contextAccessor,
			UserManager<AppUser> userManager,
			AppDbContext appDbContext

			)
		{
			_courseCategoryService = new CourseCategoryService();
			_courseModuleService = new CourseModuleService();
			_batchService = new BatchService();
			_classRoomService = new ClassRoomService();
			_batchScheduleService = new BatchScheduleService();
			_contextAccessor = contextAccessor;
			_userManager = userManager;
			_appDbContext = appDbContext;


			var currentUser = _contextAccessor.HttpContext.Items["CurrentUser"] as CurrentUser;

			firmId = currentUser?.FirmId ?? 0;
			branchId = currentUser?.BranchId ?? 0;
			firmShortName = currentUser?.FirmShortName ?? "NA";

		}

		[Authorize]
		public IActionResult Index()
		{
			ViewBag.Batches = _batchService.GetAll(firmId);
			return View();
		}


		public async Task<IActionResult> Create()
		{


			var trainersRoleId = await _appDbContext.Roles
				.Where(r => r.Name.Contains("Trainer"))
				.Select(r => r.Id)
				.ToListAsync();
	
			var trainers = await _appDbContext.Users
				.Where(u => u.FirmId == firmId)
				.Where( u => _appDbContext.UserRoles
					.Any(ur => ur.UserId == u.Id && trainersRoleId.Contains(ur.RoleId)))
				.ToListAsync();

			ViewBag.Trainers = trainers;

			ViewBag.CourseModules = _courseModuleService.GetAll(firmId, 0,0);
            ViewBag.CourseCategories = _courseCategoryService.GetAll();
			ViewBag.ClassRooms = _classRoomService.GetAll(firmId); // ClassRoomId, ClassRoomName

			return View();
		}


		[HttpPost]
		public IActionResult Create(BatchModel batch,  List<BatchScheduleItem> batchScheduleItem){
			
			if (batch == null || batchScheduleItem == null)
			{
				return BadRequest("Invalid data.");
			}

			try
				{

					// add this dynamically afeter login
					batch.BranchId = 1;

					int lastInsertedId = _batchService.Create(batch, firmId);


					// Ensure BatchSchedules is properly initialized before setting properties
					if (batch.BatchSchedules == null)
					{
						batch.BatchSchedules = new BatchScheduleModelCreate();
					}

					// Update BatchSchedules properties
					batch.BatchSchedules.BranchId = batch.BranchId;
					batch.BatchSchedules.BatchId = lastInsertedId;


					// Add items from the request to BatchSchedules
					if (batchScheduleItem != null)
					{
						foreach (var item in batchScheduleItem) // Iterate over the Items collection
						{
							// Parse the string back to DateTime before assigning
							DateTime parsedDateTime;
							if (DateTime.TryParse(item.ExpectedDateTime.ToString("yyyy-MM-dd"), out parsedDateTime))
							{
								batch.BatchSchedules.Items.Add(new BatchScheduleItem
								{
									ExpectedDateTime = parsedDateTime,  // Now ExpectedDateTime is DateTime, not string
									ExpectedTrainerId = item.ExpectedTrainerId,
									ContentIds = item.ContentIds,
									ContentNames = item.ContentNames,
								});
							}
							else
							{
								// Handle the case where the date couldn't be parsed, for example:
								Console.WriteLine("Invalid date format.");
							}
						}
					}

					_batchScheduleService.Create(batch.BatchSchedules);
					
					// Return JSON response with success message
					return Json(new { success = true, message = "Batch created successfully!", data = lastInsertedId });
				}
				catch (Exception ex)
				{
					// Handle error, returning JSON with an error message
					return Json(new { success = false, message = "An error occurred: " + ex.Message });
				}

		}

	}
}
