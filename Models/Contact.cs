using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimplestTwilio.Models;

public class Contact
{
    public int ContactId { get; set; }

    [Required]
    [Phone]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [StringLength(100)]
    [Display(Name = "Contact Name")]
    public string? Name { get; set; }

    [Required]
    public int RecipientListId { get; set; }

    public DateTime CreatedDate { get; set; }

    // Navigation property
    public virtual RecipientList RecipientList { get; set; } = null!;
}
