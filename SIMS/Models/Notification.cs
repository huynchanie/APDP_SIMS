using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        [MaxLength(500)]
        public string Message { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
