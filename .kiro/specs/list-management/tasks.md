# Implementation Plan

- [x] 1. Create database infrastructure and models


  - Create ApplicationDbContext inheriting from DbContext
  - Create Contact entity model with properties: ContactId, PhoneNumber, Name, RecipientListId, CreatedDate
  - Update RecipientList entity model to include Contacts navigation property and remove Emails property
  - Configure entity relationships in OnModelCreating method
  - Register DbContext in Program.cs with SQL Server connection string
  - _Requirements: 1.1, 2.1, 3.1, 4.1, 7.1_

- [x] 2. Create and apply database migration


  - Generate initial EF Core migration for RecipientList and Contact entities
  - Review migration code to ensure proper table structure and relationships
  - Apply migration to create database schema
  - _Requirements: 1.1, 2.1_

- [x] 3. Implement list index functionality

  - [x] 3.1 Update ListsController Index action to query all recipient lists with contact counts


    - Use async/await with ToListAsync()
    - Use Include() to eager load Contacts for count
    - Pass data to view using RecipientListsViewModel
    - _Requirements: 3.1, 3.2, 3.3_
  
  - [x] 3.2 Create Index.cshtml view to display all lists


    - Display list name, contact count, and created date in a table
    - Add action buttons for Details, Edit, Delete, and Create New
    - Show message when no lists exist
    - Use Bootstrap styling for responsive layout
    - _Requirements: 3.2, 3.3, 3.4, 3.5_

- [x] 4. Implement create list functionality

  - [x] 4.1 Update ListsController Create GET action


    - Return empty view for creating new list
    - _Requirements: 1.1_
  
  - [x] 4.2 Update ListsController Create POST action


    - Validate ModelState for required name field
    - Create new RecipientList with provided name
    - Save to database using async SaveChangesAsync()
    - Redirect to Details page on success
    - Return view with validation errors on failure
    - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5_
  
  - [x] 4.3 Create Create.cshtml view with form


    - Add form with name input field
    - Include validation messages using tag helpers
    - Add Submit and Cancel buttons
    - Use anti-forgery token
    - _Requirements: 1.2, 1.3_

- [x] 5. Implement list details functionality

  - [x] 5.1 Update ListsController Details action


    - Query recipient list by ID with Include() for Contacts
    - Return 404 if list not found
    - Pass data using RecipientListDetailsViewModel
    - _Requirements: 4.1, 4.2, 4.3_
  
  - [x] 5.2 Create Details.cshtml view


    - Display list name and metadata
    - Show table of all contacts with phone numbers and names
    - Add button to add new contact
    - Add remove button for each contact
    - Add Edit List and Back to Lists buttons
    - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5_

- [x] 6. Implement edit list functionality

  - [x] 6.1 Update ListsController Edit GET action


    - Query recipient list by ID
    - Return 404 if not found
    - Pass list to view for editing
    - _Requirements: 5.1_
  
  - [x] 6.2 Update ListsController Edit POST action


    - Validate ModelState for required name field
    - Query existing list and update name property
    - Save changes to database
    - Display confirmation message using TempData
    - Redirect to Details page on success
    - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5_
  
  - [x] 6.3 Create Edit.cshtml view


    - Pre-populate form with current list name
    - Include validation messages
    - Add Submit and Cancel buttons
    - Use anti-forgery token
    - _Requirements: 5.2, 5.3_

- [x] 7. Implement delete list functionality

  - [x] 7.1 Update ListsController Delete GET action


    - Query recipient list by ID with contact count
    - Return 404 if not found
    - Display confirmation page with list details
    - _Requirements: 6.2_
  
  - [x] 7.2 Update ListsController DeleteConfirmed POST action


    - Query recipient list by ID
    - Remove list from database (cascade deletes contacts)
    - Display confirmation message using TempData
    - Redirect to Index page
    - _Requirements: 6.1, 6.3, 6.4_
  
  - [x] 7.3 Create Delete.cshtml view


    - Show list name and contact count
    - Display warning about deleting all contacts
    - Add Confirm Delete and Cancel buttons
    - Use anti-forgery token
    - _Requirements: 6.1, 6.2_

- [x] 8. Implement add contact functionality

  - [x] 8.1 Create ListsController AddContact GET action


    - Accept listId parameter
    - Verify list exists, return 404 if not
    - Return view with empty Contact model
    - _Requirements: 2.1_
  
  - [x] 8.2 Create ListsController AddContact POST action

    - Validate ModelState for phone number format
    - Create new Contact with phone number, optional name, and RecipientListId
    - Save to database
    - Display confirmation message using TempData
    - Redirect to list Details page
    - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 2.6_
  
  - [x] 8.3 Create AddContact.cshtml view or modal


    - Add form with phone number (required) and name (optional) inputs
    - Include validation messages for phone format
    - Add Submit and Cancel buttons
    - Use anti-forgery token
    - _Requirements: 2.2, 2.3, 2.4, 2.5_

- [x] 9. Implement remove contact functionality

  - [x] 9.1 Create ListsController RemoveContact POST action

    - Accept contactId parameter
    - Query contact with Include for RecipientList
    - Return 404 if contact not found
    - Remove contact from database
    - Display confirmation message using TempData
    - Redirect to list Details page
    - _Requirements: 7.1, 7.2, 7.3, 7.4, 7.5_

- [x] 10. Add error handling and user feedback

  - Wrap database operations in try-catch blocks
  - Log errors using ILogger
  - Display user-friendly error messages
  - Add TempData success messages for create, update, delete operations
  - Create shared error view for 404 and other errors
  - _Requirements: All requirements for error scenarios_

- [x] 11. Update navigation and layout



  - Add "Lists" link to main navigation menu
  - Ensure consistent Bootstrap styling across all views
  - Add breadcrumb navigation where appropriate
  - Test responsive design on mobile devices
  - _Requirements: 3.1_
