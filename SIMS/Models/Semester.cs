using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class Semester
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string SemesterName { get; set; }

        public DateTime Year { get; set; }

        public ICollection<Course> Courses { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
