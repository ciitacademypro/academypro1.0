using LmsModels.Administrator;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsModels
{
	public class UserViewModel
	{
		[Key]
		[ValidateNever]
		[StringLength(450)]
		public string Id { get; set; }

		[ValidateNever]
		public int? BranchId { get; set; }

		[ValidateNever]
		public string? BranchName { get; set; }

		[ValidateNever]
		public int? FirmId { get; set; }

		[ValidateNever]
		public string? FirmName { get; set; }


		[Required]
		[StringLength(60)]
		public string Name { get; set; }

		[ValidateNever]
		[StringLength(30)]
		public string? Uid1 { get; set; }

		[Required]
		public string PhoneNumber { get; set; }

		[Required(ErrorMessage = "Email is required.")]
		[StringLength(256)]
		public string Email { get; set; }



		[ValidateNever]
		public bool? Status { get; set; }
		
		public string? StatusLabel { get; set; }

		[ValidateNever]
		public string RoleId { get; set; }

		[ValidateNever]
		[StringLength(60)]
		public string RoleName { get; set; }


	}
}
