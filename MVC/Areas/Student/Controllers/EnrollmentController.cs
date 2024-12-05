using LmsModels.Student;
using LmsServices.Common;
using LmsServices.Course.Implementations;
using LmsServices.Course.Interfaces;
using LmsServices.Student.Implemenatation;
using LmsServices.Student.Implementations;
using LmsServices.Student.Interface;
using LmsServices.Student.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC.Areas.Administrator.Models;
using MVC.Areas.Identity.Data;
using MVC.Models;


namespace lms.Areas.Student.Controllers
{
    [Area("Student")]
    public class EnrollmentController : Controller
    {
        IEnrollmentService _enrollmentService;
        ICourseService _courseService;
        IStudentPaymentService _studentPaymentService;
        IStudentService _studentService;
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly UserManager<AppUser> _userManager;

		private int currentUserId;
		private int firmId;
		private int branchId;
		private string firmShortName;

		public EnrollmentController(
			IHttpContextAccessor contextAccessor,
			UserManager<AppUser> userManager
			)
		{
            _enrollmentService = new EnrollmentService();
            _courseService = new CourseService();
            _studentPaymentService = new StudentPaymentService();
            _studentService = new StudentService();

			_contextAccessor = contextAccessor;
			_userManager = userManager;

			var currentUser = _contextAccessor.HttpContext.Items["CurrentUser"] as CurrentUser;

			firmId = currentUser?.FirmId ?? 0;
			branchId = currentUser?.BranchId ?? 0;
			firmShortName = currentUser?.FirmShortName ?? "NA";
			string? currentUserId = currentUser?.Id;

		}
		public IActionResult Index()
        {
            List<StudentEnrollmentModel> studentEnrollment = _enrollmentService.GetAll(firmId);
            return View(studentEnrollment);
        }
        [HttpGet]
        public IActionResult Create()
        {

            return View();
            //return View("~/Areas/Student/Views/Enrollment/CreateEnrollment.cshtml");
        }


            [HttpPost]
            public IActionResult PayInstallment( int StudentId, int EnrollmentId, float AmountPaid, string PayMode, string TransactionId, IFormFile ScreenShot )
            {

                string uploadPath = $@"uploads\students\{StudentId}\transactions";
                MultiMediaService mms = new MultiMediaService();
                string photoName = mms.ImageUpload(ScreenShot, uploadPath);
                // _studentService.UpdateProfilePhoto(StudentId, photoName);

                var pay = new PayInstallmentModel{
                    EnrollmentId = EnrollmentId,
                    AmountPaid = AmountPaid,
                    PayMode = PayMode,
                    TransactionId = TransactionId,
                    Screenshot =  photoName
                };

                _studentPaymentService.PayInstallment(pay);

                return RedirectToAction("Details", "Student", new { area = "Student", id = StudentId });

                
            }


        [HttpPost]
        public IActionResult Create(StudentEnrollmentModel student)
        {
            string PayMode =  student.PaymentStatus;
            // Add Studet Enrollment
            student.PaymentStatus = "Paid";
            student.EnrollmentDate = DateOnly.FromDateTime(DateTime.Now);

            int lastStudentEnrollmentId = _enrollmentService.Create(student);
            
            // Add Installments Cycle
            int courseFeeId = student.CourseFeeId;
            var courseFee = _courseService.GetFeesById(courseFeeId);

            List<int> installmentAmounts = CommonService.SplitInstallments((int)student.DiscountAmount, courseFee.TotalInstallments);
            int InstallmentNumber = 1;

            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            DateOnly firstInstallmentDate = new DateOnly(today.Year, today.Month, 5);
            
            if (today.Day >= 5)
            {
                firstInstallmentDate = firstInstallmentDate.AddMonths(1);
            }
            
            foreach (var amount in installmentAmounts)
            {
                var studentPayment = new StudentPaymentModel{
                    StudentEnrollmentId = lastStudentEnrollmentId,
                    InstallmentCount = InstallmentNumber,
                    InstallmentAmount = amount,
                    InstallmentDate = firstInstallmentDate.AddMonths(InstallmentNumber - 1), // Add months based on the installment number
                    Status = false
                };
                InstallmentNumber++;

                _studentPaymentService.Create(studentPayment);
            }

            // activate student
            _studentService.ToggleStatus(student.StudentId, true);
            ViewBag.msg = "Enrollment Added Succeefully";

            
            // pay registration amount here

                var pay = new PayInstallmentModel{
                    EnrollmentId = lastStudentEnrollmentId,
                    AmountPaid = student.PaidAmount,
                    PayMode = PayMode,
                    TransactionId = student.Remarks,
                    Screenshot =  "default.png"
                };

                _studentPaymentService.PayInstallment(pay);


            // registration amound pay end here
            



            return RedirectToAction("Index", "Student", new { area = "Student" }); // other redirect srd

            //            return View("~/Areas/Student/Views/Enrollment/CreateEnrollment.cshtml");
        }



        [HttpGet]
        public IActionResult Edit(int id)
        {
            StudentEnrollmentModel studentEnrollment = _enrollmentService.GetById(id, firmId);
            return View(studentEnrollment);
        }
        [HttpPost]
        public IActionResult Edit(StudentEnrollmentModel studentEnrollment)
        {
            _enrollmentService.Update(studentEnrollment);
            return RedirectToAction("Index");
            // return View("~/Areas/Student/Views/Enrollment/CreateEnrollment.cshtml");
        }
        public IActionResult Delete(int id)
        {
            _enrollmentService.Delete(id);
            //return View("~/Areas/Student/Views/Enrollment/CreateEnrollment.cshtml");
            return RedirectToAction("Index");

        }
    }
}
