using SIMS.Models;

namespace SIMS.Repositories
{
    public interface IGradeRepository
    {
        Task<List<Grade>> GetGradesByCourseAsync(int courseId);
        Task<Grade> GetGradeByIdAsync(int gradeId);
        Task<bool> AssignGradeAsync(int studentId, int courseId, double score, string comment);
        Task<bool> UpdateGradeAsync(int gradeId, double score, string comment);
        Task<bool> DeleteGradeAsync(int gradeId);
    }
}
