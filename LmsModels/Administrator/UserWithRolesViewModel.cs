using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsModels.Administrator
{
	public class UserWithRolesViewModel
	{
		public string Id { get; set; }
		public int? FirmId { get; set; }
		public string? FirmName { get; set; }
		public int? BranchId { get; set; }
		public string? BranchName { get; set; }
		public string Name { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		

		[Display(Name = "Employee Code")]
		public string? Uid1 { get; set; }

		//public string? Uid2 { get; set; }
		public bool? Status { get; set; }
		public List<string> Roles { get; set; }

	}
}
