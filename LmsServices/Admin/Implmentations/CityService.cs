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
	public class CityService : ICityService
	{
		private readonly string connString;
        public CityService()
        {
			connString = DbConnect.DefaultConnection;
		}

		public void Create(CityModel city)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "INSERT"),
				new ("@CityId", 0),
				new ("@StateId", city.StateId),
				new ("@CityName", city.CityName),
				new ("@Status", city.Status)
			};
			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Cities]", parameters);
		}

		public void Delete(int id)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "DELETE"),
				new ("@CityId", id),
				new ("@StateId", 0),
				new ("@CityName", ""),
				new ("@Status", false)
			};
			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Cities]", parameters);

		}

		public List<CityModel> GetAll()
		{
			return QueryService.Query(
				"sp_GetAll_Cities",
				reader =>
				{
					return new CityModel
					{
						CityId = Convert.ToInt16(reader["CityId"]),
						CityName = reader["CityName"].ToString(),
						StateId = Convert.ToInt16(reader["StateId"]),
						StateName = reader["StateName"].ToString(),
						CountryId = Convert.ToInt16(reader["CountryId"]),
						CountryName = reader["CountryName"].ToString(),
						Status = reader["Status"] == "1",
						StatusLabel = reader["StatusLabel"].ToString()
					};
				},
				new SqlParameter("@CityId", 0),
				new SqlParameter("@StateId", 0)
			);
		}

		public CityModel GetById(int id)
		{
			var result = QueryService.Query(
				"sp_GetAll_Cities",
				reader =>
				{
					return new CityModel
					{
						CityId = Convert.ToInt16(reader["CityId"]),
						CityName = reader["CityName"].ToString(),
						StateId = Convert.ToInt16(reader["StateId"]),
						StateName = reader["StateName"].ToString(),
						CountryId = Convert.ToInt16(reader["CountryId"]),
						CountryName = reader["CountryName"].ToString(),
						Status = reader["Status"] == "1",
						StatusLabel = reader["StatusLabel"].ToString()
					};
				},
				new SqlParameter("@CityId", id),
				new SqlParameter("@StateId", 0)
			);

			return result?.FirstOrDefault();

		}

		public List<CityModel> GetByStateId(int id)
		{
			return QueryService.Query(
				"sp_GetAll_Cities",
				reader =>
				{
					return new CityModel
					{
						CityId = Convert.ToInt16(reader["CityId"]),
						CityName = reader["CityName"].ToString(),
						StateId = Convert.ToInt16(reader["StateId"]),
						StateName = reader["StateName"].ToString(),
						CountryId = Convert.ToInt16(reader["CountryId"]),
						CountryName = reader["CountryName"].ToString(),
						Status = reader["Status"] == "1",
						StatusLabel = reader["StatusLabel"].ToString()
					};
				},
				new SqlParameter("@CityId", 0),
				new SqlParameter("@StateId", 1)
			);

		}

		public void Restore(int id)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "RESTORE"),
				new ("@CityId", id),
				new ("@StateId", 0),
				new ("@CityName", ""),
				new ("@Status", false)
			};
			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Cities]", parameters);


		}

		public void ToggleStatus(int id)
		{
			throw new NotImplementedException();
		}

		public void Update(CityModel city)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "UPDATE"),
				new ("@CityId", city.CityId),
				new ("@StateId", city.StateId),
				new ("@CityName", city.CityName),
				new ("@Status", city.Status)
			};
			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Cities]", parameters);

		}
	}
}
