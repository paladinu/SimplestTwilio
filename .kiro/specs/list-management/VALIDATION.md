# Client-Side Validation Approach

## Overview

This project uses **native HTML5 validation** with **Bootstrap 5 styling** instead of jQuery validation. This approach is:
- Lighter weight (no jQuery dependency)
- Modern and standards-based
- Compatible with Bootstrap 5
- Provides immediate user feedback

## Implementation

### 1. Form Attributes

Forms use the `novalidate` attribute to disable browser default validation UI and enable Bootstrap styling:

```html
<form asp-action="Create" method="post" novalidate>
```

### 2. Input Validation Attributes

HTML5 validation attributes are added directly to inputs:

```html
<!-- Required text field with length constraints -->
<input asp-for="Name" class="form-control" 
       required minlength="1" maxlength="100" />

<!-- Phone number with pattern validation -->
<input asp-for="PhoneNumber" type="tel" class="form-control" 
       required pattern="^\+?[1-9]\d{1,14}$" maxlength="20" />
```

### 3. Bootstrap Feedback

Bootstrap's `.invalid-feedback` class provides user-friendly error messages:

```html
<div class="invalid-feedback">
    Please provide a valid phone number starting with + followed by country code and number.
</div>
```

### 4. JavaScript Validation Handler

The `wwwroot/js/site.js` file contains a simple script that:
- Finds all forms with `novalidate` attribute
- Prevents submission if validation fails
- Adds Bootstrap's `was-validated` class to show validation state

```javascript
(function () {
    'use strict';
    const forms = document.querySelectorAll('form[novalidate]');
    Array.from(forms).forEach(function (form) {
        form.addEventListener('submit', function (event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        }, false);
    });
})();
```

## Validation Rules

### List Name
- **Required**: Yes
- **Min Length**: 1 character
- **Max Length**: 100 characters

### Phone Number
- **Required**: Yes
- **Type**: tel
- **Pattern**: `^\+?[1-9]\d{1,14}$` (E.164 format)
- **Max Length**: 20 characters
- **Format**: +[country code][number] (e.g., +1234567890)

### Contact Name
- **Required**: No
- **Max Length**: 100 characters

## Server-Side Validation

Client-side validation is for UX only. All validation is also enforced server-side using:
- Data annotations on models (`[Required]`, `[StringLength]`, `[Phone]`)
- ModelState validation in controllers
- Database constraints

## Benefits

1. **No Dependencies**: No jQuery or additional libraries needed
2. **Fast**: Native browser validation is very performant
3. **Accessible**: Works with screen readers and keyboard navigation
4. **Progressive Enhancement**: Falls back to server validation if JS disabled
5. **Bootstrap Integration**: Seamless styling with Bootstrap 5 components

## Browser Support

HTML5 form validation is supported in all modern browsers:
- Chrome/Edge (latest)
- Firefox (latest)
- Safari (latest)
- Mobile browsers

For older browsers, server-side validation ensures data integrity.
