using AppDevGroupCoursework.Request;
using AppDevGroupCoursework.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppDevGroupCoursework.Controllers
{
    public class AdminController: Controller
    {
        private readonly DatabaseContext dataBaseContext;

        public AdminController(DatabaseContext db)
        {
            dataBaseContext = db;
        }

        [HttpGet]
        public IActionResult Home(bool Iscustadd = false, bool Isdeletecust = false, bool Isdeleteuser = false, bool Isadduser = false, bool Islogin = false)
        {
            ViewBag.islogin = Islogin;
            ViewBag.iscustadd = Iscustadd;
            ViewBag.isdeletecust = Isdeletecust;
            ViewBag.isdeleteuser = Isdeleteuser;
            ViewBag.isadduser = Isadduser;
            //logged in token data retrieve
            var user = new User
            {
                username = "m",
                usernumber = 1,
                usertype = "Manager",
                email="manager@gmail.com"
            };

            return View(user);
        }

        public IActionResult Users()
        {
            return RedirectToAction("Home", "User");

        }
        public IActionResult AddUsers()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUsers(UserRequest request)
        {
            if (ModelState.IsValid)
            {
                User users = new User();
                users.name = request.fullName;
                users.usertype = request.jobTitle;
                users.username = request.username;
                users.password = request.password;
                users.email = request.email;
                users.phoneNumber = request.phoneNumber;

                try
                {
                    dataBaseContext.User.Add(users);
                    await dataBaseContext.SaveChangesAsync();
                    return RedirectToAction("ManageUsers");
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return View();
        }
        public IActionResult AddMembers()
        {
            return View();
        }

        [HttpPost]
       [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMembers(MemberRequest request)
        {
            if (ModelState.IsValid) {
                Member member = new Member();
                var findMembershipCategory = dataBaseContext.MembershipCategory.Where(x => x.membershipCategoryDescription == request.memberCategory).First();
                member.firstName = request.firstName;
                member.lastName = request.lastName;
                member.middleName = request.middleName;
                member.address = request.address;
                member.phoneNumber = request.phoneNumber;
                member.email = request.email;
                member.dateOfBirth = request.dateOfBirth;
                member.membershipCategory = findMembershipCategory;
                try
                {
                    dataBaseContext.Member.Add(member);
                    await dataBaseContext.SaveChangesAsync();
                    return RedirectToAction("ManageMember");
                }
                catch (Exception e)
                {
                    throw new Exception("sdfasdf");
                }
            }
            return View();
        }

        //to display the user's data in data table
        public IActionResult ManageUsers()
        {
            var userList = dataBaseContext.User.ToArray();
            return View(userList);
        }
        public IActionResult DeleteUsers(int? usernumber)
        {
            if (usernumber == 0)
            {
                return NotFound();
            }
            var user = dataBaseContext.User.Where(x => x.usernumber == usernumber).First();
            return View(user);
        }
        [HttpPost]
        public IActionResult DeleteUsers(int usernumber)
        {
            if (usernumber == 0)
            {
                return NotFound();
            }
            var user = dataBaseContext.User.Where(x => x.usernumber == usernumber).First();
            dataBaseContext.User.Remove(user);
            dataBaseContext.SaveChanges();
            return RedirectToAction("ManageUsers");
        }
        public IActionResult EditUsers(int? usernumber)
        {
            if (usernumber == null || usernumber == 0) {
                return NotFound();
            }
            var users = dataBaseContext.User.ToList();
            User user = users.Where(x => x.usernumber == usernumber).First();
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUsers(User request)
        {
            try
            {
                dataBaseContext.User.Update(request);
                await dataBaseContext.SaveChangesAsync();
                return Redirect("~/Admin/ManageUsers");
            }
            catch
            {
                return null;
            }
            return View();

        }
        //to display customer's data in the data table
        public IActionResult ManageMember()
        {
            var memberList = dataBaseContext.Member.Include(x=>x.membershipCategory).ToArray();
            return View(memberList);
        }
        public IActionResult DeleteMember(int? id)
        {
            var member = dataBaseContext.Member.Where(x => x.id == id).First();
            
            return View(member);
        }
        [HttpPost]
        public IActionResult DeleteMember(int id)
        {
            var member_data = dataBaseContext.Member.Where(x => x.id == id).First();
            dataBaseContext.Member.Remove(member_data);
            dataBaseContext.SaveChanges();
            return RedirectToAction("ManageMember");
        }
        public IActionResult EditMembers(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var users = dataBaseContext.Member.ToList();
            Member member = users.Where(x => x.id == id).First();
            return View(member);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMembers(Member member)
        {
            try
            {
                dataBaseContext.Member.Update(member);
                await dataBaseContext.SaveChangesAsync();
                return RedirectToAction("ManageMember");
            }
            catch (Exception)
            {
                return null;
            }
            return View();
        }
    }
}
