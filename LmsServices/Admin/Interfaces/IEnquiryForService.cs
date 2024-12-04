using LmsModels.Administrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsServices.Admin.Interfaces
{
	public interface IEnquiryForService
	{
		public void Create(EnquiryForModel enquiry);
		public void Update(EnquiryForModel enquiry);
		public void Delete(int id);
		public void Restore(int id);
		public void ToggleStatus(int id);
		public EnquiryForModel GetById(int id);
		public List<EnquiryForModel> GetAll();
	}
}
