using System;
using System.Linq;
using System.Web.Http;
using BidSystem.Data;
using BidSystem.Data.Models;
using BidSystem.RestServices.Models.BindingModels;
using BidSystem.RestServices.Models.ViewModels;
using Microsoft.AspNet.Identity;

namespace BidSystem.RestServices.Controllers
{
    [RoutePrefix("api/offers")]
    public class OffersController : ApiController
    {
        private BidSystemDbContext db = new BidSystemDbContext();

        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAll()
        {
            var offers = db.Offers
                .OrderByDescending(o => o.PublishDate)
                .Select(o => new OfferViewModelAndBidsCount()
                {
                    Id = o.Id,
                    Ttitle = o.Title,
                    Description = o.Description,
                    Seller = o.Seller.UserName,
                    DatePublished = o.PublishDate,
                    InitialPrice = o.InitialPrice,
                    ExpirationDateTime = o.ExpirationDateTime,
                    IsExpired = (o.ExpirationDateTime <= DateTime.Now),
                    BidsCount = o.Bids.Count(),
                    BidWinner = (o.ExpirationDateTime <= DateTime.Now && o.Bids.Any()) ?
                                     o.Bids
                                     .OrderByDescending(b => b.BidPrice)
                                     .Select(b => b.Bidder.UserName)
                                     .FirstOrDefault()
                                : null
                });
            //     BidWinner = o.ExpirationDateTime <= DateTime.Now && o.Bids.Count > 0 ? 
            //                 o.Bids.OrderByDescending(b => b.BidPrice).FirstOrDefault().Bidder.UserName

            return this.Ok(offers);
        }

        [HttpGet]
        [Route("active")]
        public IHttpActionResult GetAllActive()
        {
            var offers = db.Offers
                .Where(o => o.ExpirationDateTime > DateTime.Now)
                .OrderByDescending(o => o.PublishDate)
                .Select(o => new OfferViewModelAndBidsCount()
                {
                    Id = o.Id,
                    Ttitle = o.Title,
                    Description = o.Description,
                    Seller = o.Seller.UserName,
                    DatePublished = o.PublishDate,
                    InitialPrice = o.InitialPrice,
                    ExpirationDateTime = o.ExpirationDateTime,
                    IsExpired = (o.ExpirationDateTime <= DateTime.Now),
                    BidsCount = o.Bids.Count(),
                    BidWinner = (o.ExpirationDateTime <= DateTime.Now && o.Bids.Any()) ?
                                     o.Bids
                                     .OrderByDescending(b => b.BidPrice)
                                     .Select(b => b.Bidder.UserName)
                                     .FirstOrDefault()
                                : null
                });
            //     BidWinner = o.ExpirationDateTime <= DateTime.Now && o.Bids.Count > 0 ? 
            //                 o.Bids.OrderByDescending(b => b.BidPrice).FirstOrDefault().Bidder.UserName

            return this.Ok(offers);
        }

        [HttpGet]
        [Route("expired")]
        public IHttpActionResult GetAllExpired()
        {
            var offers = db.Offers
                .Where(o => o.ExpirationDateTime <= DateTime.Now)
                .OrderByDescending(o => o.ExpirationDateTime)
                .Select(o => new OfferViewModelAndBidsCount()
                {
                    Id = o.Id,
                    Ttitle = o.Title,
                    Description = o.Description,
                    Seller = o.Seller.UserName,
                    DatePublished = o.PublishDate,
                    InitialPrice = o.InitialPrice,
                    ExpirationDateTime = o.ExpirationDateTime,
                    IsExpired = (o.ExpirationDateTime <= DateTime.Now),
                    BidsCount = o.Bids.Count(),
                    BidWinner = (o.ExpirationDateTime <= DateTime.Now && o.Bids.Any()) ?
                                     o.Bids
                                     .OrderByDescending(b => b.BidPrice)
                                     .Select(b => b.Bidder.UserName)
                                     .FirstOrDefault()
                                : null
                });
            //     BidWinner = o.ExpirationDateTime <= DateTime.Now && o.Bids.Count > 0 ? 
            //                 o.Bids.OrderByDescending(b => b.BidPrice).FirstOrDefault().Bidder.UserName

            return this.Ok(offers);
        }

        [HttpGet]
        [Route("details/{id}")]
        public IHttpActionResult GetBug(int id)
        {
            var offer = db.Offers.FirstOrDefault(x => x.Id == id);
            if (offer == null)
            {
                return NotFound();
            }

            var offerView = new OfferFullDeatailViewModel()
            {
                Id = offer.Id,
                Title = offer.Title,
                Description = offer.Description,
                Seller = offer.Seller.UserName,
                DatePublished = offer.PublishDate,
                InitialPrice = offer.InitialPrice,
                ExpirationDateTime = offer.ExpirationDateTime,
                IsExpired = (offer.ExpirationDateTime <= DateTime.Now),
                BidWinner = (offer.ExpirationDateTime <= DateTime.Now && offer.Bids.Any()) ?
                                      offer.Bids.OrderByDescending(b => b.BidPrice)
                                      .Select(b => b.Bidder.UserName)
                                      .FirstOrDefault()
                                 : null,
                Bids = offer.Bids.Select(b => new BidViewModel()
                {
                    Id = b.Id,
                    OfferId = b.OfferId,
                    DateCreated = b.DateCreated,
                    Bidder = b.Bidder.UserName,
                    OfferedPrice = b.BidPrice,
                    Comment = b.Comment
                })
            };

            return this.Ok(offerView);
        }

        [Authorize]
        [HttpGet]
        [Route("my")]
        public IHttpActionResult GetPersonalOffers()
        {
            var currentUsername = User.Identity.GetUserName();

            var offers = db.Offers
                .Where(o => o.Seller.UserName == currentUsername)
                .OrderBy(o => o.PublishDate)
                .Select(o => new OfferViewModelAndBidsCount()
                {
                    Id = o.Id,
                    Ttitle = o.Title,
                    Description = o.Description,
                    Seller = o.Seller.UserName,
                    DatePublished = o.PublishDate,
                    InitialPrice = o.InitialPrice,
                    ExpirationDateTime = o.ExpirationDateTime,
                    IsExpired = (o.ExpirationDateTime <= DateTime.Now),
                    BidsCount = o.Bids.Count(),
                    BidWinner = (o.ExpirationDateTime <= DateTime.Now && o.Bids.Any()) ?
                                     o.Bids
                                     .OrderByDescending(b => b.BidPrice)
                                     .Select(b => b.Bidder.UserName)
                                     .FirstOrDefault()
                                : null
                });

            return this.Ok(offers);
        }

        [Authorize]
        [HttpPost]
        public IHttpActionResult PostOffer(OfferBindingModel model)
        {
            if (model == null)
            {
                return BadRequest("Missing offer data.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);

            var offerToPost = new Offer()
            {
                Title = model.Title,
                Description = model.Description,
                SellerId = currentUserId,
                PublishDate = DateTime.Now,
                InitialPrice = model.InitialPrice,
                ExpirationDateTime = model.ExpirationDateTime
            };

            db.Offers.Add(offerToPost);
            db.SaveChanges();


            return this.CreatedAtRoute(
                                "DefaultApi",
                                new { controller = "offers/details", id = offerToPost.Id },
                                new
                                {
                                    Id = offerToPost.Id,
                                    Seller = offerToPost.Seller.UserName,
                                    Message = "Offer created."
                                });
            //return this.CreatedAtRoute("OfferDetails",
            //    new { id = offer.Id },
            //    new { offer.Id, Seller = seller.UserName, Message = "Offer created." });
        }

    }
}


