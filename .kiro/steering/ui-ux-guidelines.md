---
inclusion: fileMatch
fileMatchPattern: ['**/*.cshtml', '**/Views/**/*', '**/Controllers/**/*']
---

# UI/UX Guidelines

## Design Principles

- Simplicity first - clean, uncluttered interfaces
- Minimize clicks to complete tasks
- Clear feedback for all actions
- Loading states for async operations
- Helpful error messages
- Confirm destructive actions

## Razor Views

**Organization:**
- One view per action
- Partial views for reusable components
- Strongly-typed models
- `_Layout.cshtml` for consistent structure

**Standard form pattern:**
```cshtml
<form asp-action="Create" method="post">
    <div asp-validation-summary="ModelOnly" class="text-danger"></div>
    
    <div class="form-group">
        <label asp-for="Name" class="control-label"></label>
        <input asp-for="Name" class="form-control" />
        <span asp-validation-for="Name" class="text-danger"></span>
    </div>
    
    <button type="submit" class="btn btn-primary">Save</button>
</form>
```

## Forms

- Clear labels for all inputs
- Group related fields
- Inline validation feedback
- Required field indicators
- Appropriate input types (text, tel, textarea)
- Client-side validation for immediate feedback
- Always validate server-side
- Field-level and summary errors

## Feature-Specific Patterns

**Recipient Lists:**
- Show list name and recipient count prominently
- Quick actions (edit, delete, send)
- Support single and bulk entry
- Real-time phone validation
- Preview before saving
- CSV import support

**Message Composition:**
- Textarea with character counter
- SMS segment count (160 chars/segment)
- Preview before sending
- Auto-save drafts
- Show available recipient lists
- Display recipient count
- Confirm before sending

**History:**
- Chronological display
- Show content, recipients, timestamp
- Success/failure status indicators
- Filtering and search
- Clear visual indicators (icons, colors)
- Retry options for failures

## Responsive Design

- Mobile-friendly layouts
- Bootstrap grid system
- Test on various screen sizes
- Prioritize mobile for key features

## Feedback

**Success:**
- Confirmation after actions
- Toast notifications or alerts
- Auto-dismiss after seconds
- Concise messages

**Errors:**
- Display prominently
- Explain what went wrong
- Suggest corrective actions
- Friendly, non-technical language

**Loading:**
- Spinners for async operations
- Disable buttons during processing
- Progress indicators for bulk operations
- Prevent duplicate submissions

## Accessibility

- Semantic HTML
- Alt text for images
- Keyboard navigation
- Sufficient color contrast
- Proper form labels

## Bootstrap

- Use Bootstrap 5.3 components (loaded via CDN)
- Bootstrap 5 uses `data-bs-*` attributes (not `data-toggle`, `data-target`)
- No jQuery dependency - uses vanilla JavaScript
- Follow Bootstrap conventions
- Consistent styling across pages

## Common Patterns

**List/Index:** Table/card layout, search/filter, pagination, action buttons

**Create/Edit:** Form with validation, Cancel/Save buttons, breadcrumbs, clear titles

**Confirmations:** For destructive actions, clear messaging, Cancel/Confirm options, modals or dedicated pages
