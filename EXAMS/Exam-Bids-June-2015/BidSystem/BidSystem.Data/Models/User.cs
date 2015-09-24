using System.Collections.Generic;

namespace BidSystem.Data.Models
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class User : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(
            UserManager<User> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

        // TODO

        private ICollection<Bid> bids;
        private ICollection<Offer> offers;


        public User()
        {
            this.bids = new HashSet<Bid>();
            this.offers = new HashSet<Offer>();
        }

        public ICollection<Bid> Bids
        {
            get { return this.bids; }
            set { this.bids = value; }
        }

        public ICollection<Offer> Offers
        {
            get { return this.offers; }
            set { this.offers = value; }
        }
    }
}
