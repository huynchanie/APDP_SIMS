using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [Required]
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
    }
}
