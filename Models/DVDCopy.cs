using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppDevGroupCoursework.Models
{
    public class DVDCopy
    {
        [Key]
        public int copyNumber { get; set; }

        [ForeignKey("DVDNumber")]
        public DVDTitle dVDTitle { get; set; }

        public DateTime datePurchased { get; set; }
    }
}
