using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.CodeAnalysis.Operations;
using MVC.Areas.Admin.Models;
using MVC.Areas.Administrator.Models;
using NuGet.Protocol.Plugins;

namespace MVC.Areas.Identity.Data;

// Add profile data for application users by adding properties to the AppUser class
public class AppUser : IdentityUser
{

	public int? BranchId { get; set; }

	[ForeignKey("BranchId")]
	public Branch? Branch { get; set; }

	[ValidateNever]
	[NotMapped]
	public string? BranchName { get; set; }
	public int? FirmId { get; set; }

	[ForeignKey("FirmId")]
	public Firm? Firm { get; set; }

	[ValidateNever]
	[NotMapped]
	public string? FirmName { get; set; }

	[ValidateNever]
	[NotMapped]
	public string? FirmShortName { get; set; }


	[Required(ErrorMessage = "Name is required")]
	[StringLength(60, MinimumLength = 2, ErrorMessage = "The {0} must be at {2} and at max {1} character long")]
	public string Name { get; set; }


	[StringLength(30, MinimumLength = 2, ErrorMessage = "The {0} must be at {2} and at max {1} character long")]
	public string? Uid1 { get; set; }

	[StringLength(30, MinimumLength = 2, ErrorMessage = "The {0} must be at {2} and at max {1} character long")]
	public string? Uid2 { get; set; }
	public bool? Status { get; set; }
	public DateTime? DeletedAt { get; set; }

	// Navigation property for user roles
	public ICollection<IdentityUserRole<string>> UserRoles { get; set; } = new List<IdentityUserRole<string>>();

}

