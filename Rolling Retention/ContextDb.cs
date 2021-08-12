using Microsoft.EntityFrameworkCore;
using Rolling_Retention.Models;

namespace Rolling_Retention
{
    public class ContextDb: DbContext
    {
        public ContextDb(DbContextOptions<ContextDb> options): base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     modelBuilder.ApplyConfiguration(new UserConfigurations());
        //     base.OnModelCreating(modelBuilder);
        // }
        
        public DbSet<User> Users { get; set; }
    }
}