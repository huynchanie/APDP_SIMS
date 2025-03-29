using SIMS.Models;

namespace SIMS.Facades
{
    public interface IEnrollmentFacade
    {
      
            Task<bool> AssignStudentToCourseAsync(int studentId, int courseId, string status = "Enrolled");
            Task<bool> RemoveEnrollmentAsync(int enrollmentId);
            Task<List<Enrollment>> GetAllEnrollmentsAsync();
            Task<IEnumerable<Course>> GetStudentCoursesAsync(int studentId, int courseId, string v);
        
    }
}
