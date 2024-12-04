using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsModels.Admin
{
	public class BranchModel
	{

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int BranchId { get; set; }
		public int FirmId { get; set; }
		public string BranchName { get; set; }
		public bool Status { get; set; }
		public string StatusLabel { get; set; }

	}
}
