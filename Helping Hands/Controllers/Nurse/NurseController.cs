using DinkToPdf;
using DinkToPdf.Contracts;
using Helping_Hands.Models;
using Helping_Hands.Models.Admin;
using Helping_Hands.Models.PasswordHash;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using System.Diagnostics.Contracts;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using static Helping_Hands.Models.CareVisitPerNurseModel;

namespace Helping_Hands.Controllers.NurseController
{
    public class NurseController : Controller
    {
        private readonly Grp0410HelpingHandsContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NurseController(Grp0410HelpingHandsContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public  IActionResult TodaysVisits()
        {
            int? loggedInNurseId = HttpContext.Session.GetInt32("NurseId");
            if (loggedInNurseId.HasValue)
            {
                DateTime currentDate = DateTime.Today; // Get today's date

                var careVisits = _dbContext.CareVisits
                    .Where(c => c.NurseId == loggedInNurseId)
                    .Join(_dbContext.CareContracts, // Join with CareContracts
                        visit => visit.ContractId,
                        contract => contract.ContractId,
                        (visit, contract) => new { visit, contract })
                    .Join(_dbContext.Patients, // Join with Patients
                        joinResult => joinResult.contract.PatientId,
                        patient => patient.PatientId,
                        (joinResult, patient) => new CareVisitViewModel
                        {
                            NurseId = loggedInNurseId.Value,
                            VisitId = joinResult.visit.VisitId,
                            ContractId = joinResult.visit.ContractId,
                            VisitDate = joinResult.visit.VisitDate,
                            ApproximateArriveTime = (TimeSpan)joinResult.visit.ApproximateArriveTime,
                            PatientFullName = patient.FirstName + " " + patient.Surname
                            
                        })
                    .Where(careVisit => careVisit.VisitDate != null && careVisit.VisitDate.Value.Date == currentDate) 
                    .ToList();

                return View(careVisits);
            }
            else
            {
                return RedirectToAction("Login");
            }
               
        }
        [HttpGet]
        public IActionResult ViewPatientDetails(int contractId)
        {
            ViewData["contractId"] = contractId;
            var patient = _dbContext.CareContracts
                .Where(c => c.ContractId == contractId)
                .Select(c => new
                {
                    ContractId = c.ContractId,
                    PatientFirstName = c.Patient.FirstName,
                    PatientSurname = c.Patient.Surname,
                    AddressLine1 = c.AddressLine1,
                    WoundDescription = c.WoundDescription,
                    ContractDate = c.ContractDate,
                    StartCareDate = c.StartCareDate,
                    SuburbName = c.Suburb.SuburbName, 
                    PostalCode = c.Suburb.PostalCode

                })
                .FirstOrDefault();
            var patientViewModel = new CareContractViewModel
            {
                ContractId = patient.ContractId,
                PatientName = patient.PatientFirstName + " " + patient.PatientSurname,
                AddressLine1 = patient.AddressLine1,
                WoundDescription = patient.WoundDescription,
                ContractDate = (DateTime)patient.ContractDate,
                StartCareDate = (DateTime)patient.StartCareDate,
                SuburbName = patient.SuburbName,
                PostalCode = patient.PostalCode
            };
            ViewBag.CareContractViewModel = patientViewModel;

            return View(patientViewModel);
        }
        [HttpGet]
        public IActionResult VisitPatient(int visitId, int contractId, DateTime visitDate,int nurseId, TimeSpan approximateArriveTime)
        {
            ViewData["visitId"] = visitId;
            ViewData["contractId"] = contractId;
            ViewData["nurseId"] = nurseId;
            ViewData["visitDate"] = visitDate;
            ViewData["approximateArriveTime"] = approximateArriveTime;
          

            return View();
        }
        [HttpPost]
        public IActionResult VisitPatient(DateTime visitDate, TimeSpan approximateArriveTime, int nurseId, int visitId, int contractId, string woundCondition, string notes, TimeSpan visitArriveTime, TimeSpan visitDepartTime)
        {
            var careVisit = _dbContext.CareVisits
                .Where(cv => cv.VisitId == visitId && cv.ContractId == contractId && cv.VisitArriveTime == null && cv.VisitDepartTime == null && cv.WoundCondition == null && cv.Notes == null && cv.Status == null)
                .FirstOrDefault();

            if (careVisit == null)
            {
                careVisit = new CareVisit
                {
                    ContractId = contractId,
                    VisitId = visitId,
                    NurseId = nurseId,
                    VisitDate = visitDate,
                    ApproximateArriveTime = approximateArriveTime
                };
            }
           
            careVisit.VisitArriveTime = visitArriveTime;
            careVisit.VisitDepartTime = visitDepartTime;
            careVisit.WoundCondition = woundCondition;
            careVisit.Status = "A";
            careVisit.Notes = notes;

            _dbContext.CareVisits.Add(careVisit);
            _dbContext.Entry(careVisit).State = EntityState.Modified;
            _dbContext.SaveChanges();

            TempData["SuccessMessage"] = "Visit information saved successfully.";
            return RedirectToAction("TodaysVisits");
        }

        [HttpGet]
        public IActionResult NewCareContracts()
        {
            var today = DateTime.Today;
            int? loggedInNurseId = HttpContext.Session.GetInt32("NurseId");

            if (loggedInNurseId.HasValue)
            {
                var careContracts = _dbContext.CareContracts
                    .Where(contract => _dbContext.PreferredSuburbs
                        .Any(preferred => preferred.SuburbId == contract.SuburbId && preferred.NurseId == loggedInNurseId && contract.ContractStatus == "New" && contract.Status == "Active")
                    )
                    .Select(contract => new CareContractViewModel
                    {
                        ContractId = contract.ContractId,
                        SuburbId = contract.SuburbId,
                        ContractDate = (DateTime)contract.ContractDate,
                        WoundDescription = contract.WoundDescription,
                        DaysOverdue = (today - (contract.ContractDate ?? today)).Days,
                        PatientFullName = contract.Patient != null ? $"{contract.Patient.FirstName} {contract.Patient.Surname}" : "N/A"
                    })
                    .ToList();

                return View(careContracts);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        [HttpGet]
        public IActionResult AssignCareContract(int contractId, int suburbId)
        {
            ViewData["contractId"] = contractId;
            var suburbName = _dbContext.Suburbs
                .Where(s => s.SuburbId == suburbId)
                .Select(s => s.SuburbName)
                .FirstOrDefault();

            var postalCode = _dbContext.Suburbs
                .Where(f => f.SuburbId == suburbId)
                .Select(f => f.PostalCode)
                .FirstOrDefault();

            var patient = _dbContext.CareContracts
                .Where(c => c.ContractId == contractId)
                .Select(c => new
                {
                    PatientFirstName = c.Patient.FirstName,
                    PatientSurname = c.Patient.Surname,
                    AddressLine1 = c.AddressLine1,
                    WoundDescription = c.WoundDescription,
                    ContractDate = c.ContractDate,
                })
                .FirstOrDefault();

            var nurseViewModel = new CareContractViewModel
            {
                SuburbName = suburbName,
                PostalCode = postalCode,
                PatientName = patient.PatientFirstName + " " + patient.PatientSurname,
                AddressLine1 = patient.AddressLine1,
                WoundDescription = patient.WoundDescription,
                ContractDate = (DateTime)patient.ContractDate,
            };

            ViewBag.CareContractViewModel = nurseViewModel;

            return View(nurseViewModel);
        }
        [HttpPost]
        public IActionResult AssignCareContract(int contractId)
        {
            int? loggedInNurseId = HttpContext.Session.GetInt32("NurseId");

            if (!loggedInNurseId.HasValue)
            {
                return RedirectToAction("Login", "User");
            }
            var careContract = _dbContext.CareContracts.FirstOrDefault(c => c.ContractId == contractId);

            if (careContract != null)
            {
                careContract.StartCareDate = DateTime.Now;
                careContract.ContractStatus = "Assigned";
                careContract.AssignedNurse = loggedInNurseId;
                _dbContext.SaveChanges();
                TempData["SuccessMessage"] = "Care Contract successfully assigned.";
            }
            else
            {
                TempData["ErrorMessage"] = "Care contract not found.";
            }

            return RedirectToAction("NewCareContracts", "Nurse");
        }

        [HttpGet]
        public IActionResult PlanCareVisit()
        {
            int? loggedInNurseId = HttpContext.Session.GetInt32("NurseId");

            if (!loggedInNurseId.HasValue)
            {
                return RedirectToAction("Login", "User");
            }
       
          
            var careContracts = _dbContext.CareContracts
                 .Where(contract => contract.AssignedNurse == loggedInNurseId.Value && contract.ContractStatus == "Assigned" && contract.Status == "Active")
                .Select(contract => new CurrentlyAssignedCareContractViewModel
                {
                    SuburbId = contract.SuburbId,
                    CareContractId = contract.ContractId,
                    PatientId = contract.PatientId,
                    AddressLine1 = contract.AddressLine1,
                    ContractStatus = contract.ContractStatus,
                    WoundDescription = contract.WoundDescription,
                    ContractDate = (DateTime)contract.ContractDate,
                    PatientFullName = contract.Patient.FirstName + " " + contract.Patient.Surname,
                    
                })
                .ToList();
            return View(careContracts);



        }
        [HttpGet]
        public IActionResult ScheduleVisit(int contractId, int suburbId)
        {
            ViewData["contractId"] = contractId;
            var suburbName = _dbContext.Suburbs
                .Where(s => s.SuburbId == suburbId)
                .Select(s => s.SuburbName)
                .FirstOrDefault();

            var postalCode = _dbContext.Suburbs
                .Where(f => f.SuburbId == suburbId)
                .Select(f => f.PostalCode)
                .FirstOrDefault();

            var patient = _dbContext.CareContracts
                .Where(c => c.ContractId == contractId)
                .Select(c => new
                {
                    PatientFirstName = c.Patient.FirstName,
                    PatientSurname = c.Patient.Surname,
                    AddressLine1 = c.AddressLine1,
                })
                .FirstOrDefault();

            var nurseViewModel = new CareVisitViewModel
            {
                SuburbName = suburbName,
                PostalCode = postalCode,
                PatientName = patient.PatientFirstName + " " + patient.PatientSurname,
                AddressLine1 = patient.AddressLine1,
            };

            ViewBag.NurseViewModel = nurseViewModel;

            return View(nurseViewModel);
        }
        [HttpPost]
        public IActionResult ScheduleVisit(int contractId, DateTime visitDate, TimeSpan approximateArriveTime)
        {
            int? loggedInNurseId = HttpContext.Session.GetInt32("NurseId");
            var newCareVisit = new CareVisit
            {
                ContractId = contractId,
                NurseId = loggedInNurseId,
                VisitDate = visitDate.Date,  // Use Date property to remove time
                ApproximateArriveTime = approximateArriveTime 
            };

            _dbContext.CareVisits.Add(newCareVisit);
            _dbContext.SaveChanges();

            TempData["SuccessMessage"] = "Care Visit successfully scheduled.";

            return RedirectToAction("PlanCareVisit", "Nurse");

        }
        [HttpGet]
        public IActionResult CheckNurseDetails()
        {
            int? loggedInNurseId = HttpContext.Session.GetInt32("NurseId");

            if (!loggedInNurseId.HasValue)
            {
                return Json(new { detailsExist = false });
            }

            var nurse = _dbContext.Nurses.FirstOrDefault(p => p.NurseId == loggedInNurseId.Value);
            bool detailsExist = !(string.IsNullOrEmpty(nurse?.FirstName) || string.IsNullOrEmpty(nurse?.Surname));

            return Json(new { detailsExist });
        }

        [HttpGet]
        public IActionResult AssignedPatientAndConditions()
        {
            int? loggedInNurseId = HttpContext.Session.GetInt32("NurseId");

            if (!loggedInNurseId.HasValue)
            {
                return RedirectToAction("Login", "User");
            }

            List<AssignedPatientAndConditions> careContracts = _dbContext.CareContracts
                .Where(contract => contract.AssignedNurse == loggedInNurseId.Value && contract.ContractStatus == "Assigned" && contract.Status == "Active")
                .OrderBy(contract => contract.Patient.FirstName)
                .Select(contract => new AssignedPatientAndConditions
                {
                    CareContractId = contract.ContractId,
                    PatientId = contract.PatientId,
                    ContractDate = contract.ContractDate,
                    WoundDescription = contract.WoundDescription,
                    StartCareDate = contract.StartCareDate,
                   
                    ContractStatus = contract.ContractStatus,
                    AddressLine1 = contract.AddressLine1,
                   
                    PatientFullName = contract.Patient.FirstName + " " + contract.Patient.Surname,
                    Conditions = _dbContext.PatientConditions
                .Where(pc => pc.PatientId == contract.PatientId)
                .Select(pc => pc.Condition.ConditionName)
                .ToList()
                })
                .ToList();

            return View("AssignedPatientAndConditions", careContracts);
        }
        [HttpGet]
        public IActionResult CurrentlyAssignedNurseCareContract()
        {
            int? loggedInNurseId = HttpContext.Session.GetInt32("NurseId");

            if (!loggedInNurseId.HasValue)
            {
                return RedirectToAction("Login", "User");
            }
             List<CurrentlyAssignedCareContractViewModel> careContracts = _dbContext.CareContracts
            .Where(contract => contract.AssignedNurse == loggedInNurseId.Value && contract.ContractStatus == "Assigned" && contract.Status == "Active")
            .OrderBy(contract => contract.Patient.FirstName)
            .Select(contract => new CurrentlyAssignedCareContractViewModel
            {
                CareContractId = contract.ContractId,
                SuburbId = contract.SuburbId,
                PatientId = contract.PatientId,
                ContractDate = (DateTime)contract.ContractDate,
                WoundDescription = contract.WoundDescription,
                StartCareDate = (DateTime)contract.StartCareDate,
                ContractStatus = contract.ContractStatus,
                AddressLine1 = contract.AddressLine1,
                AddressLine2 = contract.AddressLine2,
                PatientFullName = contract.Patient.FirstName + " " + contract.Patient.Surname
            })
            .ToList();

            return View("CurrentlyAssignedNurseCareContract", careContracts);
        }
        public IActionResult CareVisitsPerNurse()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CareVisitsPerNurse(DateTime startDate, DateTime endDate)
        {
            int? loggedInNurseId = HttpContext.Session.GetInt32("NurseId");

            if (!loggedInNurseId.HasValue)
            {
                return RedirectToAction("Login", "User");
            }

            if (endDate < startDate)
            {
                ModelState.AddModelError("", "End Date cannot be earlier than Start Date.");
                return View("CareVisitsPerNurse");
            }

            var careVisits = (from cv in _dbContext.CareVisits
                              join n in _dbContext.Nurses on cv.NurseId equals n.NurseId
                              join cc in _dbContext.CareContracts on cv.ContractId equals cc.ContractId
                              join p in _dbContext.Patients on cc.PatientId equals p.PatientId
                              where cv.VisitDate >= startDate && cv.VisitDate <= endDate
                              && n.NurseId == loggedInNurseId
                              orderby n.FirstName, n.Surname, cv.VisitDate
                              select new CareVisitPerNurseModel
                              {
                                  VisitId = cv.VisitId,
                                  VisitDate = (DateTime)cv.VisitDate,
                                  ApproximateArriveTime = (TimeSpan)cv.ApproximateArriveTime,
                                  NurseSurname = n.Surname,
                                  PatientFirstName = cc.Patient.FirstName + " " + cc.Patient.Surname,
                                 
    
                              }).ToList();
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            return View("CareVisitsPerNurse", careVisits);

        }
        [HttpGet]
        public IActionResult NurseUpdatePassword()
        {
            return View();
        }
        public IActionResult NurseUpdatePassword(UpdatePasswordViewModel model)
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
                    return RedirectToAction("NurseUpdatePassword");
                }
                else
                {
                    TempData["ErrorMessage"] = "Current password is incorrect.";
                }
            }


            return View(model);
        }
        [HttpGet]
        public IActionResult ViewContracts()
        {
            int? loggedInNurseId = HttpContext.Session.GetInt32("NurseId");

            if (loggedInNurseId.HasValue)
            {
                var careContracts = _dbContext.CareContracts
                    .Where(contract => _dbContext.PreferredSuburbs
                        .Any(preferred => preferred.SuburbId == contract.SuburbId && preferred.NurseId == loggedInNurseId && contract.ContractStatus == "New" && contract.Status == "Active")
                    )
                    .Select(contract => new CareContractAssignmentViewModel
                    {
                        ContractDate = contract.ContractDate,
                        WoundDescription = contract.WoundDescription,
                        ContractStatus = contract.ContractStatus,
                        SuburbName = _dbContext.Suburbs
                    .Where(s => s.SuburbId == contract.SuburbId)
                    .Select(s => s.SuburbName)
                    .FirstOrDefault() ?? "N/A",
                    PatientFullName = contract.Patient != null ? $"{contract.Patient.FirstName} {contract.Patient.Surname}" : "N/A"
                    })
                    .ToList();

                return View(careContracts);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }
        public IActionResult NurseDashboard()
        {
            int? loggedInNurseID = HttpContext.Session.GetInt32("NurseId");

            if (!loggedInNurseID.HasValue)
            {
                return RedirectToAction("User", "Login");
            }
            var preferredSuburbs = _dbContext.PreferredSuburbs
                .Include(ps => ps.Suburb)
                .Where(ps => ps.NurseId == loggedInNurseID.Value)
                .ToList();
            var suburbs = _dbContext.Suburbs.ToList();
            var viewModel = new PreferredSuburbViewModel
            {
                NurseID = loggedInNurseID.Value,
                PreferredSuburbs = preferredSuburbs,
                Suburbs = suburbs
            };
            return View(viewModel);
        }
        [HttpGet]
        [UserTypeAuthorization("N")]
        public IActionResult Profile()
        {
            int? loggedInNurseID = HttpContext.Session.GetInt32("NurseId");

            if (!loggedInNurseID.HasValue)
            {
                return RedirectToAction("Login", "User");
            }

            var nurse = _dbContext.Nurses.FirstOrDefault(n => n.NurseId == loggedInNurseID);

           
                if (nurse == null)
                {
                    
                    var emptyNurseProfileViewModel = new NurseProfileViewModel();
                    emptyNurseProfileViewModel.NurseId = loggedInNurseID.Value; 
                    return View(emptyNurseProfileViewModel);
                }
            

            var nurseProfileViewModel = new NurseProfileViewModel
            {
                NurseId = nurse.NurseId,
                Surname = nurse.Surname,
                FirstName = nurse.FirstName,
                Gender = nurse.Gender,
                Idnumber = nurse.Idnumber
            };

            return View(nurseProfileViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserTypeAuthorization("N")]
        public IActionResult Profile(NurseProfileViewModel model)
        {
            int? loggedInNurseID = HttpContext.Session.GetInt32("NurseId");

            if (!loggedInNurseID.HasValue)
            {
                return RedirectToAction("Login", "User");
            }

            var nurse = _dbContext.Nurses.FirstOrDefault(n => n.NurseId == loggedInNurseID);

            if (nurse == null)
            {
                // Nurse not found, create a new nurse record
                var newNurse = new Nurse
                {
                    NurseId = loggedInNurseID.Value,
                    Surname = model.Surname,
                    FirstName = model.FirstName,
                    Gender = model.Gender,
                    Idnumber = model.Idnumber
                };

                _dbContext.Nurses.Add(newNurse);
                _dbContext.SaveChanges();

                TempData["SuccessMessage"] = "Your profile has been updated.";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    nurse.Surname = model.Surname;
                    nurse.FirstName = model.FirstName;
                    nurse.Gender = model.Gender;
                    nurse.Idnumber = model.Idnumber;

                    _dbContext.SaveChanges();
                    TempData["SuccessMessage"] = "Your profile has been updated.";
                }
            }

            return RedirectToAction("Profile");
        }
        [HttpGet]
        public IActionResult GetCities()
        {
            var cities = _dbContext.Cities.ToList();
            var cityList = cities.Select(c => new SelectListItem { Value = c.CityId.ToString(), Text = c.CityName }).ToList();
            return Json(cityList);
        }
        public IActionResult GetSuburbsByCity(int cityId)
        {
            var suburbs = _dbContext.Suburbs.Where(s => s.CityId == cityId).ToList();
            var suburbList = suburbs.Select(s => new SelectListItem { Value = s.SuburbId.ToString(), Text = s.SuburbName }).ToList();
            return Json(suburbList);
        }

        [HttpGet]
        public IActionResult PreferredSuburb()
        {

            int? loggedInNurseID = HttpContext.Session.GetInt32("NurseId");

            if (!loggedInNurseID.HasValue)
            {
                return RedirectToAction("User", "Login"); 
            }

            var nurse = _dbContext.Nurses.Include(n => n.NurseNavigation).FirstOrDefault(n => n.NurseId == loggedInNurseID);

            if (nurse == null)
            {
                return NotFound(); 
            }
           var preferredSuburbs = _dbContext.PreferredSuburbs
             .Include(ps => ps.Suburb)
             .Where(ps => ps.NurseId == loggedInNurseID.Value)
             .ToList();

              var suburbs = _dbContext.Suburbs
              .OrderBy(suburb => suburb.SuburbName) // Sort by the Name property in ascending order
              .ToList();

           
            var viewModel = new PreferredSuburbViewModel
            {
                Nurse = nurse,
                PreferredSuburbs = preferredSuburbs,
                Suburbs = suburbs
            };

            return View(viewModel);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PreferredSuburb(PreferredSuburbViewModel viewModel)
        {
            int? loggedInNurseID = HttpContext.Session.GetInt32("NurseId");

           if (!loggedInNurseID.HasValue)
            {
               return RedirectToAction("User", "Login");
             }
            if (ModelState.IsValid)
            {
                if (_dbContext.PreferredSuburbs.Any(ps => ps.NurseId == loggedInNurseID.Value && ps.SuburbId == viewModel.SelectedSuburbId))
                {
                    TempData["errorMessage"] = "You have already choosen this suburb.";
                    return RedirectToAction("PreferredSuburb");
                }

                var preferredSuburb = new PreferredSuburb
                {
                    NurseId = loggedInNurseID.Value,
                    SuburbId = viewModel.SelectedSuburbId
                };

                _dbContext.PreferredSuburbs.Add(preferredSuburb);
                _dbContext.SaveChanges();
                TempData["successMessage"] = "Suburb successfully added.";
                return RedirectToAction("PreferredSuburb");


            }

            
            viewModel.Suburbs = _dbContext.Suburbs.ToList();
            return View(viewModel);
        }
        [HttpGet]
        public IActionResult CloseContract()
        {
            int? loggedInNurseId = HttpContext.Session.GetInt32("NurseId");

            if (!loggedInNurseId.HasValue)
            {
                return RedirectToAction("Login", "User");
            }
            List<CurrentlyAssignedCareContractViewModel> careContracts = _dbContext.CareContracts
           .Where(contract => contract.AssignedNurse == loggedInNurseId.Value && contract.ContractStatus == "Assigned" && contract.Status == "Active")
           .OrderBy(contract => contract.Patient.FirstName)
           .Select(contract => new CurrentlyAssignedCareContractViewModel
           {
               CareContractId = contract.ContractId,
               SuburbId = contract.SuburbId,
               PatientId = contract.PatientId,
               ContractDate = (DateTime)contract.ContractDate,
               WoundDescription = contract.WoundDescription,
               StartCareDate = (DateTime)contract.StartCareDate,
               AddressLine1 = contract.AddressLine1,
               PatientFullName = contract.Patient.FirstName + " " + contract.Patient.Surname
           })
           .ToList();

            return View("CloseContract", careContracts);
        }
        public IActionResult CloseContract(int contractId)
        {
            int? loggedInNurseId = HttpContext.Session.GetInt32("NurseId");

            if (!loggedInNurseId.HasValue)
            {
                return RedirectToAction("Login", "User");
            }
            var careContract = _dbContext.CareContracts.FirstOrDefault(c => c.ContractId == contractId && c.AssignedNurse == loggedInNurseId);

            if (careContract != null)
            {
                careContract.EndCareDate = DateTime.Now;
                careContract.ContractStatus = "Closed";
                
                _dbContext.SaveChanges();
                TempData["SuccessMessage"] = "Care Contract successfully closed.";
            }
            else
            {
                TempData["ErrorMessage"] = "Care contract not found.";
            }

            return RedirectToAction("CloseContract", "Nurse");
        }

        public IActionResult ViewClosedContracts()
        {
            int? loggedInNurseId = HttpContext.Session.GetInt32("NurseId");

            if (!loggedInNurseId.HasValue)
            {
                return RedirectToAction("Login", "User");
            }
            List<CurrentlyAssignedCareContractViewModel> careContracts = _dbContext.CareContracts
           .Where(contract => contract.AssignedNurse == loggedInNurseId.Value && contract.ContractStatus == "Closed" && contract.Status == "Active")
           .OrderBy(contract => contract.Patient.FirstName)
           .Select(contract => new CurrentlyAssignedCareContractViewModel
           {
               CareContractId = contract.ContractId,
               SuburbId = contract.SuburbId,
               PatientId = contract.PatientId,
               ContractDate = (DateTime)contract.ContractDate,
               WoundDescription = contract.WoundDescription,
               StartCareDate = (DateTime)contract.StartCareDate,
               ContractStatus = contract.ContractStatus,
               AddressLine1 = contract.AddressLine1,
               AddressLine2 = contract.AddressLine2,
               PatientFullName = contract.Patient.FirstName + " " + contract.Patient.Surname
           })
           .ToList();

            return View("ViewClosedContracts", careContracts);
        }
        [HttpGet]
        public IActionResult ViewPatientWoundProgress()
        {
            
            int? loggedInNurseId = HttpContext.Session.GetInt32("NurseId");

            if (!loggedInNurseId.HasValue)
            {
                return RedirectToAction("Login", "User");
            }

            var patients = (from cv in _dbContext.Patients
                              join n in _dbContext.CareContracts on cv.PatientId equals n.PatientId  
                              where n.AssignedNurse == loggedInNurseId
                            select new
                            {
                                PatientId = cv.PatientId,
                                PatientFullName = cv.FirstName + " " + cv.Surname
                            })
                            .Distinct()
                            .Select(p => new CareVisitViewModel
                               {
                                   PatientId = p.PatientId,
                                   PatientFullName = p.PatientFullName
                              })
                            .ToList();
            var activePatients = new SelectList(patients, "PatientId", "PatientFullName");
            ViewBag.PatientList = activePatients;
            return View(new CareVisitViewModel());
        }
        [HttpPost]
        public IActionResult ViewPatientWoundProgress(CareVisitViewModel model)
        {
            int? loggedInNurseId = HttpContext.Session.GetInt32("NurseId");

            if (!loggedInNurseId.HasValue)
            {
                return RedirectToAction("Login", "User");
            }

            int selectedPatientId = model.SelectedPatientId; 

            if (selectedPatientId == -1)
            {
                TempData["ErrorMessage"] = "No condition";
            }
            else
            {
                var visits = _dbContext.CareVisits
                    .Where(progress => _dbContext.CareContracts
                        .Any(wound => wound.ContractId == progress.ContractId && wound.ContractStatus != "New" && wound.PatientId == selectedPatientId && progress.NurseId == loggedInNurseId)
                    )
                    .Select(contract => new CareVisitViewModel
                    {
                        VisitId = contract.VisitId,
                        WoundCondition = contract.WoundCondition ?? string.Empty,
                    })
                    .ToList();

                ViewBag.WoundConditions = visits.Select(v => v.WoundCondition).ToList();
            }

            var patients = (from cv in _dbContext.Patients
                            join n in _dbContext.CareContracts on cv.PatientId equals n.PatientId
                            where n.AssignedNurse == loggedInNurseId
                            select new
                            {
                                PatientId = cv.PatientId,
                                PatientFullName = cv.FirstName + " " + cv.Surname
                            })
                            .Distinct()
                            .Select(p => new CareVisitViewModel
                            {
                                PatientId = p.PatientId,
                                PatientFullName = p.PatientFullName
                            })
                            .ToList();
            var activePatients = new SelectList(patients, "PatientId", "PatientFullName");
            ViewBag.PatientList = activePatients;

            return View();
        }



    }
}

