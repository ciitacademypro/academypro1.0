using LmsEnv;
using LmsModels.Administrator;
using LmsServices.Admin.Interfaces;
using LmsServices.Common;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsServices.Admin.Implmentations
{
	public class EnquiryForService : IEnquiryForService
	{
		private readonly string connString;

        public EnquiryForService()
        {
			connString = DbConnect.DefaultConnection;
		}
		public void Create(EnquiryForModel enquiry)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "INSERT"),
				new ("@EnquiryForId", 0),
				new ("@EnquiryForName", enquiry.EnquiryForName),
				new ("@Status", enquiry.Status)
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_EnquiryFors]", parameters);
		}

		public void Delete(int id)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "DELETE"),
				new ("@EnquiryForId", id),
				new ("@EnquiryForName", ""),
				new ("@Status", false)
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_EnquiryFors]", parameters);
		}

		public List<EnquiryForModel> GetAll()
		{
			return QueryService.Query(
				"sp_GetAll_EnquiryFors",
				reader =>
				{
					return new EnquiryForModel
					{
						EnquiryForId = Convert.ToInt16(reader["EnquiryForId"]),
						EnquiryForName = reader["EnquiryForName"].ToString(),
						Status = reader["Status"] == "1",
						StatusLabel = reader["StatusLabel"].ToString()
					};
				},
				new SqlParameter("@EnquiryForId", 0)
			);
		}

		public EnquiryForModel GetById(int id)
		{
			var result = QueryService.Query(
				"sp_GetAll_EnquiryFors",
				reader =>
				{
					return new EnquiryForModel
					{
						EnquiryForId = Convert.ToInt16(reader["EnquiryForId"]),
						EnquiryForName = reader["EnquiryForName"].ToString(),
						Status = reader["Status"] == "1",
						StatusLabel = reader["StatusLabel"].ToString()
					};
				},
				new SqlParameter("@EnquiryForId", id)
			);
			return result?.FirstOrDefault();
		}

		public void Restore(int id)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "RESTORE"),
				new ("@EnquiryForId", id),
				new ("@EnquiryForName", ""),
				new ("@Status", false)
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_EnquiryFors]", parameters);
		}

		public void ToggleStatus(int id)
		{
			throw new NotImplementedException();
		}

		public void Update(EnquiryForModel enquiry)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "UPDATE"),
				new ("@EnquiryForId", enquiry.EnquiryForId),
				new ("@EnquiryForName", enquiry.EnquiryForName),
				new ("@Status", enquiry.Status)
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_EnquiryFors]", parameters);
		}
	}
}
