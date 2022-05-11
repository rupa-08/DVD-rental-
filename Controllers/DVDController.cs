using AppDevGroupCoursework.DTO;
using AppDevGroupCoursework.Models;
using AppDevGroupCoursework.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppDevGroupCoursework.Controllers
{
    public class DVDController : Controller
    {
        private readonly DatabaseContext dataBaseContext;

        public DVDController(DatabaseContext db)
        {
            dataBaseContext = db;
        }
        public IActionResult Index()
        {
            var dvds = dataBaseContext.DVDTitle.Include(x=>x.producer).Include(x=>x.studio).Include(x=>x.dVDCategory).ToArray();
            List<DVDTitleDTO> dtos = new List<DVDTitleDTO>();
            foreach (var dvdTitle in dvds) { 
                DVDTitleDTO dto
                    = new DVDTitleDTO();
                dto.Title = dvdTitle.title;
                dto.AgeRestriction = dvdTitle.dVDCategory.ageRestriction.ToString();
                dto.Description = dvdTitle.dVDCategory.categoryDescription;
                dto.ProducerName = dvdTitle.producer.producerName;
                dto.releasedDate = dvdTitle.dateReleased.ToLongDateString();
                dto.SudioName = dvdTitle.studio.studioName;
                dtos.Add(dto);
            }
            return View(dtos);
        }
        private DatabaseContext GetDataBaseContext()
        {
            return dataBaseContext;
        }
        //GetALlDVDs
        public IActionResult DVDInventory(bool Issuccess = false, bool Isdelete = false)
        {
            ViewBag.issuccess = Issuccess;
            ViewBag.isdelete = Isdelete;
            ViewBag.dvdList = dataBaseContext.DVDTitle.ToArray();
            var dvds = dataBaseContext.DVDTitle.ToArray();
            ViewBag.DVDLists = dvds;
            return View(dvds);
        }
        public IActionResult ManageDVDTitles(string lastname,string selectOption) {
            
            var dvdTitles = dataBaseContext.DVDTitle.Include(x => x.producer).Include(x => x.studio).Include(x => x.dVDCategory).ToList();
            List<CastMember> castMembers = new List<CastMember>();
            List<Actor> actors = new List<Actor>();
            List<Actor> actorsOrderByLastName = new List<Actor>();
            List<DVDDetailsDTO> dVDDetailsDTOs = new List<DVDDetailsDTO>();
            List<DVDCopy> copies = new List<DVDCopy>();
            List<Loan> copyLoans = new List<Loan>();
            bool count = false;
            foreach (var dvdTitle in dvdTitles)
            {
                copies = dataBaseContext.DVDCopy.Where(x => x.dVDTitle == dvdTitle).ToList();
                castMembers = dataBaseContext.CastMember.Include(x => x.dvdTitle).Include(x => x.actor).Where(x => x.dvdTitle == dvdTitle).ToList();
                if (castMembers != null && copies != null)
                {
                    foreach (var castMember in castMembers)
                    {
                        Actor actor = dataBaseContext.Actor.Where(x => x.actorNumber == castMember.actor.actorNumber).First();
                        actors.Add(actor);
                    }
                    actorsOrderByLastName = actors.OrderBy(x => x.actorSurname).ToList();
                    DVDDetailsDTO dVDDetails = toDVDDetailDTO(actorsOrderByLastName, dvdTitle, copies, copyLoans);
                    actors.Clear();
                    dVDDetailsDTOs.Add(dVDDetails);
                }
            }
            if (!String.IsNullOrEmpty(lastname) && !String.IsNullOrEmpty(selectOption) && selectOption=="all")
            { 
                List<DVDDetailsDTO> detailsByActorLastName = new List<DVDDetailsDTO>();
                System.Diagnostics.Debug.WriteLine("Count " + dVDDetailsDTOs.Count().ToString());

                foreach (var dVDDetailDTO in dVDDetailsDTOs)
                {
                    foreach (var actor in dVDDetailDTO.Actors)
                    {
                        if (actor.actorSurname == lastname)
                        {
                            detailsByActorLastName.Add(dVDDetailDTO);
                        }
                    }
                }
                return View(detailsByActorLastName);
            }
            else if(String.IsNullOrEmpty(lastname) && (!String.IsNullOrEmpty(selectOption)) && selectOption == "loan")
            {
                List<DVDDetailsDTO> dVDsOnLoan = new List<DVDDetailsDTO>();
                List<Loan> copiesOnLoan = new List<Loan>();
                List<DVDCopy> copiesLoan = new List<DVDCopy>();
                foreach (var dvdTitle in dvdTitles)
                {
                    var dvdCopies = dataBaseContext.DVDCopy.Include(x => x.dVDTitle).Where(x => x.dVDTitle == dvdTitle).ToList();
                    foreach (DVDCopy copy in dvdCopies)
                    {
                        copiesOnLoan = dataBaseContext.Loan.Include(x => x.DVDCopy).Where(x => x.DVDCopy == copy && x.Status == "Loaned").ToList();
                        if (copiesOnLoan.Count > 0)
                        {
                            copiesLoan.Add(copy);
                            count = true;
                        }
                    }
                    if (count == true)
                    {
                        copies = dataBaseContext.DVDCopy.Where(x => x.dVDTitle == dvdTitle).ToList();
                        castMembers = dataBaseContext.CastMember.Include(x => x.dvdTitle).Include(x => x.actor).Where(x => x.dvdTitle == dvdTitle).ToList();
                        if (castMembers != null && copies != null)
                        {
                            foreach (var castMember in castMembers)
                            {
                                Actor actor = dataBaseContext.Actor.Where(x => x.actorNumber == castMember.actor.actorNumber).First();
                                actors.Add(actor);
                            }
                            actorsOrderByLastName = actors.OrderBy(x => x.actorSurname).ToList();
                            DVDDetailsDTO dVDDetails = toDVDDetailDTO(actorsOrderByLastName, dvdTitle, copiesLoan, copyLoans);
                            actors.Clear();
                            copiesLoan.Clear();
                            dVDsOnLoan.Add(dVDDetails);
                        }
                    }
                    count = false;
                }
                return View(dVDsOnLoan);
            }
            else if (!String.IsNullOrEmpty(lastname) && (!String.IsNullOrEmpty(selectOption)) && selectOption == "notOnLoanFor31days")
            {
                List<DVDDetailsDTO> dVDsNotOnLoan = DVDsNotOnLoan();
                List<DVDDetailsDTO> dVDsNotOnLoanByActorSurname = new List<DVDDetailsDTO>();
                foreach (var dVDNotOnLoan in dVDsNotOnLoan) {
                    foreach (var actor in dVDNotOnLoan.Actors) { 
                       if (actor.actorSurname == lastname) {
                            dVDsNotOnLoanByActorSurname.Add(dVDNotOnLoan);
                        }
                    }
                }
                return View(dVDsNotOnLoanByActorSurname);
            }
            List<DVDDetailsDTO> orderByReleaseDate = dVDDetailsDTOs.OrderBy(x => x.ReleasedDate).ToList();
            return View(orderByReleaseDate);  
        }

        public IActionResult AddDVDTitle()
        {
            return View();
        }
        //Question NO 9
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDVDTitle(DVDRequest request)
        {
            if (ModelState.IsValid) {
                Producer producer = await toProducerAsync(request.ProducerName);
                Studio studio = await toStudioAsync(request.StudioName);
                Actor actor = await toActorAsync(request);
                DVDCategory category = await toDVDCategory(request);
                DVDTitle dVDTitle = await toDVDTitleAsync(request, studio, producer, category);
                for (int i = 0; i < request.TotalNoCopies; i++) {
                    await toDVDCopy(request, dVDTitle);
                }
                await toCastMember(actor, dVDTitle);
                return RedirectToAction("ManageDVDTitles");
            }
            else
            {
                return View();
            }
        }
        public IActionResult EditDVDTitle(int? dVDNumber)
        {
            if (dVDNumber == null || dVDNumber == 0)
            {
                return NotFound();
            }
            var dVDTitles = dataBaseContext.DVDTitle.ToList();
            DVDTitle dVDTitle = dVDTitles.Where(x => x.dVDNumber == dVDNumber).First();
            return View(dVDTitle);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDVDTitle(DVDTitle request)
        {
            try
            {
                dataBaseContext.DVDTitle.Update(request);
                await dataBaseContext.SaveChangesAsync();
                return Redirect("ManageDVDTitles");
            }
            catch
            {
                return null;
            }
            return View();

        }
        public IActionResult DeleteDVDTitle(int? dVDNumber)
        { 
            var dVDTitle = dataBaseContext.DVDTitle.Where(x => x.dVDNumber == dVDNumber).First();
            return View(dVDTitle);
        }
        [HttpPost]
        public IActionResult DeleteDVDTitle(int dVDNumber)
        {
            var dVDTitle = dataBaseContext.DVDTitle.Where(x => x.dVDNumber == dVDNumber).First();
            dataBaseContext.DVDTitle.Remove(dVDTitle);
            dataBaseContext.SaveChanges();
            return RedirectToAction("ManageDVDTitles");
        }
        //Question No 13
        public IActionResult GetDVDSNotOnLoanFor31Days() { 
            List<DVDTitle> dVDTitles = dataBaseContext.DVDTitle.Include(x => x.producer).Include(x => x.studio).Include(x => x.dVDCategory).ToList();
            List<DVDTitleDTO> dtos = new List<DVDTitleDTO>();
            List<Loan> copyLoans = new List<Loan>();
            List<Loan> copyLoansForLast31Days = new List<Loan>();
            bool count = false;
            foreach (var dvdTitle in dVDTitles) {
                var dvdCopies = dataBaseContext.DVDCopy.Include(x=>x.dVDTitle).Where(x => x.dVDTitle == dvdTitle).ToList();
                foreach (DVDCopy copy in dvdCopies) {
                    copyLoans = dataBaseContext.Loan.Include(x => x.DVDCopy).Where(x => x.DVDCopy == copy && x.Status=="Loaned").ToList();
                    copyLoansForLast31Days=copyLoans.Where(x => (DateTime.Now.Date-x.DateOut.Date).TotalDays <= 31).ToList();
                    if (copyLoansForLast31Days.Count > 0) {
                        count = true;
                        break;
                    }
                }
                if (count == false) {
                    DVDTitleDTO dto
                           = new DVDTitleDTO();
                    dto.Title = dvdTitle.title;
                    dto.AgeRestriction = dvdTitle.dVDCategory.ageRestriction.ToString();
                    dto.Description = dvdTitle.dVDCategory.categoryDescription;
                    dto.ProducerName = dvdTitle.producer.producerName;
                    dto.releasedDate = dvdTitle.dateReleased.ToLongDateString();
                    dto.SudioName = dvdTitle.studio.studioName;
                    dtos.Add(dto);
                }
                count = false;
            }
            return View(dtos);
        }
        private async Task<Producer> toProducerAsync(String producerName) {
            Producer producer = new Producer();
            producer.producerName =producerName;
            try
            {
                dataBaseContext.Producer.Add(producer);
                await dataBaseContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return null;
            }
            return producer;

        }
        private async Task<Studio> toStudioAsync(String studioName)
        {
            Studio newstudio = new Studio();
            newstudio.studioName = studioName;
            try
            {
                dataBaseContext.Studio.Add(newstudio);
                await dataBaseContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return null;
            }
            return newstudio;
        }

        private async Task<Actor> toActorAsync(DVDRequest request)
        {
            CastMemberRequest castMembers = request.castMember;
            Actor actor = new Actor();
            actor.actorFirstName = request.castMember.ActorFirstName;
            actor.actorSurname = request.castMember.ActorSurname;
            try
            {
                Actor newActor = dataBaseContext.Actor.Add(actor).Entity;
                await dataBaseContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return null;
            }
            return actor;
        }
        private async Task<DVDTitle> toDVDTitleAsync(DVDRequest request,Studio studio,Producer producer,DVDCategory category)
        {
            DVDTitle dVD = new DVDTitle();
            dVD.dateAdded = DateTime.Now;
            dVD.dateReleased = request.ReleaseDate;
            dVD.studio = studio;
            dVD.producer = producer;
            dVD.dVDCategory = category;
            dVD.standardCharge = request.StandardCharge; ;
            dVD.penaltyCharge = request.PenaltyCharge;
            dVD.title = request.Title;
            try
            {
                dataBaseContext.DVDTitle.Add(dVD);
                await dataBaseContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return null;
            }
            return dVD;
        }
        private async Task<DVDCategory> toDVDCategory(DVDRequest request)
        {
            DVDCategory category = new DVDCategory();
            category.ageRestriction = request.AgeRestriction;
            category.categoryDescription = request.DvdDescription;
            try
            {
                dataBaseContext.DVDCategory.Add(category);
                await dataBaseContext.SaveChangesAsync();
            }
            catch (Exception)
            {

            }
            return category;

        }
        private async Task<CastMember> toCastMember(Actor actor,DVDTitle title) {
            CastMember member = new CastMember();
            member.actor = actor;
            member.dvdTitle = title;
            try
            {
                dataBaseContext.CastMember.Add(member);
                await dataBaseContext.SaveChangesAsync();
            }
            catch (Exception)
            {

            }
            return member;
        }

        private DVDCategory findDVDCategory(DVDRequest request) {
            DVDCategory dVDCategory = dataBaseContext.DVDCategory.Where(x => x.categoryDescription == request.DvdDescription && x.ageRestriction == request.AgeRestriction).First();
            return (DVDCategory)dVDCategory;
        }
        private async Task<DVDCopy> toDVDCopy(DVDRequest request,DVDTitle dVDTitle)
        {
            DVDCopy copy = new DVDCopy();
                DVDTitle dVD = dVDTitle;
                DateTime purchasedDate = request.DatePurchased;
                copy.dVDTitle = dVD;
                copy.datePurchased = purchasedDate;
            try
            {
                dataBaseContext.DVDCopy.Add(copy);
                await dataBaseContext.SaveChangesAsync();
            }
            catch (Exception)
            {

            }
            return copy;
        }
        private List<DVDDetailsDTO> DVDsNotOnLoan()
        {
            var dvdTitles = dataBaseContext.DVDTitle.Include(x => x.producer).Include(x => x.studio).Include(x => x.dVDCategory).ToList();
            List<CastMember> castMembers = new List<CastMember>();
            List<Actor> actors = new List<Actor>();
            List<Actor> actorsOrderByLastName = new List<Actor>();
            List<DVDDetailsDTO> dVDDetailsDTOs = new List<DVDDetailsDTO>();
            List<DVDCopy> copies = new List<DVDCopy>();
            List<Loan> copyLoans = new List<Loan>();
            List<DVDDetailsDTO> dVDsNOtOnLoan = new List<DVDDetailsDTO>();
            List<Loan> copyLoansForLast31Days = new List<Loan>();
            bool count = false;
            foreach (var dvdTitle in dvdTitles)
            {
                var dvdCopies = dataBaseContext.DVDCopy.Include(x => x.dVDTitle).Where(x => x.dVDTitle == dvdTitle).ToList();
                foreach (DVDCopy copy in dvdCopies)
                {
                    copyLoans = dataBaseContext.Loan.Include(x => x.DVDCopy).Where(x => x.DVDCopy == copy && x.Status == "Loaned").ToList();
                    copyLoansForLast31Days = copyLoans.Where(x => (DateTime.Now.Date - x.DateOut.Date).TotalDays <= 31).ToList();
                    if (copyLoansForLast31Days.Count > 0)
                    {
                        count = true;
                        break;
                    }
                }
                if (count == false)
                {
                    copies = dataBaseContext.DVDCopy.Where(x => x.dVDTitle == dvdTitle).ToList();
                    castMembers = dataBaseContext.CastMember.Include(x => x.dvdTitle).Include(x => x.actor).Where(x => x.dvdTitle == dvdTitle).ToList();
                    if (castMembers != null && copies != null)
                    {
                        foreach (var castMember in castMembers)
                        {
                            Actor actor = dataBaseContext.Actor.Where(x => x.actorNumber == castMember.actor.actorNumber).First();
                            actors.Add(actor);
                        }
                        actorsOrderByLastName = actors.OrderBy(x => x.actorSurname).ToList();
                        DVDDetailsDTO dVDDetails = toDVDDetailDTO(actorsOrderByLastName, dvdTitle, copies, copyLoans);
                        actors.Clear();
                        dVDsNOtOnLoan.Add(dVDDetails);
                    }
                }
                count = false;
            }
            return dVDsNOtOnLoan;
        }
        private DVDDetailsDTO toDVDDetailDTO(List<Actor> actorsOrderByLastName, DVDTitle dvdTitle, List<DVDCopy> copies, List<Loan> loans)
        {
            DVDDetailsDTO dVDDetails = new DVDDetailsDTO();
            dVDDetails.Title = dvdTitle.title;
            dVDDetails.ProducerName = dvdTitle.producer.producerName;
            dVDDetails.StudioName = dvdTitle.studio.studioName;
            dVDDetails.Actors = actorsOrderByLastName;
            dVDDetails.ReleasedDate = dvdTitle.dateReleased.Date;
            dVDDetails.DVDNumber = dvdTitle.dVDNumber;
            dVDDetails.TotalNumberOfCopies = copies.Count();
            dVDDetails.Category = dvdTitle.dVDCategory.categoryDescription;
            dVDDetails.AgeRestrictions = dvdTitle.dVDCategory.ageRestriction;
            dVDDetails.loans = loans;
            return dVDDetails;
        }
    }
}
