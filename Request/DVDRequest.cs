using AppDevGroupCoursework.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppDevGroupCoursework.Request
{
    [Keyless]
    public class DVDRequest
    {
        [Required]
        public string StudioName { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string ProducerName { get; set; }
        [Required]
        public double StandardCharge { get; set; }
        [Required]
        public double PenaltyCharge { get; set; }
        [Required]
        public string DvdDescription { get; set; }
        public bool AgeRestriction { get; set; }
        [Required]
        public DateTime ReleaseDate { get; set; }
        [Required]
        public DateTime DatePurchased { get; set; }

        [Required]
        public int TotalNoCopies { get; set; }

        [NotMapped]
        public  CastMemberRequest castMember { get; set; }
    }
}
