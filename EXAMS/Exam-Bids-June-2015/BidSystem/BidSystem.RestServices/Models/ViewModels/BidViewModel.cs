using System;
using BidSystem.Data.Models;

namespace BidSystem.RestServices.Models.ViewModels
{
    public class BidViewModel
    {
        public int Id { get; set; }

        public int OfferId { get; set; }

        public DateTime DateCreated { get; set; }

        public string Bidder { get; set; }

        public decimal OfferedPrice { get; set; }

        public string Comment { get; set; }
    }
}
