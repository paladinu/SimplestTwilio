using System.ComponentModel.DataAnnotations;

namespace SimplestTwilio.Models;

public class SendMessageViewModel
{
    public int MessageId { get; set; }
    public string MessageText { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select at least one recipient list")]
    public List<int> SelectedListIds { get; set; } = new();

    public List<RecipientListOption> AvailableLists { get; set; } = new();

    // Calculated properties
    public int TotalRecipients { get; set; }
    public int SmsSegments { get; set; }
    public int TotalSmsCount { get; set; }
}

public class RecipientListOption
{
    public int RecipientListId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ContactCount { get; set; }
    public bool IsSelected { get; set; }
}

public class SendResultViewModel
{
    public string MessageText { get; set; } = string.Empty;
    public int TotalRecipients { get; set; }
    public int SuccessfulSends { get; set; }
    public int FailedSends { get; set; }
    public List<string> ListNames { get; set; } = new();
    public List<SendFailure> Failures { get; set; } = new();
}
