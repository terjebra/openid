using Microsoft.EntityFrameworkCore;
using OpenIddict;

namespace OpenId
{
    public class ApplicationDbContext : OpenIddictDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }


        

    }
}
