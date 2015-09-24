
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using OnlineShop.Models;
using OnlineShop.Services.Models.BindingModels;
using OnlineShop.Services.Models.ViewModels;


namespace OnlineShop.Services.Controllers
{
    public class AdsController : BaseApiController
    {
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetAds()
        {
            var ads = this.Data.Ads
                .Where(a => a.ClosedOn == null)
                .OrderBy(a => a.Type.Name)
                .ThenBy(a => a.PostedOn)
                .Select(AdsViewModel.Create);

            return this.Ok(ads);

        }

        [Authorize]
        [HttpPost]
        public IHttpActionResult CreateAds(CreateAdBindingModel model)
        {
            var UserId = this.User.Identity.GetUserId();
            if (UserId == null)
            {
                return this.Unauthorized();
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            if (model == null)
            {
                return this.BadRequest("Model cannot be empty!");
            }

            if (!model.Categories.Any() || model.Categories.Count() > 3)
            {
                return this.BadRequest("Categories count cannot be " + model.Categories.Count());
            }

            var adTypeDb = this.Data.AdTypes.FirstOrDefault(a => a.Id == model.TypeId);

            if (adTypeDb == null)
            {
                return this.BadRequest("No type with id " + model.TypeId + " found!");
            }

            var categoris = new List<Category>();

            foreach (var catId in model.Categories)
            {
                var categoryFromDB = this.Data.Categories.Find(catId);

                if (categoryFromDB == null)
                {
                    return this.BadRequest("No category with id " + catId + " found!");
                }

                categoris.Add(categoryFromDB);
            }

            var user = this.Data.Users.Find(UserId);

            var adToAdd = new Ad()
            {
                Categories = categoris,
                Description = model.Description,
                Name = model.Name,
                Price = model.Price,
                TypeId = model.TypeId,
                PostedOn = DateTime.Now,
                Owner = user,
                Type = adTypeDb
            };

            this.Data.Ads.Add(adToAdd);
            this.Data.SaveChanges();

            var result = this.Data.Ads
                .Where(a => a.Id == adToAdd.Id)
                .Select(AdsViewModel.Create)
                .FirstOrDefault();

            return this.Ok(result);

        }

        [Authorize]
        [HttpPut]
        [Route("api/ads/{id}/close")]
        public IHttpActionResult CloseAd(int id)
        {
            Ad ad = null;
            ad = this.Data.Ads.FirstOrDefault(a => a.Id == id);
            if (ad == null)
            {
                return this.NotFound();
            }

            string loggedUserId = this.User.Identity.GetUserId();

            if (ad.OwnerId != loggedUserId)
            {
                return this.BadRequest("You are not the owner of the ad");
            }

            ad.Status = AdStatus.Closed;
            ad.ClosedOn = DateTime.Now;

            this.Data.SaveChanges();

            return this.Ok();

        }


    }
}