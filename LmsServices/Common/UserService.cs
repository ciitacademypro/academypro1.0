using LmsModels;
using LmsModels.Student;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsServices.Common
{
	public class UserService
	{

		public static List<UserViewModel> GetAll(int firmId, string role="", bool? status = null)
		{
			return QueryService.Query(
		 "sp_GetAll_Users",
		 reader =>
		 {
			 return new UserViewModel
			 {
				 Id = reader["Id"].ToString(),
				 BranchId = Convert.ToInt32(reader["BranchId"]),
				 FirmId = Convert.ToInt32(reader["FirmId"]),
				 RoleId = reader["RoleId"].ToString(),
				 RoleName = reader["RoleName"].ToString(),				 
				 BranchName = reader["BranchName"].ToString(),
				 FirmName = reader["FirmName"].ToString(),
				 Name = reader["Name"].ToString(),
				 Uid1 = reader["Uid1"].ToString(),
				 PhoneNumber = reader["PhoneNumber"].ToString(),
				 Email = reader["Email"].ToString(),
				 Status = Convert.ToBoolean(reader["Status"]),
				 StatusLabel = reader["StatusLabel"].ToString()
			 };
		 },
			new SqlParameter("@FirmId", firmId),
			new SqlParameter("@RoleList", role),
			new SqlParameter("@Status", status),
			new SqlParameter("@BranchId", 0)
		 );

		}



	}
}
