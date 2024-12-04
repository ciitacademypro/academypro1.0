using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsModels.Course
{
	public class CourseModuleModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int CourseModuleId { get; set; }
		public short CourseId { get; set; }
		public short CourseCategoryId { get; set; }
		public string? CourseCategoryName { get; set; }
		public string CourseName { get; set; }
		public string ModuleName { get; set; }
		public string ModuleDescription { get; set; }
		public short ModuleOrder { get; set; }
		public bool Status { get; set; }
		public string StatusLabel { get; set; }
	}
}
