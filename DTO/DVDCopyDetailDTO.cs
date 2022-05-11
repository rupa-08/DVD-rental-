namespace AppDevGroupCoursework.DTO
{
    public class DVDCopyDetailDTO
    {
        public string Title { get; set; }

        public int CopyNumber { get; set; }
        public string FullName { get; set; } 

        public DateTime DateOut { get; set; }

        public DateTime DueDate { get; set; }
        
        public DateTime DateReturned { get;  set;}    
    }
}
