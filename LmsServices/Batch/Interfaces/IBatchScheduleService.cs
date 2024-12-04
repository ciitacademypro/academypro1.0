using System;
using LmsModels.Batch;

namespace LmsServices.Batch.Interfaces;

public interface IBatchScheduleService
{
	public void Create(BatchScheduleModelCreate batchSchedule);
    
}
