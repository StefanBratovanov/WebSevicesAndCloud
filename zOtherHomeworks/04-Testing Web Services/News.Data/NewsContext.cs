using Microsoft.AspNet.Identity.EntityFramework;

namespace News.Data
{
    using System;
    using System.Data.Entity;
    using News.Models;

    public class NewsContext : IdentityDbContext<ApplicationUser>
    {
        public NewsContext()
            : base("NewsContext")
        {
        }
       public virtual DbSet<News> News { get; set; }

        public static NewsContext Create()
        {
            return new NewsContext();
        }
    }
}