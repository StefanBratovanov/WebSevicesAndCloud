namespace News.Repositories
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using News.Data;
    using News.Models;

    public class NewsRepository : IRepository<News>
    {
        private NewsContext context;

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        private IDbSet<News> dbSet;

        public NewsRepository() :
            this(new NewsContext())
        {
        }

        public NewsRepository(NewsContext context)
        {
            this.context = context;
            this.dbSet = this.context.Set<News>();
        }

        protected NewsContext Context
        {
            get
            {
                return this.context;
            }
        }

        protected IDbSet<News> DbSet
        {
            get
            {
                return this.dbSet;
            }
        }

        public News Add(News entity)
        {
            this.ChangeState(entity, EntityState.Added);

            return entity;
        }

        public News Find(int id)
        {
            var news = this.dbSet.Find(id);

            return news;
        }

        public IQueryable<News> All()
        {
            return this.dbSet.AsQueryable();
        }

        public void Delete(News entity)
        {
            this.ChangeState(entity, EntityState.Deleted);
        }

        public void Update(News entity)
        {
            this.ChangeState(entity, EntityState.Modified);
        }

        public void SaveChanges()
        {
            this.context.SaveChanges();
        }

        private void ChangeState(News entity, EntityState state)
        {
            var entry = this.context.Entry(entity);
            this.IsDetached(entity, entry);
            
            entry.State = state;
        }

        private void IsDetached(News entity, DbEntityEntry<News> entry)
        {
            if (entry.State == EntityState.Detached)
            {
                this.dbSet.Attach(entity);
            }
        }
    }
}