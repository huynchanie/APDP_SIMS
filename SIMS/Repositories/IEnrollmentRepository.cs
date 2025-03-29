using SIMS.Models;
using System.Collections.Generic;

namespace SIMS.Repositories
{
    public interface IEnrollmentRepository
    {
        Task<List<Enrollment>> GetAllEnrollmentsAsync();
        Task<Enrollment> GetEnrollmentByIdAsync(int enrollmentId);
        Task<List<Enrollment>> GetEnrollmentsByStudentIdAsync(int studentId);
        Task<bool> AddEnrollmentAsync(Enrollment enrollment);
        Task<bool> RemoveEnrollmentAsync(int enrollmentId);
        Task<IEnumerable<Course>> GetStudentCoursesAsync(int studentId);
    }
}
