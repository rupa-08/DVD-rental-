using AppDevGroupCoursework.Models;

namespace AppDevGroupCoursework.DTO
{
    public class DVDDetailsDTO
    {
        public int DVDNumber { get; set; }
        public string StudioName { get; set; }
        public string Title { get; set; }
        public string ProducerName { get; set; }
        public List<Actor> Actors { get; set; }
        public List<Loan> loans { get; set; }
        public DateTime ReleasedDate { get; set; }

        public string Category { get; set; }

        public int TotalNumberOfCopies { get; set; }
        public bool AgeRestrictions { get; set; }

    }
}
