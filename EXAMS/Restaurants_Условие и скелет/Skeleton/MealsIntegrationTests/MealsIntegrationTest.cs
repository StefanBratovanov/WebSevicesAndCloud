using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Owin;
using Restauranteur.Models;
using Restaurants.Data;
using Restaurants.Models;
using Restaurants.Services;

namespace MealsIntegrationTests
{
    [TestClass]
    public class MealsIntegrationTest
    {
        private static TestServer httpTestServer;
        private static HttpClient httpClient;

        [AssemblyInitialize]
        public static void TestInit(TestContext context)
        {
            // Start OWIN testing HTTP server with Web API support
            httpTestServer = TestServer.Create(appBuilder =>
            {
                var config = new HttpConfiguration();
                WebApiConfig.Register(config);

                var webAppStartup = new Startup();
                webAppStartup.Configuration(appBuilder);

                appBuilder.UseWebApi(config);
            });

            httpClient = httpTestServer.HttpClient;
            Seed();
        }

        [AssemblyCleanup]
        public static void TestCleanup()
        {
            if (httpTestServer != null)
            {
                httpTestServer.Dispose();
            }
        }

        private static void Seed()
        {
            var context = new RestaurantsContext();

            if (!context.Restaurants.Any())
            {
                context.Restaurants.Add(new Restaurant()
                {
                    Name = "Mir",
                    TownId = 1,
                });
                context.SaveChanges();
            }

            if (!context.MealTypes.Any())
            {
                var mealTypes = new[]
                {
                    new MealType {Name = "Salad", Order = 10},
                    new MealType {Name = "Soup", Order = 20},
                    new MealType {Name = "Main", Order = 30},
                    new MealType {Name = "Dessert", Order = 40}
                };

                foreach (var mealType in mealTypes)
                {
                    context.MealTypes.Add(mealType);
                }

                context.SaveChanges();
            }
            if (!context.Meals.Any())
            {
                context.Meals.Add(new Meal()
                {
                    Name = "Rice",
                    Price = 10.0m,
                    TypeId = 1,
                    RestaurantId = 1
                });
                context.SaveChanges();

            }
        }


        [TestMethod]
        public void EditExistingMeal_ShouldReturn_200_OK()
        {
            var context = new RestaurantsContext();

            var existingBug = context.Meals.FirstOrDefault();

            if (existingBug == null)
            {
                Assert.Fail("Cannot perform test - no meal in DB");
            }

            var endpoint = string.Format("api/meals/{0}", 1);
            var responce = httpClient.GetAsync(endpoint).Result;

            Assert.AreEqual(HttpStatusCode.OK, responce.StatusCode);


        }

        //[TestMethod]
        //public void EditnNonExistingMealShould_Return_404_Not_Found()
        //{
        //    var endpoint = string.Format("api/meals/{0}", -1);
        //    var responce = httpClient.PutAsync(endpoint).Result;

        //    Assert.AreEqual(HttpStatusCode.NotFound, responce.StatusCode);
        //}

    }
}
