using LmsModels.Administrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsServices.Admin.Interfaces
{
	public interface ICountryService
	{
		public void Create(CountryModel country);
		public void Update(CountryModel country);
		public void Delete(int id);
		public void Restore(int id);
		public void ToggleStatus(int id);
		public CountryModel GetById(int id);
		public List<CountryModel> GetAll();
	}
}
