using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Avia
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
        public List<Booking> Bookings { get; set; } = new();
    }

    public class Flight
    {
        public int Id { get; set; }
        public string Departure { get; set; } = null!;
        public string Destination { get; set; } = null!;
        public DateTimeOffset Date { get; set; }
        public string Airline { get; set; } = null!;
        public decimal Price { get; set; }
        public int SeatsTotal { get; set; }
        public int SeatsAvailable { get; set; }
        public string BaggageInfo { get; set; } = null!;
        public List<Booking> Bookings { get; set; } = new();
    }

    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int FlightId { get; set; }
        public Flight Flight { get; set; } = null!;
        public DateTimeOffset BookingDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = null!;
        public int SeatsReserved { get; set; }
    }

    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Flight> Flights { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Avia;Username=postgres;Password=vivi5567\r\n");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Flight>()
                .HasMany(f => f.Bookings)
                .WithOne(b => b.Flight)
                .HasForeignKey(b => b.FlightId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable(t => t.HasCheckConstraint(
                    name: "CHK_Booking_Status",
                    sql: "Status IN ('booked', 'paid', 'cancelled')"
                ));

                entity.Property(e => e.Id)
                    .UseIdentityAlwaysColumn();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable(t => t.HasCheckConstraint(
                    name: "CHK_User_Role",
                    sql: "Role IN ('admin', 'client')"
                ));
            });
            modelBuilder.Entity<User>()
                .Property(e => e.Password)
                .HasColumnType("varchar(60)");

            modelBuilder.Entity<User>()
                .HasIndex(e => e.Login)
                .IsUnique();
        }
    }

    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        public static bool VerifyPassword(string enteredPassword, string hashedPassword)
        {
            string hashedEnteredPassword = HashPassword(enteredPassword);
            return string.Equals(hashedEnteredPassword, hashedPassword, StringComparison.Ordinal);
        }
    }

    public interface IAction
    {
        void Execute();
        void Undo();
    }

    public class ActionHistory
    {
        private readonly Stack<IAction> _undoStack = new();
        private readonly Stack<IAction> _redoStack = new();

        public void Execute(IAction action)
        {
            action.Execute();
            _undoStack.Push(action);
            _redoStack.Clear();
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                var action = _undoStack.Pop();
                action.Undo();
                _redoStack.Push(action);
            }
        }

        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                var action = _redoStack.Pop();
                action.Execute();
                _undoStack.Push(action);
            }
        }

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;
    }
    public class AddFlightAction : IAction
    {
        private readonly Flight _flight;
        private readonly AppDbContext _db;

        public AddFlightAction(Flight flight, AppDbContext db)
        {
            _flight = flight;
            _db = db;
        }

        public void Execute()
        {
            _db.Flights.Add(_flight);
            _db.SaveChanges();
            ShowMessage($"Flight {_flight.Departure} - {_flight.Destination} added successfully");
        }

        public void Undo()
        {
            var flight = _db.Flights.FirstOrDefault(f => f.Id == _flight.Id);
            if (flight != null)
            {
                _db.Flights.Remove(flight);
                _db.SaveChanges();
                ShowMessage($"Flight {_flight.Departure} - {_flight.Destination} removed (undo)");
            }
        }
        private void ShowMessage(string message)
        {
            MessageBox.Show(message, "Operation Completed", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    public class DeleteFlightAction : IAction
    {
        private readonly Flight _flight;
        private readonly List<Booking> _relatedBookings;
        private readonly AppDbContext _db;

        public DeleteFlightAction(Flight flight, List<Booking> relatedBookings, AppDbContext db)
        {
            _flight = flight;
            _relatedBookings = relatedBookings;
            _db = db;
        }

        public void Execute()
        {
            _db.Bookings.RemoveRange(_relatedBookings);
            _db.Flights.Remove(_flight);
            _db.SaveChanges();
            ShowMessage($"Flight {_flight.Departure} - {_flight.Destination} and {_relatedBookings.Count} related bookings deleted successfully");
        }

        public void Undo()
        {
            _db.Flights.Add(_flight);
            _db.Bookings.AddRange(_relatedBookings);
            _db.SaveChanges();
            ShowMessage($"Flight {_flight.Departure} - {_flight.Destination} and {_relatedBookings.Count} bookings restored (undo)");
        }
        private void ShowMessage(string message)
        {
            MessageBox.Show(message, "Operation Completed", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
