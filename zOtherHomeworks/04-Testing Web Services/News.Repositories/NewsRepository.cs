namespace News.Repositories
{
    using System.Data.Entity;
    using System.Linq;
    
    public class NewsRepository : IRepository<Models.News>
    {
        private readonly DbContext dbContext;

        public NewsRepository(DbContext context)
        {
            this.dbContext = context;
        }

        public Models.News Add(Models.News entity)
        {
            this.dbContext.Set<Models.News>().Add(entity);
            return entity;
        }

        public IQueryable<Models.News> All()
        {
            return this.dbContext.Set<Models.News>();
        }

        public void Delete(Models.News entity)
        {
            this.ChangeState(entity, EntityState.Deleted);
        }

        public void Update(Models.News entity)
        {
            this.ChangeState(entity, EntityState.Modified);
        }

        public void SaveChanges()
        {
            this.dbContext.SaveChanges();
        }

        public Models.News Find(int id)
        {
            return this.dbContext.Set<Models.News>().Find(id);
        }

        private void ChangeState(Models.News news, EntityState state)
        {
            var entry = this.dbContext.Entry(news);
            if (entry.State == EntityState.Detached)
            {
                this.dbContext.Set<Models.News>().Attach(news);
            }

            entry.State = state;
        }
    }
}
