using SimplestTwilio.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Exceptions;

namespace SimplestTwilio.Services;

public class TwilioService : ITwilioService
{
    private readonly TwilioConfiguration _config;
    private readonly ILogger<TwilioService> _logger;

    public TwilioService(IConfiguration configuration, ILogger<TwilioService> logger)
    {
        _config = configuration.GetSection("Twilio").Get<TwilioConfiguration>()
                  ?? new TwilioConfiguration();
        _logger = logger;

        if (ValidateConfiguration())
        {
            TwilioClient.Init(_config.AccountSid, _config.AuthToken);
        }
    }

    public bool ValidateConfiguration()
    {
        if (string.IsNullOrWhiteSpace(_config.AccountSid))
            return false;

        if (string.IsNullOrWhiteSpace(_config.AuthToken))
            return false;

        if (string.IsNullOrWhiteSpace(_config.PhoneNumber))
            return false;

        // Validate phone number is in E.164 format
        if (!System.Text.RegularExpressions.Regex.IsMatch(_config.PhoneNumber, @"^\+[1-9]\d{1,14}$"))
            return false;

        return true;
    }

    public async Task<SendResult> SendSmsAsync(string to, string message)
    {
        if (!ValidateConfiguration())
        {
            return new SendResult
            {
                Success = false,
                ErrorMessage = "Twilio configuration is invalid or missing"
            };
        }

        try
        {
            var messageResource = await MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(_config.PhoneNumber),
                to: new Twilio.Types.PhoneNumber(to)
            );

            _logger.LogInformation("SMS sent successfully to ****{LastFour}. SID: {MessageSid}",
                to.Length >= 4 ? to.Substring(to.Length - 4) : "****",
                messageResource.Sid);

            return new SendResult
            {
                Success = true,
                MessageSid = messageResource.Sid
            };
        }
        catch (TwilioException ex)
        {
            _logger.LogError(ex, "Twilio API error sending SMS to ****{LastFour}: {ErrorMessage}",
                to.Length >= 4 ? to.Substring(to.Length - 4) : "****",
                ex.Message);

            return new SendResult
            {
                Success = false,
                ErrorMessage = GetUserFriendlyErrorMessage(ex)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending SMS to ****{LastFour}",
                to.Length >= 4 ? to.Substring(to.Length - 4) : "****");

            return new SendResult
            {
                Success = false,
                ErrorMessage = "An unexpected error occurred while sending the message"
            };
        }
    }

    public async Task<BulkSendResult> SendBulkSmsAsync(List<string> recipients, string message)
    {
        var result = new BulkSendResult
        {
            TotalRecipients = recipients.Count
        };

        foreach (var recipient in recipients)
        {
            var sendResult = await SendSmsAsync(recipient, message);

            if (sendResult.Success)
            {
                result.SuccessfulSends++;
            }
            else
            {
                result.FailedSends++;
                result.Failures.Add(new SendFailure
                {
                    PhoneNumber = recipient,
                    ErrorMessage = sendResult.ErrorMessage ?? "Unknown error"
                });
            }

            // Small delay to respect rate limits
            await Task.Delay(100);
        }

        _logger.LogInformation("Bulk SMS send completed: {Successful} successful, {Failed} failed out of {Total} total",
            result.SuccessfulSends, result.FailedSends, result.TotalRecipients);

        return result;
    }

    public int CalculateSmsSegments(string message)
    {
        if (string.IsNullOrEmpty(message))
            return 0;

        // GSM-7 character set
        const string gsm7Chars = "@£$¥èéùìòÇ\nØø\rÅåΔ_ΦΓΛΩΠΨΣΘΞÆæßÉ !\"#¤%&'()*+,-./0123456789:;<=>?¡ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÑÜ§¿abcdefghijklmnopqrstuvwxyzäöñüà";
        const string gsm7Extended = "^{}\\[~]|€";

        bool isGsm7 = true;
        int charCount = 0;

        foreach (char c in message)
        {
            if (gsm7Chars.Contains(c))
            {
                charCount++;
            }
            else if (gsm7Extended.Contains(c))
            {
                charCount += 2; // Extended characters count as 2
            }
            else
            {
                isGsm7 = false;
                break;
            }
        }

        if (isGsm7)
        {
            // GSM-7 encoding
            if (charCount <= 160)
                return 1;
            else
                return (int)Math.Ceiling(charCount / 153.0); // 153 chars per segment for concatenated messages
        }
        else
        {
            // Unicode encoding
            int unicodeLength = message.Length;
            if (unicodeLength <= 70)
                return 1;
            else
                return (int)Math.Ceiling(unicodeLength / 67.0); // 67 chars per segment for concatenated messages
        }
    }

    private string GetUserFriendlyErrorMessage(TwilioException ex)
    {
        // Return a user-friendly error message based on the exception message
        var message = ex.Message.ToLower();
        
        if (message.Contains("authenticate") || message.Contains("credentials"))
            return "Authentication failed. Please check your Twilio credentials.";
        
        if (message.Contains("phone") || message.Contains("number"))
            return "Invalid phone number format. Please use E.164 format (e.g., +1234567890).";
        
        if (message.Contains("permission"))
            return "Permission denied. Please check your Twilio account permissions.";
        
        if (message.Contains("unverified"))
            return "The phone number is not verified. Please verify it in your Twilio account.";
        
        return $"SMS delivery failed: {ex.Message}";
    }
}
