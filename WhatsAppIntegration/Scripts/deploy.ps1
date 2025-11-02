# Ustad Desktop WhatsApp Integration - Deployment Script
# PowerShell script for automated deployment on Windows

param(
    [Parameter(Mandatory=$false)]
    [string]$Environment = "Development",
    
    [Parameter(Mandatory=$false)]
    [string]$ConnectionString = "",
    
    [Parameter(Mandatory=$false)]
    [string]$ApiUrl = "http://localhost:5000",
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipDatabase,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipBuild
)

Write-Host "🚀 Ustad Desktop WhatsApp Integration Deployment" -ForegroundColor Green
Write-Host "Environment: $Environment" -ForegroundColor Yellow
Write-Host "API URL: $ApiUrl" -ForegroundColor Yellow

# Configuration
$ScriptPath = Split-Path -Parent $MyInvocation.MyCommand.Definition
$RootPath = Split-Path -Parent $ScriptPath
$DatabasePath = Join-Path $RootPath "Database"
$ApiPath = Join-Path $RootPath "WebhookAPI"
$ClientPath = Join-Path $RootPath "WinFormsClient"

# Check prerequisites
Write-Host "📋 Checking prerequisites..." -ForegroundColor Cyan

# Check .NET 8.0
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET version: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ .NET 8.0 SDK not found. Please install .NET 8.0 SDK" -ForegroundColor Red
    exit 1
}

# Check SQL Server connectivity
if (-not $SkipDatabase) {
    Write-Host "🗄️ Checking database connectivity..." -ForegroundColor Cyan
    
    if ([string]::IsNullOrEmpty($ConnectionString)) {
        $ConnectionString = "Server=localhost;Database=UstadWhatsApp;Trusted_Connection=True;TrustServerCertificate=true;"
    }
    
    try {
        $connection = New-Object System.Data.SqlClient.SqlConnection($ConnectionString)
        $connection.Open()
        $connection.Close()
        Write-Host "✅ Database connection successful" -ForegroundColor Green
    } catch {
        Write-Host "❌ Database connection failed: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "Please check your SQL Server installation and connection string" -ForegroundColor Yellow
        exit 1
    }
}

# Build projects
if (-not $SkipBuild) {
    Write-Host "🔨 Building projects..." -ForegroundColor Cyan
    
    # Build Security project
    Write-Host "Building Security library..." -ForegroundColor Yellow
    Set-Location (Join-Path $RootPath "Security")
    dotnet build -c Release
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Security project build failed" -ForegroundColor Red
        exit 1
    }
    
    # Build API project
    Write-Host "Building API library..." -ForegroundColor Yellow
    Set-Location (Join-Path $RootPath "API")
    dotnet build -c Release
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ API project build failed" -ForegroundColor Red
        exit 1
    }
    
    # Build Webhook API
    Write-Host "Building Webhook API..." -ForegroundColor Yellow
    Set-Location $ApiPath
    dotnet build -c Release
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Webhook API build failed" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "✅ All projects built successfully" -ForegroundColor Green
}

# Database deployment
if (-not $SkipDatabase) {
    Write-Host "🗄️ Deploying database..." -ForegroundColor Cyan
    
    $SchemaScript = Join-Path $DatabasePath "WhatsAppIntegration_Schema.sql"
    
    if (Test-Path $SchemaScript) {
        try {
            # Use sqlcmd to execute schema script
            sqlcmd -S "localhost" -d "master" -i $SchemaScript -v ConnectionString="$ConnectionString"
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "✅ Database schema deployed successfully" -ForegroundColor Green
            } else {
                Write-Host "⚠️ Database schema deployment completed with warnings" -ForegroundColor Yellow
            }
        } catch {
            Write-Host "❌ Database deployment failed: $($_.Exception.Message)" -ForegroundColor Red
            exit 1
        }
    } else {
        Write-Host "❌ Database schema script not found at: $SchemaScript" -ForegroundColor Red
        exit 1
    }
}

# Deploy Webhook API
Write-Host "🌐 Deploying Webhook API..." -ForegroundColor Cyan

Set-Location $ApiPath

