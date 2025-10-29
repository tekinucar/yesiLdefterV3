using System;

namespace yesildeftertest.Models
{
    /// <summary>
    /// View model for WhatsApp conversations
    /// </summary>
    public class ConversationViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string LastMessage { get; set; } = string.Empty;
        public DateTime LastMessageTime { get; set; }
        public string Status { get; set; } = "Active";
        public bool IsAssigned { get; set; }
        public string? AssignedOperator { get; set; }
        public int UnreadCount { get; set; }
        public string Priority { get; set; } = "Normal";
    }
}
