using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using BidSystem.Data;
using BidSystem.Data.Models;
using BidSystem.RestServices.Models.BindingModels;
using BidSystem.RestServices.Models.ViewModels;
using Microsoft.AspNet.Identity;

namespace BidSystem.RestServices.Controllers
{
    [RoutePrefix("api")]
    public class BidsController : ApiController
    {
        private BidSystemDbContext db = new BidSystemDbContext();

        [Authorize]
        [HttpGet]
        [Route("bids/my")]
        public IHttpActionResult GetUserBids()
        {
            var currentUsername = User.Identity.GetUserName();

            var bids = db.Bids
                .Where(b => b.Bidder.UserName == currentUsername)
                .OrderByDescending(b => b.DateCreated)
                .Select(b => new BidViewModel()
                {
                    Id = b.Id,
                    OfferId = b.OfferId,
                    DateCreated = b.DateCreated,
                    Bidder = b.Bidder.UserName,
                    OfferedPrice = b.BidPrice,
                    Comment = b.Comment
                });

            return this.Ok(bids);
        }

        [Authorize]
        [HttpGet]
        [Route("bids/won")]
        public IHttpActionResult GetUserBidsWon()
        {
            var currentUsername = User.Identity.GetUserName();

            var bids = db.Bids
                .Where(b => b.Bidder.UserName == currentUsername)
                .Where(b => b.Offer.ExpirationDateTime <= DateTime.Now)
                .Where(b => b.BidPrice == b.Offer.Bids.Max(bid => bid.BidPrice))
                .OrderBy(b => b.DateCreated)
                .Select(b => new BidViewModel()
                {
                    Id = b.Id,
                    OfferId = b.OfferId,
                    DateCreated = b.DateCreated,
                    Bidder = b.Bidder.UserName,
                    OfferedPrice = b.BidPrice,
                    Comment = b.Comment
                });

            return this.Ok(bids);
        }


        [Authorize]
        [HttpPost]
        [Route("offers/{id}/bid")]
        public IHttpActionResult BidForOffer(int id, [FromBody]BidBindingModel model)
        {
            if (model == null)
            {
                return BadRequest("Missing bid data.");
            }

            if (!ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            Offer offerDB = db.Offers.FirstOrDefault(o => o.Id == id);

            if (offerDB == null)
            {
                return NotFound();
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = this.db.Users.Find(currentUserId);

            var maxOfferPrice = offerDB.InitialPrice;

            if (offerDB.Bids.Any())
            {
                maxOfferPrice = offerDB.Bids.Max(b => b.BidPrice);
            }

            if (model.BidPrice < maxOfferPrice)
            {
                return this.Content(HttpStatusCode.BadRequest,
                  new { Message = "Your bid should be > " + offerDB.InitialPrice });
            }

            if (offerDB.ExpirationDateTime < DateTime.Now)
            {
                return this.Content(HttpStatusCode.BadRequest,
                  new { Message = "Offer has expired." });
            }

            var bid = new Bid()
            {
                BidPrice = model.BidPrice,
                OfferId = offerDB.Id,
                BidderId = currentUserId,
                DateCreated = DateTime.Now,
                Comment = model.Comment
            };

            db.Bids.Add(bid);
            db.SaveChanges();

            return this.Ok(new
            {
                Id = bid.Id,
                Bidder = bid.Bidder.UserName,
                Message = "Bid created."
            });

        }
    }
}