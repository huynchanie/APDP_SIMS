using SIMS.Data;
using SIMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SIMS.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly DataContext _context;

        public EnrollmentRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Lấy tất cả các Enrollment
        public IEnumerable<Enrollment> GetAllEnrollments()
        {
            return _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .AsNoTracking()
                .ToList();
        }

        // Lấy Enrollment theo Id
        public Enrollment GetEnrollmentById(int id)
        {
            return _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefault(e => e.EnrollmentId == id);
        }

        // Gán khóa học cho sinh viên
        public void AssignEnrollment(Enrollment enrollment)
        {
            if (enrollment == null) throw new ArgumentNullException(nameof(enrollment));

            _context.Enrollments.Add(enrollment);
            _context.SaveChanges();
        }

        // Cập nhật thông tin Enrollment
        public void UpdateEnrollment(Enrollment enrollment)
        {
            if (enrollment == null) throw new ArgumentNullException(nameof(enrollment));

            var existingEnrollment = _context.Enrollments.FirstOrDefault(e => e.EnrollmentId == enrollment.EnrollmentId);

            if (existingEnrollment != null)
            {
                existingEnrollment.StudentId = enrollment.StudentId;
                existingEnrollment.CourseId = enrollment.CourseId;
                existingEnrollment.Status = enrollment.Status;
                existingEnrollment.EnrolledAt = enrollment.EnrolledAt;

                _context.SaveChanges();
            }
        }

        // Xóa Enrollment theo Id
        public void DeleteEnrollment(int id)
        {
            var enrollment = _context.Enrollments.FirstOrDefault(e => e.EnrollmentId == id);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                _context.SaveChanges();
            }
        }
    }
}
