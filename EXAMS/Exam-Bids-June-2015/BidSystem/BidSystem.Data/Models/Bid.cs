using System;
using System.ComponentModel.DataAnnotations;

namespace BidSystem.Data.Models
{
    public class Bid
    {
        public int Id { get; set; }

        [Required]
        public decimal BidPrice { get; set; }

        public int OfferId { get; set; }

        [Required]
        public virtual Offer Offer { get; set; }

        public string BidderId { get; set; }

        public virtual User Bidder { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public string Comment { get; set; }
    }
}

//Bids belong to existing offers and have bid price, bidder (registered user),
//date and comment (optional)