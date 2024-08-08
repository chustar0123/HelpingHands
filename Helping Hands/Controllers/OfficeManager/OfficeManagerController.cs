
using Azure.Identity;
using DinkToPdf;
using DinkToPdf.Contracts;
using Helping_Hands.Models;
using Helping_Hands.Models.PasswordHash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Helping_Hands.Controllers.Office_Manager
{
    public class OfficeManagerController : Controller
    {
        private readonly Grp0410HelpingHandsContext _dbContext;
       

        public OfficeManagerController (Grp0410HelpingHandsContext dbContext)
        {
            _dbContext = dbContext;
           
        }
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

        [HttpGet]
        public IActionResult DisplayCareVisits(int contractId)
        {
            var visits = _dbContext.CareVisits
            .Where(v => v.ContractId == contractId)
            .Select(visit => new CareVisitViewModel
            {
                VisitDate = visit.VisitDate,
                ApproximateArriveTime = visit.ApproximateArriveTime,
                VisitArriveTime = (TimeSpan?)visit.VisitArriveTime,
                VisitDepartTime = (TimeSpan?)visit.VisitDepartTime,
                WoundCondition = visit.WoundCondition ?? string.Empty,
                Notes = visit.Notes ?? string.Empty
            })
            .ToList();
            return View(visits);
        }
        [HttpGet]
        public IActionResult PatientContractsAndVisits()
        {
            var patients = _dbContext.Patients
                    .Where(users => _dbContext.Users
                        .Any(c => c.UserId == users.PatientId && c.Status == "A")
                    )
                    .Select(contract => new CareContractViewModel
                    {
                        PatientId = contract.PatientId,
                        PatientFullName = contract.FirstName + " " + contract.Surname

                    })
                    .ToList();
                    var activePatients = new SelectList(patients, "PatientId", "PatientFullName");
                    ViewBag.PatientList = activePatients;


                     return View(new CareContractViewModel());
        }
        [HttpPost]
        public IActionResult PatientContractsAndVisits(CareContractViewModel model)
        {
            var contracts = _dbContext.CareContracts
            .Where(contract => contract.PatientId == model.SelectedPatientId && contract.ContractStatus != "New")
            .ToList();

            model.CareContracts = contracts;
            var patients = _dbContext.Patients
            .Where(patient => _dbContext.Users
                .Any(user => user.UserId == patient.PatientId && user.Status == "A")
            )
            .Select(contract => new CareContractViewModel
            {
                PatientId = contract.PatientId,
                PatientFullName = contract.FirstName + " " + contract.Surname
            })
            .ToList();

            var activePatients = new SelectList(patients, "PatientId", "PatientFullName");
            ViewBag.PatientList = activePatients;
            ViewBag.SelectedPatientName = patients.FirstOrDefault(p => p.PatientId == model.SelectedPatientId)?.PatientFullName;
            return View(model);
        }



        [HttpGet]
        public IActionResult ListContracts()
        {
            var today = DateTime.Today;
            var careContracts = _dbContext.CareContracts
                .Where(c => c.ContractStatus == "New")
                .Select(contract => new CareContractViewModel
                {
                    SuburbId = contract.SuburbId,
                    ContractId = contract.ContractId,
                    AddressLine1 = contract.AddressLine1,
                    ContractDate = (DateTime)contract.ContractDate,
                    PatientFullName = contract.Patient.FirstName + " " + contract.Patient.Surname,
                    DaysOverdue = (today - (contract.ContractDate ?? today)).Days
                })
                .ToList();

            return View(careContracts);

        }

        [HttpGet]
        public IActionResult AssignNurse(int contractId, int suburbId, int selectedNurseId)
        {
            ViewData["contractId"] = contractId;
            ViewData["suburbId"] = suburbId;
            ViewData["selectedNurseId"] = selectedNurseId;
            var suburbName = _dbContext.Suburbs
           .Where(s => s.SuburbId == suburbId)
           .Select(s => s.SuburbName)
           .FirstOrDefault();

            var postalCode = _dbContext.Suburbs
                   .Where(f => f.SuburbId == suburbId)
                   .Select(f => f.PostalCode)
                   .FirstOrDefault();

             var nurses = _dbContext.Nurses
            .Where(nurse => nurse.PreferredSuburbs.Any(ps => ps.SuburbId == suburbId))
            .Join(
             _dbContext.Users,
             nurse => nurse.NurseId,
             user => user.UserId,
             (nurse, user) => new
             {
                 NurseId = nurse.NurseId,
                 FirstName = nurse.FirstName,
                 Surname = nurse.Surname,
                 ContactNumber = user.PhoneNumber,
                 EmailAddress = user.Email
             })
            .ToList();

            var patients = _dbContext.CareContracts
           .Where(c => c.ContractId == contractId)
           .Select(c => new
          {
          PatientFirstName = c.Patient.FirstName,
          PatientSurname = c.Patient.Surname,
          AddressLine1 = c.AddressLine1,
          ContractDate = c.ContractDate,
          woundDescription = c.WoundDescription,
          })
          .FirstOrDefault();

            var nurseViewModels = nurses.Select(nurse => new NurseViewModel
            {
                NurseId = nurse.NurseId,
                FirstName = nurse.FirstName,
                Surname = nurse.Surname,
                ContactNumber = nurse.ContactNumber,
                EmailAddress = nurse.EmailAddress,
                SuburbName = suburbName,
                PostalCode = postalCode,
                PatientFirstName = patients.PatientFirstName +" "+ patients.PatientSurname,
                AddressLine1 = patients.AddressLine1,
                ContractDate = patients.ContractDate,
                WoundDescription = patients.woundDescription,
     
            }).ToList();
            return View(nurseViewModels);
        }
       
        [HttpGet]
        public IActionResult SaveAssignedNurse(int selectedNurseId, int contractId, int suburbId)
        {
            ViewData["contractId"] = contractId;
            ViewData["suburbId"] = suburbId;
            ViewData["selectedNurseId"] = selectedNurseId;
            var selectedNurse = from nurse in _dbContext.Nurses
                                where nurse.NurseId == selectedNurseId
                                select new NurseViewModel
                                {
                                    NurseId = nurse.NurseId,
                                    NurseFullName = nurse.FirstName + " " + nurse.Surname,
                                };
            var result = selectedNurse.ToList();
            ViewBag.SelectedNurse = result;

            var suburbName = _dbContext.Suburbs
          .Where(s => s.SuburbId == suburbId)
          .Select(s => s.SuburbName)
          .FirstOrDefault();

            var postalCode = _dbContext.Suburbs
                   .Where(f => f.SuburbId == suburbId)
                   .Select(f => f.PostalCode)
                   .FirstOrDefault();

            var nurses = _dbContext.Nurses
            .Where(nurse => nurse.PreferredSuburbs.Any(ps => ps.SuburbId == suburbId))
            .Join(
             _dbContext.Users,
             nurse => nurse.NurseId,
             user => user.UserId,
             (nurse, user) => new
             {
                 NurseId = nurse.NurseId,
                 FirstName = nurse.FirstName,
                 Surname = nurse.Surname,
                 ContactNumber = user.PhoneNumber,
                 EmailAddress = user.Email
             })
            .ToList();

            var patients = _dbContext.CareContracts
      .Where(c => c.ContractId == contractId)
      .Select(c => new
      {
          PatientFirstName = c.Patient.FirstName,
          PatientSurname = c.Patient.Surname,
          AddressLine1 = c.AddressLine1,
          ContractDate = c.ContractDate,
          woundDescription = c.WoundDescription,
      })
      .FirstOrDefault();

            var nurseViewModels = nurses.Select(nurse => new NurseViewModel
            {
                NurseId = nurse.NurseId,
                FirstName = nurse.FirstName,
                Surname = nurse.Surname,
                ContactNumber = nurse.ContactNumber,
                EmailAddress = nurse.EmailAddress,
                SuburbName = suburbName,
                PostalCode = postalCode,
                PatientFirstName = patients.PatientFirstName + " " + patients.PatientSurname,
                AddressLine1 = patients.AddressLine1,
                ContractDate = patients.ContractDate,
                WoundDescription = patients.woundDescription,

            }).ToList();
            
            return View(nurseViewModels);
           

        }
        [HttpPost]
        public IActionResult SaveAssignedNurse(int contractId, int selectedNurseId)
        {
           
            var careContract = _dbContext.CareContracts.FirstOrDefault(c => c.ContractId == contractId);

            if (careContract != null)
            {
                careContract.StartCareDate = DateTime.Now;
                careContract.ContractStatus = "Assigned";
                careContract.AssignedNurse = selectedNurseId; 
                _dbContext.SaveChanges();
                TempData["SuccessMessage"] = "Nurse assigned successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Care contract not found.";
            }

            return RedirectToAction("ListContracts", "OfficeManager");
        }

        [UserTypeAuthorization("O")]
        [HttpGet]
        public IActionResult OfficeManager()
        {
            int newCareContractCount = _dbContext.CareContracts.Count(c => c.ContractStatus == "New" && c.IsRead == false);
            ViewBag.NewCareContractCount = newCareContractCount;

            var firstNames = _dbContext.CareContracts
                .Where(c => c.ContractStatus == "New" && !c.IsRead)
                .Select(c => new
                {
                    PatientName = c.Patient.FirstName,
                    CreatedDate = c.ContractDate
                })
                .ToList();
          
            var messages = firstNames.Select(contract =>
            {
                var dateText = string.Format("{0:yyyy-MM-dd}", contract.CreatedDate);
                return $"{contract.PatientName} has created a Care Contract on {dateText}.";
            }).ToList();

            ViewBag.PatientMessages = messages;

            return View();

        }
        [HttpPost]
        public IActionResult MarkNotificationsAsRead()
        {
          
            _dbContext.CareContracts
                .Where(c => c.ContractStatus == "New" && !c.IsRead)
                .ToList()
                .ForEach(contract => contract.IsRead = true);

            _dbContext.SaveChanges();
            int newCount = _dbContext.CareContracts.Count(c => c.ContractStatus == "New" && !c.IsRead);

            return Json(new { newCount });
        }
        public IActionResult ChronicCondition()
        {
            var chronicConditions = _dbContext.ChronicConditions
                 .Where(e => e.Status == "A")
                .Select(condition => new ChronicCondition
                {
                    ConditionId = condition.ConditionId,
                    ConditionName = condition.ConditionName,
                    Description = condition.Description
                }).ToList();
            return View(chronicConditions);
        }
        [HttpGet]
        public IActionResult InsertCondition()
        {
            return View();
        }
        [HttpPost]
        public IActionResult InsertCondition(ChronicCondition model)
        {
            if(ModelState.IsValid)
            {
                model.Status = "A";

                var condition = new ChronicCondition
                {
                    ConditionName = model.ConditionName,
                    Description = model.Description,
                    Status = model.Status
                };
                _dbContext.ChronicConditions.Add(condition);
                _dbContext.SaveChanges();
                TempData["SuccessMessage"] = "Chronic condition added successfully!";

                return RedirectToAction("ChronicCondition");
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult UpdateCondition(ChronicCondition model)
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdateCondition(int conditionId)
        {

            return View();
        }

        
        public ActionResult Delete(int id)
        {
            ChronicCondition condition = _dbContext.ChronicConditions.Find(id);

            if (condition != null)
            {
                condition.Status = "I"; // Update the status
                _dbContext.SaveChanges();
            }

            return RedirectToAction("ChronicCondition");
        }
        public IActionResult NurseDetails()
        {
            var viewModel = new NurseViewModel
            {
                Suburbs = _dbContext.Suburbs.OrderBy(suburb => suburb.SuburbName).ToList()
            };
            return View(viewModel);
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


        [HttpPost]
        public IActionResult NurseDetails(NurseViewModel model)
        {
            var selectedSuburbId = model.SelectedSuburbId;
            var selectedSuburb = _dbContext.Suburbs.FirstOrDefault(s => s.SuburbId == selectedSuburbId);
            string selectedSuburbName = selectedSuburb != null ? selectedSuburb.SuburbName : "Unknown Suburb"; // Provide a default name if not found.
            var nurses = _dbContext.Nurses
                .Where(nurse => _dbContext.PreferredSuburbs.Any(ps => ps.NurseId == nurse.NurseId && ps.SuburbId == selectedSuburbId))
                .ToList();
          


            var viewModel = new NurseViewModel
            {
                Suburbs = _dbContext.Suburbs.ToList(),
                Nurses = nurses,
                SelectedSuburbId = model.SelectedSuburbId,
                SelectedSuburbName = selectedSuburbName
            };

            return View(viewModel);
        }
        [HttpGet]
       public IActionResult NurseVisitsDateRange()
        {
            return View();
        }
        [HttpPost]
        public IActionResult NurseVisitsDateRange(DateTime startDate, DateTime endDate)
        {
            

            if (endDate < startDate)
            {
                ModelState.AddModelError("", "End Date cannot be earlier than Start Date.");
                return View("NurseVisitsDateRange");
            }

            var careVisits = (from cv in _dbContext.CareVisits
                              join n in _dbContext.Nurses on cv.NurseId equals n.NurseId
                              join cc in _dbContext.CareContracts on cv.ContractId equals cc.ContractId
                              join p in _dbContext.Patients on cc.PatientId equals p.PatientId
                              where cv.VisitDate >= startDate && cv.VisitDate <= endDate 
                              orderby n.FirstName, n.Surname, cv.VisitDate
                              select new CareVisitPerNurseModel
                              {
                                  VisitId = cv.VisitId,
                                  VisitDate = (DateTime)cv.VisitDate,
                                  ApproximateArriveTime = (TimeSpan)cv.ApproximateArriveTime,
                                  NurseSurname = n.FirstName + "  " + n.Surname,
                                
                                  PatientFirstName = cc.Patient.FirstName + "  " + cc.Patient.Surname,


                              }).ToList();
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            return View("NurseVisitsDateRange", careVisits);

        }

        public IActionResult ViewNewContracts()
        {

            var today = DateTime.Today;
            var careContracts = _dbContext.CareContracts
                        .Where(contract => contract.ContractStatus == "New" && contract.Status == "Active")
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
        [HttpGet]
        public IActionResult CareContractsDateRange()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CareContractsDateRange(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
            {
                ModelState.AddModelError("", "End Date cannot be earlier than Start Date.");
                return View("CareContractsDateRange");
            }

            var careContracts = (from cv in _dbContext.CareContracts
                              where cv.ContractDate >= startDate && cv.ContractDate <= endDate && cv.Status == "Active" 
                              select new CareContractViewModel
                              {
                                  ContractId = cv.ContractId,
                                  ContractDate = (DateTime)cv.ContractDate,
                                  StartCareDate = (DateTime)cv.StartCareDate,
                                  EndCareDate = (DateTime)cv.EndCareDate,
                                  ContractStatus = cv.ContractStatus,
                                  WoundDescription = cv.WoundDescription,
                                  AddressLine1 = cv.AddressLine1,
                              }).ToList();
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            
            return View("CareContractsDateRange", careContracts);
        }
    }


    
}
