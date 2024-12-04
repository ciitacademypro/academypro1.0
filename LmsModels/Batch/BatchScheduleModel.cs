using System;

namespace LmsModels.Batch;

public class BatchScheduleModel
{

    public int BatchScheduleId { get; set; }
    public int BranchId { get; set; }
    public int BatchId { get; set; }
    public DateTime ExpectedDateTime { get; set; }
    public DateTime ActualDateTime { get; set; }
    public int ExpectedTrainerId { get; set; }
    public int ActualTrainerId { get; set; }
    public string? Remark { get; set; }
    public string Status { get; set; }

    // List of schedule items
    public List<BatchScheduleItem> Items { get; set; } = new List<BatchScheduleItem>();

}   

public class BatchScheduleModelCreate
{
   public int BranchId { get; set; }
    public int BatchId { get; set; }

    // List of schedule items
    public List<BatchScheduleItem> Items { get; set; } = new List<BatchScheduleItem>();


 

}


// Supporting class for schedule items
public class BatchScheduleItem
{
    public DateTime ExpectedDateTime { get; set; }
    public int ExpectedTrainerId { get; set; }
    public List<int> ContentIds { get; set; }
    public List<string> ContentNames { get; set; }    
}
