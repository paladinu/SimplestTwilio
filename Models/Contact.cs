using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimplestTwilio.Models;

public class Contact
{
    public int ContactId { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^\+[1-9]\d{1,14}$", ErrorMessage = "Phone number must be in E.164 format (e.g., +1234567890)")]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [StringLength(100)]
    [Display(Name = "Contact Name")]
    public string? Name { get; set; }

    [Required]
    public int RecipientListId { get; set; }

    public DateTime CreatedDate { get; set; }

    // Navigation property
    public virtual RecipientList? RecipientList { get; set; }
}
