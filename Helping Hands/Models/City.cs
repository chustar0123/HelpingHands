using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace Helping_Hands.Models;

public partial class City
{
    public int CityId { get; set; }

    [Required]
    [DisplayName("City Abbreviation")]
    public string CityAbbreviation { get; set; }
    [Required]
    [DisplayName("City Name")]
    public string CityName { get; set; }

    public string Status { get; set; }

    public virtual ICollection<Suburb> Suburbs { get; set; } = new List<Suburb>();
}
