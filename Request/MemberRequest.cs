using Microsoft.EntityFrameworkCore;

namespace AppDevGroupCoursework.Request
{
    [Keyless]
    public class MemberRequest
    {
        public string firstName { get; set; }
        public string middleName { get; set; }

        public string lastName { get; set; }

        public string email { get; set; }
        public string phoneNumber { get; set; }
        public string address { get; set; }
        public string memberCategory { get; set; }
        public DateTime dateOfBirth { get; set; }
    }
}
