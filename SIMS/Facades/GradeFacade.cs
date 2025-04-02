using SIMS.Facades;
using SIMS.Models;
using SIMS.Repositories;

public class GradeFacade : IGradeFacade
{
    private readonly IGradeRepository _gradeRepo;

    public GradeFacade(IGradeRepository gradeRepo)
    {
        _gradeRepo = gradeRepo;
    }

    public async Task<List<Grade>> GetGradesByCourseAsync(int courseId)
    {
        return await _gradeRepo.GetGradesByCourseAsync(courseId);
    }

    public async Task<Grade> GetGradeByIdAsync(int gradeId)
    {
        return await _gradeRepo.GetGradeByIdAsync(gradeId);
    }

    public async Task<bool> AssignGradeAsync(int studentId, int courseId, double score, string comment)
    {
        return await _gradeRepo.AssignGradeAsync(studentId, courseId, score, comment);
    }

    public async Task<bool> UpdateGradeAsync(int gradeId, double score, string comment)
    {
        return await _gradeRepo.UpdateGradeAsync(gradeId, score, comment);
    }

    public async Task<bool> DeleteGradeAsync(int gradeId)
    {
        return await _gradeRepo.DeleteGradeAsync(gradeId);
    }
}
