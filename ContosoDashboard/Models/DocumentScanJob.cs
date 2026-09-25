using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoDashboard.Models;

public class DocumentScanJob
{
    [Key]
    public int DocumentScanJobId { get; set; }

    [Required]
    public int DocumentId { get; set; }

    [MaxLength(255)]
    public string QueueMessageId { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Queued";

    public DateTime? StartedAtUtc { get; set; }

    public DateTime? CompletedAtUtc { get; set; }

    [MaxLength(2000)]
    public string? ScanResultDetails { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(DocumentId))]
    public virtual Document Document { get; set; } = null!;
}
