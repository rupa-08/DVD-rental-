using System.ComponentModel.DataAnnotations;

namespace AppDevGroupCoursework.Models
{
    public class Studio
    {
        [Key]
        public int studioNumber { get; set; }
        public string studioName { get; set; }
    }
}
