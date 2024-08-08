namespace Helping_Hands.Models
{
    public class SuburbViewModel
    {
        public List<City> Cities { get; set; } // List to store available cities
        public int CityId { get; set; } // Property to store the selected city
        public string SuburbName { get; set; }
    }
}
