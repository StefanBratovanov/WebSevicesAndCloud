using System;
using System.ComponentModel.DataAnnotations;
using BidSystem.Data.Models;

namespace BidSystem.RestServices.Models.ViewModels
{
    public class OfferViewModelAndBidsCount
    {
        public int Id { get; set; }

        public string Ttitle { get; set; }

        public string Description { get; set; }

        public string Seller { get; set; }

        public DateTime DatePublished { get; set; }

        public decimal InitialPrice { get; set; }

        public DateTime ExpirationDateTime { get; set; }

        public bool IsExpired { get; set; }

        public int BidsCount { get; set; }

        public string BidWinner { get; set; }
    }
}