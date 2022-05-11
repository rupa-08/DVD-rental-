using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AppDevGroupCoursework.Request
{
    [Keyless]
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
