
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LmsModels.Administrator;

namespace LmsServices.Admin.Interfaces
{
	public interface IHolidayService
	{
		public void Create(HolidayModel Holiday);
		public void Update(HolidayModel Holiday);
		public void Delete(int id);
		public void Restore(int id);
		public void ToggleStatus(int id);
		public HolidayModel GetById(int id);
		public List<HolidayModel> GetAll();
	}
}
