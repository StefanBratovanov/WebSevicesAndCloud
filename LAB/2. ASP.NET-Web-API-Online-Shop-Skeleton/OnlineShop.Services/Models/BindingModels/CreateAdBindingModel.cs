using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace OnlineShop.Services.Models.BindingModels
{
    public class CreateAdBindingModel
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        [Required]
        [MinLength(3)]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int TypeId { get; set; }

        public IEnumerable<int> Categories { get; set; }

    }
}