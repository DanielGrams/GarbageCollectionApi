using Microsoft.EntityFrameworkCore;

namespace GarbageCollectionApi.Models
{
    /// <summary>
    /// Database context
    /// </summary>
    public class GarbageCollectionContext : DbContext
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="options">Options</param>
        public GarbageCollectionContext(DbContextOptions<GarbageCollectionContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Set of towns
        /// </summary>
        public DbSet<Town> Towns { get; set; }
    }
}