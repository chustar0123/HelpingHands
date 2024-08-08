namespace Helping_Hands.Models
{
    public class AssignNurseToCareContractViewModel
    {
        public int SelectedCareContractId { get; set; }
        public int SelectedSuburbId { get; set; }
        public List<Nurse> AvailableNurses { get; set; }
        public int? SelectedNurseId { get; set; }
    }
}
