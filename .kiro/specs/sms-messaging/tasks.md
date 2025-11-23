# Implementation Plan

- [x] 1. Update database models and create migration


  - Update Message entity with Text property, validation attributes, and navigation properties
  - Update History entity with tracking fields (SentDate, TotalRecipients, SuccessfulSends, FailedSends, Status, ErrorMessage)
  - Update ApplicationDbContext OnModelCreating to configure Message and History relationships
  - Create EF Core migration for schema changes
  - Apply migration to update database
  - _Requirements: 1.1, 1.2, 7.6, 8.1_

- [ ] 2. Create Twilio service infrastructure
  - [x] 2.1 Create TwilioConfiguration class for credentials


    - Add properties for AccountSid, AuthToken, PhoneNumber
    - _Requirements: 6.1, 6.2_

  - [x] 2.2 Create result models (SendResult, BulkSendResult, SendFailure)


    - Define SendResult with Success, ErrorMessage, MessageSid properties
    - Define BulkSendResult with tracking properties and Failures list
    - Define SendFailure with PhoneNumber, ContactName, ErrorMessage
    - _Requirements: 7.5, 9.3_

  - [x] 2.3 Create ITwilioService interface


    - Define SendSmsAsync method signature
    - Define SendBulkSmsAsync method signature
    - Define ValidateConfiguration method signature
    - Define CalculateSmsSegments method signature
    - _Requirements: 6.1, 7.1, 10.3_

  - [x] 2.4 Implement TwilioService class


    - Implement constructor with IConfiguration and ILogger injection
    - Implement ValidateConfiguration to check all required credentials
    - Implement SendSmsAsync with Twilio API integration and error handling
    - Implement SendBulkSmsAsync with loop, error handling, and result aggregation
    - Implement CalculateSmsSegments with GSM-7 and Unicode detection
    - _Requirements: 6.1, 6.2, 6.4, 7.1, 7.2, 7.4, 9.6, 10.3_

  - [ ] 2.5 Write property test for SMS segment calculation
    - **Property 19: SMS segment calculation**
    - **Validates: Requirements 10.3, 10.4**

  - [x] 2.6 Register TwilioService in Program.cs


    - Add service registration as scoped service
    - _Requirements: 6.1_

- [ ] 3. Implement message CRUD operations
  - [x] 3.1 Update MessagesController with dependencies


    - Add ApplicationDbContext and ITwilioService via constructor injection
    - Add ILogger for error logging
    - _Requirements: 1.1_

  - [x] 3.2 Implement Index action


    - Query all messages with AsNoTracking
    - Count history records per message for "times sent"
    - Check Twilio configuration status
    - Pass data to view using MessageIndexViewModel
    - _Requirements: 2.1, 2.2, 2.3, 2.4_

  - [ ] 3.3 Write property test for message list completeness
    - **Property 3: Message list completeness**
    - **Validates: Requirements 2.1, 2.2, 2.3, 2.4**

  - [x] 3.4 Implement Create GET action


    - Return empty view for creating new message
    - _Requirements: 1.1_

  - [x] 3.5 Implement Create POST action

    - Validate ModelState for message text
    - Create new Message with provided text and current date
    - Save to database using async SaveChangesAsync
    - Add success message to TempData
    - Redirect to Index on success
    - Return view with validation errors on failure
    - _Requirements: 1.1, 1.2, 1.4, 1.5_

  - [ ] 3.6 Write property test for message creation persistence
    - **Property 2: Message creation persistence**
    - **Validates: Requirements 1.1**

  - [ ] 3.7 Write property test for message text validation
    - **Property 1: Message text validation consistency**
    - **Validates: Requirements 1.2, 3.2**

  - [x] 3.8 Implement Edit GET action


    - Query message by ID with AsNoTracking
    - Return 404 if not found
    - Pass message to view
    - _Requirements: 3.1_

  - [x] 3.9 Implement Edit POST action

    - Validate ModelState for message text
    - Query existing message and update Text property
    - Save changes to database
    - Add success message to TempData
    - Redirect to Index on success
    - Return view with validation errors on failure
    - _Requirements: 3.1, 3.2, 3.4, 3.5_

  - [ ] 3.10 Write property test for message update persistence
    - **Property 4: Message update persistence**
    - **Validates: Requirements 3.1**

  - [x] 3.11 Implement Delete GET action


    - Query message by ID with Include for Histories
    - Return 404 if not found
    - Display confirmation page with message details
    - _Requirements: 4.2_

  - [x] 3.12 Implement DeleteConfirmed POST action

    - Query message by ID
    - Remove message from database (preserve history via nullable FK)
    - Add success message to TempData
    - Redirect to Index
    - _Requirements: 4.1, 4.3, 4.4, 4.5_

  - [ ] 3.13 Write property test for message deletion
    - **Property 5: Message deletion removes from database**
    - **Validates: Requirements 4.1**

  - [ ] 3.14 Write property test for history preservation
    - **Property 6: History preservation after message deletion**
    - **Validates: Requirements 4.5**

