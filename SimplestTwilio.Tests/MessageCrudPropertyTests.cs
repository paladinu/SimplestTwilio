using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsCheck;
using Microsoft.EntityFrameworkCore;
using SimplestTwilio.Data;
using SimplestTwilio.Models;
using Xunit;

namespace SimplestTwilio.Tests;

public class MessageCrudPropertyTests
{
    private ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    /// <summary>
    /// Feature: sms-messaging, Property 3: Message list completeness
    /// Validates: Requirements 2.1, 2.2, 2.3, 2.4
    /// 
    /// For any set of created messages, navigating to the messages index page should display
    /// all messages with their text preview, creation date, and action buttons
    /// </summary>
    [Fact]
    public void Property_MessageListCompleteness_DisplaysAllMessages()
    {
        Gen.Int[0, 20]
            .Sample(messageCount =>
            {
                using var context = CreateInMemoryContext();
                
                // Generate random messages
                var messages = new List<Message>();
                for (int i = 0; i < messageCount; i++)
                {
                    var text = Gen.String[Gen.Char.AlphaNumeric, 1, 1600].Single();
                    messages.Add(new Message
                    {
                        Text = text,
                        CreatedDate = DateTime.UtcNow.AddDays(-i)
                    });
                }

                // Add messages to database
                context.Messages.AddRange(messages);
                context.SaveChanges();

                // Query messages as the Index action does
                var queriedMessages = context.Messages
                    .AsNoTracking()
                    .Include(m => m.Histories)
                    .OrderByDescending(m => m.CreatedDate)
                    .ToList();

                // Property: All created messages should be returned
                Assert.Equal(messageCount, queriedMessages.Count);

                // Property: Each message should have its text, creation date
                foreach (var message in messages)
                {
                    var found = queriedMessages.FirstOrDefault(m => m.MessageId == message.MessageId);
                    Assert.NotNull(found);
                    Assert.Equal(message.Text, found.Text);
                    Assert.Equal(message.CreatedDate, found.CreatedDate);
                }
            }, iter: 100);
    }

    /// <summary>
    /// Feature: sms-messaging, Property 2: Message creation persistence
    /// Validates: Requirements 1.1
    /// 
    /// For any valid message text, when a message is created, querying the database
    /// should return a message with that exact text
    /// </summary>
    [Fact]
    public void Property_MessageCreationPersistence_SavesExactText()
    {
        Gen.String[Gen.Char.AlphaNumeric, 1, 1600]
            .Sample(messageText =>
            {
                using var context = CreateInMemoryContext();

                // Create message
                var message = new Message
                {
                    Text = messageText,
                    CreatedDate = DateTime.UtcNow
                };

                context.Messages.Add(message);
                context.SaveChanges();

                // Query the message back
                var savedMessage = context.Messages
                    .AsNoTracking()
                    .FirstOrDefault(m => m.MessageId == message.MessageId);

                // Property: The saved message should have the exact same text
                Assert.NotNull(savedMessage);
                Assert.Equal(messageText, savedMessage.Text);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: sms-messaging, Property 1: Message text validation consistency
    /// Validates: Requirements 1.2, 3.2
    /// 
    /// For any message text input (create or edit operation), the system should accept
    /// text between 1 and 1600 characters and reject text outside this range
    /// </summary>
    [Fact]
    public void Property_MessageTextValidation_EnforcesLengthConstraints()
    {
        // Property: Valid text lengths (1-1600) should be accepted
        Gen.Int[1, 1600]
            .Select(length => new string('a', length))
            .Sample(text =>
            {
                var message = new Message { Text = text };
                
                // Validate using data annotations
                var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(message);
                var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
                    message, validationContext, validationResults, true);

                Assert.True(isValid, $"Text of length {text.Length} should be valid");
            }, iter: 100);

        // Property: Empty text should be rejected
        var emptyMessage = new Message { Text = "" };
        var emptyContext = new System.ComponentModel.DataAnnotations.ValidationContext(emptyMessage);
        var emptyResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var emptyValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            emptyMessage, emptyContext, emptyResults, true);
        Assert.False(emptyValid, "Empty text should be invalid");

        // Property: Text longer than 1600 should be rejected
        Gen.Int[1601, 2000]
            .Select(length => new string('a', length))
            .Sample(text =>
            {
                var message = new Message { Text = text };
                var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(message);
                var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
                    message, validationContext, validationResults, true);

                Assert.False(isValid, $"Text of length {text.Length} should be invalid");
            }, iter: 100);
    }

