using System;

namespace yesildeftertest.Models
{
    /// <summary>
    /// Event arguments for authentication events
    /// </summary>
    public class AuthenticationEventArgs : EventArgs
    {
        public bool IsAuthenticated { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; }
        public string? Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
