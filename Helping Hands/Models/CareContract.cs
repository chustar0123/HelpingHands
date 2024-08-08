using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Helping_Hands.Models;

public partial class CareContract
{
    public int ContractId { get; set; }

    public int PatientId { get; set; }

    public int SuburbId { get; set; }
 
    public Boolean IsRead { get; set; }

    [Required(ErrorMessage = "Please choose contract date.")]
    [DisplayName("Contract Date")]
    public DateTime? ContractDate { get; set; }

    [Required(ErrorMessage = "Please enter wound description.")]
    [DisplayName("Wound Description")]
    public string WoundDescription { get; set; }

    [DisplayName("Start Care Date")]
    public DateTime? StartCareDate { get; set; }

    [DisplayName("End Care Date")]
    public DateTime? EndCareDate { get; set; }
    public int? AssignedNurse { get; set; }

    [DisplayName("Contract Status")]
    public string ContractStatus { get; set; }

    [DisplayName("Address Line1")]
    [Required(ErrorMessage = "Please enter Address line 1.")]
    public string AddressLine1 { get; set; }

    [DisplayName("Address Line2")]
    public string AddressLine2 { get; set; }

    public string Status { get; set; }

    public virtual Nurse AssignedNurseNavigation { get; set; }

    public virtual ICollection<CareVisit> CareVisits { get; set; } = new List<CareVisit>();

    public virtual Patient Patient { get; set; }

    public virtual Suburb Suburb { get; set; }

    [NotMapped]
    [Required(ErrorMessage = "Please choose a city.")]
    [DisplayName("City")]
    public int SelectedCityId { get; set; }
    [NotMapped]
    [Required(ErrorMessage = "Please choose a suburb.")]
    [DisplayName("Suburb")]
    public int SelectedSuburbId { get; set; }
}
