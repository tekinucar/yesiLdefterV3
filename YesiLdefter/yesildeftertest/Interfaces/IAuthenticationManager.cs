using System;
using System.Threading.Tasks;
using yesildeftertest.Models;

namespace yesildeftertest.Interfaces
{
    /// <summary>
    /// Common interface for authentication managers
    /// </summary>
    public interface IAuthenticationManager : IDisposable
    {
        event EventHandler<AuthenticationEventArgs>? AuthenticationChanged;
        event EventHandler<string>? ConnectionStatusChanged;

        bool IsAuthenticated { get; }
        string? Username { get; }
        string? Role { get; }
        string? JwtToken { get; }

        // For demo manager
        Task<bool> LoginAsync(string username, string password);
        void Logout();
        
        // For production manager (async logout)
        Task LogoutAsync();
    }
}
