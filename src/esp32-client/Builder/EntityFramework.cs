using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace esp32_client.Builder
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        { }

        public IQueryable<T> Entity<T>() where T : BaseEntity
        {
            return this.Set<T>().AsQueryable();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Automatically discover and configure entity types that inherit from BaseEntity
            var entityTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.BaseType == typeof(BaseEntity) && !t.IsAbstract);

            foreach (var entityType in entityTypes)
            {
                modelBuilder.Entity(entityType);
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}