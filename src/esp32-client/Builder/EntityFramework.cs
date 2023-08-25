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

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     optionsBuilder.UseSqlite("Data Source=hello.db;");
        // }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     base.OnModelCreating(modelBuilder);

        // }


        // public async Task<List<Factory>> GetListFactory()
        // {
        //     return await Factory.ToListAsync();
        // }
    }
}