using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helping_Hands.Models
{
    public class CareVisitViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VisitId { get; set; }

        public int PatientId { get; set; }
        public int? NurseId { get; set; }
        public CareVisit CareVisit { get; set; }
        public int? ContractId { get; set; }
        [DisplayName("Visit Date")]
        [Required(ErrorMessage = "Visit date field cannot be left empty.")]
        public DateTime? VisitDate { get; set; } = default(DateTime?);
        [Required(ErrorMessage = "Arrive time field cannot be left empty.")]
        [DisplayName("Approximate Arrive Time")]
        public TimeSpan? ApproximateArriveTime { get; set; } = TimeSpan.Zero;
        [DisplayName("Visit Arrive Time")]
        public TimeSpan? VisitArriveTime { get; set; } = TimeSpan.Zero;
        [DisplayName("Visit Depart Time")]
        public TimeSpan? VisitDepartTime { get; set; } = TimeSpan.Zero;
        public string PatientName { get; set; }

        public int SelectedPatientId { get; set; }
        public string SelectedPatientName { get; set; }
        [DisplayName("Wound Condition")]
        public string? WoundCondition { get; set; } = string.Empty;
        public virtual Patient Patient { get; set; }

        public string? Notes { get; set; } = string.Empty;
        public string? AddressLine1 { get; set; } = string.Empty;
        public string? Status { get; set; } = string.Empty;

        public virtual CareContract Contract { get; set; }

        public virtual Nurse Nurse { get; set; }
        [NotMapped]
        public string FirstName { get; set; }
        [NotMapped]
        public string Surname { get; set; }
        [NotMapped]
       
        public string PatientFullName { get; set; }
        public string SuburbName { get; set; }
        public string PostalCode { get; set; }
    }
}
