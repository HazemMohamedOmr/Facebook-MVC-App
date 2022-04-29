using Microsoft.EntityFrameworkCore;

namespace Facebook.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }
    }
}
