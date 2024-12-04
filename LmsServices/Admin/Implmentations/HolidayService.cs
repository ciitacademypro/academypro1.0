
using LmsEnv;
using LmsServices.Common;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LmsServices.Admin.Interfaces;
using LmsModels.Administrator;
using Microsoft.AspNetCore.Mvc;

namespace LmsServices.Admin.Implmentations
{
	public class HolidayService : IHolidayService
	{
		private readonly string connString;
		public HolidayService() {
			connString = DbConnect.DefaultConnection;
		}

		public void Create(HolidayModel Holiday)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "INSERT"),
				new ("@HolidayId", 0),
                new ("@HolidayName", Holiday.HolidayName),
                new ("@HolidayDate", Holiday.HolidayDate),
                new ("@Description", Holiday.Description)

            };

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Holidays]", parameters);
	    }




		public void Delete(int id)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "DELETE"),
				new ("@HolidayId", id),
                new ("@HolidayName", ""),
                new ("@HolidayDate", 0),
                new ("@Description", 0)
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Holidays]", parameters);

		}



		public List<HolidayModel> GetAll()
		{
			return QueryService.Query(
				"sp_GetAll_Holidays",
				reader =>
				{
					return new HolidayModel
					{
						HolidayId = Convert.ToInt32(reader["HolidayId"]),
                		HolidayName = reader["HolidayName"].ToString(),
                		HolidayDate = DateOnly.FromDateTime(Convert.ToDateTime(reader["HolidayDate"])),
                		Description = reader["Description"].ToString()
					};
				},
				new SqlParameter("@HolidayId", 0)
			);
		}



		public HolidayModel GetById(int id)
		{
			var result =  QueryService.Query(
				"sp_GetAll_Holidays",
				reader =>
				{
					return new HolidayModel
					{
						HolidayId = Convert.ToInt32(reader["HolidayId"]),
						HolidayName = reader["HolidayName"].ToString(),
						HolidayDate = DateOnly.FromDateTime(Convert.ToDateTime(reader["HolidayDate"])),
						Description = reader["Description"].ToString()
					};
				},
				new SqlParameter("@HolidayId", id)
			);
			return result?.FirstOrDefault();
		}




		public void Restore(int id)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "RESTORE"),
				new ("@HolidayId", id),
                new ("@HolidayName", ""),
                new ("@HolidayDate", 0),
                new ("@Description", "")
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Holidays]", parameters);
		}

        public void ToggleStatus(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(HolidayModel Holiday)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "UPDATE"),
				new ("@HolidayId", Holiday.HolidayId),
                new ("@HolidayName", Holiday.HolidayName),
                new ("@HolidayDate", Holiday.HolidayDate),
                new ("@Description", Holiday.Description)
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Holidays]", parameters);

		}


    }
}
