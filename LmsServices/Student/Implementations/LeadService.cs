using LmsEnv;
using LmsModels.Admin;
using LmsModels.Student;
using LmsServices.Common;
using LmsServices.Student.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace LmsServices.Student.Implementations
{
	public class LeadService : ILeadService
	{
		private readonly string connString;
		public LeadService()
		{
			connString = DbConnect.DefaultConnection;
		}

		public void Create(LeadModel lead)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "INSERT"),
				new ("@LeadId", 0),
				new ("@FirmId", lead.FirmId),
				new ("@BranchId", lead.BranchId),
				new ("@FirstName", lead.FirstName),
				new ("@LastName", lead.LastName),
				new ("@EmailAddress", lead.EmailAddress),
				new ("@MobileNumber", lead.MobileNumber),
				new ("@LeadSourceId", lead.LeadSourceId),
				new ("@CourseId", lead.CourseId),
				new ("@QualificationId", lead.QualificationId),
				new ("@PassoutYear", lead.PassoutYear),
				new ("@Status", lead.Status)
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Leads]", parameters);
		}

		public void AssignTo(int employeeId, int leadId,  int currentUser, int firmId)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{

				new ("@Type", "ASSIGNTO"),
				new ("@LeadId", leadId),
				new ("@FirmId", firmId),
				new ("@FirstName", ""),
				new ("@LastName", ""),
				new ("@EmailAddress", ""),
				new ("@MobileNumber", ""),
				new ("@LeadSourceId", 0),
				new ("@CourseId", 0),
				new ("@QualificationId", 0),
				new ("@PassoutYear", ""),
				new ("@Status", ""),
				new ("@CreatedUpdatedBy", currentUser),
				new ("@AssignedTo", employeeId)

			};
			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Leads]", parameters);

		}

		public void UpdateRemark(int leadId, string status, string remark, int currentUser)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@LeadId", leadId),
				new ("@Status", status),
				new ("@Remark", remark),
				new ("@UpdatedBy", currentUser)
			};
			QueryService.NonQuery("[sp_UpdateRemark_Leads]", parameters);
		}

		public void Delete(int id, int firmId)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "DELETE"),
				new ("@LeadId", id),
				new ("@FirstName", ""),
				new ("@LastName", ""),
				new ("@EmailAddress", ""),
				new ("@MobileNumber", ""),
				new ("@LeadSourceId", 0),
				new ("@CourseId", 0),
				new ("@QualificationId", 0),

				new ("@PassoutYear", ""),
				new ("@Status", "")
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Leads]", parameters);
		}

		public List<LeadModel> GetAll(int firmId, int LeadId=0, string? SearchByColumn = null, int? SearchByValue = 0)
		{
			SearchByValue = SearchByValue == -1 ? null : SearchByValue;

			return QueryService.Query(
				"sp_GetAll_Leads",
				reader =>
				{
					return new LeadModel
					{
						LeadId = Convert.ToInt32(reader["LeadId"]),
						FirstName = reader["FirstName"].ToString(),
						LastName = reader["LastName"].ToString(),
						EmailAddress = reader["EmailAddress"].ToString(),
						MobileNumber = reader["MobileNumber"].ToString(),
						LeadSourceId = Convert.ToInt32(reader["LeadSourceId"]),
						LeadSourceName = reader["LeadSourceName"].ToString(),
						CourseId = reader["CourseId"] != DBNull.Value ? (int?)Convert.ToInt32(reader["CourseId"]) : null,
						CourseName = reader["CourseName"].ToString(),
						QualificationId = reader["QualificationId"] != DBNull.Value ? (int?)Convert.ToInt32(reader["QualificationId"]) : null,
						QualificationName = reader["QualificationName"].ToString(),
						PassoutYear = reader["PassoutYear"].ToString(),
						Status = reader["Status"].ToString(),
						Remark = reader["Remark"].ToString(),
						AssignedTo = reader["AssignedTo"] != DBNull.Value ? reader["AssignedTo"].ToString() : null,
						AssignedToName = reader["AssignedToName"].ToString(),
						AssignedBy = reader["AssignedBy"] != DBNull.Value ? reader["AssignedBy"].ToString() : null,
						AssignedByName = reader["AssignedByName"].ToString(),
						AssignedAt = reader["AssignedAt"] != DBNull.Value ? (DateTime?)reader["AssignedAt"] : null,
						CreatedBy = reader["CreatedBy"] != DBNull.Value ? reader["CreatedBy"].ToString() : null,
						CreatedByName = reader["CreatedByName"].ToString(),
						CreatedAt = reader["CreatedAt"] != DBNull.Value ? (DateTime?)reader["CreatedAt"] : null,
						UpdatedBy = reader["UpdatedBy"] != DBNull.Value ? reader["UpdatedBy"].ToString() : null,
						UpdatedByName = reader["UpdatedByName"].ToString(),
						UpdatedAt = reader["UpdatedAt"] != DBNull.Value ? (DateTime?)reader["UpdatedAt"] : null

					};
				},

				new SqlParameter("@FirmId", firmId),
				new SqlParameter("@LeadId", LeadId),
				new SqlParameter("@SearchByColumn", SearchByColumn),
				new SqlParameter("@SearchByValue", SearchByValue)
			);

		}

		public LeadModel GetById(int id, int firmId)
		{
			var result =  QueryService.Query(
				"sp_GetAll_Leads",
				reader =>
				{
					return new LeadModel
					{
						LeadId = Convert.ToInt32(reader["LeadId"]),
						FirstName = reader["FirstName"].ToString(),
						LastName = reader["LastName"].ToString(),
						EmailAddress = reader["EmailAddress"].ToString(),
						MobileNumber = reader["MobileNumber"].ToString(),
						LeadSourceId = Convert.ToInt32(reader["LeadSourceId"]),
						LeadSourceName = reader["LeadSourceName"].ToString(),
						CourseId = Convert.ToInt32(reader["CourseId"]),
						CourseName = reader["CourseName"].ToString(),
						QualificationId = Convert.ToInt32(reader["QualificationId"]),
						QualificationName = reader["QualificationName"].ToString(),
						PassoutYear = reader["PassoutYear"].ToString(),
						Status = reader["Status"].ToString(),
						Remark = reader["Remark"].ToString(),
						AssignedTo = reader["AssignedTo"].ToString(),
						AssignedToName = reader["AssignedToName"].ToString(),
						AssignedBy = reader["AssignedBy"].ToString(),
						AssignedByName = reader["AssignedByName"].ToString(),
						AssignedAt = reader["AssignedAt"] != DBNull.Value ? (DateTime)reader["AssignedAt"] : default(DateTime),
						CreatedBy = reader["CreatedBy"].ToString(),
						CreatedByName = reader["CreatedByName"].ToString(),
						CreatedAt = reader["CreatedAt"] != DBNull.Value ? (DateTime)reader["CreatedAt"] : default(DateTime),

						UpdatedBy = reader["UpdatedBy"].ToString(),
						UpdatedByName = reader["UpdatedByName"].ToString(),
						UpdatedAt = reader["UpdatedAt"] != DBNull.Value ? (DateTime)reader["UpdatedAt"] : default(DateTime)
					};
				},
				new SqlParameter("@FirmId", firmId),
				new SqlParameter("@LeadId", id),
				new SqlParameter("@SearchByColumn", null),
				new SqlParameter("@SearchByValue", null)

			);
			return result?.FirstOrDefault();
		}

		public void Restore(int id, int firmId)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "RESTORE"),
				new ("@LeadId", id),
				new ("@FirmId", firmId),
				new ("@FirstName", ""),
				new ("@LastName", ""),
				new ("@EmailAddress", ""),
				new ("@MobileNumber", ""),
				new ("@LeadSourceId", 0),
				new ("@CourseId", 0),
				new ("@QualificationId", 0),
				new ("@PassoutYear", ""),
				new ("@Status", "")
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Leads]", parameters);

		}

		public void Update(LeadModel lead)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "UPDATE"),
				new ("@FirmId", lead.FirmId),
				new ("@LeadId", lead.LeadId),
				new ("@FirstName", lead.FirstName),
				new ("@LastName", lead.LastName),
				new ("@EmailAddress", lead.EmailAddress),
				new ("@MobileNumber", lead.MobileNumber),
				new ("@LeadSourceId", lead.LeadSourceId),
				new ("@CourseId", lead.CourseId),
				new ("@QualificationId", lead.QualificationId),
				new ("@PassoutYear", lead.PassoutYear),
				new ("@Status", lead.Status)
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Leads]", parameters);

		}
	}
}
