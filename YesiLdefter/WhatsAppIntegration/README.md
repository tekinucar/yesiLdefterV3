# 🚀 USTAD DESKTOP WHATSAPP INTEGRATION

A comprehensive WhatsApp Business Cloud API integration for the Ustad Desktop educational management system. This solution provides multi-operator customer service capabilities with real-time messaging, role-based access control, and enterprise-grade security.

## 📋 Features

### 🔐 Authentication & Security

- **PBKDF2 Password Hashing** with 200,000+ iterations
- **JWT Token Authentication** with automatic refresh
- **Account Lockout Protection** (5 failed attempts = 15min lockout)
- **Password Reset System** via SMS and Email
- **Role-Based Access Control** (Admin/Agent roles)
- **Comprehensive Audit Logging** for compliance

### 💬 WhatsApp Integration

- **WhatsApp Business Cloud API** v20.0+ support
- **Real-time Message Handling** via SignalR
- **Multi-operator Support** with conversation assignment
- **Media Support** (images, documents, audio, video)
- **Template Messages** for 24h+ customer outreach
- **Message Status Tracking** (sent, delivered, read)

### 🖥️ Windows Forms Client

- **DevExpress Integration** for consistent UI
- **Real-time Notifications** for incoming messages
- **Conversation Management** with assignment and status tracking
- **Message History** with search and filtering
- **Admin Panel** for operator and conversation management
- **Multi-language Support** (Turkish primary, extensible)

### 📱 Cross-Platform Ready

- **Database Schema** compatible with web and mobile versions
- **REST API Endpoints** for integration with Next.js web app
- **SignalR Hub** for real-time updates across platforms
- **Shared Business Logic** for consistent behavior

## 🏗️ Architecture

```
┌─────────────────────┐    ┌──────────────────────┐    ┌─────────────────────┐
│   Windows Forms     │    │   ASP.NET Core       │    │   WhatsApp Cloud    │
│   Client            │◄──►│   Webhook API        │◄──►│   API               │
│   (Desktop)         │    │   + SignalR Hub      │    │   (Meta)            │
└─────────────────────┘    └──────────────────────┘    └─────────────────────┘
           │                           │                           │
           │                           │                           │
           ▼                           ▼                           ▼
┌─────────────────────┐    ┌──────────────────────┐    ┌─────────────────────┐
│   Local Storage     │    │   SQL Server         │    │   SMS/Email         │
│   (Settings)        │    │   Database           │    │   Services          │
└─────────────────────┘    └──────────────────────┘    └─────────────────────┘
```

## 🗄️ Database Schema

### Core Tables

- **WhatsApp_Operators**: User accounts with PBKDF2 security
- **WhatsApp_Conversations**: Customer conversation tracking
- **WhatsApp_Messages**: Complete message history
- **WhatsApp_AuditLogs**: Security and compliance logging
- **WhatsApp_Templates**: WhatsApp message templates
- **WhatsApp_FirmSettings**: Multi-company configuration

### Key Features

- **Multi-company Support** via FirmId
- **Foreign Key Relationships** for data integrity
- **Optimized Indexes** for performance
- **Stored Procedures** for complex operations

## 🚀 Quick Start

### Prerequisites

- Windows 10/11 or Windows Server 2019+
- .NET 8.0 Runtime
- SQL Server 2019+ (Express or higher)
- Visual Studio 2022 (for development)
- WhatsApp Business Account with Cloud API access

### 1. Database Setup

```sql
-- Run the database schema script
sqlcmd -S localhost -i Database/WhatsAppIntegration_Schema.sql

-- Create your first admin user (replace with secure password)
EXEC sp_WhatsApp_CreateOperator
    @FirmId = 1,
    @UserName = 'admin',
    @FullName = 'System Administrator',
    @Email = 'admin@yourcompany.com',
    @Phone = '905551234567',
    @Role = 'Admin',
    @Password = 'YourSecurePassword123!';
```

### 2. WhatsApp Business API Setup

