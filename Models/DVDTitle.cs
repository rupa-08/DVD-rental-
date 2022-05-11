using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppDevGroupCoursework.Models
{
    public class DVDTitle
    {
        [Key]
        public int dVDNumber { get; set; }
        
        [ForeignKey("ProducerNumber")]
        public Producer producer { get; set; }
        
        [ForeignKey("CategoryNumber")]
        public DVDCategory dVDCategory { get; set; }
        
        [ForeignKey("StudioNumber")]
        public Studio studio { get; set; }

        public String title { get; set; }
        public DateTime dateReleased { get; set; }

        public double standardCharge { get; set; }

        public double penaltyCharge { get; set; }

        public DateTime dateAdded { get; set; }

    }
}
