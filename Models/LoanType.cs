using System.ComponentModel.DataAnnotations;

namespace AppDevGroupCoursework.Models
{
    public class LoanType
    {
        [Key]
        public int loanTypeNumber { get; set; }
        public string loan_type { get; set; }

        public string loanDuration { get; set; }
    }
}
