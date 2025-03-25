using SIMS.Models;

namespace SIMS.Repositories
{
    public interface ICourseRepository
    {
        IEnumerable<Course> GetAllCourses();
        Course GetCourseById(int id);
        void CreateCourse(Course course);
        void EditCourse(Course course);
        void DeleteCourse(int id);
    }
}
