using LmsEnv;
using LmsModels.Course;
using LmsServices.Common;
using LmsServices.Course.Interfaces;
using Microsoft.Data.SqlClient;


namespace LmsServices.Course.Implementations
{
    public class CourseModuleContentService : ICourseModuleContentService
    {
		private readonly string connString;
		public CourseModuleContentService()
		{
			connString = DbConnect.DefaultConnection;
		}

        public void Create(CourseModuleContentModel content)
        {
			var parameters = new List<KeyValuePair<string, object>>
			{                
				new ("@Type", "INSERT"),
				new ("@CourseModuleContentId", 0),
				new ("@CourseModuleId", content.CourseModuleId),
				new ("@ContentName", content.ContentName),
				new ("@ContentDescription", content.ContentDescription),
                new ("@DurationInHrs", content.DurationInHrs),
				new ("@ContentOrder", 1),
				new ("@Status", content.Status),
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_CourseModuleContents]", parameters);
        }

        public void Delete(int id)
        {
			var parameters = new List<KeyValuePair<string, object>>
			{
                
				new ("@Type", "DELETE"),
				new ("@CourseModuleContentId", id),
				new ("@CourseModuleId", 0),
				new ("@ContentName", ""),
				new ("@ContentDescription", ""),
                new ("@DurationInHrs", 0),
				new ("@ContentOrder", 1),
				new ("@Status", false),
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_CourseModuleContents]", parameters);
        }

        public List<CourseModuleContentModel> GetAll(int contentId = 0, int moduleId = 0)
        {
			return QueryService.Query(
				"sp_GetAll_CourseModuleContents",
				reader =>
				{
					return new CourseModuleContentModel
					{
						CourseModuleContentId = Convert.ToInt32(reader["CourseModuleContentId"]),
						CourseModuleId = Convert.ToInt32(reader["CourseModuleId"]),
						ModuleName = reader["ModuleName"].ToString(), 
						CourseId = Convert.ToInt32(reader["CourseId"]),
						CourseName = reader["CourseName"].ToString(), 
						ContentName = reader["ContentName"].ToString(), 
						ContentDescription = reader["ContentDescription"].ToString(), 
						DurationInHrs = Convert.ToByte(reader["DurationInHrs"]),
						ContentOrder = Convert.ToInt16(reader["ContentOrder"]),

						Status = Convert.ToBoolean(reader["Status"]),
						StatusLabel = reader["StatusLabel"].ToString()
					};
				},
                    new SqlParameter("@CourseModuleContentId", contentId),
                    new SqlParameter("@CourseModuleId", moduleId)
                
                );
        }

        public CourseModuleContentModel GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Restore(int id)
        {
            throw new NotImplementedException();
        }

        public void ToggleStatus(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(CourseModuleContentModel content)
        {
			var parameters = new List<KeyValuePair<string, object>>
			{                
				new ("@Type", "UPDATE"),
				new ("@CourseModuleContentId", content.CourseModuleContentId),
				new ("@CourseModuleId", content.CourseModuleId),
				new ("@ContentName", content.ContentName),
				new ("@ContentDescription", content.ContentDescription),
                new ("@DurationInHrs", content.DurationInHrs),
				new ("@ContentOrder", 1),
				new ("@Status", content.Status),
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_CourseModuleContents]", parameters);

        }
    }
}