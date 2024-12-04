using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MVC.Areas.Identity.Data;

namespace MVC.Areas.Administrator.Models
{
	public class Firm
	{

		[Key] // Specifies the primary key
		public int FirmId { get; set; }

		[Required]
		[StringLength(8)]
		public string FirmShortName { get; set; }

		[Required]
		[StringLength(50)]
		public string FirmName { get; set; }

		[StringLength(100)]
		public string? FirmLogo { get; set; }

		[Required]
		[EmailAddress]
		[StringLength(50)]
		public string FirmEmail { get; set; }

		[Required]
		[StringLength(13)]
		public string? FirmContact { get; set; }
		public string? FirmWeb { get; set; }
		public string? FirmDescription { get; set; }

		[DefaultValue("active")] // Default value for the Status column
		public string? Status { get; set; } // comment('inactive:Fresh firm, active:live firm, suspended:subscription-end');

		[Column(TypeName = "datetime2")]
		[DefaultValue(null)] // Defaults to null
		public DateTime DeletedAt { get; set; }

		public ICollection<AppUser>? Users { get; set; }

	}
}
