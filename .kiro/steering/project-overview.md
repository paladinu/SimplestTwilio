---
inclusion: always
---

# Project Overview

SimplestTwilio is an SMS bulk communications client for managing recipient lists and sending SMS via Twilio accounts.

## Tech Stack

- ASP.NET Core 8.0 MVC with C#
- SQLite with EF Core 8.0
- Razor Views for UI
- ASP.NET Core Identity for auth
- Docker support

## Architecture

**Controllers/** - HTTP request handlers
- `HomeController` - Landing page
- `MessagesController` - Message CRUD
- `ListsController` - Recipient list management
- `HistoryController` - Communication tracking

**Models/** - Entities and view models
- `Message`, `RecipientList`, `History` - Domain entities
- View models for UI presentation

**Views/** - Razor templates organized by controller

## Core Features

- Recipient list management
- Message composition and templates
- Bulk SMS via Twilio
- Communication history tracking

## Design Principles

- Simplicity first - straightforward interface and minimal complexity
- Twilio-powered - all SMS via Twilio API
- User-managed credentials - each user provides their own Twilio account
