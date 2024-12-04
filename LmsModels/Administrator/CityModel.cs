using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsModels.Administrator
{
	public class CityModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public short CityId { get; set; }
		public string CityName { get; set; }
		public short StateId { get; set; }
		public string StateName { get; set; }
		public short CountryId { get; set; }
		public string CountryName { get; set; }
		public bool Status { get; set; }
		public string StatusLabel { get; set; }

	}
}