1. **Create WhatsApp Business Account**

   - Go to [Facebook Business](https://business.facebook.com)
   - Create a business account and verify your business
   - Add WhatsApp Business product

2. **Get API Credentials**

   - Phone Number ID
   - Access Token (permanent token recommended)
   - Webhook Verify Token (create your own secure string)
   - Business Account ID (WABA ID)

3. **Configure Webhook**
   - Webhook URL: `https://your-domain.com/webhook`
   - Verify Token: Your secure verify token
   - Subscribe to: messages, message_status

### 3. API Configuration

Edit `WhatsAppIntegration/WebhookAPI/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=UstadWhatsApp;Trusted_Connection=True;"
  },
  "Jwt": {
    "Key": "your-secure-256-bit-key-minimum-32-characters",
    "ExpiresMinutes": "480"
  },
  "WhatsApp": {
    "PhoneNumberId": "your-phone-number-id",
    "AccessToken": "your-access-token",
    "VerifyToken": "your-verify-token"
  },
  "Email": {
    "SmtpHost": "smtp.office365.com",
    "SmtpUsername": "your-email@domain.com",
    "SmtpPassword": "your-app-password"
  },
  "Sms": {
    "NetGsm": {
      "Username": "your-netgsm-username",
      "Password": "your-netgsm-password",
      "Header": "YOUR-SMS-HEADER"
    }
  }
}
```

### 4. Deploy Webhook API

```bash
# Build the API
cd WhatsAppIntegration/WebhookAPI
dotnet publish -c Release -o publish

# Deploy to IIS or run as service
dotnet WhatsAppWebhookAPI.dll
```

### 5. Build Windows Forms Client

```bash
# Build the client
cd WhatsAppIntegration/WinFormsClient
dotnet build -c Release

# Create installer (optional)
# Use Visual Studio Installer Projects or ClickOnce
```

## 🔧 Configuration

### Security Settings

```json
{
  "Security": {
    "MaxFailedLoginAttempts": 5,
    "LockoutMinutes": 15,
    "PasswordResetTokenExpiryHours": 1,
    "JwtRefreshIntervalMinutes": 60
  }
}
```

### SMS/Email Integration

#### Gmail Setup

1. Enable 2-Factor Authentication
2. Generate App Password
3. Use app password in configuration

#### Office365 Setup

1. Enable SMTP AUTH for your mailbox
2. Use your regular password or app password
3. Configure SMTP settings

#### NetGSM Setup

1. Create NetGSM account
2. Get API credentials
3. Register SMS header/sender name
4. Configure in appsettings.json

## 📱 Usage

### For Operators (Agents)

1. **Login**

   - Launch WhatsApp client
   - Enter username/password
   - System connects to SignalR hub

2. **Handle Messages**

   - Incoming messages appear in real-time
   - Click conversation to view history
   - Type response and press Enter
   - Messages sync across all operator screens

3. **Manage Conversations**
   - View assigned conversations
   - Update conversation status
   - Access message history
   - Handle media attachments

### For Administrators

1. **Operator Management**

   - Create/edit operator accounts
   - Assign roles (Admin/Agent)
   - Reset passwords
   - Monitor login activity

2. **Conversation Management**

   - Assign conversations to operators
   - Monitor conversation status
   - View system-wide message statistics
   - Generate reports

3. **System Monitoring**
   - View audit logs
   - Monitor API usage
   - Check system health
   - Manage WhatsApp templates

## 🔒 Security Best Practices

### Password Security

- Minimum 8 characters with complexity requirements
- PBKDF2 hashing with 200,000+ iterations
- Unique salt per password
- Timing-safe comparison to prevent timing attacks

### Access Control

- Role-based permissions (Admin/Agent)
- JWT tokens with configurable expiration
- Automatic token refresh
- Session management and cleanup

### Audit & Compliance

- Complete audit trail of all actions
- IP address and user agent logging
- Failed login attempt tracking
- Data retention policies

### Network Security

- HTTPS required for all communications
- SignalR over secure WebSocket
- API rate limiting and throttling
- CORS policies for web integration

## 🚨 Troubleshooting

### Common Issues

#### Connection Problems

```
Error: SignalR connection failed
Solution: Check API URL and JWT token validity
```

#### WhatsApp API Errors

```
Error: 401 Unauthorized
Solution: Verify Access Token and Phone Number ID
```

#### Database Connection Issues

```
Error: Cannot connect to SQL Server
Solution: Check connection string and SQL Server service
```

### Debugging

1. **Enable Debug Logging**

   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Debug",
         "Microsoft.AspNetCore.SignalR": "Debug"
       }
     }
   }
   ```

2. **Check Event Logs**

   - Windows Event Viewer
   - Application logs
   - SQL Server logs

3. **Test API Endpoints**

   ```bash
   # Test webhook verification
   curl "http://localhost:5000/webhook?hub.mode=subscribe&hub.verify_token=your-token&hub.challenge=test"

   # Test login
   curl -X POST http://localhost:5000/auth/login \
     -H "Content-Type: application/json" \
     -d '{"UserName":"admin","Password":"password"}'
   ```

## 📊 Performance Optimization

### Database

- Regular index maintenance
- Query optimization
- Connection pooling
- Partition large tables by date

### SignalR

- Use sticky sessions for scale-out
- Configure message buffer sizes
- Monitor connection counts
- Implement backpressure handling

### API

- Enable response caching
- Use compression
- Implement rate limiting
- Monitor memory usage

## 🔄 Integration with Existing Systems

### Web Application (Next.js)

- Shared database schema
- REST API endpoints for data access
- WebSocket connections for real-time updates
- Consistent authentication tokens

### Mobile Application (React Native)

- Same API endpoints
- Push notifications for messages
- Offline message queuing
- Synchronized conversation state

### Ustad Desktop Integration

- Shared business logic components
- Common configuration management
- Integrated user management
- Consistent UI/UX patterns

## 📈 Monitoring & Analytics

### Key Metrics

- Message volume and response times
- Operator productivity metrics
- Customer satisfaction scores
- System uptime and performance

### Reporting

- Daily/weekly/monthly reports
- Conversation analytics
- Operator performance reports
- System health dashboards

## 🛠️ Development

### Prerequisites

- Visual Studio 2022
- .NET 8.0 SDK
- SQL Server Developer Edition
- DevExpress WinForms (licensed)

### Build Process

```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Publish for deployment
dotnet publish -c Release
```

### Contributing

1. Fork the repository
2. Create feature branch
3. Follow coding standards
4. Add unit tests
5. Submit pull request

## 📝 License

This project is part of the Ustad Desktop educational management system.
© 2024 Ustad Yazılım. All rights reserved.

## 🆘 Support

For technical support and questions:

- Email: support@ustad.com
- Documentation: [Internal Wiki]
- Issue Tracking: [Internal System]

---

**Note**: This is an enterprise-grade solution designed for production use. Ensure proper security measures, regular backups, and monitoring are in place before deploying to production environments.
