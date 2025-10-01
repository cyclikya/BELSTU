using Microsoft.EntityFrameworkCore;
using GREPO;


namespace DALMSQLXG
{
    public class Context : DbContext 
    {
        public Context() : base()

        {
            Database.EnsureCreated();
        }


        public DbSet<WSRef>? WSRefs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=80.94.168.145; Initial Catalog=2025_UGORENKO_PP_1; TrustServerCertificate=True; User Id=Student; Password=Pa$$w0rd");

        }
    }
}
