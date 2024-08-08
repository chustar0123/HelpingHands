namespace Helping_Hands.Models
{
    public class PreferredViewModel
    {
        public List<PreferredSuburb> PreferredSuburbs { get; set; }
        public List<Suburb> AllSuburbs { get; set; }
        public int? SelectedSuburbID { get; set; }
    }
}
