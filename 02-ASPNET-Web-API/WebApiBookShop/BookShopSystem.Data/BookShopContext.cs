
using BookShopSystem.Data.Migrations;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BookShopSystem.Data
{
    using System.Data.Entity;
    using Models;

    public class BookShopContext : IdentityDbContext<ApplicationUser>
    {

        public BookShopContext()
            : base("name=BookShopContext")
        {
            var migrationStrategy = new MigrateDatabaseToLatestVersion<BookShopContext, Configuration>();
            Database.SetInitializer(migrationStrategy);
        }

        public IDbSet<Book> Books { get; set; }
        public IDbSet<Author> Authors { get; set; }
        public IDbSet<Category> Categories { get; set; }
        public IDbSet<Purchase> Purchases { get; set; }


        public static BookShopContext Create()
        {
            return new BookShopContext();
        }

        public System.Data.Entity.DbSet<BookShopSystem.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}