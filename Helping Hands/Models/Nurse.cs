using System;
using System.Collections.Generic;

namespace Helping_Hands.Models;

public partial class Nurse
{
    public int NurseId { get; set; }

    public string Surname { get; set; }

    public string FirstName { get; set; }

    public string Gender { get; set; }

    public string Idnumber { get; set; }

    public virtual ICollection<CareContract> CareContracts { get; set; } = new List<CareContract>();

    public virtual ICollection<CareVisit> CareVisits { get; set; } = new List<CareVisit>();

    public virtual User NurseNavigation { get; set; }

    public virtual ICollection<PreferredSuburb> PreferredSuburbs { get; set; } = new List<PreferredSuburb>();
}
