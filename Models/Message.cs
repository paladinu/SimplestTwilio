using System.ComponentModel.DataAnnotations;

namespace SimplestTwilio.Models;

public class Message
{
    public int MessageId { get; set; }

    [Required]
    [StringLength(1600, MinimumLength = 1)]
    [Display(Name = "Message Text")]
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    // Navigation properties
    public virtual ICollection<History> Histories { get; set; } = new List<History>();
}
