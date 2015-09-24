
using System.ComponentModel.DataAnnotations;

namespace Restaurants.Services.Models.BindingModels
{
    public class EditMealBindingModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int TypeId { get; set; }
    }
}