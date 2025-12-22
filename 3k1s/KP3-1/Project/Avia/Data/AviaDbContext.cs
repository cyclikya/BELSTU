using Avia.Data.Entities;
using Avia.Data.Converters;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Avia.Data;

public class AviaDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Ticket> Tickets { get; set; }

    public AviaDbContext(DbContextOptions<AviaDbContext> options) : base(options)
    {
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Устанавливаем search_path перед сохранением
        await EnsureSearchPathAsync();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        // Устанавливаем search_path перед сохранением
        EnsureSearchPathAsync().GetAwaiter().GetResult();
        return base.SaveChanges();
    }

    private async Task EnsureSearchPathAsync()
    {
        try
        {
            var connection = Database.GetDbConnection();
            var wasClosed = connection.State == System.Data.ConnectionState.Closed;
            
            if (wasClosed)
            {
                await Database.OpenConnectionAsync();
            }

            try
            {
                var command = connection.CreateCommand();
                command.CommandText = "SET search_path TO avia";
                await command.ExecuteNonQueryAsync();
            }
            finally
            {
                if (wasClosed)
                {
                    await Database.CloseConnectionAsync();
                }
            }
        }
        catch
        {
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Устанавливаем схему по умолчанию
        modelBuilder.HasDefaultSchema("avia");

        // Configure User entity
        // PostgreSQL приводит имена к нижнему регистру, поэтому используем нижний регистр
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users", "avia");
            entity.Property(e => e.UserId).HasColumnName("userid");
            entity.Property(e => e.Pass).HasColumnName("pass");
            entity.Property(e => e.PassportNumber).HasColumnName("passportnumber");
            entity.Property(e => e.LastName).HasColumnName("lastname");
            entity.Property(e => e.FirstName).HasColumnName("firstname");
            entity.Property(e => e.MiddleName).HasColumnName("middlename");
            entity.Property(e => e.AccessRole)
                .HasColumnName("accessrole")
                .HasConversion(
                    v => v.ToString().ToLowerInvariant(),
                    v => Enum.Parse<RoleType>(v, true));
            entity.Property(e => e.BirthDate)
                .HasColumnName("birthdate")
                .HasColumnType("date");
            entity.Property(e => e.CreatedAt)
                .HasColumnName("createdat")
                .HasColumnType("timestamp")
                .HasConversion(
                    v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            entity.Property(e => e.LastLogin)
                .HasColumnName("lastlogin")
                .HasColumnType("timestamp")
                .HasConversion(
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Unspecified) : (DateTime?)null,
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null);
            entity.HasIndex(e => e.PassportNumber).IsUnique();
        });

        // Configure Flight entity
        modelBuilder.Entity<Flight>(entity =>
        {
            entity.ToTable("flights", "avia");
            entity.Property(e => e.FlightId).HasColumnName("flightid");
            entity.Property(e => e.DepartureCity).HasColumnName("departurecity");
            entity.Property(e => e.ArrivalCity).HasColumnName("arrivalcity");
            entity.Property(e => e.DepartureDate).HasColumnName("departuredate");
            entity.Property(e => e.DepartureTime).HasColumnName("departuretime");
            entity.Property(e => e.ArrivalDate).HasColumnName("arrivaldate");
            entity.Property(e => e.ArrivalTime).HasColumnName("arrivaltime");
            entity.Property(e => e.Airline).HasColumnName("airline");
            entity.Property(e => e.EconomyPrice).HasColumnName("economyprice");
            entity.Property(e => e.BusinessPrice).HasColumnName("businessprice");
            entity.Property(e => e.EconomySeats).HasColumnName("economyseats");
            entity.Property(e => e.BusinessSeats).HasColumnName("businessseats");
            entity.Property(e => e.BaggagePrice).HasColumnName("baggageprice");
            entity.HasIndex(e => new { e.DepartureCity, e.ArrivalCity });
        });

        // Configure Ticket entity
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("tickets", "avia");
            entity.Property(e => e.TicketId).HasColumnName("ticketid");
            entity.Property(e => e.UserId).HasColumnName("userid");
            entity.Property(e => e.FlightId).HasColumnName("flightid");
            entity.Property(e => e.ClassType)
                .HasColumnName("classtype")
                .HasConversion(
                    v => v.ToString().ToLowerInvariant(),
                    v => Enum.Parse<ClassType>(v, true));
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasConversion(
                    v => v.ToString().ToLowerInvariant(),
                    v => Enum.Parse<TicketStatus>(v, true));
            entity.Property(e => e.Baggage).HasColumnName("baggage");
            entity.Property(e => e.PurchaseDate).HasColumnName("purchasedate");
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.FlightId);
        });
    }
}

