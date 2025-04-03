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
            return await _context.Grades.FindAsync(gradeId);
        }

        public async Task<bool> AssignGradeAsync(int studentId, int courseId, double score, string comment)
        {
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
