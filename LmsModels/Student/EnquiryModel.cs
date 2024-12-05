using System.ComponentModel.DataAnnotations;

namespace LmsModels.Student
{
	public class EnquiryModel
	{
		public int EnquiryId { get; set; }
		public int FirmId { get; set; }
		public int BranchId { get; set; }

		[Display(Name = "Enquiry Date")]
		public DateTime EnquiryDate { get; set; }

		[Display(Name = "Candidate Name")]
		public string CandidateName { get; set; }

		[Display(Name = "Email Address")]
		public string EmailAddress { get; set; }

		[Display(Name = "Mobile Number")]
		public string MobileNumber { get; set; }

		[Display(Name = "City Name")]
		public int CityId { get; set; }

		[Display(Name = "City Name")]
		public string CityName { get; set; }

		[Display(Name = "Local Address")]
		public string LocalAddress { get; set; }

		[Display(Name = "Gender")]
		public string Gender { get; set; }

		[Display(Name = "Qualification")]
		public int QualificationId { get; set; }

		[Display(Name = "Qualification")]
		public string QualificationName { get; set; }

		[Display(Name = "Date of Birth")]
		public DateTime DateOfBirth { get; set; }

		[Display(Name = "Lead Sourse")]
		public int LeadSourceId { get; set; }

		[Display(Name = "Lead Sourse")]
		public string LeadSourceName { get; set; }

		[Display(Name = "Enquiry For")]
		public int EnquiryForId { get; set; }

		[Display(Name = "EnquiryFor Name")]
		public string EnquiryForName { get; set; }


		[Display(Name = "Branch Name")]
		public string BranchName { get; set; }

		[Display(Name = "Status")]
		public string? Status { get; set; }


		[Display(Name = "Remark")]
		public string Remark { get; set; }
	}
}