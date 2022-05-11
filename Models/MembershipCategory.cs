using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppDevGroupCoursework.Models
{
    public class MembershipCategory
    {
        [Key]
        public int membershipCategoryNumber { get; set; }
        public string membershipCategoryDescription { get; set; }
        public int membershipCategoryTotalLoans { get; set; }
    }
}
