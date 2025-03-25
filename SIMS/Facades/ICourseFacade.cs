using SIMS.Models;

namespace SIMS.Facades
{
    public interface ICourseFacade
    {
        void CreateCourse(Course course);
        void UpdateCourse(Course course);
        void DeleteCourse(int id);
        Course GetCourseById(int id);
        IEnumerable<Course> GetAllCourses();
    }
}
