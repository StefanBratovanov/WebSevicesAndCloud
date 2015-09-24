
using System.ComponentModel.DataAnnotations;


namespace OnlineShop.Services.Models.ViewModels
{
    public class OwnerViewMOdel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}