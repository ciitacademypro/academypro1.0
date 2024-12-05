using LmsEnv;
using LmsModels;
using LmsModels.Student;
using LmsServices.Common;
using LmsServices.Student.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsServices.Student.Implemenatation
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly string connstring;
        public EnrollmentService()
        {
            connstring = DbConnect.DefaultConnection;
        }
        public int Create(StudentEnrollmentModel studentEnrollment)
        {

            var parameters = new List<KeyValuePair<string, object>>
            {
                new("@Type", "INSERT"),
                new("@StudentEnrollmentId", 0),
				new("@StudentId", studentEnrollment.StudentId),
				new("@FirmId", studentEnrollment.FirmId),
				new("@EnrollmentDate",studentEnrollment.EnrollmentDate),
                new("@CourseId",studentEnrollment.CourseId),
                new("@EnrollmentType", studentEnrollment.EnrollmentType),
                new("@PaymentStatus",studentEnrollment.PaymentStatus),
                new("@CourseFeeId",studentEnrollment.CourseFeeId),
                new("@DiscountCode", studentEnrollment.DiscountCode),
                new("@DiscountAmount", studentEnrollment.DiscountAmount),
                new("@PaidAmount",studentEnrollment.PaidAmount),
                new("@StartDate",studentEnrollment.StartDate),
                new("@DroppedDate",studentEnrollment.DroppedDate),
                new("@Remarks",studentEnrollment.Remarks)

            };

            return QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Enrollments]", parameters, "@LastInsertedId");

        }

        public void Delete(int id)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new("@Type", "DELETE"),
                new("@StudentEnrollmentId", id),
                new("@StudentId", 0),
                new("@EnrollmentDate",null),
                new("@CourseId",0),
                new("@EnrollmentType", ""),
                new("@PaymentStatus",""),
                new("@CourseFeeId",0),
                new("@DiscountCode", ""),
                new("@DiscountAmount", 0),
                new("@PaidAmount",0),
                new("@StartDate", null),
                new("@DroppedDate",null),
                new("@Remarks", "")
            };

            QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Enrollments]", parameters);

        }

        public List<StudentEnrollmentModel> GetAll(int firmId, bool? status = null)
        {
            return QueryService.Query(
         "sp_GetAll_Enrollments",
         reader =>
         {
             return new StudentEnrollmentModel
             {
                 StudentEnrollmentId = Convert.ToInt32(reader["StudentEnrollmentId"]),
                 StudentId = reader["StudentId"].ToString(),
                 StudentName = reader["StudentName"].ToString(),
                 EnrollmentDate = DateOnly.FromDateTime(Convert.ToDateTime(reader["EnrollmentDate"])),
                 CourseId = Convert.ToInt32(reader["CourseId"]),
                 CourseName = reader["CourseName"].ToString(),
                 EnrollmentType = reader["EnrollmentType"].ToString(),
                 PaymentStatus = reader["PaymentStatus"].ToString(),
                 CourseFeeId = Convert.ToInt32(reader["CourseFeeId"]),
                 DiscountCode = reader["DiscountCode"].ToString(),
                 DiscountAmount = Convert.ToSingle(reader["DiscountAmount"]),
                 PaidAmount = Convert.ToSingle(reader["PaidAmount"]),
                 StartDate = reader["StartDate"] != DBNull.Value
                            ? DateOnly.FromDateTime(Convert.ToDateTime(reader["StartDate"]))
                            : (DateOnly?)null,
                 //  StartDate = DateOnly.FromDateTime(Convert.ToDateTime(reader["StartDate"])),
                 //DroppedDate = DateOnly.FromDateTime(Convert.ToDateTime(reader["DroppedDate"])),
                 //DroppedDate = reader["DroppedDate"] as DateTime? is DateTime dt ? DateOnly.FromDateTime(dt) : (DateOnly?)null,

                 Remarks = reader["Remarks"].ToString(),
                 Status = Convert.ToBoolean(reader["Status"]),
                 StatusLabel = reader["StatusLabel"].ToString()
             };
         },
			new SqlParameter("@FirmId", firmId),
			new SqlParameter("@StudentEnrollmentId", 0),
			new SqlParameter("@Status", status)
         );
        }

        public StudentEnrollmentModel GetById(int id, int firmId)
        {

            var result = QueryService.Query(
         "sp_GetAll_Enrollments",
         reader =>
         {
             return new StudentEnrollmentModel
             {

                 StudentEnrollmentId = Convert.ToInt32(reader["StudentEnrollmentId"]),
                 StudentId = reader["StudentId"].ToString(),
                 StudentName = reader["StudentName"].ToString(),
                 EnrollmentDate = DateOnly.FromDateTime(Convert.ToDateTime(reader["EnrollmentDate"])),
                 CourseId = Convert.ToInt32(reader["CourseId"]),
                 CourseName = reader["CourseName"].ToString(),
                 EnrollmentType = reader["EnrollmentType"].ToString(),
                 PaymentStatus = reader["PaymentStatus"].ToString(),
                 CourseFeeId = Convert.ToInt32(reader["CourseFeeId"]),
                 DiscountCode = reader["DiscountCode"].ToString(),
                 DiscountAmount = Convert.ToSingle(reader["DiscountAmount"]),
                 PaidAmount = Convert.ToSingle(reader["PaidAmount"]),
                 StartDate = reader["StartDate"] != DBNull.Value
                            ? DateOnly.FromDateTime(Convert.ToDateTime(reader["StartDate"]))
                            : (DateOnly?)null,
                 DroppedDate = reader["DroppedDate"] != DBNull.Value
                            ? DateOnly.FromDateTime(Convert.ToDateTime(reader["DroppedDate"]))
                            : (DateOnly?)null,
                 //DroppedDate = reader["DroppedDate"] as DateTime? is DateTime dt ? DateOnly.FromDateTime(dt) : (DateOnly?)null,

                 Remarks = reader["Remarks"].ToString(),
                 Status = Convert.ToBoolean(reader["Status"]),
                 StatusLabel = reader["StatusLabel"].ToString()
                
             };
         },
            new SqlParameter("StudentEnrollmentId", id),
			new SqlParameter("@FirmId", firmId)
		 );
            return result?.FirstOrDefault();
        }

        public void Restore(int id)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new("@Type", "RESTORE"),
                new("@StudentEnrollmentId", id),
                new("@StudentId", 0),
                new("@EnrollmentDate",null),
                new("@CourseId",0),
                new("@EnrollmentType", ""),
                new("@PaymentStatus",""),
                new("@CourseFeeId",0),
                new("@DiscountCode", ""),
                new("@DiscountAmount", 0),
                new("@PaidAmount",0),
                new("@StartDate", null),
                new("@DroppedDate",null),
                new("@Remarks", "")

            };

            QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Enrollments]", parameters);
        }

        public void Update(StudentEnrollmentModel studentEnrollment)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new ("@Type", "UPDATE"),
                new("@StudentEnrollmentId", studentEnrollment.StudentEnrollmentId),
                new("@StudentId", studentEnrollment.StudentId),
                new("@EnrollmentDate",studentEnrollment.EnrollmentDate),
                new("@CourseId", studentEnrollment.CourseId),
                new("@EnrollmentType", studentEnrollment.EnrollmentType),
                new("@PaymentStatus",studentEnrollment.PaymentStatus),
                new("@CourseFeeId",studentEnrollment.CourseFeeId),
                new("@DiscountCode", studentEnrollment.DiscountCode),
                new("@DiscountAmount", studentEnrollment.DiscountAmount),
                new("@PaidAmount",studentEnrollment.PaidAmount),
                new("@StartDate",studentEnrollment.StartDate),
                new("@DroppedDate",studentEnrollment.DroppedDate),
                new("@Remarks",studentEnrollment.Remarks),

            };

            QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Enrollments]", parameters);

        }

        public async Task<List<EnrollmentViewModel>> GetEnrollmentWisePayments(int studentId)
        {
            var enrollments = new List<EnrollmentViewModel>();

            // Query service to fetch data from stored procedure
            var result = QueryService.Query(
                "sp_GetAll_EnrollmentWiseStudentPayments",
                reader =>
                {
                    return new EnrollmentWithPaymentResult
                    {
                        StudentEnrollmentId = Convert.ToInt32(reader["StudentEnrollmentId"]),
                        CourseName = reader["CourseName"].ToString(),
                        EnrollmentDate = Convert.ToDateTime(reader["EnrollmentDate"]).Date,
                        InstallmentCount = Convert.ToInt32(reader["InstallmentCount"]),
                        InstallmentAmount = Convert.ToDouble(reader["InstallmentAmount"]),
                        AmountPaid = Convert.ToDouble(reader["AmountPaid"]),
                        InstallmentDate = Convert.ToDateTime(reader["InstallmentDate"]).Date,
                        PaymentStatus = Convert.ToBoolean(reader["PaymentStatus"])  // Assuming this is whether the payment is fully paid
                    };
                },
                new SqlParameter("@StudentId", studentId)
            );

            // Group the result by StudentEnrollmentId to create a one-to-many relationship
            foreach (var enrollmentGroup in result.GroupBy(r => r.StudentEnrollmentId))
            {
                var enrollment = new EnrollmentViewModel
                {
                    StudentEnrollmentId = enrollmentGroup.Key,
                    CourseName = enrollmentGroup.First().CourseName,
                    EnrollmentDate = enrollmentGroup.First().EnrollmentDate,
                    Payments = enrollmentGroup.Select(p => new StudentPaymentViewModel
                    {
                        InstallmentCount = p.InstallmentCount,
                        InstallmentAmount = p.InstallmentAmount,
                        AmountPaid = p.AmountPaid,
                        InstallmentDate = p.InstallmentDate,
                        Status = p.PaymentStatus  // Assuming true means fully paid
                    }).ToList()
                };

                enrollments.Add(enrollment);
            }

            return enrollments;
        }




    }
}
