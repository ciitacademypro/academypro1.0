using System;
using System.ComponentModel.DataAnnotations;

namespace LmsModels.Batch;

public class BatchModel
{
    public int BatchId { get; set; }
    public string BatchCode { get; set; }
	
    [Display(Name = "Firm Id")]
	public int FirmId { get; set; }
	public int BranchId { get; set; }
    public int CourseModuleId { get; set; }
    public string? ModuleName { get; set; }    
    public string TrainerId { get; set; }
    public string? TrainerName { get; set; }    
    public int ClassRoomId { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? ActualStartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateOnly? ActualEndDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public byte BatchDurationInHr { get; set; }
    public bool Status { get; set; }
    public string? StatusLabel { get; set; }
    
    public List<int> WeekDaysList { get; set; }

    public IEnumerable<BatchWeekDayModel> BatchWeekDays { get; set; }

    public BatchScheduleModelCreate BatchSchedules { get; set; }


}

public class BatchWeekDayModel
{
    public int BatchWeekDayId { get; set; }
    public int BatchId { get; set; }
    public byte WeekDayCode { get; set; }
    
    public string WeekDayName { get; set; }


}

