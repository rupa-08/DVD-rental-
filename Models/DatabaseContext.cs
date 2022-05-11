using Microsoft.EntityFrameworkCore;
using AppDevGroupCoursework.Request;

namespace AppDevGroupCoursework.Models
{
    public class DatabaseContext: DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        } 
        public DbSet<Actor> Actor { get; set; }
        public DbSet<Studio> Studio { get; set; }
        public DbSet<DVDCategory> DVDCategory { get; set; }
        public DbSet<Producer> Producer { get; set; }
        public DbSet<DVDCopy> DVDCopy { get; set; }
        public DbSet<Loan> Loan { get; set; }
        public DbSet<LoanType> LoanType { get; set; }
        public DbSet<Member> Member { get; set; }
        public DbSet<MembershipCategory> MembershipCategory { get; set; }
        public DbSet<User> User { get; set; }

        public DbSet<DVDTitle> DVDTitle { get; set; }

        public DbSet<CastMember> CastMember { get; set; }

        public DbSet<AppDevGroupCoursework.Request.DVDRequest> DVDRequest { get; set; }

        public DbSet<AppDevGroupCoursework.Request.LoginRequest> LoginRequest { get; set; }

    }
}
