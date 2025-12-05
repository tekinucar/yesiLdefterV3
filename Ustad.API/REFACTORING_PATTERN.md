# Ustad.API Controller Refactoring Pattern

## Overview

This document defines the standard refactoring pattern for all controllers in `Ustad.API`. Follow this pattern for consistency and code quality.

## ✅ Standard Pattern

### 1. **Namespace Organization**

```csharp
/* Core Namespace */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

/* Database Namespace */
using Microsoft.Data.SqlClient;

/* JWT Namespace */
using System.IdentityModel.Tokens.Jwt;

/* HTTP Namespace */
using System.Net.Http;
using System.Net.Http.Json;

/* Threading Namespace */
using System.Threading.Tasks;
```

**Rules:**
- Group related namespaces with comments
- Use clear, descriptive comments
- Order: Core → Database → Domain-specific → Threading

### 2. **Class Structure**

```csharp
[ApiController]
[Route("controller-name")]
public class ControllerName : ControllerBase
{
    // Dependencies (in order: Configuration, Services, Logger)
    private readonly IConfiguration _configuration;
    private readonly Classes.ServiceName _service;
    private readonly ILogger<ControllerName> _logger;

    // SQL Query Constants (if applicable)
    private const string QUERY_NAME = @"SELECT ...";

    // Constructor
    public ControllerName(
        IConfiguration configuration, 
        Classes.ServiceName service, 
        ILogger<ControllerName> logger)
    {
        _configuration = configuration;
        _service = service;
        _logger = logger;
    }

    // Regions (in order):
    // #region Public Models/Classes
    // #region Private Helper Methods
    // #region Public API Endpoints
}
```

### 3. **Region Organization**

```csharp
#region Public Models/Classes
/// <summary>
/// Request/Response model description
/// </summary>
public class ModelName
{
    /// <summary>
    /// Property description
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;
}
#endregion

#region Private Helper Methods
/// <summary>
/// Helper method description
/// </summary>
private ReturnType HelperMethod(parameters) { }
#endregion

#region Public API Endpoints
/// <summary>
/// Endpoint description
/// </summary>
[HttpPost("endpoint")]
public async Task<IActionResult> EndpointName([FromBody] RequestModel request) { }
#endregion
```

### 4. **XML Documentation Pattern**

**For Public Methods:**
```csharp
/// <summary>
/// Brief description of what the method does (one sentence)
/// </summary>
/// <param name="paramName">Parameter description</param>
/// <returns>Return value description</returns>
/// <response code="200">Success description</response>
/// <response code="400">Bad request description</response>
/// <response code="401">Unauthorized description</response>
```

**For Private Methods:**
```csharp
/// <summary>
/// Brief description of what the helper method does
/// </summary>
/// <param name="paramName">Parameter description</param>
/// <returns>Return value description</returns>
```

**For Classes/Models:**
```csharp
/// <summary>
/// Brief description of the class/model purpose
/// </summary>
public class ClassName
{
    /// <summary>
    /// Property description
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;
}
```

### 5. **Connection Management Pattern**

**✅ CORRECT:**
```csharp
using (var con = new SqlConnection(connStr))
{
    await con.OpenAsync();
    // Use connection
    using (var cmd = con.CreateCommand())
    {
        cmd.CommandText = "...";
        // Execute command
    }
} // Automatically closed and disposed - NO manual close needed
```

**❌ WRONG:**
```csharp
using (var con = new SqlConnection(connStr))
{
    await con.OpenAsync();
}
con.Close(); // ERROR: Connection already disposed by 'using' statement
```

**Rules:**
- Always use `using` statements for connections
- Never manually call `con.Close()` after a `using` block
- Let the `using` statement handle disposal automatically

### 6. **Error Handling Pattern**

**For Task Methods (no return value):**
```csharp
private async Task MethodName()
{
    try
    {
        // Operation
    }
    catch (Exception ex)
    {
        // Log but don't return - Task methods can't return IActionResult
        _logger.LogWarning("[Context] Error occurred - Error: {Error}", ex.Message);
        // Optionally: throw if critical, or just log if non-critical
    }
}
```

**For IActionResult Methods:**
```csharp
public async Task<IActionResult> MethodName()
{
    try
    {
        // Operation
        return Ok(result);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "[Context] Error occurred");
        return StatusCode(500, "Error message");
    }
}
```

### 7. **Variable Naming Consistency**

**✅ CORRECT:**
```csharp
public async Task<IActionResult> MethodName([FromBody] RequestModel request)
{
    if (request == null) // Use parameter name consistently
    {
        return BadRequest();
    }
    // Use 'request' throughout the method
}
```

