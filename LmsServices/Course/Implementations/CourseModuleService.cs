using LmsEnv;
using LmsModels.Course;
using LmsServices.Common;
using LmsServices.Course.Interfaces;
using Microsoft.Data.SqlClient;

namespace LmsServices.Course.Implementations
{
	public class CourseModuleService: ICourseModuleService
	{
		private readonly string connString;
		public CourseModuleService()
		{
			connString = DbConnect.DefaultConnection;
		}

		public void Create(CourseModuleModel courseModule)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new("@type", "INSERT"),
				new("@CourseModuleId", 0),
				new("@CourseId", courseModule.CourseId),
				new("@ModuleName", courseModule.ModuleName),
				new("@ModuleDescription", courseModule.ModuleDescription?? ""),
				new("@ModuleOrder", 1),
				new("@Status", courseModule.Status),

			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_CourseModules]", parameters);
		}

		public void Delete(int id)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
					new("@type", "DELETE"),
					new("@CourseModuleId", id),
					new("@CourseId", 0),  
					new("@ModuleName",  ""),  
					new("@ModuleDescription", ""),  
					new("@ModuleOrder", ""),
					new("@Status", (object)DBNull.Value),
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_CourseModules]", parameters);
		}

		public List<CourseModuleModel> GetAll(int CourseModuleId = 0, short CourseId = 0)
		{

			return QueryService.Query(
				"sp_GetAll_CourseModules",
				reader =>
				{
					return new CourseModuleModel
					{
						CourseModuleId = Convert.ToInt32(reader["CourseModuleId"]),
						CourseId = Convert.ToInt16(reader["CourseId"]),
						CourseName = reader["CourseName"].ToString(),
						CourseCategoryId = Convert.ToInt16(reader["CourseCategoryId"]),
						CourseCategoryName = reader["CourseCategoryName"].ToString(),
						ModuleName = reader["ModuleName"].ToString(),
						ModuleDescription = reader["ModuleDescription"].ToString(),
						ModuleOrder = Convert.ToInt16(reader["ModuleOrder"]),
						Status = reader["Status"] == "1",
						StatusLabel = reader["StatusLabel"].ToString(),
					};
				},
				new SqlParameter("@CourseModuleId", CourseModuleId),
				new SqlParameter("@CourseId", CourseId)
		);
		}

		public CourseModuleModel GetById(int id)
		{
			var result = QueryService.Query(
				"sp_GetAll_CourseModules",
				reader =>
				{
					return new CourseModuleModel
					{
						CourseModuleId = Convert.ToInt32(reader["CourseModuleId"]),
						CourseId = Convert.ToInt16(reader["CourseId"]),
						CourseName = reader["CourseName"].ToString(),
						CourseCategoryId = Convert.ToInt16(reader["CourseCategoryId"]),
						CourseCategoryName = reader["CourseCategoryName"].ToString(),
						ModuleName = reader["ModuleName"].ToString(),
						ModuleDescription = reader["ModuleDescription"].ToString(),
						ModuleOrder = Convert.ToInt16(reader["ModuleOrder"]),
						Status = Convert.ToBoolean(reader["Status"]),
						StatusLabel = reader["StatusLabel"].ToString(),
					};
				},
				new SqlParameter("@CourseModuleId", id),
				new SqlParameter("@CourseId", 0)  
			);

			return result?.FirstOrDefault();
		}


		public void Restore(int id)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
					new("@type", "RESTORE"),
					new("@CourseModuleId", id),
					new("@CourseId", 0),  
					new("@ModuleName",  ""),  
					new("@ModuleDescription", ""),  
					new("@ModuleOrder", ""),
					new("@Status", (object)DBNull.Value),
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_CourseModules]", parameters);

		}

		public void ToggleStatus(int id)
		{
			throw new NotImplementedException();
		}

		public void Update(CourseModuleModel courseModule)
		{
			var parameters = new List<KeyValuePair<string, object>>
				{
					new("@type", "UPDATE"),
					new("@CourseModuleId", courseModule.CourseModuleId),
					new("@CourseId", courseModule.CourseId),
					new("@ModuleName", courseModule.ModuleName),
					new("@ModuleDescription", courseModule.ModuleDescription ?? ""),
					new("@ModuleOrder", courseModule.ModuleOrder),
					new("@Status", courseModule.Status)
				};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_CourseModules]", parameters);
		}
	}
}
