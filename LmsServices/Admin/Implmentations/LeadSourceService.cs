using LmsEnv;
using LmsModels.Administrator;
using LmsServices.Admin.Interfaces;
using LmsServices.Common;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsServices.Admin.Implmentations
{
	public class LeadSourceService : ILeadSourceService
	{
		private readonly string connString;
		public LeadSourceService() 
		{
			connString = DbConnect.DefaultConnection;
		}

		public void Create(LeadSourceModel leadSource)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "INSERT"),
				new ("@LeadSourceId", 0),
				new ("@LeadSourceName", leadSource.LeadSourceName),
				new ("@Status", leadSource.Status)
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_LeadSources]", parameters);

		}

		public void Delete(int id)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "DELETE"),
				new ("@LeadSourceId", id),
				new ("@LeadSourceName", ""),
				new ("@Status", false)
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_LeadSources]", parameters);

		}

		public List<LeadSourceModel> GetAll()
		{
			return QueryService.Query(
				"sp_GetAll_LeadSources",
				reader =>
				{
					return new LeadSourceModel
					{
						LeadSourceId = Convert.ToInt16(reader["LeadSourceId"]),
						LeadSourceName = reader["LeadSourceName"].ToString(),
						Status = reader["Status"] == "1",
						StatusLabel = reader["StatusLabel"].ToString()
					};
				},
				new SqlParameter("@LeadSourceId", 0)
			); 
		}

		public LeadSourceModel GetById(int id)
		{
			var result = QueryService.Query(
				"sp_GetAll_LeadSources",
				reader =>
				{
					return new LeadSourceModel
					{
						LeadSourceId = Convert.ToInt16(reader["LeadSourceId"]),
						LeadSourceName = reader["LeadSourceName"].ToString(),
						Status = reader["Status"] == "1",
						StatusLabel = reader["StatusLabel"].ToString()
					};
				},
				new SqlParameter("@LeadSourceId", id)
			);
			return result?.FirstOrDefault();
		}

		public void Restore(int id)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "RESTORE"),
				new ("@LeadSourceId", id),
				new ("@LeadSourceName", ""),
				new ("@Status", false)
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_LeadSources]", parameters);

		}

		public void ToggleStatus(int id)
		{
			throw new NotImplementedException();
		}

		public void Update(LeadSourceModel leadSource)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "UPDATE"),
				new ("@LeadSourceId", leadSource.LeadSourceId),
				new ("@LeadSourceName", leadSource.LeadSourceName),
				new ("@Status", leadSource.Status)
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_LeadSources]", parameters);

		}
	}
}
