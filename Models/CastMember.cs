using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppDevGroupCoursework.Models
{
    public class CastMember
    {
        [Key]
        public int castMemberNumber { get; set; }

        [ForeignKey("DVDNumber")]
        public DVDTitle dvdTitle { get; set; }

        [ForeignKey("ActorNumber")]
        public Actor actor { get; set; }
    }
}
