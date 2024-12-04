
using LmsEnv;
using LmsServices.Common;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LmsServices.Batch.Interfaces;
using LmsModels.Batch;

namespace LmsServices.Batch.Implmentations
{
	public class ClassRoomService : IClassRoomService
	{
		private readonly string connString;
		public ClassRoomService() {
			connString = DbConnect.DefaultConnection;
		}

		public void Create(ClassRoomModel ClassRoom, int firmId)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "INSERT"),
				new ("@ClassRoomId", 0),
                new ("@FirmId", firmId),
                new ("@BranchId", ClassRoom.BranchId),
                new ("@ClassRoomName", ClassRoom.ClassRoomName),
                new ("@Status", ClassRoom.Status)

            };

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_ClassRooms]", parameters);
	    }




		public void Delete(int id)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "DELETE"),
				new ("@ClassRoomId", id),
                new ("@BranchId", 0),
                new ("@ClassRoomName", ""),
                new ("@Status", false)
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_ClassRooms]", parameters);

		}



		public List<ClassRoomModel> GetAll(int firmId)
		{
			return QueryService.Query(
				"sp_GetAll_ClassRooms",
				reader =>
				{
					return new ClassRoomModel
					{
						ClassRoomId = Convert.ToInt32(reader["ClassRoomId"]),
						BranchId = Convert.ToInt32(reader["BranchId"]),
						ClassRoomName = reader["ClassRoomName"].ToString(),
						Status = Convert.ToBoolean(reader["Status"]),
						StatusLabel = Convert.ToString(reader["StatusLabel"])
					};
				},
				new SqlParameter("@ClassRoomId", 0),
				new SqlParameter("@firmId", firmId)
			);
		}



		public ClassRoomModel GetById(int id)
		{
			var result =  QueryService.Query(
				"sp_GetAll_ClassRooms",
				reader =>
				{
					return new ClassRoomModel
					{
						ClassRoomId = Convert.ToInt32(reader["ClassRoomId"]),
                BranchId = Convert.ToInt32(reader["BranchId"]),
                ClassRoomName = reader["ClassRoomName"].ToString(),
                Status = Convert.ToBoolean(reader["Status"])
					};
				},
				new SqlParameter("@ClassRoomId", id)
			);
			return result?.FirstOrDefault();
		}




		public void Restore(int id)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "RESTORE"),
				new ("@ClassRoomId", id),
                new ("@BranchId", 0),
                new ("@ClassRoomName", ""),
                new ("@Status", false)
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_ClassRooms]", parameters);
		}

        public void ToggleStatus(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(ClassRoomModel ClassRoom)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "UPDATE"),
				new ("@ClassRoomId", ClassRoom.ClassRoomId),
				new ("@FirmId", ClassRoom.FirmId),
				new ("@BranchId", ClassRoom.BranchId),
                new ("@ClassRoomName", ClassRoom.ClassRoomName),
                new ("@Status", ClassRoom.Status)
			};

			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_ClassRooms]", parameters);

		}


    }
}