**❌ WRONG:**
```csharp
public async Task<IActionResult> MethodName([FromBody] RequestModel request)
{
    if (req == null) // ERROR: Parameter is 'request', not 'req'
    {
        return BadRequest();
    }
}
```

### 8. **SQL Query Constants Pattern**

**For Repeated Queries:**
```csharp
// At class level
private const string PHASE1_PASSWORD_QUERY = @"
SELECT 
    u.UserId, 
    COALESCE(u.UserKey, '') AS UserKey,
    sp.PasswordHash,
    sp.Salt,
    sp.Iterations
FROM UstadUsers u
LEFT JOIN UstadUserSecurePasswords sp ON u.UserId = sp.UserId
WHERE (u.UserEMail = @u OR u.UserTcNo = @u OR u.UserMobileNo = @u) 
  AND u.IsActive = 1";

// In method
cmd.CommandText = PHASE1_PASSWORD_QUERY;
```

**Benefits:**
- Single source of truth
- Easier to maintain
- Better readability

### 9. **Helper Methods Pattern**

**For Common Operations:**
```csharp
#region Private Helper Methods
/// <summary>
/// Parse DbTypeId from database value, ensuring it fits in short range
/// </summary>
private short ParseDbTypeId(int dbTypeIdInt)
{
    if (dbTypeIdInt > short.MaxValue) return short.MaxValue;
    if (dbTypeIdInt < short.MinValue) return short.MinValue;
    return (short)dbTypeIdInt;
}
#endregion

// Usage
dbTypeId = ParseDbTypeId(r.GetInt32(r.GetOrdinal("DbTypeId")));
```

### 10. **Logging Pattern**

**Always Include:**
```csharp
private readonly ILogger<ControllerName> _logger;

public ControllerName(..., ILogger<ControllerName> logger)
{
    _logger = logger;
}
```

**Log Levels:**
- `LogInformation`: Normal operations, successful flows
- `LogWarning`: Non-critical errors, validation failures
- `LogError`: Critical errors, exceptions

**Log Format:**
```csharp
_logger.LogInformation("[Context] Action description - Key: {Key}, Value: {Value}", key, value);
_logger.LogWarning("[Context] Warning description - Error: {Error}", error);
_logger.LogError(ex, "[Context] Error description");
```

### 11. **Code Duplication Elimination**

**Before (Duplicate Logic):**
```csharp
if (condition1 && condition2 && condition3)
{
    DoSomething();
}
else if (condition2 && condition3 && condition4)
{
    DoSomething(); // Duplicate
}
```

**After (Simplified):**
```csharp
if (condition2 && condition3) // Common conditions
{
    DoSomething();
}
```

### 12. **Response Model Pattern**

**Always Define Response Models:**
```csharp
/// <summary>
/// Response model description
/// </summary>
public class ResponseModel
{
    /// <summary>
    /// Property description
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Property description
    /// </summary>
    public int Id { get; set; }
}
```

**Rules:**
- Define all response models used in `ProducesResponseType`
- Use descriptive property names
- Initialize with default values (`= string.Empty`, `= 0`, etc.)

## 🔍 Code Review Checklist

Before committing, verify:

- [ ] All compilation errors fixed
- [ ] No `con.Close()` after `using` statements
- [ ] Variable names match parameters consistently
- [ ] All response models defined
- [ ] Logging infrastructure present
- [ ] Error handling follows pattern
- [ ] No code duplication
- [ ] XML documentation complete
- [ ] Regions properly organized
- [ ] SQL queries extracted to constants (if repeated)
- [ ] Helper methods for common operations

## 📝 Example: Complete Controller Structure

```csharp
/* Core Namespace */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ustad.API.Controllers
{
    [ApiController]
    [Route("example")]
    public class ExampleController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ExampleController> _logger;

        // SQL Query Constants
        private const string GET_DATA_QUERY = @"SELECT ...";

        public ExampleController(IConfiguration configuration, ILogger<ExampleController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        #region Public Models
        public class RequestModel { }
        public class ResponseModel { }
        #endregion

        #region Private Helper Methods
        private ReturnType HelperMethod() { }
        #endregion

        #region Public API Endpoints
        [HttpPost("endpoint")]
        public async Task<IActionResult> Endpoint([FromBody] RequestModel request) { }
        #endregion
    }
}
```

## 🎯 Key Principles

1. **Consistency**: Follow the same pattern across all controllers
2. **Clarity**: Code should be self-documenting with XML comments
3. **Maintainability**: Extract common patterns to helper methods and constants
4. **Reliability**: Proper error handling and logging
5. **Safety**: Use `using` statements, validate inputs, handle exceptions

## 📚 Related Documentation

- `AuthController.cs` - Reference implementation
- `AuthController_REVIEW.md` - Common issues and fixes
- This pattern applies to all controllers in `Ustad.API`

