using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimplestTwilio.Models;

public class RecipientList
{
    public int RecipientListId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    [Display(Name = "List Name")]
    public string Name { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    // Navigation properties
    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    [NotMapped]
    public int ContactCount => Contacts?.Count ?? 0;
}
