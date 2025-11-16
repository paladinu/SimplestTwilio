namespace SimplestTwilio.Models;

public class RecipientListsViewModel
{
    public List<RecipientListSummary> Lists { get; set; } = new();
}

public class RecipientListSummary
{
    public int RecipientListId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ContactCount { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class RecipientListDetailsViewModel
{
    public int RecipientListId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<ContactSummary> Contacts { get; set; } = new();
}

public class ContactSummary
{
    public int ContactId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Name { get; set; }
}
