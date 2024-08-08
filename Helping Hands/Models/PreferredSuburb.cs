using System;
using System.Collections.Generic;

namespace Helping_Hands.Models;

public partial class PreferredSuburb
{
    public int PreferredSuburbId { get; set; }

    public int NurseId { get; set; }

    public int SuburbId { get; set; }

    public virtual Nurse Nurse { get; set; }

    public virtual Suburb Suburb { get; set; }
}
