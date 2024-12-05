using LmsEnv;
using LmsServices.Common;
using Microsoft.Data.SqlClient;
using LmsServices.Student.Interfaces;
using LmsModels.Student;

namespace LmsServices.Student.Implmentations
{
	public class EnquiryService : IEnquiryService
	{
		private readonly string connString;
		public EnquiryService()
		{
			connString = DbConnect.DefaultConnection;
		}

		public void Create(EnquiryModel Enquiry)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "INSERT"),
				new ("@EnquiryId", 0),
				new ("@FirmId", Enquiry.FirmId),
				new ("@BranchId", Enquiry.BranchId),
				new ("@EnquiryDate", Enquiry.EnquiryDate),
				new ("@CandidateName", Enquiry.CandidateName),
				new ("@EmailAddress", Enquiry.EmailAddress),
				new ("@MobileNumber", Enquiry.MobileNumber),
				new ("@CityId", Enquiry.CityId),
				new ("@LocalAddress", Enquiry.LocalAddress),
				new ("@Gender", Enquiry.Gender),
				new ("@QualificationId", Enquiry.QualificationId),
				new ("@DateOfBirth", Enquiry.DateOfBirth),
				new ("@LeadSourceId", Enquiry.LeadSourceId),
				new ("@EnquiryForId", Enquiry.EnquiryForId),
				new ("@Status", Enquiry.Status??"New"),
				new ("@Remark", Enquiry.Remark),
				new ("@LastInsertedId", 0)
				
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Enquiries]", parameters);
		}




		public void Delete(int id, int firmId)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "DELETE"),
				new ("@EnquiryId", id),
				new ("@FirmId", firmId),
				new ("@EnquiryDate", 0),
				new ("@CandidateName", ""),
				new ("@EmailAddress", ""),
				new ("@MobileNumber", ""),
				new ("@CityId", 0),
				new ("@LocalAddress", ""),
				new ("@Gender", ""),
				new ("@QualificationId", 0),
				new ("@DateOfBirth", 0),
				new ("@LeadSourceId", 0),
				new ("@EnquiryForId", 0),
				new ("@BranchId", 0),
				new ("@Remark", "")
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Enquiries]", parameters);
		}


		// EnquiryDate = reader["EnquiryDate"] != DBNull.Value? (DateTime) reader["EnquiryDate"] : default(DateTime);


		public List<EnquiryModel> GetAll(int firmId, int branchId = 0)
		{
			return QueryService.Query(
				"sp_GetAll_Enquiries",
				reader =>
				{
					return new EnquiryModel
					{
						EnquiryId = Convert.ToInt32(reader["EnquiryId"]),
						 EnquiryDate = reader["EnquiryDate"] != DBNull.Value ? (DateTime)reader["EnquiryDate"] : default(DateTime),
						// EnquiryDate =	 reader["EnquiryDate"] != DBNull.Value? ((DateTime)reader["EnquiryDate"]).ToString("dd MMM yyyy"): default(DateTime),
						CandidateName = reader["CandidateName"].ToString(),
						EmailAddress = reader["EmailAddress"].ToString(),
						MobileNumber = reader["MobileNumber"].ToString(),
						CityId = Convert.ToInt32(reader["CityId"]),
						CityName = reader["CityName"].ToString(),
						LocalAddress = reader["LocalAddress"].ToString(),
						Gender = reader["Gender"].ToString(),
						QualificationId = Convert.ToInt32(reader["QualificationId"]),
						QualificationName = reader["QualificationName"].ToString(),
						DateOfBirth = reader["DateOfBirth"] != DBNull.Value ? (DateTime)reader["DateOfBirth"] : default(DateTime),
						LeadSourceId = Convert.ToInt32(reader["LeadSourceId"]),
						LeadSourceName = reader["LeadSourceName"].ToString(),
						EnquiryForId = Convert.ToInt32(reader["EnquiryForId"]),
						EnquiryForName = reader["EnquiryForName"].ToString(),
						BranchId = Convert.ToInt32(reader["BranchId"]),
						BranchName = reader["BranchName"].ToString(),
						Remark = reader["Remark"].ToString(),
						Status = reader["Status"].ToString(),

					};
				},
				new SqlParameter("@EnquiryId", 0),
				new SqlParameter("@FirmId", firmId),
				new SqlParameter("@BranchId", branchId)
			);
		}



		public EnquiryModel GetById(int id, int firmId, int branchId)
		{
			var result = QueryService.Query(
				"sp_GetAll_Enquiries",
				reader =>
				{
					return new EnquiryModel
					{
						EnquiryId = Convert.ToInt32(reader["EnquiryId"]),
						EnquiryDate = reader["EnquiryDate"] != DBNull.Value ? (DateTime)reader["EnquiryDate"] : default(DateTime),
						CandidateName = reader["CandidateName"].ToString(),
						EmailAddress = reader["EmailAddress"].ToString(),
						MobileNumber = reader["MobileNumber"].ToString(),
						CityId = Convert.ToInt32(reader["CityId"]),
						CityName = reader["CityName"].ToString(),
						LocalAddress = reader["LocalAddress"].ToString(),
						Gender = reader["Gender"].ToString(),
						QualificationId = Convert.ToInt32(reader["QualificationId"]),
						QualificationName = reader["QualificationName"].ToString(),
						DateOfBirth = reader["DateOfBirth"] != DBNull.Value ? (DateTime)reader["DateOfBirth"] : default(DateTime),
						LeadSourceId = Convert.ToInt32(reader["LeadSourceId"]),
						LeadSourceName = reader["LeadSourceName"].ToString(),
						EnquiryForId = Convert.ToInt32(reader["EnquiryForId"]),
						EnquiryForName = reader["EnquiryForName"].ToString(),
						BranchId = Convert.ToInt32(reader["BranchId"]),
						BranchName = reader["BranchName"].ToString(),
						Remark = reader["Remark"].ToString(),
						Status = reader["Status"].ToString()

					};
				},
				new SqlParameter("@EnquiryId", id),
				new SqlParameter("@FirmId", firmId),
				new SqlParameter("@BranchId", branchId)
			);
			return result?.FirstOrDefault();
		}




		public void Restore(int id, int firmId)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "RESTORE"),
				new ("@EnquiryId", id),
				new ("@FirmId", firmId),
				new ("@EnquiryDate", 0),
				new ("@CandidateName", ""),
				new ("@EmailAddress", ""),
				new ("@MobileNumber", ""),
				new ("@CityId", 0),
				new ("@LocalAddress", ""),
				new ("@Gender", ""),
				new ("@QualificationId", 0),
				new ("@DateOfBirth", 0),
				new ("@LeadSourceId", 0),
				new ("@EnquiryForId", 0),
				new ("@BranchId", 0),
				new ("@Remark", "")
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Enquiries]", parameters);
		}

		public void ToggleStatus(int id)
		{
			throw new NotImplementedException();
		}

		public void UpdateRemark(int enquiryId, string remark)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@EnquiryId", enquiryId),	
				new ("@Remark", remark)
			};
			QueryService.NonQuery("[sp_UpdateRemark_Enquiries]", parameters);
		}


		public void Update(EnquiryModel Enquiry)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "UPDATE"),
				new ("@EnquiryId", Enquiry.EnquiryId),
				new ("@FirmId", Enquiry.FirmId),
				new ("@BranchId", Enquiry.BranchId),
				new ("@EnquiryDate", Enquiry.EnquiryDate),
				new ("@CandidateName", Enquiry.CandidateName),
				new ("@EmailAddress", Enquiry.EmailAddress),
				new ("@MobileNumber", Enquiry.MobileNumber),
				new ("@CityId", Enquiry.CityId),
				new ("@LocalAddress", Enquiry.LocalAddress),
				new ("@Gender", Enquiry.Gender),
				new ("@QualificationId", Enquiry.QualificationId),
				new ("@DateOfBirth", Enquiry.DateOfBirth),
				new ("@LeadSourceId", Enquiry.LeadSourceId),
				new ("@EnquiryForId", Enquiry.EnquiryForId),
				new ("@BranchId", Enquiry.BranchId),
				new ("@Status", Enquiry.Status),
				new ("@Remark", Enquiry.Remark),
				new ("@LastInsertedId", 0)
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Enquiries]", parameters);

		}


	}
}
