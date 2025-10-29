using System;

namespace yesildeftertest.Models
{
    /// <summary>
    /// View model for WhatsApp operators
    /// </summary>
    public class OperatorViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime LastLoginTime { get; set; }
        public int ActiveConversations { get; set; }
        public string Status { get; set; } = "Offline"; // Online, Offline, Busy
    }
}
