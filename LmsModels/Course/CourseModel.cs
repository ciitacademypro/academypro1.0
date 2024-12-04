using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsModels.Course
{
	public class CourseModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int CourseId { get; set; }
		public int FirmId { get; set; }
		public short CourseCategoryId { get; set; }
		public string CourseCategoryName { get; set; }
		public string CourseName { get; set; }
		public string CourseDescription { get; set; }
		public string CourseLevel { get; set; }
		public bool Status { get; set; }
		public string StatusLabel { get; set; }
		public short CourseOrder { get; set; }
	}

}
