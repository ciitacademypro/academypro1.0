using MVC.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Areas.Admin.Models
{
	public class Branch
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int BranchId { get; set; }
		public int FirmId { get; set; }
		public string BranchName { get; set; }
		public bool Status { get; set; }
		//public string? StatusLabel { get; set; }

		public ICollection<AppUser>? Users { get; set; }

	}
}
