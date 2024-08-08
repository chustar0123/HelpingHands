using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Helping_Hands.Models
{
    public class PreferredSuburbViewModel
    {
        public int NurseID { get; set; } 
        public Nurse Nurse { get; set; }
        public List<PreferredSuburb> PreferredSuburbs { get; set; }
        public List<Suburb> Suburbs { get; set; }
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
    }
}
