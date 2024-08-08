namespace Helping_Hands.Models
{
    public class AssignedPatientAndConditions
    {
        public int CareContractId { get; set; }
        public int PatientId { get; set; }
        public DateTime? ContractDate { get; set; }
        public string WoundDescription { get; set; }
        public DateTime? StartCareDate { get; set; }
        public DateTime? EndCareDate { get; set; }
        public string ContractStatus { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PatientFullName { get; set; }
        public List<string> Conditions { get; set; }
    }
}
