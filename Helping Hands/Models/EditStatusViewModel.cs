
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace Helping_Hands.Models
{
    public class EditStatusViewModel
    {
        [DisplayName("User")]
        public int SelectedUserId { get; set; }

        [DisplayName("Status")]
        public string SelectedStatus { get; set; }

        public SelectList Users { get; set; }

        public SelectList StatusList { get; set; }
        

    }
}
