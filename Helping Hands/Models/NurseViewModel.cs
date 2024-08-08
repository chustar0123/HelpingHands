using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helping_Hands.Models
{
    public class NurseViewModel
    {
        public int NurseId { get; set; }
        public int ContractId { get; set; }
        public int SuburbId { get; set; }
        public List<Nurse> Nurses { get; set; }
        [Required(ErrorMessage = "Please select a suburb.")]
        public string SelectedSuburb { get; set; }
        public List<Suburb> Suburbs { get; set; }
        public string NurseFullName { get; set; }
        public string WoundDescription { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientSurname { get; set; }
        public string AddressLine1 { get; set; }
        public DateTime? ContractDate { get; set; }
        public DateTime? StartCareDate { get; set; }
        public string PostalCode { get; set; }
        public string SuburbName { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; }
        [NotMapped]
        [Required(ErrorMessage = "Please choose a city.")]
        [DisplayName("City")]
        public int SelectedCityId { get; set; }
        [NotMapped]
        [Required(ErrorMessage = "Please choose a suburb.")]
        [DisplayName("Suburb")]
        public int SelectedSuburbId { get; set; }
        public string SelectedSuburbName { get; set; }
        public List<City> Cities { get; set; }
      
        public virtual User NurseNavigation { get; set; }

        public virtual ICollection<PreferredSuburb> PreferredSuburbs { get; set; } = new List<PreferredSuburb>();
    }
}