# Publish API
$PublishPath = Join-Path $ApiPath "publish"
dotnet publish -c Release -o $PublishPath

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ API publish failed" -ForegroundColor Red
    exit 1
}

# Update configuration
$ConfigPath = Join-Path $PublishPath "appsettings.json"
if (Test-Path $ConfigPath) {
    $config = Get-Content $ConfigPath | ConvertFrom-Json
    
    # Update connection string if provided
    if (-not [string]::IsNullOrEmpty($ConnectionString)) {
        $config.ConnectionStrings.DefaultConnection = $ConnectionString
    }
    
    # Update environment-specific settings
    if ($Environment -eq "Production") {
        $config.Logging.LogLevel.Default = "Warning"
        $config.Logging.LogLevel."Microsoft.AspNetCore" = "Warning"
    }
    
    $config | ConvertTo-Json -Depth 10 | Set-Content $ConfigPath
    Write-Host "✅ Configuration updated for $Environment environment" -ForegroundColor Green
}

Write-Host "✅ Webhook API deployed to: $PublishPath" -ForegroundColor Green

# Create Windows Service (optional)
$CreateService = Read-Host "Create Windows Service for Webhook API? (y/N)"
if ($CreateService -eq "y" -or $CreateService -eq "Y") {
    Write-Host "🔧 Creating Windows Service..." -ForegroundColor Cyan
    
    $ServiceName = "UstadWhatsAppWebhookAPI"
    $ServiceDisplayName = "Ustad WhatsApp Webhook API"
    $ServiceDescription = "WhatsApp Business API webhook service for Ustad Desktop"
    $ExePath = Join-Path $PublishPath "WhatsAppWebhookAPI.exe"
    
    # Stop service if it exists
    try {
        Stop-Service -Name $ServiceName -Force -ErrorAction SilentlyContinue
        Start-Sleep -Seconds 2
    } catch {
        # Service doesn't exist, which is fine
    }
    
    # Remove existing service
    try {
        sc.exe delete $ServiceName | Out-Null
    } catch {
        # Service doesn't exist, which is fine
    }
    
    # Create new service
    New-Service -Name $ServiceName -BinaryPathName $ExePath -DisplayName $ServiceDisplayName -Description $ServiceDescription -StartupType Automatic
    
    Write-Host "✅ Windows Service '$ServiceName' created" -ForegroundColor Green
    Write-Host "   Use 'net start $ServiceName' to start the service" -ForegroundColor Yellow
}

# Build Windows Forms Client
Write-Host "🖥️ Building Windows Forms Client..." -ForegroundColor Cyan

if (Test-Path $ClientPath) {
    Set-Location $ClientPath
    dotnet build -c Release
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Windows Forms Client built successfully" -ForegroundColor Green
        
        $ClientExe = Join-Path $ClientPath "bin\Release\net8.0-windows\WhatsAppWinFormsClient.exe"
        if (Test-Path $ClientExe) {
            Write-Host "   Client executable: $ClientExe" -ForegroundColor Yellow
        }
    } else {
        Write-Host "⚠️ Windows Forms Client build failed" -ForegroundColor Yellow
    }
} else {
    Write-Host "⚠️ Windows Forms Client project not found" -ForegroundColor Yellow
}

# Create desktop shortcut
$CreateShortcut = Read-Host "Create desktop shortcut for WhatsApp Client? (y/N)"
if ($CreateShortcut -eq "y" -or $CreateShortcut -eq "Y") {
    $WshShell = New-Object -comObject WScript.Shell
    $Shortcut = $WshShell.CreateShortcut("$($env:USERPROFILE)\Desktop\Ustad WhatsApp.lnk")
    $Shortcut.TargetPath = $ClientExe
    $Shortcut.WorkingDirectory = Split-Path $ClientExe
    $Shortcut.Description = "Ustad WhatsApp Customer Service"
    $Shortcut.Save()
    Write-Host "✅ Desktop shortcut created" -ForegroundColor Green
}

# Generate configuration template
Write-Host "📝 Generating configuration template..." -ForegroundColor Cyan

$ConfigTemplate = @"
# Ustad Desktop WhatsApp Integration Configuration

