
using Helping_Hands.Models.PasswordHash;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Helping_Hands.Models.Admin;
using Helping_Hands.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Globalization;
using System.Linq;
using System.Drawing.Printing;
using System.Reflection.Metadata.Ecma335;
using iTextSharp.text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Helping_Hands.Controllers.Admin
{
    public class UserController : Controller
    {
        public readonly Grp0410HelpingHandsContext _dbContext;
        public UserController(Grp0410HelpingHandsContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
       
        

        public IActionResult ActivateNurse(int pageNumber = 1, int pageSize = 5)
        {
            var data = _dbContext.Users.FromSqlRaw("exec spActivateNurses").ToList();

            // Paging
            int totalRecords = data.Count;
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            bool hasPreviousPage = pageNumber > 1;
            bool hasNextPage = pageNumber < totalPages;

            var paginatedData = data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.HasPreviousPage = hasPreviousPage;
            ViewBag.HasNextPage = hasNextPage;

            return View(paginatedData);
        }
        public IActionResult DeactivateNurse(int pageNumber = 1, int pageSize = 5)
        {
            var data = _dbContext.Users.FromSqlRaw("exec spDeactivateNurses").ToList();

            // Paging
            int totalRecords = data.Count;
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            bool hasPreviousPage = pageNumber > 1;
            bool hasNextPage = pageNumber < totalPages;
            var paginatedData = data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.HasPreviousPage = hasPreviousPage;
            ViewBag.HasNextPage = hasNextPage;

            return View(paginatedData);
        }
        public IActionResult ActivateOfficeManager(int pageNumber = 1, int pageSize = 5)
        {
            var data = _dbContext.Users.FromSqlRaw("exec spActivateOfficeManager").ToList();

            // Paging
            int totalRecords = data.Count;
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            bool hasPreviousPage = pageNumber > 1;
            bool hasNextPage = pageNumber < totalPages;
            var paginatedData = data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.HasPreviousPage = hasPreviousPage;
            ViewBag.HasNextPage = hasNextPage;

            return View(paginatedData);
        }
        public IActionResult DeactivateOfficeManager(int pageNumber = 1, int pageSize = 5)
        {
            var data = _dbContext.Users.FromSqlRaw("exec spDeactivateOfficeManager").ToList();

            // Paging
            int totalRecords = data.Count;
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            bool hasPreviousPage = pageNumber > 1;
            bool hasNextPage = pageNumber < totalPages;
            var paginatedData = data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.HasPreviousPage = hasPreviousPage;
            ViewBag.HasNextPage = hasNextPage;

            return View(paginatedData);
        }

        public IActionResult CountCareContractsByMonth()
        {
            
            return View();
        }
        public IActionResult AdminDashboard()
        {
            var contractCounts = _dbContext.CareContracts
    .GroupBy(c => new { c.ContractDate.Value.Year, c.ContractDate.Value.Month })
    .Select(g => new CareContractsCountViewModel
    {
        MonthYear = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month)} {g.Key.Year}",
        ContractCount = g.Count(),
        PatientCount = 0
    })
    .ToList();

            var userCounts = _dbContext.Users
                .Where(u => u.DateAssigned.HasValue)
                .Where(u => u.UserType == "P") // Filter for patients (UserType == "P")
                .GroupBy(u => new { u.DateAssigned.Value.Year, u.DateAssigned.Value.Month })
                .Select(g => new CareContractsCountViewModel
                {
                    MonthYear = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month)} {g.Key.Year}",
                    ContractCount = 0,
                    PatientCount = g.Count()
                })
                .ToList();

            var allCounts = contractCounts.Concat(userCounts).ToList();

            // Create a dictionary to combine the counts for each month
            var combinedCounts = new Dictionary<string, CareContractsCountViewModel>();

            foreach (var count in allCounts)
            {
                if (!combinedCounts.ContainsKey(count.MonthYear))
                {
                    combinedCounts[count.MonthYear] = new CareContractsCountViewModel
                    {
                        MonthYear = count.MonthYear,
                        ContractCount = 0,
                        PatientCount = 0
                    };
                }

          
                if (count.ContractCount > 0)
                {
                    combinedCounts[count.MonthYear].ContractCount = count.ContractCount;
                }

                if (count.PatientCount > 0)
                {
                    combinedCounts[count.MonthYear].PatientCount = count.PatientCount;
                }
            }

            
            var sortedCounts = combinedCounts.Values.OrderBy(c => DateTime.ParseExact(c.MonthYear, "MMMM yyyy", CultureInfo.InvariantCulture)).ToList();

          
            sortedCounts = sortedCounts.Where(c => c.ContractCount > 0 || c.PatientCount > 0).ToList();

            return View(sortedCounts);

        }
        [HttpGet]
        public IActionResult AddNurse()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddNurse(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Password = PasswordHasher.HashPassword(model.Password);// This line of code make the password to be encypted
                //These lines of code save the data entered by the Admin
                model.Status = "A";
                model.UserType = "N";
                model.DateAssigned = DateTime.Now;
                var data = new User()
                {
                    UserName = model.UserName,
                    Password = model.Password,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    UserType = model.UserType,
                    Status = model.Status,
                    DateAssigned  = model.DateAssigned,
                };

                _dbContext.Users.Add(data);
                _dbContext.SaveChanges();
                TempData["NurseMessage"] = true;
                return RedirectToAction("AddNurse","User");



            }


            return View(model);
        }
        [HttpGet]
        public IActionResult AddOfficeManager()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOfficeManager(AddUserViewModel model)
        {

            if (ModelState.IsValid)
            {
                model.Password = PasswordHasher.HashPassword(model.Password);// This line of code make the password to be encypted
                //These lines of code save the data entered by the Admin
                model.Status = "A";
                model.UserType = "O";
                model.DateAssigned = DateTime.Now;
                var data = new User()
                {
                    UserName = model.UserName,
                    Password = model.Password,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    UserType = model.UserType,
                    Status = model.Status,
                    DateAssigned = model.DateAssigned,

                };

                _dbContext.Users.Add(data);
                _dbContext.SaveChanges();
                TempData["OfficeManagerMessage"] = true;
                return RedirectToAction("AddOfficeManager","User");



            }


            return View(model);

        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var data = _dbContext.Users.Where(e => e.UserName == model.UserName).SingleOrDefault();
                if (data != null)
                {
              
                    var user = _dbContext.Users.FirstOrDefault(u => u.UserName == model.UserName);
                    if (user != null && PasswordHasher.VerifyPassword(model.Password, user.Password))
                    {
                       
                        if (user.Status == "A")
                        {
                            HttpContext.Session.SetString("Username", user.UserName);
                            HttpContext.Session.SetString("UserType", user.UserType);
                            HttpContext.Session.SetInt32("NurseId", user.UserId);
                            HttpContext.Session.SetInt32("UserId", user.UserId);
                            HttpContext.Session.SetInt32("PatientId", user.UserId);
                            if (data.UserType == "N")
                            {
                                return RedirectToAction("NurseDashboard", "Nurse");
                            }
                            else if (data.UserType == "O")
                            {
                                return RedirectToAction("OfficeManager", "OfficeManager");
                            }
                            else if (data.UserType == "A")
                            {
                                return RedirectToAction("AdminDashboard", "User");
                            }
                            else if (data.UserType == "P")
                            {
                                return RedirectToAction("PatientDashboard", "Patient");
                            }
                           
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            TempData["errorStatus"] = "Your account is locked, contact the Admin";
                            return View(model);
                        }
                        
                    }
                    else
                    {
                        TempData["errorPassword"] = "Invalid email or password";
                        return View(model);
                    }

                }
                else
                {

                    TempData["errorUsername"] = "Invalid email or password";

                    return View(model);
                }
            }
            else
            {
                return View(model);
            }

        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FromItoA(int userId)
        {
            var user = _dbContext.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }
            user.Status = "A";
            _dbContext.SaveChanges();
            return RedirectToAction("ActivateNurse");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FromAtoI(int userId)
        {
            var user = _dbContext.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }
            user.Status = "I";
            _dbContext.SaveChanges();
            return RedirectToAction("DeactivateNurse");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OfficeFromItoA(int userId)
        {
            var user = _dbContext.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }
            user.Status = "A";
            _dbContext.SaveChanges();
            return RedirectToAction("ActivateOfficeManager");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OfficeFromAtoI(int userId)
        {
            var user = _dbContext.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }
            user.Status = "I";
            _dbContext.SaveChanges();
            return RedirectToAction("DeactivateOfficeManager");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("Username");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Cities()
        {
            var cities = _dbContext.Cities
                 .Where(e => e.Status == "A")
                .Select(city => new City
                {
                    CityId = city.CityId,
                    CityName = city.CityName,
                    CityAbbreviation = city.CityAbbreviation,
                   
                    
                }).ToList();
            return View(cities);
        }
        [HttpGet]
        public IActionResult AddCity()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCity(int cityId)
        {
            var city = _dbContext.Cities.Find(cityId);
            if (city == null)
            {
                return NotFound();
            }
            city.Status = "I";
            _dbContext.SaveChanges();
            TempData["ErrorMessage"] = "City deleted successfully!";
            return RedirectToAction("Cities");
        }
        [HttpPost]
        
        public IActionResult AddCity(City model)
        {
            if (ModelState.IsValid)
            {
                model.Status = "A";

                var city = new City
                {
                    CityId = model.CityId,
                    CityName = model.CityName,
                    CityAbbreviation = model.CityAbbreviation,
                    Status = model.Status
                };
                _dbContext.Cities.Add(city);
                _dbContext.SaveChanges();
                TempData["SuccessMessage"] = "City added successfully!";

                return RedirectToAction("Cities");
            }
            return View(model);
        }
        public IActionResult Suburb(int pageNumber = 1, int pageSize = 5)
        {

            var data = _dbContext.Suburbs.FromSqlRaw("exec spActivateSuburbs").ToList();
            int totalRecords = data.Count;
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            bool hasPreviousPage = pageNumber > 1;
            bool hasNextPage = pageNumber < totalPages;

            var paginatedData = data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.HasPreviousPage = hasPreviousPage;
            ViewBag.HasNextPage = hasNextPage;

            return View(paginatedData);
        }
        [HttpGet]
        public IActionResult AddSuburb()
        {
            var cities = _dbContext.Cities.
                Where(e => e.Status == "A")
                .Select(c => new SelectListItem
            {
                Value = c.CityId.ToString(),
                Text = c.CityName
            }).ToList();

            ViewBag.Cities = cities;

           

            return View();
        }
        [HttpPost]
        public IActionResult AddSuburb(Suburb suburb)
        {
            if (ModelState.IsValid)
            {
                suburb.Status = "A";
                _dbContext.Suburbs.Add(suburb);
                _dbContext.SaveChanges();

                TempData["SuccessMessage"] = "Suburb added successfully.";
                return RedirectToAction("Suburb");
            }

            var cities = _dbContext.Cities.Select(c => new SelectListItem
            {
                Value = c.CityId.ToString(),
                Text = c.CityName
            }).ToList();

            ViewBag.Cities = cities;

            return View(suburb);
        }
        public IActionResult DeleteSuburb(int suburbId)
        {
            var suburb = _dbContext.Suburbs.Find(suburbId);
            if (suburb == null)
            {
                return NotFound();
            }
            suburb.Status = "I";
            _dbContext.SaveChanges();
            TempData["ErrorMessage"] = "Suburb deleted successfully!";
            return RedirectToAction("Suburb");
        }
        public IActionResult BusinessInfor()
        {
            var business = _dbContext.Businesses.FromSqlRaw("exec spBusiness").ToList();
            return View(business);
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword(UpdatePasswordViewModel model)
        {
            int? loggedInUserId = HttpContext.Session.GetInt32("UserId");

            if (loggedInUserId == null)
            {
                return RedirectToAction("Login");
            }

            var user = _dbContext.Users.Find(loggedInUserId.Value);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                bool isCurrentPasswordCorrect = PasswordHasher.VerifyPassword(model.CurrentPassword, user.Password);
                if (isCurrentPasswordCorrect)
                {

                    var newPasswordHash = PasswordHasher.HashPassword(model.NewPassword);
                    user.Password = newPasswordHash;
                    _dbContext.SaveChanges();
                    TempData["SuccessMessage"] = "Password updated successfully.";
                    return RedirectToAction("ChangePassword");
                }
                else
                {
                    TempData["ErrorMessage"] = "Current password is incorrect.";
                }
            }


            return View(model);
        }
    }

}

