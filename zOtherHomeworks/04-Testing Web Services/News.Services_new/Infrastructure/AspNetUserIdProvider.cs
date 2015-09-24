using System.Threading;
using Microsoft.AspNet.Identity;

namespace News.Services_new.Infrastructure
{
    public class AspNetUserIdProvider : IUserIdProvider
    {
        public string GetUserId()
        {
            return Thread.CurrentPrincipal.Identity.GetUserId();
        }
    }
}