using SIMS.Models;
using System.Collections.Generic;

namespace SIMS.Facades
{
    public interface IEnrollmentFacade
    {
        IEnumerable<Enrollment> GetAllEnrollments();
        Enrollment GetEnrollmentById(int id);
        void CreateEnrollment(Enrollment enrollment);
        void UpdateEnrollment(Enrollment enrollment);
        void DeleteEnrollment(int id);
    }
}
