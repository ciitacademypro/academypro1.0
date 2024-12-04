using LmsModels.Administrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsServices.Admin.Interfaces
{
	public interface ICityService
	{
		public void Create(CityModel city);
		public void Update(CityModel city);
		public void Delete(int id);
		public void Restore(int id);
		public void ToggleStatus(int id);
		public CityModel GetById(int id);
		public List<CityModel> GetByStateId(int id);
		public List<CityModel> GetAll();

	}
}
