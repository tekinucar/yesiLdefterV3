using System;

namespace yesildeftertest.Models
{
    /// <summary>
    /// View model for WhatsApp messages
    /// </summary>
    public class MessageViewModel
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string SenderType { get; set; } = "Customer"; // Customer, Operator, System
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
        public string? MediaUrl { get; set; }
        public string MessageType { get; set; } = "Text"; // Text, Image, Document, etc.
    }
}