    /// <summary>
    /// Feature: sms-messaging, Property 4: Message update persistence
    /// Validates: Requirements 3.1
    /// 
    /// For any existing message and valid new text, when the message is updated,
    /// querying the database should return the message with the updated text
    /// </summary>
    [Fact]
    public void Property_MessageUpdatePersistence_SavesUpdatedText()
    {
        Gen.String[Gen.Char.AlphaNumeric, 1, 1600]
            .Sample(originalText =>
            {
                using var context = CreateInMemoryContext();

                // Create initial message
                var message = new Message
                {
                    Text = originalText,
                    CreatedDate = DateTime.UtcNow
                };

                context.Messages.Add(message);
                context.SaveChanges();
                var messageId = message.MessageId;

                // Generate new text
                var newText = Gen.String[Gen.Char.AlphaNumeric, 1, 1600].Single();

                // Update the message
                var messageToUpdate = context.Messages.Find(messageId);
                Assert.NotNull(messageToUpdate);
                messageToUpdate.Text = newText;
                context.SaveChanges();

                // Query the message back
                var updatedMessage = context.Messages
                    .AsNoTracking()
                    .FirstOrDefault(m => m.MessageId == messageId);

                // Property: The updated message should have the new text
                Assert.NotNull(updatedMessage);
                Assert.Equal(newText, updatedMessage.Text);
                Assert.NotEqual(originalText, updatedMessage.Text);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: sms-messaging, Property 5: Message deletion removes from database
    /// Validates: Requirements 4.1
    /// 
    /// For any message, when it is deleted, querying the database for that message
    /// should return no results
    /// </summary>
    [Fact]
    public void Property_MessageDeletion_RemovesFromDatabase()
    {
        Gen.String[Gen.Char.AlphaNumeric, 1, 1600]
            .Sample(messageText =>
            {
                using var context = CreateInMemoryContext();

                // Create message
                var message = new Message
                {
                    Text = messageText,
                    CreatedDate = DateTime.UtcNow
                };

                context.Messages.Add(message);
                context.SaveChanges();
                var messageId = message.MessageId;

                // Verify message exists
                var existingMessage = context.Messages.Find(messageId);
                Assert.NotNull(existingMessage);

                // Delete the message
                context.Messages.Remove(existingMessage);
                context.SaveChanges();

                // Property: The message should no longer exist in the database
                var deletedMessage = context.Messages
                    .AsNoTracking()
                    .FirstOrDefault(m => m.MessageId == messageId);

                Assert.Null(deletedMessage);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: sms-messaging, Property 6: History preservation after message deletion
    /// Validates: Requirements 4.5
    /// 
    /// For any message with associated history records, deleting the message should
    /// not delete the history records
    /// </summary>
    [Fact]
    public void Property_HistoryPreservation_AfterMessageDeletion()
    {
        Gen.Int[1, 10]
            .Sample(historyCount =>
            {
                using var context = CreateInMemoryContext();

                // Create a recipient list first
                var recipientList = new RecipientList
                {
                    Name = "Test List",
                    CreatedDate = DateTime.UtcNow
                };
                context.RecipientLists.Add(recipientList);
                context.SaveChanges();

                // Create message
                var message = new Message
                {
                    Text = Gen.String[Gen.Char.AlphaNumeric, 1, 1600].Single(),
                    CreatedDate = DateTime.UtcNow
                };

                context.Messages.Add(message);
                context.SaveChanges();
                var messageId = message.MessageId;

                // Create history records
                var historyRecords = new List<History>();
                for (int i = 0; i < historyCount; i++)
                {
                    var history = new History
                    {
                        MessageId = messageId,
                        RecipientListId = recipientList.RecipientListId,
                        SentDate = DateTime.UtcNow.AddDays(-i),
                        TotalRecipients = Gen.Int[1, 100].Single(),
                        SuccessfulSends = Gen.Int[0, 100].Single(),
                        FailedSends = 0,
                        Status = "Completed"
                    };
                    historyRecords.Add(history);
                }

                context.Histories.AddRange(historyRecords);
                context.SaveChanges();

                // Store history IDs before deletion
                var historyIds = historyRecords.Select(h => h.HistoryId).ToList();

                // Verify history records exist
                var existingHistories = context.Histories
                    .Where(h => h.MessageId == messageId)
                    .Count();
                Assert.Equal(historyCount, existingHistories);

                // Delete the message
                var messageToDelete = context.Messages.Find(messageId);
                Assert.NotNull(messageToDelete);
                context.Messages.Remove(messageToDelete);
                context.SaveChanges();

                // Property: History records should still exist (MessageId will be null due to SetNull)
                var remainingHistories = context.Histories
                    .Where(h => historyIds.Contains(h.HistoryId))
                    .ToList();

                Assert.Equal(historyCount, remainingHistories.Count);
                
                // Verify MessageId is null after deletion (due to SetNull cascade)
                foreach (var history in remainingHistories)
                {
                    Assert.Null(history.MessageId);
                }
            }, iter: 100);
    }
}
