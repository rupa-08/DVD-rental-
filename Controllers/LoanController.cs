using AppDevGroupCoursework.DTO;
using AppDevGroupCoursework.Models;
using AppDevGroupCoursework.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppDevGroupCoursework.Controllers
{
    public class LoanController : Controller
    {
        private readonly DatabaseContext dataBaseContext;
        public LoanController(DatabaseContext db)
        {
            dataBaseContext = db;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LoanDetails(string name,string selectOptions)
        {
            List<LoanDetailDTO>  loanDetails= CopiesOnLoan();
            if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(selectOptions) && selectOptions == "loanfor31days")
            { 
               loanDetails = loanDetails.Where(x => x.Name == name && (DateTime.Now.Date - x.DateOut.Date).TotalDays <= 31).ToList();
            }
            else if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(selectOptions) && selectOptions == "loaned")
            {
                loanDetails = loanDetails.Where(x => x.Name == name && x.status== "loaned").ToList();
            }
            else if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(selectOptions) && selectOptions == "returned")
            {
                loanDetails = loanDetails.Where(x => x.Name == name && x.status == "returned").ToList();
                
            }
            return View(loanDetails);
        }
        public IActionResult SaveLoan()
        {
            return View();
        }
        //Question No 6
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveLoan(LoanRequest request)
        {
            if (ModelState.IsValid)
            {
                Member member = toMember(request.memberNumber);
                MembershipCategory membershipCategory = toMembershipCategory(member.membershipCategory.membershipCategoryNumber);
                DVDCopy dVDCopy = ToDVDCopy(request.copyNumber);
                DVDTitle dVD = ToDVDTitle(dVDCopy.dVDTitle.dVDNumber);
                DVDCategory category = toDVDCategory(dVD.dVDCategory.categoryNumber);
                LoanType loanType = ToLoanType(request.loanType);
                CheckAgeRestricions(member, category);
                CheckLoans(membershipCategory, member);
                Loan loan = ToLoan(loanType, member, dVDCopy);
                Loan savedLoan = await Save(loan);
                return RedirectToAction("LoanDetails");
            }
            else
            {
                return View();
            }
        }
        public IActionResult UpdateLoan(int? loanNumber)
        {
            if (loanNumber == null || loanNumber == 0)
            {
                return NotFound();
            }
            List<LoanDetailDTO> loans = CopiesOnLoan();
            LoanDetailDTO dVDOnLoan = loans.Where(x => x.LoanNumber == loanNumber).First();
            if (dVDOnLoan == null)
            {
                return NotFound();
            }
            return View(dVDOnLoan);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLoan(int loanNumber)
        {
            var loans = dataBaseContext.Loan.ToList();
            Loan loan = loans.Where(x=>x.LoanNumber==loanNumber).First();
            Loan updatedLoan = await UpdateLoanDetail(loan);
            return RedirectToAction("LoanDetails");
        }
        private List<LoanDetailDTO> CopiesOnLoan()
        {
            List<LoanDetailDTO> copiesOnLoan = new List<LoanDetailDTO>();
            List<DVDTitle> dVDTitles = dataBaseContext.DVDTitle.ToList();
            List<DVDCopy> dVDCopies = new List<DVDCopy>();
            List<Loan> copyLoans = new List<Loan>();
            Member member = new Member();
            foreach (var dVDTitle in dVDTitles)
            {
                dVDCopies = dataBaseContext.DVDCopy.Include(x => x.dVDTitle).Where(x => x.dVDTitle == dVDTitle).ToList();
                foreach (var copy in dVDCopies)
                {
                    copyLoans = dataBaseContext.Loan.Include(x => x.DVDCopy).Include(x => x.Member).Where(x => x.DVDCopy == copy).ToList();
                    if (copyLoans != null)
                    {
                        foreach (var loan in copyLoans)
                        {
                            double standardCharge = 0;
                            double penaltyCharge = 0;
                            double totalCharge = 0;
                            double dueDays = 0;
                            if (DateTime.Now.Date > loan.DateDue.Date)
                            {
                                dueDays = (DateTime.Now.Date - loan.DateDue.Date).TotalDays;
                                penaltyCharge = dueDays * dVDTitle.penaltyCharge;
                            }
                            standardCharge = dVDTitle.standardCharge * (loan.DateDue.Date- loan.DateOut.Date).TotalDays;
                            totalCharge = standardCharge + penaltyCharge;
                            member = dataBaseContext.Member.Where(x => x.id == loan.Member.id).First();
                            LoanDetailDTO detailDto = new LoanDetailDTO();
                            detailDto.DateOut = loan.DateOut;
                            detailDto.DueDate = loan.DateDue;
                            detailDto.LoanNumber = loan.LoanNumber;
                            detailDto.Title = dVDTitle.title;
                            detailDto.StandardCharge = standardCharge;
                            detailDto.PenaltyCharge = penaltyCharge;
                            detailDto.TotalCharge = totalCharge;
                            detailDto.Name = member.firstName + " " + member.lastName;
                            detailDto.CopyNumber = copy.copyNumber;
                            detailDto.NumberOfDays = dueDays;
                            detailDto.status = loan.Status;
                            copiesOnLoan.Add(detailDto);
                        }
                    }
                }
            }
            return copiesOnLoan.OrderByDescending(x => x.DateOut).ThenBy(x => x.Title).ToList();
        }
        private void CheckLoans(MembershipCategory membershipCategory, Member member)
        {
            var loans = dataBaseContext.Loan.Where(x => x.Member == member && x.Status == "loaned").ToArray();
            if (loans.Length == membershipCategory.membershipCategoryTotalLoans)
            {
                throw new Exception(" The user has racehed his maximum capaicty of taking loan");
            }
        }

        public IActionResult CheckAgeRestricions(Member member, DVDCategory category)
        {
            double days = (DateTime.Now.Date - member.dateOfBirth.Date).TotalDays;
            double years = days / 365;
            if (years < 18 && category.ageRestriction == true)
            {
                return View("/Home/Error");
            }
            return View();
        }
        private Loan ToLoan(LoanType loanType, Member member, DVDCopy dVDCopy)
        {
            Loan loan = new Loan();
            loan.DateOut = DateTime.Now;
            loan.Member = member;
            loan.DVDCopy = dVDCopy;
            loan.LoanType = loanType;
            loan.Status = "loaned";
            loan.DateDue = ToDateDue(loanType.loanDuration);
            return loan;
            }

        private DateTime ToDateDue(string loanDuration)
        {
            DateTime dateDue = DateTime.Now;
            if (loanDuration.ToLower() == "week")
            {
                dateDue = DateTime.Now.AddDays(7);
            }
            if (loanDuration == "fortnight")
            {
                dateDue = DateTime.Now.AddDays(14);
            }
            if (loanDuration.ToLower() == "month")
            {
                dateDue = DateTime.Now.AddDays(30);
            }
            return dateDue;
        }

        private LoanType ToLoanType(string loanType)
        {
            LoanType type = dataBaseContext.LoanType.Where(x => x.loan_type == loanType).First();
            return type;
        }

        private Member toMember(int memberNumber)
        {
            Member member = dataBaseContext.Member.Include(x => x.membershipCategory).Where(x => x.id == memberNumber).First();
            return member;
        }
        private DVDCopy ToDVDCopy(int copynumber)
        {
            DVDCopy copy = dataBaseContext.DVDCopy.Include(x => x.dVDTitle).Where(x => x.copyNumber == copynumber).First();
            return copy;
        }
        private DVDTitle ToDVDTitle(int dvdNumber)
        {
            DVDTitle title = dataBaseContext.DVDTitle.Include(x => x.dVDCategory).Where(x => x.dVDNumber == dvdNumber).First();
            return title;
        }
        private DVDCategory toDVDCategory(int categoryNumber)
        {
            DVDCategory category = dataBaseContext.DVDCategory.Where(x => x.categoryNumber == categoryNumber).First();
            return category;
        }
        private MembershipCategory toMembershipCategory(int membershipCategoryNumber)
        {
            MembershipCategory category = dataBaseContext.MembershipCategory.Where(x => x.membershipCategoryNumber == membershipCategoryNumber).First();
            return category;
        }
        private async Task<Loan> Save(Loan loan)
        {
            Loan saveLoan = new Loan();
            try
            {
                saveLoan = dataBaseContext.Loan.Add(loan).Entity;
                await dataBaseContext.SaveChangesAsync();
            }
            catch (Exception)
            {

            }
            return saveLoan;
        }
        private async Task<Loan> UpdateLoanDetail(Loan loan)
        {
            loan.Status = "returned";
            loan.DateReturned = DateTime.Now;
            Loan updatedLoan = new Loan();
            try
            {
                updatedLoan = dataBaseContext.Loan.Update(loan).Entity;
                await dataBaseContext.SaveChangesAsync();
            }
            catch (Exception)
            {

            }
            return updatedLoan;
        }
        /*private void CheckDVDCopyOnLoan(DVDCopy dVDCopy)
        {
            var loans = dataBaseContext.Loan.ToList();
            var loan = loans.Where(x=>x.DVDCopy== dVDCopy && x.Status== "loaned").First();
            if (loan != null) {
                throw new Exception("The copy is already in loan");
            }
        }*/
    }
}
