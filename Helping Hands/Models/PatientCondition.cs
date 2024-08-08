using System;
using System.Collections.Generic;

namespace Helping_Hands.Models;

public partial class PatientCondition
{
    public int PatientConditionId { get; set; }

    public int PatientId { get; set; }

    public int ConditionId { get; set; }

    public virtual ChronicCondition Condition { get; set; }

    public virtual Patient Patient { get; set; }
}
