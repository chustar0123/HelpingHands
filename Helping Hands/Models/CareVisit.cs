using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helping_Hands.Models;
#nullable enable
public partial class CareVisit
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int VisitId { get; set; }

    public int? NurseId { get; set; }

    public int? ContractId { get; set; }

    public DateTime? VisitDate { get; set; } = default(DateTime?);

    public TimeSpan? ApproximateArriveTime { get; set; } = TimeSpan.Zero;

    public TimeSpan? VisitArriveTime { get; set; } = TimeSpan.Zero;

    public TimeSpan? VisitDepartTime { get; set; } = TimeSpan.Zero;

    public string? WoundCondition { get; set; } = string.Empty;

    public string? Notes { get; set; } = string.Empty;

    public string? Status { get; set; } = string.Empty;

    public virtual CareContract Contract { get; set; }

    public virtual Nurse Nurse { get; set; }
    [NotMapped]
    public string FirstName { get; set; }
    [NotMapped]
    public string Surname { get; set; }
  
   
   
}
