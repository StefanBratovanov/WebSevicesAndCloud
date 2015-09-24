
using System;
using System.Net;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Restauranteur.Models;
using Restaurants.Data;
using Restaurants.Models;
using Restaurants.Services.Models.BindingModels;

namespace Restaurants.Services.Controllers
{
    [RoutePrefix("api/meals")]
    public class MealsController : ApiController
    {
        private RestaurantsContext db = new RestaurantsContext();

        [Authorize]
        [HttpPost]
        public IHttpActionResult CreateNewMeal([FromBody]MealBindingModel model)
        {
            if (model == null)
            {
                return BadRequest("Missing meal data.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);

            var restaurantId = model.RestaurantId;
            var restraunt = db.Restaurants.Find(restaurantId);

            if (restraunt == null)
            {
                return BadRequest("No such restraunt");
            }

            if (currentUserId != restraunt.OwnerId)
            {
                return this.Content(HttpStatusCode.Unauthorized,
                  new { Message = "Your are not owner of this restraunt." });
            }

            if (model.TypeId < 1 || model.TypeId > 4)
            {
                return BadRequest("Incorrect meal type");
            }

            var mealToPost = new Meal()
            {
                Name = model.Name,
                RestaurantId = model.RestaurantId,
                Price = model.Price,
                TypeId = model.TypeId,
            };

            db.Meals.Add(mealToPost);
            db.SaveChanges();

            return this.CreatedAtRoute(
                "DefaultApi",
                new { controller = "meals", id = mealToPost.Id },
                new { mealToPost.Id, mealToPost.Name, mealToPost.Price, mealToPost.Type });
        }

        [Authorize]
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult EditMeal(int id, [FromBody]EditMealBindingModel model)
        {
            var meal = db.Meals.Find(id);

            if (meal == null)
            {
                return this.NotFound();
            }

            if (model == null)
            {
                return this.BadRequest("Missing meal data.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);

            var restaurantId = meal.RestaurantId;
            var restraunt = db.Restaurants.Find(restaurantId);

            if (currentUserId != restraunt.OwnerId)
            {
                return this.Content(HttpStatusCode.Unauthorized,
                  new { Message = "Your are not owner of this restraunt." });
            }

            if (model.TypeId < 1 || model.TypeId > 4)
            {
                return BadRequest("Incorrect meal type");
            }

            meal.Name = model.Name;
            meal.Price = model.Price;
            meal.TypeId = model.TypeId;

            db.SaveChanges();

            return this.Content(HttpStatusCode.OK,
                   new { id = meal.Id, name = meal.Name, price = meal.Price, type = meal.Type.Name });

        }

        [Authorize]
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteMealById(int id)
        {
            var meal = db.Meals.Find(id);

            if (meal == null)
            {
                return this.NotFound();
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);

            var restaurantId = meal.RestaurantId;
            var restraunt = db.Restaurants.Find(restaurantId);

            if (currentUserId != restraunt.OwnerId)
            {
                return this.Content(HttpStatusCode.Unauthorized,
                  new { Message = "Your are not owner of this restraunt." });
            }

            db.Meals.Remove(meal);
            db.SaveChanges();

            return this.Ok(new
            {
                Message = "Meal #" + id + " deleted"
            });
        }

        [Authorize]
        [HttpPost]
        [Route("{id}/order")]
        public IHttpActionResult CreateOrder(int id, [FromBody]OrderBindingModel model)
        {
            if (model == null)
            {
                return BadRequest("Missing order data.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var meal = db.Meals.Find(id);

            if (meal == null)
            {
                return this.NotFound();
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);

            var order = new Order()
            {
                MealId = meal.Id,
                Quantity = model.Quantity,
                UserId = currentUserId,
                OrderStatus = OrderStatus.Pending,
                CreatedOn = DateTime.Now,
            };

            db.Orders.Add(order);
            db.SaveChanges();

            return this.Ok();

        }




    }
}