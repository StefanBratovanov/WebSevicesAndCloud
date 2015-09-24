

using System;
using System.ComponentModel.DataAnnotations;
using BidSystem.Data.Models;

namespace BidSystem.RestServices.Models.BindingModels
{
    public class BidBindingModel
    {
        [Required]
        public decimal BidPrice { get; set; }

        public string Comment { get; set; }
    }
}
