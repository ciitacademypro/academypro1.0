using LmsModels.Administrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsServices.Admin.Interfaces
{
	public interface IQualificationService
	{
		public void Create(QualificationModel qualification);
		public void Update(QualificationModel qualification);
		public void Delete(int id);
		public void Restore(int id);
		public void ToggleStatus(int id);
		public QualificationModel GetById(int id);
		public List<QualificationModel> GetAll();
	}
}
