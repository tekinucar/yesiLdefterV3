# Swagger Documentation Update Guide

## Current Configuration

Your Swagger is already configured to use XML comments! Here's what's set up:

### ✅ Already Configured

1. **XML Documentation Generation** (in `Ustad.API.csproj`):
   ```xml
   <GenerateDocumentationFile>true</GenerateDocumentationFile>
   ```

2. **Swagger XML Comments** (in `Startup.cs`):
   ```csharp
   var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
   var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
   if (File.Exists(xmlPath))
   {
       c.IncludeXmlComments(xmlPath);
   }
   ```

## How to Update Swagger Documentation

### Step 1: Rebuild the Project

The XML documentation file is generated during build. After adding/updating XML comments:

```powershell
cd C:\UstadProjects\Ustad.API
dotnet build
```

Or in Visual Studio:
- Right-click project → **Rebuild**

### Step 2: Verify XML File Generation

After building, check if the XML file exists:

```powershell
# Check Debug build
dir bin\Debug\net8.0\Ustad.API.xml

# Check Release build
dir bin\Release\net8.0\Ustad.API.xml
```

The file should be named `Ustad.API.xml` and contain all your XML comments.

### Step 3: Run the Application

Start your API:

```powershell
dotnet run
```

Or in Visual Studio: Press F5

### Step 4: View Swagger Documentation

Navigate to:
- **Swagger UI**: `http://localhost:5000/swagger` (or your configured port)
- **Swagger JSON**: `http://localhost:5000/swagger/v1/swagger.json`

Your XML comments should now appear in:
- Endpoint descriptions
- Parameter descriptions
- Response code descriptions
- Model property descriptions

## What Appears in Swagger

### From Your XML Comments:

1. **Method Summary** → Appears as endpoint description
   ```csharp
   /// <summary>
   /// Authenticates a user and returns access and refresh tokens
   /// </summary>
   ```
   Shows in Swagger as the endpoint description.

2. **Parameter Documentation** → Appears in parameter tooltips
   ```csharp
   /// <param name="request">Login credentials including username, password...</param>
   ```

3. **Response Codes** → Appears in response section
   ```csharp
   /// <response code="200">Login successful, returns tokens and user info</response>
   /// <response code="401">Authentication failed</response>
   ```

4. **Model Properties** → Appears in schema documentation
   ```csharp
   /// <summary>
   /// User name from login form
   /// </summary>
   public string UserName { get; set; }
   ```

## Troubleshooting

### Issue: XML Comments Not Appearing in Swagger

**Solution 1: Check XML File Location**
```csharp
// In Startup.cs, verify the path is correct
var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
Console.WriteLine($"Looking for XML at: {xmlPath}"); // Debug line
```

**Solution 2: Ensure XML File is Copied to Output**
The `.csproj` file now includes:
```xml
<DocumentationFile>bin\$(Configuration)\$(TargetFramework)\$(AssemblyName).xml</DocumentationFile>
```

**Solution 3: Rebuild Clean**
```powershell
dotnet clean
dotnet build
```

### Issue: Some Comments Missing

- Ensure all public methods/classes have XML comments
- Check for XML comment warnings (CS1591) - these are suppressed but verify comments are correct
- Make sure comments use triple slashes `///` not double slashes `//`

## Example: What You'll See

### Before (No XML Comments):
```
POST /auth/login
```

### After (With XML Comments):
```
POST /auth/login
Authenticates a user and returns access and refresh tokens

Parameters:
  - request (LoginRequest): Login credentials including username, password, and optional Cloudflare Turnstile token

Responses:
  200: Login successful, returns tokens and user info
  400: Invalid request (missing username or password)
  401: Authentication failed (invalid credentials or Cloudflare Turnstile validation failed)
```

## Quick Test

1. **Build**: `dotnet build`
2. **Run**: `dotnet run`
3. **Open**: `http://localhost:5000/swagger`
4. **Check**: Expand `/auth/login` endpoint - you should see all your XML comments!

## Additional Swagger Enhancements

You can also add:

### Operation Tags
```csharp
[HttpPost("login")]
[Tags("Authentication")]
public async Task<IActionResult> Login(...)
```

### Example Values
```csharp
/// <summary>
/// Login request payload
/// </summary>
/// <example>
/// {
///   "userName": "user@example.com",
///   "password": "password123",
///   "turnstileToken": ""
/// }
/// </example>
public class LoginRequest { }
```

### Response Examples
```csharp
[ProducesResponseType(typeof(LoginResponse), 200)]
[SwaggerResponseExample(200, typeof(LoginResponseExample))]
```

## Summary

✅ **XML Documentation**: Already enabled  
✅ **Swagger Configuration**: Already set up  
✅ **Next Step**: Just rebuild and run - your comments will appear!

Your XML comments in `AuthController.cs` will automatically appear in Swagger after rebuilding the project.

