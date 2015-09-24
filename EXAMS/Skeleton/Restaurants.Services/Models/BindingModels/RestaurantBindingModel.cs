using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Restaurants.Models;

namespace Restaurants.Services.Models.BindingModels
{
    public class RestaurantBindingModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string TownId { get; set; }


    }
}