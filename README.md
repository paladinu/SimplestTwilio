# SimplestTwilio

A simple SMS bulk communications client built with ASP.NET Core 8.0 MVC that enables users to manage recipient lists and send SMS messages using their Twilio accounts.

## Features

- **Recipient List Management**: Create and manage lists of SMS recipients
- **Message Composition**: Create and store SMS message templates
- **Bulk SMS Sending**: Send messages to recipient lists via Twilio API
- **Communication History**: Track sent messages and their recipients

## Technology Stack

- **Framework**: ASP.NET Core 8.0 MVC
- **Language**: C# (.NET 8.0)
- **Database**: SQL Server (LocalDB for development)
- **ORM**: Entity Framework Core 8.0
- **SMS Provider**: Twilio API
- **Frontend**: Razor Views with Bootstrap
- **Containerization**: Docker support included

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server or SQL Server LocalDB
- [Twilio Account](https://www.twilio.com/) with Account SID, Auth Token, and Phone Number

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd SimplestTwilio
```

### 2. Configure Twilio Credentials

For development, use .NET User Secrets to store your Twilio credentials securely:

```bash
dotnet user-secrets init
dotnet user-secrets set "Twilio:AccountSid" "your-account-sid"
dotnet user-secrets set "Twilio:AuthToken" "your-auth-token"
dotnet user-secrets set "Twilio:PhoneNumber" "your-twilio-phone-number"
```

For production, use environment variables or Azure Key Vault.

### 3. Update Database Connection String

Edit `appsettings.json` if needed to configure your database connection:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SimplestTwilio;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### 4. Apply Database Migrations

```bash
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet run
```

Navigate to `https://localhost:5001` (or the port shown in the console).

## Docker Support

### Build Docker Image

```bash
docker build -t simplestwilio .
```

### Run Docker Container

```bash
docker run -p 8080:8080 -p 8081:8081 ^
  -e Twilio__AccountSid="your-account-sid" ^
  -e Twilio__AuthToken="your-auth-token" ^
  -e Twilio__PhoneNumber="your-phone-number" ^
  simplestwilio
```

## Project Structure

```
SimplestTwilio/
├── Controllers/          # MVC controllers
│   ├── HomeController.cs
│   ├── MessagesController.cs
│   ├── ListsController.cs
│   └── HistoryController.cs
├── Models/              # Domain entities and view models
│   ├── Message.cs
│   ├── RecipientList.cs
│   ├── History.cs
│   └── ViewModels/
├── Views/               # Razor view templates
│   ├── Home/
│   ├── Messages/
│   ├── Lists/
│   ├── History/
│   └── Shared/
├── wwwroot/            # Static files (CSS, JS, images)
├── Program.cs          # Application entry point
└── appsettings.json    # Configuration
```

## Usage

### Managing Recipient Lists

1. Navigate to **Lists** from the main menu
2. Click **Create New List**
3. Enter a list name and phone numbers (one per line in E.164 format: +1234567890)
4. Save the list

### Sending Messages

1. Navigate to **Messages**
2. Create a new message or select an existing template
3. Choose recipient lists
4. Preview and confirm before sending
5. Messages are sent via Twilio API

### Viewing History

1. Navigate to **History**
2. View all sent messages with timestamps and recipient counts
3. Check delivery status and any error messages

## Phone Number Format

All phone numbers must be in E.164 format:
- Include country code
- No spaces or special characters
- Example: `+12345678901`

## Security Considerations

- **Never commit credentials** to source control
- Use User Secrets for development
- Use environment variables or Key Vault for production
- All forms include CSRF protection
- Input validation on both client and server side
- HTTPS enforced in production

## Development Guidelines

### Coding Standards

- Follow C# naming conventions (PascalCase for classes/methods, camelCase for variables)
- Use async/await for I/O operations
- Keep controllers thin - delegate logic to services
- Use dependency injection for services
- Enable nullable reference types

### Database Migrations

Create a new migration after model changes:

```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Adding New Features

1. Create/update models in `Models/`
2. Add controller actions in `Controllers/`
3. Create corresponding views in `Views/`
4. Register services in `Program.cs`
5. Update database with migrations if needed

## Contributing

This is a simple demonstration project. Feel free to fork and extend it with:
- User authentication and authorization
- Advanced message scheduling
- Message templates with variables
- Delivery status tracking
- Contact import from CSV
- Multi-language support

## License

[Your License Here]

## Support

For Twilio API documentation, visit: https://www.twilio.com/docs/sms

## Acknowledgments

Built with ASP.NET Core 8.0 and Twilio API.
