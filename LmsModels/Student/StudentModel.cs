using LmsModels.Administrator;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
        public string Name { get; set; }
        public string Email { get; set; }
		public int EnquiryId { get; set; }
		public int CourseId { get; set; }
		public int BranchId { get; set; }
		
		[ValidateNever]
		public string PasswordHash { get; set; }

		[Required]
		public string PhoneNumber { get; set; }

		[ValidateNever]
		[StringLength(60)]
		public List<string> Roles { get; set; }


		[ValidateNever]
		// Navigation property for related Firm
		[ForeignKey("FirmId")]
		public virtual FirmModel Firm { get; set; }


	}

}