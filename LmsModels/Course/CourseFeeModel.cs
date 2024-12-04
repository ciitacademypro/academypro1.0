using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsModels.Course
{
	public class CourseFeeModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		
		public int CourseFeeId { get; set; }
		public int CourseId { get; set; }
		public string CourseName { get; set; }
		public short TotalInstallments { get; set; }
		public float FeeAmount { get; set; }
		public float GstPercentage { get; set; }

		public float GstAmount { get; set; }
		public float TotalAmountWithGst { get; set; }
		
	}

	public class CourseFeeJson
	{
		public short txtTotalInstallments { get; set; }
		public float textGstPercentage { get; set; }
		public float txtTotalAmount { get; set; }
	}

}

//[{"txtTotalInstallments":1,"textGstPercentage":12,"txtTotalAmount":2000},{ "txtTotalInstallments":3,"textGstPercentage":18,"txtTotalAmount":5000}]

