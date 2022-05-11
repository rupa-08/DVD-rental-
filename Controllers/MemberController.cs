using AppDevGroupCoursework.DTO;
using AppDevGroupCoursework.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppDevGroupCoursework.Controllers
{
    public class MemberController: Controller
    {
        private readonly DatabaseContext dataBaseContext;

        public int MemberLoanDetailDTOs { get; private set; }

        public MemberController(DatabaseContext db)
        {
            dataBaseContext = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        private DatabaseContext GetDataBaseContext()
        {
            return dataBaseContext;
        }
        //Question No 8
        public IActionResult MemberLoanDetails() {
            List<MemberLoanDetailsDTO> dtos = new List<MemberLoanDetailsDTO>();
            List<Member> memberList = dataBaseContext.Member.Include(x=>x.membershipCategory).ToList();
            String remarks = "";
            if (memberList != null) {
                foreach (Member member in memberList)
                {
                    var membershipCategory = dataBaseContext.MembershipCategory.Where(x => x.membershipCategoryNumber == member.membershipCategory.membershipCategoryNumber).First();
                    int totalLoan= dataBaseContext.Loan.Include(x=>x.Member).Where(x => x.Member == member 
                        && x.Status == "loaned").ToArray().Length;
                    if (totalLoan > membershipCategory.membershipCategoryTotalLoans) {
                        remarks = "Too many DVDs";
                    }
                    if (totalLoan > 0)
                    {
                        MemberLoanDetailsDTO dto = new MemberLoanDetailsDTO();
                        dto.Address = member.address;
                        dto.Email = member.email;
                        dto.FirstName = member.firstName;
                        dto.LastName = member.lastName;
                        dto.PhoneNumber = member.phoneNumber;
                        dto.Remarks = remarks;
                        dto.DateOfBirth = member.dateOfBirth.ToLongDateString();
                        dto.TotalLoans = totalLoan;
                        dto.Description = membershipCategory.membershipCategoryDescription;
                        dtos.Add(dto);
                        remarks = "";
                    }
                }
            }
            List<MemberLoanDetailsDTO> orderedDtos= dtos.OrderBy(x=>x.FirstName).ToList();
            ViewBag.DTOS = orderedDtos;
            return View(orderedDtos);
        }
        //question no 12
        public IActionResult MemberDetailsWhoHaveNotTakenLoanFor31days() {
            List<Member> members = new List<Member>();
            members = dataBaseContext.Member.ToList();
            List<MemberDetailsNotTakenLoanDTO> memberDTOs = new List<MemberDetailsNotTakenLoanDTO>();
            List<Loan> memberLoans = new List<Loan>();
            DVDCopy dvdCopy= new DVDCopy();
            Loan memberLoan = new Loan();
            String  title = "";
            foreach (var member in members) {
                memberLoans = dataBaseContext.Loan.Include(x => x.DVDCopy).Where(x => x.Member == member).ToList();
                var Loans = memberLoans.Where(x=>(DateTime.Now.Date - x.DateOut.Date).TotalDays > 31).ToList();
                foreach (var loan in Loans) {
                     dvdCopy = dataBaseContext.DVDCopy.Include(x=>x.dVDTitle).Where(x=>x.copyNumber==loan.DVDCopy.copyNumber).First();
                     memberLoan = loan;
                    var titles = dataBaseContext.DVDTitle.Where(x => x.dVDNumber == dvdCopy.dVDTitle.dVDNumber);
                    foreach (var dVDTitle in titles)
                    {
                        title = dVDTitle.title;
                    }
                }
                 if (Loans.Count > 0) {
                    MemberDetailsNotTakenLoanDTO dto = new MemberDetailsNotTakenLoanDTO();
                    dto.firstName = member.firstName;
                    dto.lastName = member.lastName;
                    dto.address=    member.address;
                    dto.DvdTitle = title;
                    dto.dateOut = memberLoan.DateOut.Date.ToLongDateString();
                    dto.numberOfDays = (DateTime.Now.Date - memberLoan.DateOut).TotalDays;
                    memberDTOs.Add(dto);
                }
                
            }
            return View(memberDTOs);
        }
        
    }
}
