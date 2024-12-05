using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LmsModels.Student
{
	public class StudentModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int StudentId { get; set; }

		[Required(ErrorMessage = "Student name is required.")]
		public string StudentName { get; set; }

		[Required(ErrorMessage = "Student name is required.")]

		public string StudentCode { get; set; }

		[Required(ErrorMessage = "mobile no is required.")]
		public string MobileNumber { get; set; }

		[Required(ErrorMessage = "email is required.")]
		public string EmailAddress { get; set; }
		[Required(ErrorMessage = "password required.")]
		public string Password { get; set; }

		public string? ProfilePhoto { get; set; }

		// public IFormFile ProfilePhotoFile { get; set; }
		public bool Status { get; set; }
		public string? StatusLabel { get; set; }
	}

	public class StudentAddModel
	{
        public string StudentName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
		public int EnquiryId { get; set; }
		public int CourseId { get; set; }

	}

}