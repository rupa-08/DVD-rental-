using Microsoft.EntityFrameworkCore;

namespace AppDevGroupCoursework.Request
{
    [Keyless]
    public class CastMemberRequest
    {
        public string ActorSurname { get; set; }
        public string ActorFirstName { get; set; }  
    }
}
