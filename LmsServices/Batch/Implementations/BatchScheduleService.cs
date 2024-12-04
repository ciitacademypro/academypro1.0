using System;
using System.Data;
using LmsModels.Batch;
using LmsServices.Batch.Interfaces;
using LmsServices.Common;

namespace LmsServices.Batch.Implementations;

public class BatchScheduleService: IBatchScheduleService
{


	public void Create(BatchScheduleModelCreate batchSchedule){
            // add this dynamically afeter login
            batchSchedule.BranchId = 1;
            // Convert the list of CourseFee to DataTable

            //  pass "ContentIds" which is int []

            DataTable BatchScheduleTable = new DataTable();
            BatchScheduleTable.Columns.Add("ExpectedDateTime", typeof(DateTime));
            BatchScheduleTable.Columns.Add("ExpectedTrainerId", typeof(int));
            BatchScheduleTable.Columns.Add("ContentIds", typeof(string)); // We can store the ContentIds as a string (comma-separated), or as an array if your stored procedure expects an array.


            foreach (var item in batchSchedule.Items)
            {
                string contentIds = string.Join(",", item.ContentIds);

                BatchScheduleTable.Rows.Add(item.ExpectedDateTime, item.ExpectedTrainerId, contentIds);
            }

            var parameters = new List<KeyValuePair<string, object>>
            {
                new("@BranchId", batchSchedule.BranchId),
                new("@BatchId", batchSchedule.BatchId),
                new("@BatchSchedules", BatchScheduleTable)
            };

            QueryService.NonQuery("[sp_Create_BatchSchedules]", parameters);
            
    }

}
