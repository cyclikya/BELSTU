using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Avia.Data.Entities;

[Table("Flights", Schema = "avia")]
public class Flight
{
    [Key]
    [Column("FlightID")]
    public int FlightId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("DepartureCity")]
    public string DepartureCity { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("ArrivalCity")]
    public string ArrivalCity { get; set; } = string.Empty;

    [Required]
    [Column("DepartureDate")]
    public DateTime DepartureDate { get; set; }

    [Required]
    [Column("DepartureTime")]
    public TimeSpan DepartureTime { get; set; }

    [Required]
    [Column("ArrivalDate")]
    public DateTime ArrivalDate { get; set; }

    [Required]
    [Column("ArrivalTime")]
    public TimeSpan ArrivalTime { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("Airline")]
    public string Airline { get; set; } = string.Empty;

    [Required]
    [Column("EconomyPrice", TypeName = "decimal(10,2)")]
    public decimal EconomyPrice { get; set; }

    [Required]
    [Column("BusinessPrice", TypeName = "decimal(10,2)")]
    public decimal BusinessPrice { get; set; }

    [Required]
    [Column("EconomySeats")]
    public int EconomySeats { get; set; }

    [Required]
    [Column("BusinessSeats")]
    public int BusinessSeats { get; set; }

    [Column("BaggagePrice", TypeName = "decimal(10,2)")]
    public decimal BaggagePrice { get; set; } = 0;

    // Navigation properties
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    // Computed properties
    public DateTime DepartureDateTime => DepartureDate.Date.Add(DepartureTime);
    public DateTime ArrivalDateTime => ArrivalDate.Date.Add(ArrivalTime);
}

