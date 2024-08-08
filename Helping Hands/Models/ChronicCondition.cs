using System;
using System.Collections.Generic;

namespace Helping_Hands.Models;

public partial class ChronicCondition
{
    public int ConditionId { get; set; }

    public string ConditionName { get; set; }

    public string Description { get; set; }

    public string Status { get; set; }

    public virtual ICollection<PatientCondition> PatientConditions { get; set; } = new List<PatientCondition>();
}
