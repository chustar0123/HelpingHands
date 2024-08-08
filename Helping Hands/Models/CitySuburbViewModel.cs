using Microsoft.AspNetCore.Mvc.Rendering;

namespace Helping_Hands.Models
{
    public class CitySuburbViewModel
    {
        public int SelectedCityId { get; set; }
        public List<SelectListItem> Cities { get; set; }
        public int SelectedSuburbId { get; set; }
        public List<SelectListItem> Suburbs { get; set; }
    }
}