- [ ] 4. Create message views
  - [x] 4.1 Create Messages/Index.cshtml


    - Display table of all message templates
    - Show text preview (first 100 chars), created date, times sent
    - Add action buttons for Send, Edit, Delete
    - Add Create New button
    - Show warning banner if Twilio not configured
    - Show empty state message when no messages exist
    - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5_

  - [x] 4.2 Create Messages/Create.cshtml


    - Add form with textarea for message text
    - Add character counter showing remaining characters (1600 max)
    - Add SMS segment calculator display
    - Include validation messages
    - Add Submit and Cancel buttons
    - Use anti-forgery token
    - _Requirements: 1.2, 1.3_

  - [x] 4.3 Create Messages/Edit.cshtml


    - Pre-populate form with message text
    - Add character counter and segment calculator
    - Include validation messages
    - Add Submit and Cancel buttons
    - Use anti-forgery token
    - _Requirements: 3.2, 3.3_


  - [x] 4.4 Create Messages/Delete.cshtml

    - Show message text preview
    - Display warning about preserving history
    - Add Confirm Delete and Cancel buttons
    - Use anti-forgery token
    - _Requirements: 4.2_

- [ ] 5. Implement message sending workflow
  - [x] 5.1 Create SendMessageViewModel and related view models


    - Create SendMessageViewModel with message and list selection properties
    - Create RecipientListOption for list display
    - Create SendResultViewModel for result display
    - _Requirements: 5.1, 5.3, 7.5_

  - [x] 5.2 Implement Send GET action in MessagesController


    - Query message by ID
    - Return 404 if not found
    - Query all recipient lists with contact counts
    - Pass data to view using SendMessageViewModel
    - _Requirements: 5.1, 5.3_

  - [ ] 5.3 Write property test for recipient list display
    - **Property 7: Recipient list display completeness**
    - **Validates: Requirements 5.1, 5.3**

  - [x] 5.4 Implement SendConfirm POST action

    - Validate at least one list selected
    - Query selected lists with Include for Contacts
    - Calculate total recipients across all lists
    - Calculate SMS segments for message
    - Calculate total SMS count (recipients × segments)
    - Pass data to confirmation view
    - _Requirements: 5.2, 5.4, 10.1, 10.2, 10.3, 10.4_

  - [ ] 5.5 Write property test for list selection validation
    - **Property 8: List selection validation**
    - **Validates: Requirements 5.4**

  - [ ] 5.6 Write property test for recipient count calculation
    - **Property 18: Recipient count calculation**
    - **Validates: Requirements 10.1, 10.2**

  - [x] 5.7 Implement SendExecute POST action

    - Validate Twilio configuration
    - Query selected lists with Include for Contacts
    - Collect all phone numbers from all selected lists
    - Call TwilioService.SendBulkSmsAsync with message and recipients
    - Create History record for each list with send results
    - Save history records to database
    - Pass results to result view
    - _Requirements: 6.5, 7.1, 7.3, 7.4, 7.5, 7.6_

  - [ ] 5.8 Write property test for bulk send completeness
    - **Property 10: Bulk send completeness**
    - **Validates: Requirements 7.1**

  - [ ] 5.9 Write property test for phone validation before send
    - **Property 11: Phone number validation before send**
    - **Validates: Requirements 7.4**

  - [ ] 5.10 Write property test for send result accuracy
    - **Property 12: Send result summary accuracy**
    - **Validates: Requirements 7.5**

  - [ ] 5.11 Write property test for history record creation
    - **Property 13: History record creation**
    - **Validates: Requirements 7.6**

  - [ ] 5.12 Write property test for send resilience
    - **Property 17: Send resilience**
    - **Validates: Requirements 9.6**

  - [ ] 5.13 Write property test for failure identification
    - **Property 16: Send failure identification**
    - **Validates: Requirements 9.3**

