using System;
using System.Data.Entity;
using News.Data.Repositories;

namespace News.Data.DataClasses
{
    public class NewsData : INewsData
    {
        private DbContext dbContext;

        public NewsData(DbContext context)
        {
            this.dbContext = context;
        }

        public NewsRepository NewsRepository
        {
            get
            {
                return new NewsRepository(this.dbContext);
            }
        }

        public int SaveChanges()
        {
            return this.dbContext.SaveChanges();
        }
    }
}
