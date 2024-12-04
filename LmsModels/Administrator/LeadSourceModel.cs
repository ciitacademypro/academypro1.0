using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsModels.Administrator
{
	public class LeadSourceModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public short LeadSourceId { get; set; }
		public string LeadSourceName { get; set; }
		public bool Status { get; set; }
		public string StatusLabel { get; set; }
	}
}
