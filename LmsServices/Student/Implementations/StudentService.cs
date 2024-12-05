using LmsEnv;
using LmsModels.Common;
using LmsModels.Student;
using LmsServices.Common;
using LmsServices.Student.Interfaces;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LmsServices.Student.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly string connString;
        public StudentService()
        {
            connString = DbConnect.DefaultConnection;
        }
            
        public int RowCount()
        {
            return CommonService.RowCount("students");
        }

        public int Create(StudentAddModel student)
        {
            string StudentCode = $"STD{(RowCount()+1):000000}"; // Ensures leading zeros up to 6 digits
            string password = CommonService.GenerateRandomPassword(6);

            var parameters = new List<KeyValuePair<string, object>>
            {

				new("@Type", "INSERT"),
                new("@StudentId", 0),
                new("@Password",password),
                new("@StudentName",student.StudentName),
                new("@StudentCode",StudentCode),
                new("@MobileNumber",student.MobileNumber),
                new("@EmailAddress",student.EmailAddress),
                new("@ProfilePhoto",@"uploads\students\defaultAvatar.png"),
                //new("@ProfilePhoto",student.ProfilePhoto),
                new("@Status",false)                
              //  new("@LastInsertedId", 0) // Output parameter should be initialized with a value (commonly 0)
            };

           return QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Students]", parameters, "@LastInsertedId");
        }

        public int Delete(int id)
        {
                var parameters = new List<KeyValuePair<string, object>>
                {
                        new("@Type", "DELETE"), 
                        new("@StudentId", id),   
                        new("@StudentName", ""), 
                        new("@StudentCode", ""),
                        new("@MobileNumber", ""),
                        new("@EmailAddress", ""),
                        new("@Password", ""),
                        new("@ProfilePhoto", null),
                        new("@Status", false),
                        new("@LastInsertedId", 0) // Output parameter should be initialized with a value (commonly 0)

                };

             return  QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Students]", parameters);
        }


        public List<StudentModel> GetAll(bool? status = null)
        {
            return QueryService.Query(
         "sp_GetAll_Students",
         reader =>
         {
             return new StudentModel
             {
                 StudentId = Convert.ToInt32(reader["StudentId"]),
                 StudentName = reader["StudentName"].ToString(),
                 StudentCode = reader["StudentCode"].ToString(),
                 MobileNumber = reader["MobileNumber"].ToString(),
                 EmailAddress = reader["EmailAddress"].ToString(),
                 ProfilePhoto = reader["ProfilePhoto"].ToString(),
                 Status = Convert.ToBoolean(reader["Status"]),
                 StatusLabel = reader["StatusLabel"].ToString()
             };
         },
            new SqlParameter("@StudentId", 0),
            new SqlParameter("@Status", status)
         );

        }

        public StudentModel GetById(int id)
        {
            var result = QueryService.Query(
            "sp_GetAll_Students",
            reader =>
            {
                return new StudentModel
                {
                    StudentId = Convert.ToInt32(reader["StudentId"]),
                    StudentName = reader["StudentName"].ToString(),
                    StudentCode = reader["StudentCode"].ToString(),
                    MobileNumber = reader["MobileNumber"].ToString(),
                    EmailAddress = reader["EmailAddress"].ToString(),
                    ProfilePhoto = reader["ProfilePhoto"].ToString(),
                    Status =  Convert.ToBoolean(reader["Status"]),
                    StatusLabel = reader["StatusLabel"].ToString()

                };
            },
            new SqlParameter("@StudentId", id)
        );

            return result?.FirstOrDefault();
        }

        public int Restore(int id)
        {
            var parameters = new List<KeyValuePair<string, object>>
                    {
                        new("@Type", "RESTORE"),         
                        new("@StudentId", id),            
                        new("@StudentName", ""), 
                        new("@StudentCode", ""),
                        new("@MobileNumber", ""),
                        new("@EmailAddress", ""),
                        new("@Password", ""),
                        new("@ProfilePhoto", ""),
                        new("@Status", false),
                        new("@LastInsertedId", 0) // Output parameter should be initialized with a value (commonly 0)
                    };

            return QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Students]", parameters);
        }

        public int ToggleStatus(int id, bool status = false)
        {
            var parameters = new List<KeyValuePair<string, object>>
                    {
                        new("@Type", "STATUS"),         
                        new("@StudentId", id),            
                        new("@StudentName", ""), 
                        new("@StudentCode", ""),
                        new("@MobileNumber", ""),
                        new("@EmailAddress", ""),
                        new("@Password", ""),
                        new("@ProfilePhoto", ""),
                        new("@Status", status),
                        new("@LastInsertedId", 0) // Output parameter should be initialized with a value (commonly 0)

                    };

            return QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Students]", parameters);

        }

        public int Update(StudentModel student)
        {
            var parameters = new List<KeyValuePair<string, object>>
                {
                    new("@Type", "UPDATE"),              
                    new("@StudentId", student.StudentId), 
                    new("@StudentName", student.StudentName), 
                    new("@StudentCode", student.StudentCode), 
                    new("@MobileNumber", student.MobileNumber),
                    new("@EmailAddress", student.EmailAddress), 
                    //new("@Password", student.Password),   
                    //new("@ProfilePhoto", student.ProfilePhoto ?? (object)DBNull.Value), 
                    new("@Status", student.Status),
                    new("@LastInsertedId", 0) // Output parameter should be initialized with a value (commonly 0)

                };

            return QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Students]", parameters);
        }

		public Dictionary<string, string> CheckExist(string StudentCode)
		{
			var conditions = new List<ColumnValuePairModel>
			{
				new(){ ColumnName = "StudentCode", Value = StudentCode.Trim() }
			};

			var results = CheckExisitService.Record("Students", "StudentId", conditions);
			var errors = new Dictionary<string, string>(); // Dictionary to store error messages

			foreach (var row in results)
			{
				var columnName = row["ColumnName"].ToString();
				var primaryKeyValue = Convert.ToInt32(row["PrimaryKeyValue"]);

				// Check if PrimaryKeyValue is not zero, and add an error message if so
				if (primaryKeyValue != 0)
				{
					errors[columnName] = $"{columnName} already exists."; // Store column name and error message
				}

			}

			return errors;
		}

		public bool ChangePassword(int studentId, string newPassword)
		{
			try
			{
				var parameters = new List<KeyValuePair<string, object>>
				{
					new("@Type", "ChangePassword"),
					new("@StudentId", studentId),
					new("@Password", newPassword)
				};

				QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Students]", parameters);
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}

		}

		public void UpdateProfilePhoto(int studentId, string profilePhoto)
		{
			try
			{
				var parameters = new List<KeyValuePair<string, object>>
				{
					new("@Type", "ProfilePhoto"),
					new("@StudentId", studentId),
					new("@ProfilePhoto", profilePhoto)
				};

				QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_Students]", parameters, "@LastInsertedId");
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred while updating the profile photo.", ex);
			}
		}

    }
}