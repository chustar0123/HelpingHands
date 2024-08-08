namespace Helping_Hands.Models
{
    public class CareContractAssignmentViewModel
    {
        public int ContractId { get; set; }
        public CareContract CareContract { get; set; }
        public DateTime? ContractDate { get; set; }
        public List<Nurse> Nurses { get; set; }
        public int DaysOverDue { get; set; }
        public string PatientFullName { get; set; }
        public string SuburbName { get; set; }
        public string WoundDescription { get; set; }
        public string ContractStatus { get; set; }
    }
}
