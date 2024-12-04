using System;
using LmsModels.Batch;

namespace LmsServices.Batch.Interfaces;

public interface IBatchService
{
		public int Create(BatchModel batch, int firmId);
		public List<BatchModel> GetAll(int firmId, int batchId = 0);

		public void Update(BatchModel batch);
		public void Delete(int id);
		public void Restore(int id);
		public void ToggleStatus(int id);
		public BatchModel GetById(int id);

}
