using Microsoft.EntityFrameworkCore;
using SIMS.Data;
using SIMS.Models;
using SIMS.Repositories;
using System;
using System.Collections.Generic;

namespace SIMS.Facades
{
    public class EnrollmentFacade : IEnrollmentFacade

    {
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly IEnrollmentRepository _enrollmentRepository;
        public EnrollmentFacade(IEnrollmentRepository enrollmentRepo, IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepo = enrollmentRepo;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<bool> AssignStudentToCourseAsync(int studentId, int courseId, string status )
        {
            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                Status = status,
                EnrolledAt = DateTime.UtcNow
            };

            return await _enrollmentRepo.AddEnrollmentAsync(enrollment);
        }

        public async Task<List<Enrollment>> GetAllEnrollmentsAsync()
        {
            return await _enrollmentRepo.GetAllEnrollmentsAsync();
        }

        public async Task<IEnumerable<Course>> GetStudentCoursesAsync(int studentId, int courseId, string v)
        {
            return await _enrollmentRepo.GetStudentCoursesAsync(studentId);
        }

        public async Task<bool> RemoveEnrollmentAsync(int enrollmentId)
        {
            return await _enrollmentRepo.RemoveEnrollmentAsync(enrollmentId);
        }
    }
}
