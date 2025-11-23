using SimplestTwilio.Models;

namespace SimplestTwilio.Services;

public interface ITwilioService
{
    Task<SendResult> SendSmsAsync(string to, string message);
    Task<BulkSendResult> SendBulkSmsAsync(List<string> recipients, string message);
    bool ValidateConfiguration();
    int CalculateSmsSegments(string message);
}
