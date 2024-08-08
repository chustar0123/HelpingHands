using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Helping_Hands.Models;

public partial class Business
{
    public int BusinessId { get; set; }
    [DisplayName("Organization Name")]
    public string OrganizationName { get; set; }
    [DisplayName("NPO Number")]
    public string Nponumber { get; set; }
    public string Address { get; set; }
    [DisplayName("Contact Number")]
    public string ContactNumber { get; set; }
    public string Email { get; set; }
    [DisplayName("Operating Hours")]
    public string OperatingHours { get; set; }
}
