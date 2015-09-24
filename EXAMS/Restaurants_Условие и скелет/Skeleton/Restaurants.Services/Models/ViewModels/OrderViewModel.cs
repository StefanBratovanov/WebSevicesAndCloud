using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Services.Models.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        public MealViewModels Meal { get; set; }

        public int Quantity { get; set; }

        public string Status { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}