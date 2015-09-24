using System.Security.Principal;
using OnlineShop.Data.Migrations;

namespace OnlineShop.Data
{
    using OnlineShop.Models;
    using System.Data.Entity;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class OnlineShopContext : IdentityDbContext<ApplicationUser>
    {

        public OnlineShopContext()
            : base("name=OnlineShopContext")
        {
            var migStrategy = new MigrateDatabaseToLatestVersion<OnlineShopContext, Configuration>();
            Database.SetInitializer(migStrategy);
        }

        public IDbSet<Ad> Ads { get; set; }
        public IDbSet<AdType> AdTypes { get; set; }
        public IDbSet<Category> Categories { get; set; }

        public static OnlineShopContext Create()
        {
            return new OnlineShopContext();
        }


        // DELETE this SHIT!
      //  public System.Data.Entity.DbSet<OnlineShop.Models.ApplicationUser> ApplicationUsers { get; set; }


    }


}