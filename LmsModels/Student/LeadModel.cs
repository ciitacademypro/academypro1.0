using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsModels.Student
{
	public class LeadModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int LeadId { get; set; }
		public int FirmId { get; set; }
		public int BranchId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string EmailAddress { get; set; }
		public string MobileNumber { get; set; }
		public int LeadSourceId { get; set; }
		public string LeadSourceName { get; set; }
		public int? CourseId { get; set; } // Nullable
		public string CourseName { get; set; }
		public int? QualificationId { get; set; } // Nullable
		public string QualificationName { get; set; }
		public string PassoutYear { get; set; }
		public string Status { get; set; }
		public string? Remark { get; set; }
		
		public string? AssignedTo { get; set; } // Nullable
		public string AssignedToName { get; set; }
		public string? AssignedBy { get; set; } // Nullable
		public string AssignedByName { get; set; }
		public DateTime? AssignedAt { get; set; } // Nullable
		public string? CreatedBy { get; set; } // Nullable
		public string CreatedByName { get; set; }
		public DateTime? CreatedAt { get; set; } // Nullable
		public string? UpdatedBy { get; set; } // Nullable
		public string UpdatedByName { get; set; }
		public DateTime? UpdatedAt { get; set; } // Nullable

	}
}
