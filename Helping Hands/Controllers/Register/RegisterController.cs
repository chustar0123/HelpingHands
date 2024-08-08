
using Helping_Hands.Models;
using Helping_Hands.Models.Admin;
using Helping_Hands.Models.PasswordHash;
using Helping_Hands.Models.Register;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;

namespace Helping_Hands.Controllers.Patient
{
    public class RegisterController : Controller
    {
        public readonly Grp0410HelpingHandsContext _DbContext;
        public RegisterController(Grp0410HelpingHandsContext dbContext)
        {
            _DbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {

            if(ModelState.IsValid)
            {
                model.Password = PasswordHasher.HashPassword(model.Password);// This line of code make the password to be encypted
                
                model.UserType = "P";
                model.Status = "A";
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

                _DbContext.Users.Add(data);
                _DbContext.SaveChanges();
                TempData["RegistrationSuccess"] = true;
                return RedirectToAction("Register", "Register");



            }

            return View(model);


        }
    }
}
