# ✅ DEPLOYMENT CHECKLIST - USTAD WHATSAPP INTEGRATION

## 🎯 Pre-Deployment Requirements

### ✅ WhatsApp Business API Setup

- [ ] **Meta Business Account** verified and approved
- [ ] **WhatsApp Business Profile** created and configured
- [ ] **Phone Number** verified and connected to Business API
- [ ] **API Credentials** obtained:
  - [ ] Phone Number ID
  - [ ] Permanent Access Token
  - [ ] Business Account ID (WABA)
  - [ ] Webhook Verify Token (your choice)

### ✅ Infrastructure Requirements

- [ ] **Windows Server 2019+** or Windows 10/11
- [ ] **.NET 8.0 Runtime** installed
- [ ] **SQL Server 2019+** (Express or higher)
- [ ] **IIS** or hosting environment for API service
- [ ] **HTTPS Certificate** for webhook URL (required by WhatsApp)

### ✅ Network Configuration

- [ ] **Firewall Rules** configured for API port (default 5000)
- [ ] **Public Domain** with HTTPS for webhook URL
- [ ] **DNS Configuration** pointing to your server
- [ ] **Load Balancer** setup (if using multiple servers)

## 🗄️ Database Deployment

### ✅ Database Setup

```bash
# 1. Create database and tables
sqlcmd -S localhost -i Setup/WhatsAppIntegration_Setup.sql

# 2. Verify tables created
sqlcmd -S localhost -Q "SELECT name FROM sys.tables WHERE name LIKE 'WhatsApp_%'"

# 3. Check test accounts
sqlcmd -S localhost -d UstadWhatsApp -Q "SELECT UserName, FullName, UserRole FROM WhatsApp_Operators"
```

### ✅ Database Configuration

- [ ] **Connection String** updated in appsettings.json
- [ ] **Backup Strategy** configured
- [ ] **Performance Monitoring** enabled
- [ ] **Index Maintenance** scheduled
- [ ] **Security Permissions** configured

## 🌐 API Service Deployment

### ✅ Build and Publish

```bash
# 1. Navigate to API project
cd ../WhatsAppIntegration/WebhookAPI

# 2. Restore packages
dotnet restore

# 3. Build project
dotnet build -c Release

# 4. Publish for deployment
dotnet publish -c Release -o ./publish
```

### ✅ Configuration

- [ ] **appsettings.json** updated with production values:
  ```json
  {
    "ConnectionStrings": {
      "DefaultConnection": "Production SQL Server connection"
    },
    "Jwt": {
      "Key": "256-bit-secure-random-key-for-production"
    },
    "WhatsApp": {
      "PhoneNumberId": "your-production-phone-id",
      "AccessToken": "your-production-access-token",
      "VerifyToken": "your-secure-verify-token"
    }
  }
  ```

### ✅ IIS Deployment

```powershell
# 1. Create IIS Application Pool
New-WebAppPool -Name "UstadWhatsAppAPI" -Force

# 2. Create IIS Website
New-Website -Name "UstadWhatsAppAPI" -Port 5000 -ApplicationPool "UstadWhatsAppAPI" -PhysicalPath "C:\inetpub\wwwroot\UstadWhatsAppAPI"

# 3. Copy published files
Copy-Item -Path "./publish/*" -Destination "C:\inetpub\wwwroot\UstadWhatsAppAPI" -Recurse -Force

# 4. Start services
Start-WebAppPool -Name "UstadWhatsAppAPI"
Start-Website -Name "UstadWhatsAppAPI"
```

### ✅ Windows Service (Alternative)

```powershell
# Create Windows Service
sc.exe create "UstadWhatsAppAPI" binPath="C:\Apps\UstadWhatsAppAPI\WhatsAppWebhookAPI.exe" start=auto
net start UstadWhatsAppAPI
```

## 🖥️ Desktop Client Deployment

### ✅ Build Client

```bash
# 1. Navigate to client project
cd yesildeftertest

# 2. Restore packages
dotnet restore

# 3. Build for Windows
dotnet build -c Release

# 4. Publish self-contained
dotnet publish -c Release --self-contained -r win-x64
```

### ✅ Client Configuration

- [ ] **appsettings.json** configured with API URL
- [ ] **Registry permissions** verified for settings storage
- [ ] **Network access** confirmed to API service
- [ ] **Desktop shortcuts** created for operators

### ✅ MSI Installer (Optional)

- [ ] **Visual Studio Installer Project** or **WiX Toolset**
- [ ] **Digital Signature** for installer security
- [ ] **Auto-Update** mechanism configured
- [ ] **Uninstall** process tested

## 📱 WhatsApp Business API Configuration

### ✅ Webhook Setup

1. **Meta Business Manager** → WhatsApp → Configuration
2. **Webhook URL**: `https://your-domain.com/webhook`
3. **Verify Token**: Your secure verify token
4. **Subscribe to Events**:
   - [ ] messages
   - [ ] message_status
   - [ ] message_echoes

### ✅ Webhook Testing

