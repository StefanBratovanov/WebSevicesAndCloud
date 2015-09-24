using BookShop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using BookShopService.Models;
using BookShop.Models;

namespace BookShop.Service.Controllers
{
    public class UsersController : ApiController
    {
        private BookShopContext context = new BookShopContext();

        // Get all user purchases
        // Example: GET /api/user/{username}/purchases
        [HttpGet]
        [Route("api/users/{username}/purchases")]
        public IHttpActionResult GetUserPurcgases(string username)
        {
            if (!context.Users.Select(u => u.UserName).ToList().Contains(username))
            {
                return this.BadRequest("No such user.");
            }

            var userPurchases = context.Users
                .Where(u => u.UserName == username)
                .Select(u => new
                {
                    Username = u.UserName,
                    Purchases = u.Purchases
                        .OrderBy(p => p.DateOfPurchase)
                        .Select(p => new 
                        {
                            BookTitle = p.Book.Title,
                            PurchasePrice = p.Price,
                            DateOfPurchase = p.DateOfPurchase,
                            IsRecalled = p.IsRecalled
                        })
                });
            
            return this.Ok(userPurchases);
        }

        // Add new role to user
        // PUT /api/users/{username}/roles
        [HttpPut]
        [Route("api/users/{username}/roles")]
        [Authorize(Roles="Administrator")]
        public IHttpActionResult AddRole([FromBody]RoleBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest("Model cannot be empty.");
            }

            var user = context.Users.FirstOrDefault(u => u.UserName == model.Username);
            if (user == null)
            {
                return this.BadRequest("No such user.");
            }

            var moderatorRole = context.Roles.FirstOrDefault(r => r.Name == "Moderator");
            var hasUserSameRole = user.Roles.FirstOrDefault(ur => ur.RoleId == moderatorRole.Id) == null ? false : true;

            if (hasUserSameRole)
            {
                return this.BadRequest("User already has that role.");
            }

            user.Roles.Add(new IdentityUserRole() 
            {
                RoleId = moderatorRole.Id
            });

            context.SaveChanges();

            return this.Ok();
        }

        // Remove role from user
        // DELETE api/users/{username}/roles
        [HttpDelete]
        [Route("api/users/{username}/roles")]
        [Authorize(Roles = "Administrator")]
        public IHttpActionResult DelereRole([FromBody]RoleBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest("Model cannot be empty.");
            }

            var user = context.Users.FirstOrDefault(u => u.UserName == model.Username);
            if (user == null)
            {
                return this.BadRequest("No such user.");
            }

            var moderatorRole = context.Roles.FirstOrDefault(r => r.Name == "Moderator");
            var hasUserSameRole = user.Roles.FirstOrDefault(ur => ur.RoleId == moderatorRole.Id) == null ? false : true;

            if (!hasUserSameRole)
            {
                return this.BadRequest("User do note have that role.");
            }


            var role = user.Roles.FirstOrDefault(r => r.RoleId == moderatorRole.Id);
            user.Roles.Remove(role);

            context.SaveChanges();

            return this.Ok();
        }
    }
}
