using System.ComponentModel.DataAnnotations;

namespace AppDevGroupCoursework.Models
{
    public class Actor
    {
        [Key]
        public int actorNumber { get; set; }
        public string actorSurname { get; set; }

        public string actorFirstName { get; set; }
    }
}
