using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LmsModels.Admin;

namespace LmsServices.Admin.Interfaces
{
	public interface IBranchService
	{
		public int Create(BranchModel branch);
		public void Update(BranchModel branch);
		public void Delete(int id);
		public void Restore(int id);
		public void ToggleStatus(int id);
		public BranchModel GetById(int id, int firmId);
		public List<BranchModel> GetAll(int firmId);
	}
}