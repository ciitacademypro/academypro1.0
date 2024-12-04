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
	public class QualificationService : IQualificationService
	{
		private readonly string connString;
		public QualificationService()
		{
			connString = DbConnect.DefaultConnection;
		}

		public void Create(QualificationModel qualification)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "INSERT"),
				new ("@QualificationId", 0),
				new ("@QualificationName", qualification.QualificationName),
				new ("@Status", qualification.Status)
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Qualifications]", parameters);
		}

		public void Delete(int id)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "DELETE"),
				new ("@QualificationId", id),
				new ("@QualificationName", ""),
				new ("@Status", false)
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Qualifications]", parameters);
		}

		public List<QualificationModel> GetAll()
		{
			return QueryService.Query(
				"sp_GetAll_Qualifications",
				reader =>
				{
					return new QualificationModel
					{
						QualificationId = Convert.ToInt16(reader["QualificationId"]),
						QualificationName = reader["QualificationName"].ToString(),
						Status = reader["Status"] == "1",
						StatusLabel = reader["StatusLabel"].ToString()
					};
				},
				new SqlParameter("@QualificationId", 0)
			);
		}

		public QualificationModel GetById(int id)
		{
			var result =  QueryService.Query(
				"sp_GetAll_Qualifications",
				reader =>
				{
					return new QualificationModel
					{
						QualificationId = Convert.ToInt16(reader["QualificationId"]),
						QualificationName = reader["QualificationName"].ToString(),
						Status = reader["Status"] == "1",
						StatusLabel = reader["StatusLabel"].ToString()
					};
				},
				new SqlParameter("@QualificationId", id)
				);
			return result?.FirstOrDefault();

		}

		public void Restore(int id)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "RESTORE"),
				new ("@QualificationId", id),
				new ("@QualificationName", ""),
				new ("@Status", false)
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Qualifications]", parameters);
		}

		public void ToggleStatus(int id)
		{
			throw new NotImplementedException();
		}

		public void Update(QualificationModel qualification)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "UPDATE"),
				new ("@QualificationId", qualification.QualificationId),
				new ("@QualificationName", qualification.QualificationName),
				new ("@Status", qualification.Status)
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Qualifications]", parameters);

		}
	}
}
