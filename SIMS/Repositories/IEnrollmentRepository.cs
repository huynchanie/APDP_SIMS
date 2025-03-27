using SIMS.Models;
using System.Collections.Generic;

namespace SIMS.Repositories
{
    public interface IEnrollmentRepository
    {
        IEnumerable<Enrollment> GetAllEnrollments();
        Enrollment GetEnrollmentById(int id);
        void AssignEnrollment(Enrollment enrollment);
        void UpdateEnrollment(Enrollment enrollment);
        void DeleteEnrollment(int id);
    }
}
