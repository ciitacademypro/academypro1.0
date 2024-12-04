using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsModels.Course
{
	public class CourseCategoryModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public short CourseCategoryId { get; set; }
		public short? ParentId { get; set; }
		public string? ParentCategoryName { get; set; }
		public string CourseCategoryName { get; set; } = null!;
		public bool Status { get; set; }
		public string StatusLabel { get; set; }
		public virtual CourseCategoryModel? ParentCategory { get; set; }
	}
}
