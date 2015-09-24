using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BidSystem.RestServices.Models.ViewModels
{
    public class OfferFullDeatailViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Seller { get; set; }

        public DateTime DatePublished { get; set; }

        public decimal InitialPrice { get; set; }

        public DateTime ExpirationDateTime { get; set; }

        public bool IsExpired { get; set; }

        public string BidWinner { get; set; }

        public virtual IEnumerable<BidViewModel>Bids  { get; set; }
    }
}