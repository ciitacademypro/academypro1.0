using LmsModels.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsServices.Course.Interfaces
{
	public interface ICourseService
	{
		public void Create(CourseModel course);
		public void CreateWithFees(CourseModel course, string FeesJsonString);
		public void Update(CourseModel course);
		public void Delete(int id);
		public void Restore(int id);
		public void ToggleStatus(int id);
		public CourseModel GetById(int id);

		public List<CourseFeeModel> GetCourseFees(int courseId);
		public CourseFeeModel GetFeesById(int CourseFeeId);

		public List<CourseModel> GetAll(int courseId = 0, int categoryId = 0);
	}
}