```bash
# Test webhook verification
curl "https://your-domain.com/webhook?hub.mode=subscribe&hub.verify_token=your-token&hub.challenge=test123"

# Expected response: "test123"
```

### ✅ Message Templates (If Needed)

- [ ] **Template Categories** defined (Marketing, Utility, Authentication)
- [ ] **Template Content** created and submitted
- [ ] **Template Approval** received from Meta
- [ ] **Template Testing** completed

## 🔒 Security Deployment

### ✅ Production Security

- [ ] **Default Passwords** changed for all test accounts
- [ ] **JWT Secret Key** generated (256-bit minimum)
- [ ] **HTTPS Certificates** installed and configured
- [ ] **SQL Server Security** configured (Windows Auth recommended)
- [ ] **Network Security** firewall rules and VPN access

### ✅ Access Control

- [ ] **Admin Accounts** created for system administrators
- [ ] **Agent Accounts** created for customer service operators
- [ ] **Role Permissions** verified and tested
- [ ] **Account Policies** configured (lockout, password complexity)

## 📧 Notification Services

### ✅ Email Configuration

- [ ] **SMTP Server** configured (Gmail/Office365)
- [ ] **App Passwords** generated for email accounts
- [ ] **Email Templates** customized for your brand
- [ ] **Test Emails** sent and received successfully

### ✅ SMS Configuration

- [ ] **NetGSM Account** created and configured
- [ ] **SMS Header** registered and approved
- [ ] **API Credentials** configured in appsettings.json
- [ ] **Test SMS** sent and received successfully

## 🔧 Testing Checklist

### ✅ Authentication Testing

- [ ] **Valid Login** with test accounts (admin/admin123!, agent/agent123!)
- [ ] **Invalid Login** properly rejected
- [ ] **Account Lockout** after 5 failed attempts
- [ ] **Password Reset** flow working end-to-end
- [ ] **Token Refresh** automatic renewal working

### ✅ WhatsApp Testing

- [ ] **Send Message** from desktop client to test number
- [ ] **Receive Message** webhook processing working
- [ ] **Real-time Updates** SignalR broadcasting working
- [ ] **Media Messages** image/document handling working
- [ ] **Conversation Assignment** admin features working

### ✅ Cross-Platform Testing

- [ ] **Desktop → Web** message synchronization
- [ ] **Web → Mobile** real-time updates
- [ ] **Mobile → Desktop** conversation sync
- [ ] **Multi-Operator** concurrent usage testing

## 📊 Monitoring Setup

### ✅ Application Monitoring

- [ ] **Windows Event Logs** configured for errors
- [ ] **Performance Counters** enabled for monitoring
- [ ] **Health Check** endpoints configured
- [ ] **Alerting** setup for critical failures

### ✅ Database Monitoring

- [ ] **SQL Server Monitoring** enabled
- [ ] **Query Performance** tracking configured
- [ ] **Backup Monitoring** automated backup verification
- [ ] **Space Monitoring** database growth tracking

### ✅ WhatsApp API Monitoring

- [ ] **Webhook Health** monitoring endpoint responses
- [ ] **API Rate Limits** tracking and alerting
- [ ] **Message Delivery** success rate monitoring
- [ ] **Error Rate** tracking and investigation

## 🎯 Go-Live Checklist

### ✅ Final Verification

- [ ] **All Components** deployed and running
- [ ] **All Tests** passing successfully
- [ ] **Documentation** updated and distributed
- [ ] **Training** completed for operators and administrators

### ✅ Launch Sequence

1. [ ] **Database** deployed and verified
2. [ ] **API Service** started and health-checked
3. [ ] **WhatsApp Webhook** verified and subscribed
4. [ ] **Desktop Clients** installed and tested
5. [ ] **Operator Training** completed
6. [ ] **Go-Live** announcement and support ready

### ✅ Post-Launch

- [ ] **Monitor Performance** first 24 hours closely
- [ ] **Support Team** ready for issues
- [ ] **Backup Verification** ensure backups working
- [ ] **User Feedback** collection and analysis

## 🆘 Emergency Procedures

### ✅ Rollback Plan

- [ ] **Database Backup** taken before deployment
- [ ] **Previous Version** available for quick restore
- [ ] **Rollback Scripts** prepared and tested
- [ ] **Communication Plan** for notifying users

### ✅ Support Contacts

- [ ] **Technical Support** contact information distributed
- [ ] **WhatsApp Business Support** contact available
- [ ] **Database Administrator** on standby
- [ ] **Network Administrator** available for connectivity issues

---

## 🎊 Success Criteria

### ✅ Technical Success

- [ ] **99%+ Uptime** for API service
- [ ] **< 2 second** message delivery time
- [ ] **Zero Security** incidents
- [ ] **100% Message** delivery rate

### ✅ Business Success

- [ ] **Operator Satisfaction** with new interface
- [ ] **Customer Response** time improvement
- [ ] **Cross-Platform** usage adoption
- [ ] **ROI Achievement** cost savings and efficiency gains

**🚀 Ready for Production Deployment!**
