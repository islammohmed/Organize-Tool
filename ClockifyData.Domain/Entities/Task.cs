using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClockifyData.Domain.Entities;

public class Task
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TaskId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [ForeignKey(nameof(Project))]
    public int ProjectId { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal EstimateHours { get; set; }

    // Navigation properties
    public virtual Project Project { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}
