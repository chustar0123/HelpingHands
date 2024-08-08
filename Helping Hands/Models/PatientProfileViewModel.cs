using System.ComponentModel;

namespace Helping_Hands.Models
{
    public class PatientProfileViewModel
    {
        public int PatientId { get;set; }
        public string Surname { get; set; }
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        public string Gender { get; set; }
        [DisplayName("ID Number")]
        public string Idnumber { get; set; }

        [DisplayName("Date Of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [DisplayName("Emergency Contact Person")]
        public string EmergencyContactPerson { get; set; }
        [DisplayName("Emergency Contact Number")]
        public string EmergencyContactNumber { get; set; }

        [DisplayName("Additional Information")]
        public string AdditionalInformation { get; set; }
    }
}
