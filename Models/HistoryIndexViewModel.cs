namespace SimplestTwilio.Models;

public class HistoryIndexViewModel
{
    public List<HistorySummary> Histories { get; set; } = new();
}

public class HistorySummary
{
    public int HistoryId { get; set; }
    public string MessageText { get; set; } = string.Empty;
    public string ListName { get; set; } = string.Empty;
    public DateTime SentDate { get; set; }
    public int TotalRecipients { get; set; }
    public int SuccessfulSends { get; set; }
    public int FailedSends { get; set; }
    public string Status { get; set; } = string.Empty;
}
