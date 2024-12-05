using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsModels.Student
{
    public class StudentEnrollmentModel
    {
        public int StudentEnrollmentId { get; set; }
		public int StudentId { get; set; }
		public string? StudentName { get; set; }
		public DateOnly EnrollmentDate { get; set; }
		public int CourseId { get; set; }
		public string? CourseName { get; set; }
		public string EnrollmentType { get; set; }
        public string? PaymentStatus { get; set; }
        public int CourseFeeId { get; set; }
        public string DiscountCode { get; set; }
        public float DiscountAmount { get; set; }
        public float PaidAmount { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? DroppedDate { get; set; }
        public string? Remarks { get; set; }
        public string? StatusLabel { get; set; }        
        public bool Status { get; set; }
    }


}
