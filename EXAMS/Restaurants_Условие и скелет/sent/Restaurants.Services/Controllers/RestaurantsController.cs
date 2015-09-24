using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using Restaurants.Data;
using Restaurants.Models;
using Restaurants.Services.Models.BindingModels;
using Restaurants.Services.Models.ViewModels;

namespace Restaurants.Services.Controllers
{
    [RoutePrefix("api/restaurants")]
    public class RestaurantsController : ApiController
    {
        private RestaurantsContext db = new RestaurantsContext();


        [HttpGet]
        public IHttpActionResult GetRestaurants([FromUri] string townId)
        {
            var id = int.Parse(townId);
            var town = db.Towns.Find(id);
            if (town == null)
            {
                return NotFound();
            }
            // .Where(b => b.BidPrice == b.Offer.Bids.Max(bid => bid.BidPrice))



            var restaurants = town.Restaurants
                .OrderByDescending(r => r.Ratings.Average(rating => rating.Stars))
                .ThenBy(r => r.Name)
                .Select(r => new RestrauntViewModels()
                {
                    Id = r.Id,
                    Name = r.Name,
                    Rating = r.Ratings.Any() ? r.Ratings.Average(rating => rating.Stars) : 0,
                    //Rating =  r.Ratings.Average(rating => rating.Stars),
                    Town = (new TownViewModelRestaunt
                    {
                        Id = town.Id,
                        Name = town.Name,
                    })
                });

            return Ok(restaurants);
        }

        [Authorize]
        [HttpPost]
        public IHttpActionResult CreateNewRestaurant([FromBody]RestaurantBindingModel model)
        {
            if (model == null)
            {
                return BadRequest("Missing restraunt data.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);

            var restrauntToPost = new Restaurant()
            {
                Name = model.Name,
                TownId = int.Parse(model.TownId),
                OwnerId = currentUserId
            };

            db.Restaurants.Add(restrauntToPost);
            db.SaveChanges();

            return this.CreatedAtRoute(
                "DefaultApi",
                new { controller = "restaurants", id = restrauntToPost.Id },
                new { restrauntToPost.Id, restrauntToPost.Name });
        }


        [Authorize]
        [HttpPost]
        [Route("{id}/rate")]
        public IHttpActionResult RateResraunt(int id, [FromBody]RateRestaurantBindingModel model)
        {
            if (model == null)
            {
                return BadRequest("Missing restraunt rating data.");
            }

            if (!ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            Restaurant restrauntDB = db.Restaurants.FirstOrDefault(o => o.Id == id);

            if (restrauntDB == null)
            {
                return NotFound();
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = this.db.Users.Find(currentUserId);

            if (currentUserId == restrauntDB.OwnerId)
            {
                return this.Content(HttpStatusCode.BadRequest,
                  new { Message = "Your cannot rate your own restraunt." });
            }

            //if (!restrauntDB.Ratings.Any())
            //{
            //   ratings.Add(model.Stars);
            //}

            if (model.Stars < 0 || model.Stars > 10)
            {
                return this.Content(HttpStatusCode.BadRequest,
                  new { Message = "Your rating should be in [1..10] interval" });
            }

            var rating = new Rating()
            {
                Stars = model.Stars,
                RestaurantId = restrauntDB.Id,
                UserId = currentUserId
            };

            db.Ratings.Add(rating);
            db.SaveChanges();

            return this.Ok();

        }

        [HttpGet]
        [Route("{id}/meals")]
        public IHttpActionResult GetMealsByRestaurants([FromUri] int id)
        {

            var restraunt = db.Restaurants.Find(id);
            if (restraunt == null)
            {
                return NotFound();
            }

            var meals = restraunt.Meals
                .OrderBy(m => m.Type.Name == "salad")
                .ThenBy(m => m.Type.Name == "soup")
                .ThenBy(m => m.Type.Name == "main")
                .ThenBy(m => m.Type.Name == "dessert")
                .ThenBy(m => m.Name)
                .Select(m => new MealViewModels()
                {
                    Id = m.Id,
                    Name = m.Name,
                    Price = m.Price,
                    Type = m.Type.Name
                });

            return Ok(meals);
        }



    }
}