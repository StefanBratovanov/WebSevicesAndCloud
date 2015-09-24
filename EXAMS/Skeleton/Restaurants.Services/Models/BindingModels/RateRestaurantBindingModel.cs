
using System.ComponentModel.DataAnnotations;

namespace Restaurants.Services.Models.BindingModels
{
    public class RateRestaurantBindingModel
    {
        [Required]
        public int Stars { get; set; }
    }
}