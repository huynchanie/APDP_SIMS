using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class Grade
    {
        [Key]
        public int GradeId { get; set; }
        [Required]
        [Range(0, 100)]
        public double Score { get; set; }
        [MaxLength(500)]
        public string Comment { get; set; }
        [Required] 
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public User User { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

      

    }
}