## Database Connection
ConnectionString: Server=localhost;Database=UstadWhatsApp;Trusted_Connection=True;

## WhatsApp Business API (Get from Meta Business Manager)
PhoneNumberId: YOUR_PHONE_NUMBER_ID
AccessToken: YOUR_PERMANENT_ACCESS_TOKEN
VerifyToken: YOUR_WEBHOOK_VERIFY_TOKEN
BusinessAccountId: YOUR_WABA_ID

## JWT Security (Generate secure random key)
JwtKey: $(New-Guid)$(New-Guid) -replace '-',''

## Email Configuration (Office365 example)
SmtpHost: smtp.office365.com
SmtpPort: 587
SmtpUsername: your-email@domain.com
SmtpPassword: your-app-password

## SMS Configuration (NetGSM example)
NetGsmUsername: your-netgsm-username
NetGsmPassword: your-netgsm-password
NetGsmHeader: YOUR-SMS-HEADER

## URLs
WebhookUrl: https://your-domain.com/webhook
ClientApiUrl: http://localhost:5000

## Security Settings
MaxFailedLoginAttempts: 5
LockoutMinutes: 15
PasswordResetTokenExpiryHours: 1

## Deployment Notes
1. Replace all YOUR_* placeholders with actual values
2. Ensure HTTPS for production webhook URL
3. Configure firewall rules for API port
4. Set up SSL certificate for production
5. Configure backup strategy for database
6. Monitor logs and system performance

## First Admin User Creation
After database deployment, create your first admin user:

sqlcmd -S localhost -d UstadWhatsApp -Q "
EXEC sp_WhatsApp_CreateOperator 
    @FirmId = 1,
    @UserName = 'admin',
    @FullName = 'System Administrator',
    @Email = 'admin@yourcompany.com',
    @Phone = '905551234567',
    @Role = 'Admin',
    @Password = 'ChangeThisPassword123!';
"
"@

$ConfigTemplate | Out-File -FilePath (Join-Path $RootPath "DEPLOYMENT_CONFIG.txt") -Encoding UTF8
Write-Host "✅ Configuration template saved to DEPLOYMENT_CONFIG.txt" -ForegroundColor Green

# Final summary
Write-Host "`n🎉 Deployment Summary" -ForegroundColor Green
Write-Host "===================" -ForegroundColor Green
Write-Host "✅ Projects built successfully" -ForegroundColor Green

if (-not $SkipDatabase) {
    Write-Host "✅ Database schema deployed" -ForegroundColor Green
}

Write-Host "✅ Webhook API published to: $PublishPath" -ForegroundColor Green
Write-Host "✅ Windows Forms Client built" -ForegroundColor Green
Write-Host "✅ Configuration template generated" -ForegroundColor Green

Write-Host "`n📋 Next Steps:" -ForegroundColor Yellow
Write-Host "1. Review and update DEPLOYMENT_CONFIG.txt with your actual values" -ForegroundColor White
Write-Host "2. Update appsettings.json in the published API folder" -ForegroundColor White
Write-Host "3. Create your first admin user using the SQL command in the config template" -ForegroundColor White
Write-Host "4. Configure WhatsApp Business API webhook URL" -ForegroundColor White
Write-Host "5. Start the Webhook API service" -ForegroundColor White
Write-Host "6. Test the Windows Forms Client login" -ForegroundColor White

if ($Environment -eq "Production") {
    Write-Host "`n🔒 Production Security Checklist:" -ForegroundColor Red
    Write-Host "- Use HTTPS for all communications" -ForegroundColor White
    Write-Host "- Generate secure random JWT key (minimum 256 bits)" -ForegroundColor White
    Write-Host "- Use strong passwords for all accounts" -ForegroundColor White
    Write-Host "- Configure firewall rules" -ForegroundColor White
    Write-Host "- Set up regular database backups" -ForegroundColor White
    Write-Host "- Monitor system logs and performance" -ForegroundColor White
    Write-Host "- Test disaster recovery procedures" -ForegroundColor White
}

Write-Host "`n✨ Deployment completed successfully!" -ForegroundColor Green

# Return to original directory
Set-Location $ScriptPath
