using LmsModels.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsServices.Student.Interfaces
{
	public interface ILeadService
	{
		public void Create(LeadModel lead);
		public void Update(LeadModel lead);
		public void Delete(int id, int firmId);
		public void AssignTo(int employeeId, int leadId, int currentUser, int firmId);
		public void UpdateRemark(int leadId, string status, string remark, int currentUser);
		public void Restore(int id, int firmId);
		public LeadModel GetById(int id, int firmId);
		//public List<LeadModel> GetAll();
		public List<LeadModel> GetAll(int firmId, int LeadId, string? SearchByColumn, int? SearchByValue);
	}
}
