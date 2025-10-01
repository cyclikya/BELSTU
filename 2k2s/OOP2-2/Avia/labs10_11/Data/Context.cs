using labs10_11.Models;
using Microsoft.EntityFrameworkCore;

namespace labs10_11.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=OOP;Username=postgres;Password=vivi5567");
        }
    }
}
