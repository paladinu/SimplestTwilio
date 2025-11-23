using System.ComponentModel.DataAnnotations;

namespace SimplestTwilio.Models;

public class History
{
    public int HistoryId { get; set; }

    [Required]
    public int MessageId { get; set; }

    [Required]
    public int RecipientListId { get; set; }

    public DateTime SentDate { get; set; }

    [Required]
    public int TotalRecipients { get; set; }

    [Required]
    public int SuccessfulSends { get; set; }

    [Required]
    public int FailedSends { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = "Completed";

    [StringLength(500)]
    public string? ErrorMessage { get; set; }

    // Navigation properties
    public virtual Message? Message { get; set; }
    public virtual RecipientList? RecipientList { get; set; }
}
