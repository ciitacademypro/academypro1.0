using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsModels.Administrator
{
	public class EnquiryForModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public short EnquiryForId { get; set; }
		public string EnquiryForName { get; set; }
		public bool Status { get; set; }
		public string StatusLabel { get; set; }
	}
}
