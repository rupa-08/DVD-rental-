using System.ComponentModel.DataAnnotations;

namespace AppDevGroupCoursework.Models
{
    public class DVDCategory
    {
        [Key]
        public int categoryNumber { get; set; }
        public string categoryDescription { get; set; }
        public bool ageRestriction { get; set; }
    }
}
