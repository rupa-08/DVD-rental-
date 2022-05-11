using System.ComponentModel.DataAnnotations;

namespace AppDevGroupCoursework.Models
{
    public class Producer
    {
        [Key]
        public int producerNumber { get; set; }
        public string producerName { get; set; }
    }
}
