using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsModels.Administrator
{
	public class CountryModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public short CountryId { get; set; }
		public string CountryName { get; set; }
		public bool Status { get; set; }
		public string StatusLabel { get; set; }
	}
}
