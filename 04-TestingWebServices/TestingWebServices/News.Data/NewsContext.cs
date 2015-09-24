using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using News.Data.Migrations;
using News.Models;

namespace News.Data
{
    public class NewsContext : IdentityDbContext<ApplicationUser>
    {
        
        public NewsContext()
            : base("name=NewsContext")
        {
            var migStrategy = new MigrateDatabaseToLatestVersion<NewsContext, Configuration>();
            Database.SetInitializer(migStrategy);
        }

        public IDbSet<NewsRecord> News { get; set; }

        public static NewsContext Create()
        {
            return new NewsContext();
        }

        //public System.Data.Entity.DbSet<News.Models.NewsRecord> NewsRecords { get; set; }
    }

}