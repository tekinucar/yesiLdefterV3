# Secure Authentication Flow Documentation

## Overview
This document explains the secure authentication flow for the YesiLdefter desktop application, where database connections are established **AFTER** user authentication via API, preventing exposure of sensitive database credentials in the compiled DLL.

## Environment Variables

The API uses environment variables for database credentials (recommended for production) with fallback to `appsettings.json`:

### Required Environment Variables:
- `DB_HOST` - Database server host/IP (e.g., `46.101.255.224`)
- `DB_PORT` - Database port (default: `1433`)
- `DB_USER` - Database username (e.g., `sa`)
- `DB_PASS` - Database password (e.g., `ustad84352Yazilim`)
- `DB_NAME` - Database name (e.g., `UstadCrmV1`)

### Configuration Priority:
1. **Environment Variables** (highest priority - most secure)
2. **appsettings.json** `Db` section (fallback)
3. **Hardcoded defaults** (lowest priority - only for development)

## Current Issues in tStarter.cs

### ❌ Problem: SQL Connections Before Authentication

**Location**: `YesiLdefter/Tkn/tStarter.cs` - `InitStart()` method

**Current Flow (INSECURE)**:
```csharp
Line 112: InitPreparingConnection()  // Sets up connection strings with hardcoded passwords
Line 115: Db_Open(v.active_DB.managerMSSQLConn)  // ❌ OPENS DB CONNECTION BEFORE AUTH
Line 126: InitLoginUser()  // Shows login form
```

**Issues**:
1. **Line 115**: Opens ManagerDB connection **BEFORE** user authentication
2. **Line 112**: `InitPreparingConnection()` uses hardcoded passwords from `tVariable.cs`:
   - `v.mainManagerPass = "Password = ustad84352Yazilim;"`
   - `v.publishManagerPass = "Password = ustad84352Yazilim;"`
3. These passwords are compiled into the DLL and can be reverse-engineered

### ✅ Required Refactoring

**New Secure Flow**:
```csharp
1. Read INI files (server names, DB names - no passwords)
2. Show login form (InitLoginUser) - NO DB CONNECTION YET
3. User authenticates via API (/auth/login)
4. After successful authentication, get DB connection info from API (/auth/db-connection-info)
5. Decrypt connection strings using JWT token
6. NOW establish DB connections
7. Continue with rest of initialization
```

## SQL Connections Analysis

### Connections Needed BEFORE Authentication: **NONE** ✅

The login form (`ms_User`) now uses API authentication (`checkedInputApi()` method) and does **NOT** require database connections. The old `checkedInput()` method is marked `[Obsolete]` and uses direct DB access.

### Connections Needed AFTER Authentication:

1. **ManagerDB** - Used for:
   - User firm information
   - System settings
   - Layout metadata (MS_LAYOUT table)

2. **UstadCRM DB** - Used for:
   - Computer registration (`InitLoginComputer()`)
   - User information
   - Firm details

3. **Master DB** - Used for:
   - Database management operations
   - System-level queries

## API Endpoints

### 1. `/auth/login` (POST)
- **Authentication**: Not required (public endpoint)
- **Purpose**: Authenticate user and get JWT token
- **Returns**: JWT access token, refresh token, user info

### 2. `/auth/db-connection-info` (GET)
- **Authentication**: **REQUIRED** (JWT Bearer token)
- **Purpose**: Get encrypted database connection strings
- **Returns**: 
  - Server names, database names, usernames (unencrypted)
  - Encrypted connection strings (decrypt using JWT key)

## Implementation Steps

### Step 1: Fix Environment Variables ✅
- Standardized to use `DB_PASS` (not `DB_PASSWORD`)
- Added `Db` section to `appsettings.json`

### Step 2: Refactor InitStart() (TODO)
- Move `InitLoginUser()` BEFORE `InitPreparingConnection()`
- Remove `Db_Open()` call before authentication
- Add API authentication flow
- Get DB connection info from API after authentication
- Establish DB connections only after successful authentication

### Step 3: Remove Hardcoded Passwords (TODO)
- Remove `mainManagerPass` and `publishManagerPass` from `tVariable.cs`
- Update `InitPreparingConnection()` to use API-provided connection strings

### Step 4: Update Login Form (Already Done ✅)
- `ms_User` form already uses `checkedInputApi()` method
- Uses `UstadApiClient` for authentication
- No direct database access required

## Security Benefits

1. **No Hardcoded Credentials**: Passwords stored only in environment variables on server
2. **Encrypted Transmission**: Connection strings encrypted using JWT key
3. **Token-Based Access**: Database info only available after authentication
4. **Reverse Engineering Protection**: No sensitive data in compiled DLL

## Testing Checklist

- [ ] API environment variables configured correctly
- [ ] Login works without database connection
- [ ] Database connection info retrieved after authentication
- [ ] Connection strings decrypted successfully
- [ ] Database connections established after authentication
- [ ] All initialization steps complete successfully
- [ ] No hardcoded passwords in compiled DLL

