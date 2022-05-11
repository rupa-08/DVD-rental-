using AppDevGroupCoursework.DTO;
using AppDevGroupCoursework.Models;
using AppDevGroupCoursework.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppDevGroupCoursework.Controllers
{
    public class DVDCopyController : Controller
    {
        private readonly DatabaseContext dataBaseContext;
        public DVDCopyController(DatabaseContext db)
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
        //Question No 5
        public IActionResult lastLoanDetailsOfDVDCopy(int copyNumber) {
            List<DVDCopyDetailDTO> dVDCopyDetailDTOs = new List<DVDCopyDetailDTO>();
            if (copyNumber != 0) {
                Loan loan = new();
                DVDCopyDetailDTO detailDTO = new DVDCopyDetailDTO();
                Member member = new Member();
                DVDTitle dvd = new DVDTitle();
                DVDCopy dvdCopy = dataBaseContext.DVDCopy.Include(x => x.dVDTitle).Where(x => x.copyNumber == copyNumber).First();

                if (dvdCopy != null)
                {
                    loan = dataBaseContext.Loan.Include(x => x.DVDCopy).ThenInclude(x => x.dVDTitle).Include(x => x.Member).Where(x => x.DVDCopy == dvdCopy).OrderBy(x=>x.DateOut).Last();
                }
                if (dvdCopy != null)
                {
                    dvd = dataBaseContext.DVDTitle.Where(x => x.dVDNumber == dvdCopy.dVDTitle.dVDNumber).First();
                }
                if (loan != null)
                {
                    member = dataBaseContext.Member.Where(x => x.id == loan.Member.id).First();
                    detailDTO.FullName = member.firstName + " " + member.lastName;
                    detailDTO.Title = dvd.title;
                    detailDTO.CopyNumber = dvdCopy.copyNumber;
                    detailDTO.DateOut = loan.DateOut;
                    detailDTO.DueDate = loan.DateDue;
                    detailDTO.DateReturned = loan.DateReturned;
                    dVDCopyDetailDTOs.Add(detailDTO);
                }
            }
            return View(dVDCopyDetailDTOs);

        }
        public IActionResult DVDCopiesOnLoan()
        {
            List<DVDCopiesOnLoanDTO> dVDCopiesOnloan= CopiesOnLoan();
            return View(dVDCopiesOnloan);
        }
        public IActionResult listOfDVDsOlderThan365Days()
        {
            List<DVDCopy> dvdCopiesNotOnLoan = getDVDCopyNotOnLoan();
            return View(dvdCopiesNotOnLoan);
        }
        public async Task<IActionResult> deleteListOfDVDsOlderThan365Days() {
            List<DVDCopy> dvdCopiesNotOnLoan = getDVDCopyNotOnLoan();
            foreach (var dvdCopy in dvdCopiesNotOnLoan) { 
               try
                {
                    DVDCopy copyData = dataBaseContext.DVDCopy.Where(x => x.copyNumber == dvdCopy.copyNumber).First();
                    dataBaseContext.DVDCopy.Remove(copyData);
                    await dataBaseContext.SaveChangesAsync();
                }
                catch (Exception)
                {
                    return null;
                }
            }
            ViewBag.DeleteMessage = "The list of DVD Copies have been deleted";
            return RedirectToAction("listOfDVDsOlderThan365Days");
        }
        private Loan findDVDOnLoan(DVDCopy dVDCopy)
        {
            Loan findLoan = dataBaseContext.Loan.Include(x=>x.Member).Include(x=>x.DVDCopy).Include(x=>x.LoanType).Where(x => x.DVDCopy == dVDCopy && x.Status == "loaned").First();
            return findLoan;

        }

        

       
        private List<DVDCopy> getDVDCopyNotOnLoan()
        {
            List<DVDCopy> dvdCopies = dataBaseContext.DVDCopy.Include(x=>x.dVDTitle).ToList();
            List<DVDCopy> newcopies= dvdCopies.Where(x => (DateTime.Now.Date - x.datePurchased.Date).TotalDays >= 365).ToList();
            List<DVDCopy> dvdCopiesNotOnLoan = new List<DVDCopy>();
            foreach (var copy in newcopies)
            {
                List<Loan> copyLoans = dataBaseContext.Loan.Where(x => x.DVDCopy == copy && x.Status == "loaned").ToList();
                if (copyLoans.Count == 0)
                {
                    dvdCopiesNotOnLoan.Add(copy);
                }
            }
            return dvdCopiesNotOnLoan;
        }
        private List<DVDCopiesOnLoanDTO> CopiesOnLoan()
        {
            List<DVDCopiesOnLoanDTO> copiesOnLoan = new List<DVDCopiesOnLoanDTO>();
            List<DVDTitle> dVDTitles = dataBaseContext.DVDTitle.ToList();
            List<DVDCopy> dVDCopies = new List<DVDCopy>();
            List<Loan> copyLoans = new List<Loan>();
            Member member = new Member();
            foreach (var dVDTitle in dVDTitles)
            {
                dVDCopies = dataBaseContext.DVDCopy.Include(x => x.dVDTitle).Where(x => x.dVDTitle == dVDTitle).ToList();
                foreach (var copy in dVDCopies)
                {
                    copyLoans = dataBaseContext.Loan.Include(x => x.DVDCopy).Include(x => x.Member).Where(x => x.DVDCopy == copy && x.Status == "loaned").ToList();
                    if (copyLoans != null)
                    {
                        foreach (var loan in copyLoans)
                        {
                            member = dataBaseContext.Member.Where(x => x.id == loan.Member.id).First();
                            DVDCopiesOnLoanDTO detailDto = new DVDCopiesOnLoanDTO();
                            detailDto.dateOut = loan.DateOut;
                            detailDto.title = dVDTitle.title;
                            detailDto.name = member.firstName + " " + member.lastName;
                            detailDto.copyNumber = copy.copyNumber;
                            copiesOnLoan.Add(detailDto);
                        }
                    }
                }
            }
            return copiesOnLoan.OrderByDescending(x => x.dateOut).ThenBy(x => x.title).ToList();
        }
        private void CheckDVDCopyOnLoan(DVDCopy dVDCopy)
        {
            var loan = dataBaseContext.Loan.Where(x => x.DVDCopy == dVDCopy && x.Status == "loaned").First();
            if (loan != null)
            {
                throw new Exception("The DVD copy is in loan");
            }
        }

    }
}
