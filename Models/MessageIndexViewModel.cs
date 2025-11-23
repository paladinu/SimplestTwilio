namespace SimplestTwilio.Models;

public class MessageIndexViewModel
{
    public List<MessageSummary> Messages { get; set; } = new();
    public bool TwilioConfigured { get; set; }
}

public class MessageSummary
{
    public int MessageId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string TextPreview { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public int TimesSent { get; set; }
}
