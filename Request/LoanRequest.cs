using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AppDevGroupCoursework.Request
{
 [Keyless]
    public class LoanRequest
    {
        [Required]
        public int memberNumber { get; set; }
        [Required]
        public int copyNumber { get; set; }
        [Required]
        public string loanType { get; set; }
    }
}
