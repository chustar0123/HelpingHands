using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Helping_Hands.Models
{
    public class ListCareContractViewModel
    {
        
        
            public int ContractId { get; set; }
            public DateTime ContractDate { get; set; }
            public string WoundDescription { get; set; }
            public int SuburbId { get; set; }
            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }
            public string ContractStatus { get; set; }
           
        



    }
}
