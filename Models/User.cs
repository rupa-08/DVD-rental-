using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppDevGroupCoursework.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int usernumber { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string phoneNumber { get; set; }

        [Required]
        public string username { get; set; }

        [Required]
        public string usertype { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(16, MinimumLength = 8,
            ErrorMessage = "Password must be between 8 and 16 characters long")]
        public string password { get; set; }
    }
}
