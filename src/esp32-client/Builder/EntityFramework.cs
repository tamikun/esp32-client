using esp32_client.Domain;
using Microsoft.EntityFrameworkCore;

namespace esp32_client.Builder
{
    public class Context : DbContext
    {
        public DbSet<Factory> Factory { get; set; }
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
            Factory = Set<Factory>();
        }
    }
}