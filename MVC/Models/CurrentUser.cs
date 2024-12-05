using LmsModels.Admin;
using LmsModels.Administrator;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MVC.Models
{
	public class CurrentUser
	{
		public string? Id { get; set; }
		public int? BranchId { get; set; }

		public string? BranchName { get; set; }
		public int? FirmId { get; set; }

		public string? FirmName { get; set; }
		public string? FirmShortName { get; set; }
		

		public string Name { get; set; }
		public string Email { get; set; }
		public string Roles { get; set; }


		public string? Uid1 { get; set; }

		public bool? Status { get; set; }

	}
}
