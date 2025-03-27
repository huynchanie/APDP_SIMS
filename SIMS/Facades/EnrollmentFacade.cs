using SIMS.Models;
using SIMS.Repositories;
using System;
using System.Collections.Generic;

namespace SIMS.Facades
{
    public class EnrollmentFacade : IEnrollmentFacade
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public EnrollmentFacade(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository ?? throw new ArgumentNullException(nameof(enrollmentRepository));
        }

        // Lấy tất cả Enrollment
        public IEnumerable<Enrollment> GetAllEnrollments()
        {
            return _enrollmentRepository.GetAllEnrollments();
        }

        // Lấy Enrollment theo Id
        public Enrollment GetEnrollmentById(int id)
        {
            return _enrollmentRepository.GetEnrollmentById(id)
                   ?? throw new KeyNotFoundException($"Enrollment with id {id} not found.");
        }

        // Tạo mới Enrollment
        public void CreateEnrollment(Enrollment enrollment)
        {
            if (enrollment == null) throw new ArgumentNullException(nameof(enrollment));
            _enrollmentRepository.AssignEnrollment(enrollment);
        }

        // Cập nhật Enrollment
        public void UpdateEnrollment(Enrollment enrollment)
        {
            if (enrollment == null) throw new ArgumentNullException(nameof(enrollment));
            _enrollmentRepository.UpdateEnrollment(enrollment);
        }

        // Xóa Enrollment theo Id
        public void DeleteEnrollment(int id)
        {
            var enrollment = _enrollmentRepository.GetEnrollmentById(id);
            if (enrollment != null)
            {
                _enrollmentRepository.DeleteEnrollment(id);
            }
            else
            {
                throw new KeyNotFoundException($"Enrollment with id {id} not found.");
            }
        }
    }
}
