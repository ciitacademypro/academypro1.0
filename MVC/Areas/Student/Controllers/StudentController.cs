
using LmsModels.Student;
using LmsServices.Common;
using LmsServices.Course.Implementations;
using LmsServices.Course.Interfaces;
using LmsServices.Student.Implemenatation;
using LmsServices.Student.Implementations;
using LmsServices.Student.Implmentations;
using LmsServices.Student.Interface;
using LmsServices.Student.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Areas.Identity.Data;
using MVC.Models;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace lms.Areas.Student.Controllers
{
	[Area("Student")]
	public class StudentController : Controller
	{
		private readonly IStudentService _studentService;
		private readonly IEnquiryService _enquiryService;
		private readonly IEnrollmentService _enrollmentService;
		private readonly ICourseCategoryService _courseCategoryService;
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly IUserStore<AppUser> _userStore;
		private readonly UserManager<AppUser> _userManager;
		private readonly IUserEmailStore<AppUser> _emailStore;


		private int currentUserId;
		private int firmId;
		private int branchId;
		private string firmShortName;



		public StudentController(
			IHttpContextAccessor contextAccessor,
			IUserStore<AppUser> userStore,
			UserManager<AppUser> userManager

		)
		{
			_studentService = new StudentService();
			_enquiryService = new EnquiryService();
			_enrollmentService = new EnrollmentService();
			_courseCategoryService = new CourseCategoryService();

			_contextAccessor = contextAccessor;
			_userStore = userStore;
			_userManager = userManager;
			_emailStore = GetEmailStore();


			var currentUser = _contextAccessor.HttpContext.Items["CurrentUser"] as CurrentUser;

			firmId = currentUser?.FirmId ?? 0;
			branchId = currentUser?.BranchId ?? 0;
			firmShortName = currentUser?.FirmShortName ?? "NA";
			string? currentUserId = currentUser?.Id;
		}

		public IActionResult Index(int status = -1)
		{
			ViewBag.CourseCategories = _courseCategoryService.GetAll();

			// Fetch all students once
				var allStudents = _studentService.GetAll(firmId);
				var allEnrollment = _enrollmentService.GetAll(firmId);

				// Calculate counts from the fetched list
				ViewBag.studentListAllCount = allStudents.Count();
				ViewBag.studentListActiveCount = allStudents.Count(s => s.Status == true);
				ViewBag.studentListInactiveCount = allStudents.Count(s => s.Status == false);

				ViewBag.enrollmentAllCount = allEnrollment.Count();
				ViewBag.enrollmentActiveCount = allEnrollment.Count(s => s.Status == true);
				ViewBag.enrollmentInactiveCount = allEnrollment.Count(s => s.Status == false);


				bool statusBool = status != 0;
				
				// Filter based on status
				List<StudentModel> studentList = status == -1 ? allStudents : allStudents.Where(s => s.Status == statusBool).ToList();

			return View(studentList);
		}

		public async Task<IActionResult> Details(int id)
		{
			StudentModel student = _studentService.GetById(id, firmId);
			if (student == null)
			{
				TempData["msg"] = "Student not found.";
				return RedirectToAction("Index");
			}


				var enrollments = await _enrollmentService.GetEnrollmentWisePayments(id);

				ViewBag.payments = enrollments;

				// Find the first enrollment and its first payment with status = 0
				var firstPendingEnrollment = enrollments
					.Where(e => e.Payments.Any(p => p.Status == false)) // Filter enrollments with pending payments
					.Select(e => new
					{
						e.StudentEnrollmentId,  // Include the enrollment ID
						Payment = e.Payments.FirstOrDefault(p => p.Status == false) // Get the first payment with status = 0
					})
					.FirstOrDefault();

				// Pass the data to the view
				ViewBag.FirstPendingPayment = firstPendingEnrollment?.Payment;
				ViewBag.FirstPendingEnrollmentId = firstPendingEnrollment?.StudentEnrollmentId;

				if(student.Status == false){
					return RedirectToAction("Index");
				}

				return View(student);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult UpdateProfilePhoto(int studentId, IFormFile profilePhoto)
		{
			var student = _studentService.GetById(studentId, firmId);
			if (student == null)
			{
				TempData["msg"] = "Student not found.";
				return RedirectToAction("Index");
			}


			string uploadPath = $@"uploads\students\{studentId}\ProfilePhoto";

			MultiMediaService mms = new MultiMediaService();
			string photoName = mms.ImageUpload(profilePhoto, uploadPath);
			_studentService.UpdateProfilePhoto(studentId, photoName);

/*
			if (profilePhoto != null && profilePhoto.Length > 0)
			{
				string rootFolder = Path.Combine(Directory.GetCurrentDirectory(), uploadPath);
				string studentFolder = Path.Combine(uploadPath, student.StudentName);

				if (!Directory.Exists(studentFolder))
				{
					Directory.CreateDirectory(studentFolder);
				}

				string photoName = student.StudentName + Path.GetExtension(profilePhoto.FileName);
				string photoPath = Path.Combine(studentFolder, photoName);

				using (var stream = new FileStream(photoPath, FileMode.Create))
				{
					profilePhoto.CopyTo(stream);
				}

				_studentService.UpdateProfilePhoto(studentId, photoName);
				TempData["success"] = "Profile photo updated successfully.";
			}
			else
			{
				TempData["msg"] = "Please select a photo to upload.";
			}
*/
			TempData["success"] = "Profile photo updated successfully.";

			return RedirectToAction("Details", new { id = studentId });
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ChangePassword(int studentId, string Password, string ConfirmPassword)
		{
			var student = _studentService.GetById(studentId, firmId);
			if (student == null)
			{
				TempData["msg"] = "Student not found.";
				return RedirectToAction("Index");
			}

			bool isUpdated = _studentService.ChangePassword(studentId, Password);
			if (isUpdated)
			{
				TempData["success"] = "Password changed successfully.";
			}
			else
			{
				TempData["msg"] = "Failed to change the password. Please try again.";
			}


			return RedirectToAction("Details", new { id = studentId });
		}

		public IActionResult Create()
		{
            return View("~/Areas/Student/Views/Student/Create.cshtml");
		}


		[HttpPost]
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(StudentAddModel model)
		{

            if (!ModelState.IsValid)
			{
				int totalUsers = CommonService.RowCountFirmUser(firmId);
				totalUsers++;
				string EmployeeCode = $"{firmShortName}-s-{(totalUsers):000000}"; // Ensures leading zeros up to 6 digits


				var user = CreateUser();
				await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
				await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);

				// Set the Name property
				user.Name = model.Name;
				user.FirmId = firmId;
				user.BranchId = model.BranchId;
				user.Uid1 = EmployeeCode;
				user.PhoneNumber = model.PhoneNumber;
				user.Status = true;

				var result = await _userManager.CreateAsync(user, "Admin@123");

				if (result.Succeeded)
				{
					//await _userManager.AddToRoleAsync(user, model.Roles);

						var roleResult = await _userManager.AddToRolesAsync(user, new List<string> { "Student" });

						if (roleResult.Succeeded)
						{
							TempData["success"] = "User created and roles assigned successfully!";

							// Update Enquiry Status
							var oldEnquiry = _enquiryService.GetById(model.EnquiryId, firmId, branchId);
							oldEnquiry.Status = "Registered";
							_enquiryService.Update(oldEnquiry);


							return RedirectToAction("Index");
						}
						else
						{
							TempData["error"] = "User created, but assigning roles failed.";
							foreach (var error in roleResult.Errors)
							{
								ModelState.AddModelError("Roles", error.Description);
							}
							return View(model);
						}

				}

				return View(model);

			}
		
			TempData["success"] = "Student Added successfully";

			return RedirectToAction("Index");
		}

		// [HttpGet]
		// public IActionResult Edit(int id)
		// {
		// 	StudentModel student = _studentService.GetById(id);
		// 	if (student == null)
		// 	{
		// 		TempData["msg"] = "Student not found.";
		// 		return RedirectToAction("Index");
		// 	}
		// 	return View(student);
		// }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(int StudentId, string StudentName, string MobileNumber, string EmailAddress, string redirectTo="Index")
		{

			var student = new StudentModel{
				StudentId = StudentId,
				StudentName = StudentName,
				MobileNumber = MobileNumber,
				EmailAddress = EmailAddress
			};

			if (!ModelState.IsValid)
			{
				// Get the referring URL (the previous page)
				var refererUrl = Request.Headers["Referer"].ToString();
				
				// If referer exists, redirect back to it
				if (!string.IsNullOrEmpty(refererUrl))
				{
					return Redirect(refererUrl);
				}

				// If no referer, you can default to another action like Index
				if(redirectTo == "Index")
				{
					return RedirectToAction("Index");
				}else{
					return RedirectToAction("Details", new { id = StudentId });
				}
			}

			student.Status = redirectTo == "Index"?false:true;

			_studentService.Update(student);
			TempData["success"] = "Student updated successfully.";

			if(redirectTo == "Index")
			{
				return RedirectToAction("Index");
			}else{
			return RedirectToAction("Details", new { id = StudentId });
			}

		}

		public IActionResult Delete(int id)
		{
			var student = _studentService.GetById(id, firmId);
			if (student != null)
			{
				_studentService.Delete(id);
				TempData["success"] = "Student deleted successfully.";
			}
			else
			{
				TempData["msg"] = "Student not found.";
			}
			return RedirectToAction("Index");
		}

		private AppUser CreateUser()
		{
			try
			{
				return Activator.CreateInstance<AppUser>();
			}
			catch
			{
				throw new InvalidOperationException($"Can't create an instance of '{nameof(AppUser)}'. " +
					$"Ensure that '{nameof(AppUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
					$"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
			}
		}

		private IUserEmailStore<AppUser> GetEmailStore()
		{
			if (!_userManager.SupportsUserEmail)
			{
				throw new NotSupportedException("The default UI requires a user store with email support.");
			}
			return (IUserEmailStore<AppUser>)_userStore;
		}

	}
}

