using Microsoft.EntityFrameworkCore;

namespace DAL_LES
{
    public class Context : DbContext
    {
        public DbSet<Celebrity> Celebrities { get; set; }
        public DbSet<LifeEvent> LifeEvents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=80.94.168.145; Initial Catalog=2025_UGORENKO_PP_4; TrustServerCertificate=True; User Id=Student; Password=Pa$$w0rd");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LifeEvent>()
                .HasOne(le => le.Celebrity)
                .WithMany(c => c.Lifeevents)
                .HasForeignKey(le => le.CelebrityId);
        }
    }
}