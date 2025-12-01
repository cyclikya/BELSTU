using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Avia.Data.Entities;

[Table("Users", Schema = "avia")]
public class User
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("Pass")]
    public string Pass { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("PassportNumber")]
    public string PassportNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("LastName")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("FirstName")]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("MiddleName")]
    public string? MiddleName { get; set; }

    [Required]
    [Column("AccessRole")]
    public RoleType AccessRole { get; set; } = RoleType.Client;

    [Required]
    [Column("BirthDate")]
    public DateTime BirthDate { get; set; }

    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("LastLogin")]
    public DateTime? LastLogin { get; set; }

    // Navigation properties
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}

public enum RoleType
{
    Admin,
    Client
}

