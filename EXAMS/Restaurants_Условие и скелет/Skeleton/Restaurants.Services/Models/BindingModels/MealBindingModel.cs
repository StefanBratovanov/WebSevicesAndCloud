
using System.ComponentModel.DataAnnotations;

namespace Restaurants.Services.Models.BindingModels
{
    public class MealBindingModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int RestaurantId { get; set; }

        [Required]
        public int TypeId { get; set; }

    }
}