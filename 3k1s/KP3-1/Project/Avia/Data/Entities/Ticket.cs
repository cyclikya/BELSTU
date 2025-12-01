using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Avia.Data.Entities;

[Table("Tickets", Schema = "avia")]
public class Ticket
{
    [Key]
    [Column("TicketID")]
    public int TicketId { get; set; }

    [Required]
    [Column("FlightID")]
    public int FlightId { get; set; }

    [Required]
    [Column("UserID")]
    public int UserId { get; set; }

    [Required]
    [Column("ClassType")]
    public ClassType ClassType { get; set; }

    [Column("Baggage")]
    public bool Baggage { get; set; } = false;

    [Column("PurchaseDate")]
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

    [Required]
    [Column("Status")]
    public TicketStatus Status { get; set; } = TicketStatus.Active;

    // Navigation properties
    [ForeignKey(nameof(FlightId))]
    public virtual Flight Flight { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
}

public enum ClassType
{
    Economy,
    Business
}

public enum TicketStatus
{
    Active,
    Cancelled
}