- [ ] 6. Create sending views
  - [x] 6.1 Create Messages/Send.cshtml


    - Display message text in read-only format
    - Show checkboxes for selecting recipient lists
    - Display contact count for each list
    - Show calculated total recipients
    - Add Next button to proceed to confirmation
    - Add Cancel button
    - Use anti-forgery token
    - _Requirements: 5.1, 5.2, 5.3, 5.4_

  - [x] 6.2 Create Messages/SendConfirm.cshtml


    - Display message text
    - Show selected list names
    - Display total recipients count
    - Display SMS segments per message
    - Display total SMS count (recipients × segments)
    - Add Confirm Send button
    - Add Back button
    - Use anti-forgery token
    - _Requirements: 10.1, 10.2, 10.3, 10.4, 10.5_

  - [x] 6.3 Create Messages/SendResult.cshtml


    - Display success/failure summary with counts
    - Show list of selected lists
    - Display table of failures with phone number, name, and error
    - Add Back to Messages button
    - _Requirements: 7.5, 9.3_

- [ ] 7. Implement history tracking
  - [x] 7.1 Create HistoryIndexViewModel and HistorySummary


    - Define view models for history display
    - _Requirements: 8.1_

  - [x] 7.2 Update HistoryController Index action


    - Query all history records with Include for Message and RecipientList
    - Use AsNoTracking for read-only query
    - Order by SentDate descending
    - Pass data to view using HistoryIndexViewModel
    - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5, 8.6_

  - [ ] 7.3 Write property test for history display completeness
    - **Property 14: History display completeness**
    - **Validates: Requirements 8.1, 8.2, 8.3, 8.4, 8.5**

  - [ ] 7.4 Write property test for history ordering
    - **Property 15: History ordering**
    - **Validates: Requirements 8.6**

  - [x] 7.5 Update HistoryController Details action

    - Query history by ID with Include for Message and RecipientList
    - Return 404 if not found
    - Pass detailed history to view
    - _Requirements: 8.1_

- [ ] 8. Create history views
  - [x] 8.1 Update History/Index.cshtml


    - Display table of all sent messages
    - Show message preview, list names, sent date, status
    - Show success/failure counts
    - Add link to view details for each entry
    - Add filter/sort options
    - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5, 8.6_

  - [x] 8.2 Create History/Details.cshtml


    - Display full message text
    - Show recipient list details
    - Display detailed send statistics
    - Show error message if any
    - Add Back to History button
    - _Requirements: 8.1_

- [ ] 9. Add client-side enhancements
  - Add JavaScript for character counter on message forms
  - Add JavaScript for SMS segment calculator
  - Add JavaScript for recipient count calculator on send page
  - Add JavaScript for select all/none checkboxes on send page
  - Add loading indicators for send operations
  - _Requirements: 1.2, 5.2, 10.1, 10.3_

- [x] 10. Configure Twilio credentials


  - Document user secrets setup in README
  - Add appsettings.json structure for Twilio section
  - Create example configuration in appsettings.Development.json
  - Add validation error messages for missing configuration
  - _Requirements: 6.1, 6.2, 6.3_

- [x] 11. Add error handling and logging


  - Wrap all database operations in try-catch blocks
  - Add comprehensive logging for Twilio API calls
  - Add user-friendly error messages for common scenarios
  - Implement proper exception handling in TwilioService
  - Add TempData messages for all user actions
  - _Requirements: 9.1, 9.2, 9.4, 9.5_

- [x] 12. Update navigation and layout


  - Ensure Messages link exists in main navigation
  - Ensure History link exists in main navigation
  - Add breadcrumb navigation where appropriate
  - Ensure consistent Bootstrap styling across all views
  - Test responsive design on mobile devices
  - _Requirements: 2.1, 8.1_

- [ ] 13. Final checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise
