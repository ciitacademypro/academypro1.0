using LmsModels.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsServices.Course.Interfaces
{
	public interface ICourseCategoryService
	{
		public void Create(CourseCategoryModel category);
		public void Update(CourseCategoryModel category);
		public void Delete(int id);
		public void Restore(int id);
		public void ToggleStatus(int id);
		public CourseCategoryModel GetById(int id);
		public List<CourseCategoryModel> GetAll();
	}
}
