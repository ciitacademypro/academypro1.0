using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LmsModels.Administrator;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LmsModels.Employee
{
	public class EmployeeModel
	{
		[Key]
		[ValidateNever]
		[StringLength(450)]
		public string Id { get; set; }

		[ValidateNever]
		public int? BranchId { get; set; }

		[ValidateNever]
		public int? FirmId { get; set; }

		[Required]
		[StringLength(60)]
		public string Name { get; set; }

		[ValidateNever]
		[StringLength(30)]
		public string? Uid1 { get; set; }

		[ValidateNever]
		public bool? Status { get; set; }


		[ValidateNever]
		[StringLength(256)]
		public string UserName { get; set; }


		[Required(ErrorMessage = "Email is required.")]
		[StringLength(256)]
		public string Email { get; set; }

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
