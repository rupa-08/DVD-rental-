using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AppDevGroupCoursework.Request
{
    [Keyless]
    public class UserRequest
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string fullName { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string phoneNumber { get; set; }
        [Required]
        public string jobTitle { get; set; }
        [Required]
        public string password { get; set; }
    }
}
