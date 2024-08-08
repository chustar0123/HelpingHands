using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Helping_Hands.Models
{
    public class CareContractViewModel
    {

        public int ContractId { get; set; }

        public int PatientId { get; set; }

        public int SuburbId { get; set; }
       

        [Required(ErrorMessage = "Please choose contract date.")]
        [DisplayName("Contract Date")]
        public DateTime? ContractDate { get; set; }

        [Required(ErrorMessage = "Please enter wound description.")]
        [DisplayName("Wound Description")]
        public string WoundDescription { get; set; }

        [DisplayName("Start Care Date")]
        public DateTime? StartCareDate { get; set; }

        [DisplayName("End Care Date")]
        public DateTime? EndCareDate { get; set; }

        [DisplayName("Assigned Nurse")]
        public string AssignedNurse { get; set; }

        [DisplayName("Contract Status")]
        public string ContractStatus { get; set; }

        [DisplayName("Address Line1")]
        [Required(ErrorMessage = "Please enter Address line 1.")]
        public string AddressLine1 { get; set; }
        public List<CareContract> CareContracts { get; set; }
        public List<Helping_Hands.Models.CareContractViewModel> Patients { get; set; }

        [DisplayName("Address Line2")]
        public string AddressLine2 { get; set; }
        public string SuburbName { get; set; }
        public string PostalCode { get; set; }
        public string PatientName {  get; set; }
       
        public int SelectedPatientId { get; set; }
        public string SelectedPatientName { get; set; }
        public virtual ICollection<CareVisit> CareVisits { get; set; } = new List<CareVisit>();

        public virtual Patient Patient { get; set; }

        public virtual Suburb Suburb { get; set; }

        [Required(ErrorMessage = "Please choose a city.")]
        [DisplayName("City")]
        public int SelectedCityId { get; set; }

        [Required(ErrorMessage = "Please choose a suburb.")]
        [DisplayName("Suburb")]
        public int SelectedSuburbId { get; set; }
        public string PatientFullName { get; set; }
        [DisplayName("Days Overdue")]
        public int DaysOverdue { get; set; }
        public List<NurseViewModel> Nurses { get; set; }
        public List<SelectListItem> CityList { get; set; } // For storing the list of cities
        public List<SelectListItem> SuburbList { get; set; }



    }
}
