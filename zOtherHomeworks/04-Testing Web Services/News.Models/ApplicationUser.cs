using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace News.Models
{
    public class ApplicationUser : IdentityUser
    {
        private ICollection<News> ownNews;

        public ApplicationUser()
        {
            this.ownNews = new HashSet<News>();
        }

        public ICollection<News> OwnNews
        {
            get { return this.ownNews; }
            set { this.ownNews = value; }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(
            UserManager<ApplicationUser> manager,
            string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(
                this,
                authenticationType);

            return userIdentity;
        }
    }
}
