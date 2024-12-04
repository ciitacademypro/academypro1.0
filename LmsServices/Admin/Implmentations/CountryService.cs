using LmsEnv;
using LmsModels.Administrator;
using LmsServices.Admin.Interfaces;
using LmsServices.Common;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsServices.Admin.Implmentations
{
	public class CountryService : ICountryService
	{
		private readonly string connString;
        public CountryService()
        {
			connString = DbConnect.DefaultConnection;
		}

		public void Create(CountryModel country)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "INSERT"),
				new ("@CountryId", 0),
				new ("@CountryName", country.CountryName),
				new ("@Status", country.Status)
			};
			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Countries]", parameters);

		}

		public void Delete(int id)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "DELETE"),
				new ("@CountryId", id),
				new ("@CountryName", ""),
				new ("@Status", false)
			};
			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Countries]", parameters);
		}

		public List<CountryModel> GetAll()
		{
			return QueryService.Query(
				"sp_GetAll_Countries",
				reader =>
				{
					return new CountryModel
					{
						CountryId = Convert.ToInt16(reader["CountryId"]),
						CountryName = reader["CountryName"].ToString(),
						Status = reader["Status"] == "1",
						StatusLabel = reader["StatusLabel"].ToString()
					};
				},
				new SqlParameter("@CountryId", 0)
			);
		}

		public CountryModel GetById(int id)
		{
			var result =  QueryService.Query(
				"sp_GetAll_Countries",
				reader =>
				{
					return new CountryModel
					{
						CountryId = Convert.ToInt16(reader["CountryId"]),
						CountryName = reader["CountryName"].ToString(),
						Status = reader["Status"] == "1",
						StatusLabel = reader["StatusLabel"].ToString()
					};
				},
				new SqlParameter("@CountryId", id)
			);

			return result?.FirstOrDefault();

		}

		public void Restore(int id)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "RESTORE"),
				new ("@CountryId", id),
				new ("@CountryName", ""),
				new ("@Status", false)
			};
			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Countries]", parameters);

		}

		public void ToggleStatus(int id)
		{
			throw new NotImplementedException();
		}

		public void Update(CountryModel country)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "UPDATE"),
				new ("@CountryId", country.CountryId),
				new ("@CountryName", country.CountryName),
				new ("@Status", country.Status)
			};
			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Countries]", parameters);

		}
	}
}
