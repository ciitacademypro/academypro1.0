using LmsModels.Administrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsServices.Admin.Interfaces
{
	public interface IStateService
	{
		public void Create(StateModel state);
		public void Update(StateModel state);
		public void Delete(int id);
		public void Restore(int id);
		public void ToggleStatus(int id);
		public StateModel GetById(int id);
		public List<StateModel> GetByCountryId(int id);
		public List<StateModel> GetAll();
	}
}
