using System;
using System.Collections.Generic;
using System.Linq;
using CsCheck;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimplestTwilio.Services;
using Xunit;

namespace SimplestTwilio.Tests;

public class TwilioServiceTests
{
    private ITwilioService CreateService()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Twilio:AccountSid"] = "test",
                ["Twilio:AuthToken"] = "test",
                ["Twilio:PhoneNumber"] = "+1234567890"
            })
            .Build();

        var logger = LoggerFactory.Create(builder => builder.AddConsole())
            .CreateLogger<TwilioService>();

        return new TwilioService(configuration, logger);
    }

    /// <summary>
    /// Feature: sms-messaging, Property 19: SMS segment calculation
    /// Validates: Requirements 10.3, 10.4
    /// 
    /// For any message text, the segment count should follow SMS segmentation rules:
    /// - GSM-7 encoding: 160 characters per segment for single, 153 per segment for multi-segment
    /// - Unicode encoding: 70 characters per segment for single, 67 per segment for multi-segment
    /// - Extended GSM-7 characters count as 2 characters
    /// </summary>
    [Fact]
    public void Property_SmsSegmentCalculation_FollowsSegmentationRules()
    {
        var service = CreateService();

        // Property: Empty message should return 0 segments
        var segments = service.CalculateSmsSegments("");
        Assert.Equal(0, segments);

        // Property: GSM-7 single segment (1-160 chars)
        Gen.Int[1, 160]
            .Select(length => new string('a', length))
            .Sample(message =>
            {
                var segs = service.CalculateSmsSegments(message);
                Assert.Equal(1, segs);
            }, iter: 100);

        // Property: GSM-7 multi-segment follows 153 char rule
        Gen.Int[161, 500]
            .Select(length => new string('a', length))
            .Sample(message =>
            {
                var segs = service.CalculateSmsSegments(message);
                var expectedSegments = (int)Math.Ceiling(message.Length / 153.0);
                Assert.Equal(expectedSegments, segs);
            }, iter: 100);

        // Property: Unicode single segment (1-70 chars with non-GSM chars)
        Gen.Int[1, 70]
            .Select(length => new string('中', length)) // Chinese character
            .Sample(message =>
            {
                var segs = service.CalculateSmsSegments(message);
                Assert.Equal(1, segs);
            }, iter: 100);

        // Property: Unicode multi-segment follows 67 char rule
        Gen.Int[71, 200]
            .Select(length => new string('中', length))
            .Sample(message =>
            {
                var segs = service.CalculateSmsSegments(message);
                var expectedSegments = (int)Math.Ceiling(message.Length / 67.0);
                Assert.Equal(expectedSegments, segs);
            }, iter: 100);

        // Property: Extended GSM-7 characters count as 2
        Gen.Int[1, 80]
            .Sample(count =>
            {
                var message = new string('^', count); // ^ is extended GSM-7
                var segs = service.CalculateSmsSegments(message);
                var charCount = count * 2; // Each ^ counts as 2
                var expectedSegments = charCount <= 160 ? 1 : (int)Math.Ceiling(charCount / 153.0);
                Assert.Equal(expectedSegments, segs);
            }, iter: 100);

        // Property: Mixed GSM-7 and extended characters
        Gen.Int[1, 50]
            .Sample(extendedCount =>
            {
                var regularCount = 50;
                var message = new string('a', regularCount) + new string('{', extendedCount);
                var segs = service.CalculateSmsSegments(message);
                var totalCharCount = regularCount + (extendedCount * 2);
                var expectedSegments = totalCharCount <= 160 ? 1 : (int)Math.Ceiling(totalCharCount / 153.0);
                Assert.Equal(expectedSegments, segs);
            }, iter: 100);
    }

    /// <summary>
    /// Property: Segment calculation is deterministic
    /// For any message, calculating segments multiple times should return the same result
    /// </summary>
    [Fact]
    public void Property_SmsSegmentCalculation_IsDeterministic()
    {
        var service = CreateService();

        Gen.Int[0, 500]
            .Select(length => new string('a', length))
            .Sample(message =>
            {
                var segments1 = service.CalculateSmsSegments(message);
                var segments2 = service.CalculateSmsSegments(message);
                Assert.Equal(segments1, segments2);
            }, iter: 100);
    }

    /// <summary>
    /// Property: Segment count never decreases when adding characters
    /// For any message, adding more characters should never decrease the segment count
    /// </summary>
    [Fact]
    public void Property_SmsSegmentCalculation_MonotonicIncreasing()
    {
        var service = CreateService();

        Gen.Int[0, 300]
            .Select(length => new string('a', length))
            .Sample(message =>
            {
                var segments1 = service.CalculateSmsSegments(message);
                var longerMessage = message + "a";
                var segments2 = service.CalculateSmsSegments(longerMessage);
                Assert.True(segments2 >= segments1, 
                    $"Adding a character decreased segments: {segments1} -> {segments2}");
            }, iter: 100);
    }
}
