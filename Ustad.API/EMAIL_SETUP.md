# Email Configuration Guide

This guide explains how to configure email sending for password reset functionality.

## Quick Start - Free SMTP Providers

### Option 1: Gmail (Free, Easy Setup)

1. **Enable 2-Step Verification** on your Google account
2. **Generate App Password**:
   - Go to Google Account → Security
   - Enable 2-Step Verification (if not already)
   - Under "App passwords", create a new app password for "Mail"
   - Copy the 16-character password
iyhr gqnr baen jlcf

3. **Configure in appsettings.json**:
```json
{
  "Email": {
    "Smtp": {
      "Host": "smtp.gmail.com",
      "Port": 587,
      "Username": "your-email@gmail.com",
      "Password": "your-app-password",
      "UseSsl": true
    },
    "From": {
      "Address": "your-email@gmail.com",
      "Name": "Ustad yesiLdefter"
    },
    "BaseUrl": "https://yourdomain.com"
  }
}
```

### Option 2: SMTP2Go (Free - 1000 emails/month)

1. **Sign up** at https://www.smtp2go.com (free account)
2. **Get SMTP credentials** from dashboard
3. **Configure in appsettings.json**:
```json
{
  "Email": {
    "Smtp": {
      "Host": "mail.smtp2go.com",
      "Port": 587,
      "Username": "your-smtp2go-username",
      "Password": "your-smtp2go-password",
      "UseSsl": true
    },
    "From": {
      "Address": "noreply@yourdomain.com",
      "Name": "Ustad yesiLdefter"
    },
    "BaseUrl": "https://yourdomain.com"
  }
}
```

### Option 3: Brevo/Sendinblue (Free - 300 emails/day)

1. **Sign up** at https://www.brevo.com (free account)
2. **Go to SMTP & API** section
3. **Get SMTP credentials**
4. **Configure in appsettings.json**:
```json
{
  "Email": {
    "Smtp": {
      "Host": "smtp-relay.brevo.com",
      "Port": 587,
      "Username": "your-brevo-username",
      "Password": "your-brevo-smtp-key",
      "UseSsl": true
    },
    "From": {
      "Address": "noreply@yourdomain.com",
      "Name": "Ustad yesiLdefter"
    },
    "BaseUrl": "https://yourdomain.com"
  }
}
```

### Option 4: Mailjet (Free - 200 emails/day)

1. **Sign up** at https://www.mailjet.com (free account)
2. **Get SMTP credentials** from Settings → SMTP
3. **Configure in appsettings.json**:
```json
{
  "Email": {
    "Smtp": {
      "Host": "in-v3.mailjet.com",
      "Port": 587,
      "Username": "your-mailjet-api-key",
      "Password": "your-mailjet-secret-key",
      "UseSsl": true
    },
    "From": {
      "Address": "noreply@yourdomain.com",
      "Name": "Ustad yesiLdefter"
    },
    "BaseUrl": "https://yourdomain.com"
  }
}
```

## Environment Variables (Recommended for Production)

Instead of storing credentials in appsettings.json, use environment variables:

```bash
# Windows
set SMTP_HOST=smtp.gmail.com
set SMTP_PORT=587
set SMTP_USERNAME=your-email@gmail.com
set SMTP_PASSWORD=your-app-password
set SMTP_USE_SSL=true
set EMAIL_FROM=noreply@yourdomain.com
set EMAIL_FROM_NAME=Ustad yesiLdefter
set FRONTEND_URL=https://yourdomain.com

# Linux/Mac
export SMTP_HOST=smtp.gmail.com
export SMTP_PORT=587
export SMTP_USERNAME=your-email@gmail.com
export SMTP_PASSWORD=your-app-password
export SMTP_USE_SSL=true
export EMAIL_FROM=noreply@yourdomain.com
export EMAIL_FROM_NAME=Ustad yesiLdefter
export FRONTEND_URL=https://yourdomain.com
```

## Testing

1. **Start the API** with email configuration
2. **Test password reset** via:
   ```bash
   POST http://localhost:5000/auth/resetPasswordRequest
   {
     "UserName": "user@example.com"
   }
   ```
3. **Check email inbox** for the reset link

## Development Mode

If SMTP credentials are not configured, the system will:
- Still return success (for security - doesn't reveal if user exists)
- Log email sending attempt to debug output
- Not send actual emails (to prevent errors in development)

## Security Notes

- Never commit email credentials to version control
- Use environment variables in production
- The email sending happens asynchronously (doesn't block the API response)
- Always returns success message (security best practice - doesn't reveal user existence)

## Troubleshooting

### Emails not sending
1. Check SMTP credentials are correct
2. Verify firewall allows outbound SMTP (port 587 or 465)
3. Check spam folder
4. For Gmail: Ensure App Password is used (not regular password)

### SSL/TLS errors
- Try setting `UseSsl: false` if your provider doesn't require it
- Verify port number matches your provider's requirements

### Authentication errors
- Double-check username and password
- For Gmail: Must use App Password, not account password
- Ensure 2FA is enabled for Gmail accounts

