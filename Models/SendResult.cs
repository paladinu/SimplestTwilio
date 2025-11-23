namespace SimplestTwilio.Models;

public class SendResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? MessageSid { get; set; }
}

public class BulkSendResult
{
    public int TotalRecipients { get; set; }
    public int SuccessfulSends { get; set; }
    public int FailedSends { get; set; }
    public List<SendFailure> Failures { get; set; } = new();
}

public class SendFailure
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string? ContactName { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
