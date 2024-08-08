using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helping_Hands.Models;

public partial class Suburb
{
    public int SuburbId { get; set; }

    public string SuburbName { get; set; }

    public string PostalCode { get; set; }
    
    public int? CityId { get; set; }


    public string Status { get; set; }

    public virtual ICollection<CareContract> CareContracts { get; set; } = new List<CareContract>();

    public virtual City City { get; set; }

    public virtual ICollection<PreferredSuburb> PreferredSuburbs { get; set; } = new List<PreferredSuburb>();
}
