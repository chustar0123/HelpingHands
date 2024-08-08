using System.ComponentModel;

namespace Helping_Hands.Models
{
    public class CurrentlyNewCareContractViewModel
    {
        public int CareContractId { get; set; }
        public int SuburbId { get; set; }
        public int PatientId { get; set; }
        public DateTime? ContractDate { get; set; }
        public string WoundDescription { get; set; }
        public DateTime? StartCareDate { get; set; }
        public DateTime? EndCareDate { get; set; }
        public string ContractStatus { get; set; }
        public string SuburbName { get; set; }
        public string PostalCode { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PatientFullName { get; set; }
        [DisplayName("Days Overdue")]
        public int DaysOverdue { get; set; }
    }
}
