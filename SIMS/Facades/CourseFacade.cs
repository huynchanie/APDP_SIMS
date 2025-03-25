using SIMS.Facades;
using SIMS.Models;
using SIMS.Repositories;
using System.Collections.Generic;

public class CourseFacade : ICourseFacade
{
    private readonly ICourseRepository _courseRepository;

    public CourseFacade(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public void CreateCourse(Course course)
    {
        _courseRepository.CreateCourse(course);
    }

    public void EditCourse(Course course)
    {
        _courseRepository.EditCourse(course);
    }

    public void DeleteCourse(int id)
    {
        _courseRepository.DeleteCourse(id);
    }

    public Course GetCourseById(int id)
    {
        return _courseRepository.GetCourseById(id);
    }

    public IEnumerable<Course> GetAllCourses()
    {
        return _courseRepository.GetAllCourses();
    }
}
