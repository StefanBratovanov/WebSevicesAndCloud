
using System.Linq;
using System.Web.Http;
using Restaurants.Data;
using Restaurants.Models;
using Restaurants.Services.Models.ViewModels;

namespace Restaurants.Services.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private RestaurantsContext db = new RestaurantsContext();

        [Authorize]
        [HttpGet]
        public IHttpActionResult GetPendingOrders(
            [FromUri] string startPage = null,
            [FromUri] string limit = null,
            [FromUri] string mealId = null)
        {
            var orders = db.Orders
                .Where(o => o.OrderStatus == OrderStatus.Pending)
                .OrderByDescending(o => o.CreatedOn)
                .Select(o => new OrderViewModel()
                {
                    Id = o.Id,
                    Meal = new MealViewModels()
                    {
                        Id = o.MealId,
                        Name = o.Meal.Name,
                        Price = o.Meal.Price,
                        Type = o.Meal.Type.Name
                    },
                    Quantity = o.Quantity,
                    Status = o.OrderStatus.ToString(),
                    CreatedOn = o.CreatedOn
                }).AsQueryable();

            var startP = 0;
            var limitOrders = 0;
            int limitOfOrders = 0;
            var idMeal = 0;

            if (startPage != null)
            {
                startP = int.Parse(startPage);
            }
            if (limit != null)
            {
                limitOrders = int.Parse(limit);
                if (limitOrders >= 2 && limitOrders <= 10)
                {
                    limitOfOrders = limitOrders;
                }
                else
                {
                    return this.BadRequest("Limit should be in [2..10] range.");
                }
            }
            if (mealId != null)
            {
                idMeal = int.Parse(mealId);
            }

            return this.Ok(orders
                .Skip(startP * limitOrders)
                .Take(limitOfOrders));
        }
    }
}