using Helping_Hands.Models;
using Helping_Hands.Models.PasswordHash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Helping_Hands.Controllers.Patient
{
    public class PatientController : Controller
    {
        public readonly Grp0410HelpingHandsContext _dbContext;
        
        public PatientController(Grp0410HelpingHandsContext dbContext)
        {
            _dbContext = dbContext;
            

        }
        public async Task<string> UpdatePatientPasswordAsync(int userId, string newPassword, string confirmationPassword)
        {
            var result = new SqlParameter("@Result", SqlDbType.NVarChar, 255)
            {
                Direction = ParameterDirection.Output
            };

            await _dbContext.Database.ExecuteSqlRawAsync(
                "EXEC @Result = UpdatePatientPassword @UserId, @NewPassword, @ConfirmationPassword",
                new SqlParameter("@UserId", userId),
                new SqlParameter("@NewPassword", newPassword),
                new SqlParameter("@ConfirmationPassword", confirmationPassword),
                result
            );

            return result.Value?.ToString();
        }
        [HttpGet]
        public IActionResult UpdatePassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult UpdatePassword(UpdatePasswordViewModel model)
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
                    return RedirectToAction("UpdatePassword");
                }
                else
                {
                    TempData["ErrorMessage"] = "Current password is incorrect.";
                }
            }

           
            return View(model);
        }


        [HttpGet]
        public IActionResult ListCareContracts()
        {
            int? loggedInPatientId = HttpContext.Session.GetInt32("PatientId");

            if (loggedInPatientId.HasValue)
            {
                var careContracts = _dbContext.CareContracts
                .Where(contract => contract.PatientId == loggedInPatientId && contract.Status == "Active")
                .Select(contract => new ListCareContractViewModel
                {
                    ContractDate = (DateTime)contract.ContractDate,
                    WoundDescription = contract.WoundDescription,
                    ContractStatus = contract.ContractStatus,
                    AddressLine1 = contract.AddressLine1,
                    AddressLine2 = contract.AddressLine2
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
        [UserTypeAuthorization("P")]
        public IActionResult PatientProfile()
        {
            int? loggedInPatientId = HttpContext.Session.GetInt32("PatientId");

            if (!loggedInPatientId.HasValue)
            {
                return RedirectToAction("Login", "User");
            }

            var patient = _dbContext.Patients.FirstOrDefault(p => p.PatientId == loggedInPatientId);

            if (patient == null)
            {
                var emptyPatientProfileViewModel = new PatientProfileViewModel();
                emptyPatientProfileViewModel.PatientId = loggedInPatientId.Value;
                return View(emptyPatientProfileViewModel);
            }

            var patientProfileViewModel = new PatientProfileViewModel
            {
                PatientId = patient.PatientId,
                Surname = patient.Surname,
                FirstName = patient.FirstName,
                Gender = patient.Gender,
                Idnumber = patient.Idnumber,
                DateOfBirth = patient.DateOfBirth,
                EmergencyContactPerson = patient.EmergencyContactPerson,
                EmergencyContactNumber = patient.EmergencyContactNumber,
                AdditionalInformation = patient.AdditionalInformation
            };

            return View(patientProfileViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserTypeAuthorization("P")]
        public IActionResult PatientProfile(PatientProfileViewModel model)
        {
            int? loggedInPatientId = HttpContext.Session.GetInt32("PatientId");

            if (!loggedInPatientId.HasValue)
            {
                return RedirectToAction("Login", "User");
            }

            if (ModelState.IsValid)
            {
                var patient = _dbContext.Patients.FirstOrDefault(p => p.PatientId == loggedInPatientId);

                if (patient == null)
                {

                    var newPatient = new Models.Patient
                    {
                        PatientId = loggedInPatientId.Value,
                        Surname = model.Surname,
                        FirstName = model.FirstName,
                        Gender = model.Gender,
                        Idnumber = model.Idnumber,
                        DateOfBirth = model.DateOfBirth,
                        EmergencyContactPerson = model.EmergencyContactPerson,
                        EmergencyContactNumber = model.EmergencyContactNumber,
                        AdditionalInformation = model.AdditionalInformation
                    };

                    _dbContext.Patients.Add(newPatient);
                    _dbContext.SaveChanges();

                    TempData["SuccessMessage"] = "Your profile has been created.";
                }
                else
                {

                    patient.Surname = model.Surname;
                    patient.FirstName = model.FirstName;
                    patient.Gender = model.Gender;
                    patient.Idnumber = model.Idnumber;
                    patient.DateOfBirth = model.DateOfBirth;
                    patient.EmergencyContactPerson = model.EmergencyContactPerson;
                    patient.EmergencyContactNumber = model.EmergencyContactNumber;
                    patient.AdditionalInformation = model.AdditionalInformation;

                    _dbContext.SaveChanges();
                    TempData["SuccessMessage"] = "Your profile has been updated.";
                }
            }
            else
            {

                return View(model);
            }

            return RedirectToAction("PatientProfile");

        }
        public IActionResult PatientDashboard()
        {
            return View();
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
        public IActionResult CareContract()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CareContract(CareContract model)
        {
            if (ModelState.IsValid)
            {
                int? loggedInPatientId = HttpContext.Session.GetInt32("PatientId");

                if (loggedInPatientId.HasValue)
                {
                    model.PatientId = loggedInPatientId.Value;
                    model.ContractStatus = "New";
                    model.Status = "Active";
                    var care = new CareContract()
                    {
                        WoundDescription = model.WoundDescription,
                        ContractDate = model.ContractDate,
                        AddressLine1 = model.AddressLine1,
                        AddressLine2 = model.AddressLine2,
                        ContractStatus = "New",
                        PatientId = model.PatientId,
                        SuburbId = model.SelectedSuburbId,
                        Status = model.Status,
                };

                    _dbContext.CareContracts.Add(care);
                    _dbContext.SaveChanges();
                   
                   
                    TempData["SuccessMessage"] = "Care Contract created successfully!";

                    return RedirectToAction("ListCareContracts");
                }
            }

        
            return View(model);
        }


        [HttpGet]
        public IActionResult CheckPatientDetails()
        {
            int? loggedInPatientId = HttpContext.Session.GetInt32("PatientId");

            if (!loggedInPatientId.HasValue)
            {
                return Json(new { detailsExist = false });
            }

            var patient = _dbContext.Patients.FirstOrDefault(p => p.PatientId == loggedInPatientId.Value);
            bool detailsExist = !(string.IsNullOrEmpty(patient?.FirstName) || string.IsNullOrEmpty(patient?.Surname));

            return Json(new { detailsExist });
        }


        [HttpGet]
        public IActionResult SpecityCondition()
        {
            int? loggedInPatientId = HttpContext.Session.GetInt32("PatientId");

            if (!loggedInPatientId.HasValue)
            {
                return RedirectToAction("Login"); 
            }
            ViewBag.LoggedInPatientId = loggedInPatientId;
            var patient = _dbContext.Patients
                .Include(p => p.PatientConditions)
                .SingleOrDefault(p => p.PatientId == loggedInPatientId.Value);

            if (patient == null)
            {
                return NotFound();
            }

            var viewModel = new SpecifyConditionViewModel
            {
                Patient = patient,
                selectedConditions = new List<int>()
            };
            var availableConditions = _dbContext.ChronicConditions.ToList();
            ViewBag.Conditions = new MultiSelectList(availableConditions, "ConditionId", "ConditionName", viewModel.selectedConditions);

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SpecityCondition(SpecifyConditionViewModel viewModel)
        {
            int? loggedInPatientId = HttpContext.Session.GetInt32("PatientId");

            if (!loggedInPatientId.HasValue)
            {
                return RedirectToAction("Login"); 
            }

            var patient = _dbContext.Patients
                .Include(p => p.PatientConditions)
                .SingleOrDefault(p => p.PatientId == loggedInPatientId.Value);

            if (patient == null)
            {
                return NotFound();
            }

            if (viewModel.selectedConditions == null)
            {
                viewModel.selectedConditions = new List<int>();
            }

            foreach (var conditionId in viewModel.selectedConditions)
            {
               
                if (patient.PatientConditions.Any(pc => pc.ConditionId == conditionId))
                {
                    TempData["ErrorMessage"] = "The condition already exists.";
                    
                    var availableConditions = _dbContext.ChronicConditions.ToList();
                    ViewBag.Conditions = new MultiSelectList(availableConditions, "ConditionId", "ConditionName", viewModel.selectedConditions);
                    return RedirectToAction("SpecityCondition");
                }
            }

           
            var newConditions = viewModel.selectedConditions.Select(conditionId => new PatientCondition
            {
                PatientId = loggedInPatientId.Value,
                ConditionId = conditionId
            });

            _dbContext.PatientConditions.AddRange(newConditions);
            _dbContext.SaveChanges(); 

            TempData["SuccessMessage"] = "Condition added successfully!";

            return RedirectToAction("SpecityCondition");


        }
        [HttpPost]
        public IActionResult DeleteConditions(int conditionId)
        {

            var patientConditionToDelete = _dbContext.PatientConditions
                .FirstOrDefault(pc => pc.ConditionId == conditionId);

            if (patientConditionToDelete != null)
            {
                _dbContext.PatientConditions.Remove(patientConditionToDelete);
                _dbContext.SaveChanges();
               
            }
            TempData["SuccessMessage"] = "Condition successfully deleted.";
            return RedirectToAction("SpecityCondition", "Patient"); 
        }
        [HttpGet]
        public IActionResult CancelContract()
        {
            int? loggedInPatientId = HttpContext.Session.GetInt32("PatientId");

            if (loggedInPatientId.HasValue)
            {
                var careContracts = _dbContext.CareContracts
                .Where(contract => contract.PatientId == loggedInPatientId && contract.Status == "Active" && contract.ContractStatus != "Closed")
                .Select(contract => new ListCareContractViewModel
                {
                    ContractId =contract.ContractId,
                    ContractDate = (DateTime)contract.ContractDate,
                    WoundDescription = contract.WoundDescription,
                    ContractStatus = contract.ContractStatus,
                    AddressLine1 = contract.AddressLine1,
                    AddressLine2 = contract.AddressLine2
                })
                .ToList();

                return View(careContracts);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelContract(int contractId)
        {
            var contract = _dbContext.CareContracts.Find(contractId);
            if (contract == null)
            {
                return NotFound();
            }
            contract.Status = "InActive";
            _dbContext.SaveChanges();
            TempData["SuccessMessage2"] = "Care Contract successfully canceled.";
            return RedirectToAction("CancelContract");
        }

    }
}
