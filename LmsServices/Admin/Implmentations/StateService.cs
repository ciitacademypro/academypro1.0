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
	public class StateService: IStateService
	{
		private readonly string connString;
		public StateService()
		{
			connString = DbConnect.DefaultConnection;
		}

		public void Create(StateModel state)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "INSERT"),
				new ("@StateId", 0),
				new ("@CountryId", state.CountryId),
				new ("@StateName", state.StateName),
				new ("@Status", state.Status)
			};
			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_States]", parameters);

		}

		public void Delete(int id)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "DELETE"),
				new ("@StateId", id),
				new ("@CountryId", 0),
				new ("@StateName", ""),
				new ("@Status", false)
			};
			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_States]", parameters);
		}

		public List<StateModel> GetAll()
		{
			return QueryService.Query(
				"sp_GetAll_States",
				reader =>
				{
					return new StateModel
					{
						StateId = Convert.ToInt16(reader["StateId"]),
						StateName = reader["StateName"].ToString(),
						CountryId = Convert.ToInt16(reader["CountryId"]),
						CountryName = reader["CountryName"].ToString(),
						Status = reader["Status"] == "1",
						StatusLabel = reader["StatusLabel"].ToString()
					};
				},
				new SqlParameter("@StateId", 0),
				new SqlParameter("@CountryId", 0)
			);
		}

		public List<StateModel> GetByCountryId(int id)
		{
            return QueryService.Query(
                "sp_GetAll_States",
                reader =>
                {
                    return new StateModel
                    {
                        StateId = Convert.ToInt16(reader["StateId"]),
                        StateName = reader["StateName"].ToString(),
                        CountryId = Convert.ToInt16(reader["CountryId"]),
                        CountryName = reader["CountryName"].ToString(),
                        Status = reader["Status"] == "1",
                        StatusLabel = reader["StatusLabel"].ToString()
                    };
                },
                new SqlParameter("@StateId", 0),
                new SqlParameter("@CountryId", id)
            );

        }

        public StateModel GetById(int id)
		{
            var result = QueryService.Query(
                "sp_GetAll_States",
                reader =>
                {
                    return new StateModel
                    {
                        StateId = Convert.ToInt16(reader["StateId"]),
                        StateName = reader["StateName"].ToString(),
                        CountryId = Convert.ToInt16(reader["CountryId"]),
                        CountryName = reader["CountryName"].ToString(),
                        Status = reader["Status"] == "1",
                        StatusLabel = reader["StatusLabel"].ToString()
                    };
                },
                new SqlParameter("@StateId", id),
                new SqlParameter("@CountryId", 0)
            );
            return result?.FirstOrDefault();
        }

		public void Restore(int id)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "RESTORE"),
				new ("@StateId", id),
				new ("@CountryId", 0),
				new ("@StateName", ""),
				new ("@Status", false)
			};
			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_States]", parameters);

		}

		public void ToggleStatus(int id)
		{
			throw new NotImplementedException();
		}

		public void Update(StateModel state)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "UPDATE"),
				new ("@StateId", state.StateId),
				new ("@CountryId", state.CountryId),
				new ("@StateName", state.StateName),
				new ("@Status", state.Status)
			};
			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_States]", parameters);
		}
	}
}
