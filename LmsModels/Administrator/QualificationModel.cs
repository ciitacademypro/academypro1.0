using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsModels.Administrator
{
	public class QualificationModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]

		[Key]
		public int QualificationId { get; set; }
		public string QualificationName { get; set; }
		public bool Status { get; set; }

		[NotMapped]
		public string? StatusLabel { get; set; }
		public DateTime? DeletedAt { get; set; }
		
	}
}
