using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Services.Models.ViewModels
{
    public class RestrauntViewModels
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double? Rating { get; set; }

        public TownViewModelRestaunt Town { get; set; }
    }
}