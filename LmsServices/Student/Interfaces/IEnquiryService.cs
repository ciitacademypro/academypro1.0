
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LmsModels.Student;

namespace LmsServices.Student.Interfaces
{
	public interface IEnquiryService
	{
		public void Create(EnquiryModel Enquiry);
		public void Update(EnquiryModel Enquiry);
		public void UpdateRemark(int enquiryId, string remark);
		public void Delete(int id, int firmId);
		public void Restore(int id, int firmId);
		public void ToggleStatus(int id);
		public EnquiryModel GetById(int id, int firmId, int branchId);
		public List<EnquiryModel> GetAll(int firmId, int branchId);
	}
}
