using Microsoft.EntityFrameworkCore;
using SIMS.Data;
using SIMS.Models;

namespace SIMS.Repositories
{
    public class GradeRepository : IGradeRepository
    {
        private readonly DataContext _context;

        public GradeRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Grade>> GetGradesByCourseAsync(int courseId)
        {
            return await _context.Grades
                .Where(g => g.CourseId == courseId)
                .Include(g => g.User)
                .Include(g => g.Course)
                .ToListAsync();
        }

        public async Task<Grade> GetGradeByIdAsync(int gradeId)
        {
            return await _context.Grades
                .Include(g => g.User)
                .Include(g => g.Course)
                .FirstOrDefaultAsync(g => g.GradeId == gradeId);
        }

        public async Task<bool> AssignGradeAsync(int studentId, int courseId, double score, string comment)
        {
            // Check if the student is enrolled in this course
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
            
            if (enrollment == null) 
            {
                // Student is not enrolled in this course
                return false;
            }

            // Check if a grade already exists for this student and course
            var existingGrade = await _context.Grades
                .FirstOrDefaultAsync(g => g.UserId == studentId && g.CourseId == courseId);
            
            if (existingGrade != null)
            {
                // Update existing grade instead of creating a new one
                existingGrade.Score = score;
                existingGrade.Comment = comment;
                existingGrade.CreateTime = DateTime.UtcNow;
                
                _context.Grades.Update(existingGrade);
                return await _context.SaveChangesAsync() > 0;
            }

            // Create new grade
            var newGrade = new Grade
            {
                UserId = studentId,
                CourseId = courseId,
                Score = score,
                Comment = comment,
                CreateTime = DateTime.UtcNow
            };

            _context.Grades.Add(newGrade);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateGradeAsync(int gradeId, double score, string comment)
        {
            var grade = await _context.Grades.FindAsync(gradeId);
            if (grade == null) return false;

            grade.Score = score;
            grade.Comment = comment;
            grade.CreateTime = DateTime.UtcNow;

            _context.Grades.Update(grade);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteGradeAsync(int gradeId)
        {
            var grade = await _context.Grades.FindAsync(gradeId);
            if (grade == null) return false;

            _context.Grades.Remove(grade);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
