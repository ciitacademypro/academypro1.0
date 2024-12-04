using LmsModels.Course;

namespace LmsServices.Course.Interfaces
{
    public interface ICourseModuleContentService
    {
		public void Create(CourseModuleContentModel content);
		public void Update(CourseModuleContentModel content);
		public void Delete(int id);
		public void Restore(int id);
		public void ToggleStatus(int id);
		public CourseModuleContentModel GetById(int id);
		public List<CourseModuleContentModel> GetAll(int contentId = 0, int moduleId = 0);        
        
    }
}