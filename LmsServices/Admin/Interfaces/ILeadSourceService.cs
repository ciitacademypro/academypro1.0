using LmsModels.Administrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsServices.Admin.Interfaces
{
	public interface ILeadSourceService
	{
		public void Create(LeadSourceModel leadSource);
		public void Update(LeadSourceModel leadSource);
		public void Delete(int id);
		public void Restore(int id);
		public void ToggleStatus(int id);
		public LeadSourceModel GetById(int id);
		public List<LeadSourceModel> GetAll();
	}
}
