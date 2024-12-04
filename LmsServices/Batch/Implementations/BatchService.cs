using System;
using System.Data;
using LmsEnv;
using LmsModels.Batch;
using LmsModels.Course;
using LmsServices.Batch.Interfaces;
using LmsServices.Common;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;


namespace LmsServices.Batch.Implementations;

public class BatchService : IBatchService
{

    private readonly string connString;
    public BatchService()
    {
        connString = DbConnect.DefaultConnection;
    }

		public List<CourseFeeModel> MapJsonToCourseFeeList(string jsonData)
		{
			var courseFeeJsonList = JsonConvert.DeserializeObject<List<CourseFeeJson>>(jsonData);

			var courseFeeList = courseFeeJsonList.Select(json => new CourseFeeModel
			{
				TotalInstallments = json.txtTotalInstallments,
				GstPercentage = json.textGstPercentage,
				FeeAmount = json.txtTotalAmount
			}).ToList();

			return courseFeeList;
		}


    public int Create(BatchModel batch, int firmId)
    {
            // Convert the list of CourseFee to DataTable
            DataTable weekDaysTable = new DataTable();
            weekDaysTable.Columns.Add("WeekDayCode", typeof(int));
            weekDaysTable.Columns.Add("WeekDayName", typeof(string));

            var weekName = new List<string>{"Sun","Mon","Tue","Wed","Thu","Fri","Sat"};

            foreach (var week in batch.WeekDaysList)
            {
                int index = batch.WeekDaysList.IndexOf(week);
                weekDaysTable.Rows.Add(week, weekName[index]);
            }

            var parameters = new List<KeyValuePair<string, object>>
            {
                // new("@BatchId", 0),
                new("@BatchCode", batch.BatchCode),
				new("@BranchId", batch.BranchId),
				new("@FirmId", firmId),
				new("@CourseModuleId", batch.CourseModuleId),
                new("@TrainerId", batch.TrainerId),
                new("@ClassRoomId", batch.ClassRoomId),
                new("@StartDate", batch.StartDate),
                new("@EndDate", batch.EndDate),
                new("@StartTime", batch.StartTime),
                new("@BatchDurationInHr", batch.BatchDurationInHr),

                new("@WeekDaysList", weekDaysTable)
            };

            return QueryService.NonQuery("[sp_Create_Batches]", parameters,"@BatchId");

    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public List<BatchModel> GetAll(int firmId, int batchId = 0)
    {
			return QueryService.Query(
				"sp_GetAll_Batches",
				reader =>
				{
					return new BatchModel
					{
						BatchId = Convert.ToInt32(reader["BatchId"]),
						BatchCode = reader["BatchCode"].ToString(),
                        BranchId = Convert.ToInt32(reader["BranchId"]),
                        CourseModuleId = Convert.ToInt32(reader["CourseModuleId"]),
						ModuleName = reader["ModuleName"].ToString(),
                        TrainerId = reader["TrainerId"].ToString(),
						TrainerName = reader["TrainerName"].ToString(),
                        ClassRoomId = Convert.ToInt32(reader["ClassRoomId"]),
                        StartDate = reader["StartDate"] != DBNull.Value ? DateOnly.FromDateTime(Convert.ToDateTime(reader["StartDate"])) : (DateOnly?)null,
                        ActualStartDate = reader["ActualStartDate"] != DBNull.Value ? DateOnly.FromDateTime(Convert.ToDateTime(reader["ActualStartDate"])) : (DateOnly?)null,
                        EndDate = reader["EndDate"] != DBNull.Value ? DateOnly.FromDateTime(Convert.ToDateTime(reader["EndDate"])) : (DateOnly?)null,
                        ActualEndDate = reader["ActualEndDate"] != DBNull.Value ? DateOnly.FromDateTime(Convert.ToDateTime(reader["ActualEndDate"])) : (DateOnly?)null,
                        StartTime = reader["StartTime"] != DBNull.Value  ? (TimeSpan?)reader["StartTime"]  : null,
                        BatchDurationInHr = Convert.ToByte(reader["BatchDurationInHr"]),
						Status = Convert.ToBoolean(reader["Status"]),
						StatusLabel = Convert.ToString(reader["StatusLabel"])
					};
				},
				new SqlParameter("@BatchId", 0),
				new SqlParameter("@FirmId", firmId)
			);
    }

    public BatchModel GetById(int id)
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

    public void Update(BatchModel batch)
    {
        throw new NotImplementedException();
    }
}
