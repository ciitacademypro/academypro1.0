using LmsModels.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsServices.Course.Interfaces
{
	public interface ICourseModuleService
	{
		public void Create(CourseModuleModel courseModule);
		public List<CourseModuleModel> GetAll(int CourseModuleId = 0, short CourseId = 0);

		public void Update(CourseModuleModel courseModule);
		public void Delete(int id);
		public void Restore(int id);
		public void ToggleStatus(int id);
		public CourseModuleModel GetById(int id);

	}
}
