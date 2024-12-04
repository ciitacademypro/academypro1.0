
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LmsModels.Batch;

namespace LmsServices.Batch.Interfaces
{
	public interface IClassRoomService
	{
		public void Create(ClassRoomModel ClassRoom, int firmId);
		public void Update(ClassRoomModel ClassRoom);
		public void Delete(int id);
		public void Restore(int id);
		public void ToggleStatus(int id);
		public ClassRoomModel GetById(int id);
		public List<ClassRoomModel> GetAll(int firmId);
	}
}
