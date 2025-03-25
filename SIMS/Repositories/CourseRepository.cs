using SIMS.Models;
using System.Collections.Generic;
using System.Linq;

using SIMS.Data;

namespace SIMS.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly DataContext _context;

        public CourseRepository(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<Course> GetAllCourses()
        {
            return _context.Courses.ToList();
        }

        public Course GetCourseById(int id)
        {
            return _context.Courses.Find(id);
        }

        public void CreateCourse(Course course)
        {
            try
            {
                Console.WriteLine($"[CourseRepository] Create Course: {course.CourseName}");
                _context.Courses.Add(course);
                _context.SaveChanges();
                Console.WriteLine("[CourseRepository] Course saved successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[CourseRepository] Error: " + ex.Message);
            }
        }

        public void EditCourse(Course course)
        {
            try
            {
                _context.Courses.Update(course);
                _context.SaveChanges();
                Console.WriteLine($"[CourseRepository] Updated Course: {course.CourseName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[CourseRepository] Error: " + ex.Message);
                throw;
            }
        }

        public void DeleteCourse(int id)
        {
            var course = _context.Courses.Find(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                _context.SaveChanges();
                Console.WriteLine($"[CourseRepository] Course with ID {id} deleted successfully.");
            }
            else
            {
                Console.WriteLine($"[CourseRepository] Course with ID {id} not found.");
            }
        }



    }
}

