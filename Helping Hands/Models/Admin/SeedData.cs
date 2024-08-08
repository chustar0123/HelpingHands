
using System.Security.Cryptography;
using System.Text;

namespace Helping_Hands.Models.Admin
{
    public static class SeedData
    {
        public static void Initialize(Grp0410HelpingHandsContext _dbContext)
        {
            // Check if there are any admins already in the database
            if (!_dbContext.Users.Any())
            {
                // Hardcode the Admin credentials (You may want to change these values)


                string userName = "Admin";
                string password = "HelpingHands";
                string emailAddress = "helpHands@gmail.com";
                string contactNumber = "0682581235";
                string userType = "A";
                string Status = "A";

                // Create a password hash for the admin password
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                // Add the admin credentials to the database
                var adminCredentials = new User
                {
                    UserName = userName,
                    Password = hashedPassword,
                    Email = emailAddress,
                    PhoneNumber = contactNumber,
                    UserType = userType,
                    Status = Status,

                };

                _dbContext.Users.Add(adminCredentials);
                _dbContext.SaveChanges();
            }
        }


    }
}
