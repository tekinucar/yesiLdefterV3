using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;
using System.Linq;
using yesildeftertest.Authentication;

namespace yesildeftertest.Services
{
    /// <summary>
    /// Core WhatsApp service for message handling and conversation management
    /// Integrates with the broader Ustad ecosystem (Desktop, Web, Mobile)
    /// </summary>
    public class WhatsAppService : tBase, IDisposable
    {
        #region tanımlar

        private readonly AuthenticationManager _authManager;
        private HubConnection? _hubConnection;
        private readonly List<ConversationModel> _conversations;
        private readonly List<MessageModel> _messages;
        private bool _disposed = false;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ConversationUpdatedEventArgs> ConversationUpdated;
        public event EventHandler<string> StatusChanged;

        #endregion

        #region Properties

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
        public IReadOnlyList<ConversationModel> Conversations => _conversations.AsReadOnly();
        public IReadOnlyList<MessageModel> CurrentMessages => _messages.AsReadOnly();

        #endregion

        #region constructor

        public WhatsAppService(AuthenticationManager authManager)
        {
            _authManager = authManager ?? throw new ArgumentNullException(nameof(authManager));
            _conversations = new List<ConversationModel>();
            _messages = new List<MessageModel>();

            // Subscribe to authentication events
            _authManager.AuthenticationChanged += OnAuthenticationChanged;
            _authManager.ConnectionStatusChanged += OnConnectionStatusChanged;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start the WhatsApp service
        /// </summary>
        public async Task<bool> StartAsync()
        {
            return await InitializeAsync();
        }

        /// <summary>
        /// Initialize WhatsApp service and establish connections
        /// </summary>
        public async Task<bool> InitializeAsync()
        {
            try
            {
                if (!_authManager.IsAuthenticated)
                {
                    debugMessage("Cannot initialize WhatsApp service - user not authenticated");
                    return false;
                }

                _hubConnection = _authManager.GetHubConnection();
                if (_hubConnection == null)
                {
                    debugMessage("SignalR connection not available");
                    return false;
                }

                // Register SignalR event handlers
                RegisterSignalREvents();

                // Load initial data
                await LoadConversationsAsync();

                debugMessage("WhatsApp service initialized successfully");
                return true;
            }
            catch (Exception ex)
            {
                debugMessage($"WhatsApp service initialization error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Send message to customer
        /// </summary>
        public async Task<bool> SendMessageAsync(string customerNumber, string message, string messageType = "text")
        {
            try
            {
                if (!IsConnected)
                {
                    OnStatusChanged("Bağlantı yok - mesaj gönderilemedi");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(customerNumber) || string.IsNullOrWhiteSpace(message))
                {
                    debugMessage("Invalid message parameters");
                    return false;
                }

                await _hubConnection.InvokeAsync("SendMessage", customerNumber, message, messageType);

                debugMessage($"Message sent to {customerNumber}: {message.Substring(0, Math.Min(50, message.Length))}");
                return true;
            }
            catch (Exception ex)
            {
                debugMessage($"Send message error: {ex.Message}");
                OnStatusChanged($"Mesaj gönderme hatası: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Assign conversation to operator (Admin only)
        /// </summary>
        public async Task<bool> AssignConversationAsync(int conversationId, int? operatorId)
        {
            try
            {
                if (!IsConnected || GlobalVariables.tWhatsAppOperator.OperatorRole != "Admin")
                {
                    return false;
                }

                await _hubConnection.InvokeAsync("AssignConversation", conversationId, operatorId);

                debugMessage($"Conversation {conversationId} assigned to operator {operatorId}");
                return true;
            }
            catch (Exception ex)
            {
                debugMessage($"Assign conversation error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Update conversation status
        /// </summary>
        public async Task<bool> UpdateConversationStatusAsync(int conversationId, string status)
        {
            try
            {
                if (!IsConnected)
                    return false;

                await _hubConnection.InvokeAsync("UpdateConversationStatus", conversationId, status);

                debugMessage($"Conversation {conversationId} status updated to {status}");
                return true;
            }
            catch (Exception ex)
            {
                debugMessage($"Update conversation status error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Load conversation history
        /// </summary>
        public async Task<bool> LoadConversationMessagesAsync(int conversationId)
        {
            try
            {
                if (!IsConnected)
                    return false;

                await _hubConnection.InvokeAsync("GetConversationHistory", conversationId, 50, 0);
                return true;
            }
            catch (Exception ex)
            {
                debugMessage($"Load conversation messages error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Send template message via WhatsApp
        /// </summary>
        public async Task<bool> SendTemplateAsync(string phoneNumber, string templateName, string language, string[] parameters)
        {
            try
            {
                if (!IsConnected)
                {
                    OnStatusChanged("Bağlantı yok - template mesajı gönderilemedi");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(templateName))
                {
                    debugMessage("Invalid template message parameters");
                    return false;
                }

                await _hubConnection.InvokeAsync("SendTemplate", phoneNumber, templateName, language, parameters);

                debugMessage($"Template message sent to {phoneNumber}: {templateName}");
                return true;
            }
            catch (Exception ex)
            {
                debugMessage($"Send template message error: {ex.Message}");
                OnStatusChanged($"Template mesaj gönderme hatası: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Mark messages as read
        /// </summary>
        public async Task<bool> MarkMessagesAsReadAsync(string[] messageIds)
        {
            try
            {
                if (!IsConnected || messageIds == null || messageIds.Length == 0)
                    return false;

                await _hubConnection.InvokeAsync("MarkMessagesAsRead", messageIds);
                return true;
            }
            catch (Exception ex)
            {
                debugMessage($"Mark messages as read error: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region SignalR Event Handling

        /// <summary>
        /// Register SignalR event handlers for real-time updates
        /// </summary>
        private void RegisterSignalREvents()
        {
            if (_hubConnection == null)
                return;

            // Customer message received
            _hubConnection.On<string, string, string, int>("CustomerMessageReceived",
                (customerNumber, message, messageType, conversationId) =>
                {
                    HandleIncomingMessage(customerNumber, message, messageType, conversationId);
                });

            // Message sent confirmation
            _hubConnection.On<object>("MessageSent", (messageData) =>
            {
                HandleMessageSent(messageData);
            });

            // Conversation assigned
            _hubConnection.On<object>("ConversationAssigned", (assignmentData) =>
            {
                HandleConversationAssigned(assignmentData);
            });

            // Conversation history received
            _hubConnection.On<int, object[]>("ConversationHistory", (conversationId, messages) =>
            {
                HandleConversationHistory(conversationId, messages);
            });

            // Error handling
            _hubConnection.On<string>("Error", (errorMessage) =>
            {
                OnStatusChanged($"Hata: {errorMessage}");
            });
        }

        /// <summary>
        /// Handle incoming customer message
        /// </summary>
        private void HandleIncomingMessage(string customerNumber, string message, string messageType, int conversationId)
        {
            try
            {
                // Update or add conversation
                var conversation = _conversations.FirstOrDefault(c => c.Id == conversationId);
                if (conversation != null)
                {
                    conversation.LastMessage = message;
                    conversation.LastMessageTime = DateTime.Now;
                    conversation.UnreadCount++;
                }
                else
                {
                    _conversations.Insert(0, new ConversationModel
                    {
                        Id = conversationId,
                        CustomerNumber = customerNumber,
                        LastMessage = message,
                        LastMessageTime = DateTime.Now,
                        Status = "Open",
                        UnreadCount = 1
                    });
                }

                // Add to current messages if this conversation is selected
                var messageModel = new MessageModel
                {
                    ConversationId = conversationId,
                    Direction = "In",
                    MessageType = messageType,
                    Body = message,
                    CreatedAt = DateTime.Now,
                    IsRead = false
                };

                // Raise event for UI updates
                OnMessageReceived(new MessageReceivedEventArgs(messageModel, conversation));

                debugMessage($"Incoming message processed: {customerNumber}");
            }
            catch (Exception ex)
            {
                debugMessage($"Handle incoming message error: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle message sent confirmation
        /// </summary>
        private void HandleMessageSent(object messageData)
        {
            try
            {
                // Parse message data and update UI
                debugMessage("Message sent confirmation received");
            }
            catch (Exception ex)
            {
                debugMessage($"Handle message sent error: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle conversation assignment
        /// </summary>
        private void HandleConversationAssigned(object assignmentData)
        {
            try
            {
                // Update conversation assignment in local data
                debugMessage("Conversation assignment updated");
            }
            catch (Exception ex)
            {
                debugMessage($"Handle conversation assigned error: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle conversation history received
        /// </summary>
        private void HandleConversationHistory(int conversationId, object[] messages)
        {
            try
            {
                _messages.Clear();

                foreach (var messageObj in messages)
                {
                    if (messageObj is JsonElement jsonElement)
                    {
                        var messageModel = new MessageModel
                        {
                            ConversationId = conversationId,
                            Direction = jsonElement.GetProperty("Direction").GetString(),
                            MessageType = jsonElement.GetProperty("MessageType").GetString(),
                            Body = jsonElement.GetProperty("Body").GetString(),
                            CreatedAt = jsonElement.GetProperty("CreatedAt").GetDateTime(),
                            IsRead = jsonElement.GetProperty("IsRead").GetBoolean(),
                            OperatorName = jsonElement.TryGetProperty("OperatorName", out var opName)
                                ? opName.GetString() : null
                        };

                        _messages.Add(messageModel);
                    }
                }

                debugMessage($"Conversation history loaded: {conversationId} - {_messages.Count} messages");
            }
            catch (Exception ex)
            {
                debugMessage($"Handle conversation history error: {ex.Message}");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Load conversations from server
        /// </summary>
        private async Task LoadConversationsAsync()
        {
            try
            {
                if (_hubConnection?.State == HubConnectionState.Connected)
                {
                    await _hubConnection.InvokeAsync("GetOperatorConversations");
                }
            }
            catch (Exception ex)
            {
                debugMessage($"Load conversations error: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle authentication state changes
        /// </summary>
        private async void OnAuthenticationChanged(object sender, AuthenticationEventArgs e)
        {
            if (e.IsAuthenticated)
            {
                await InitializeAsync();
            }
            else
            {
                _conversations.Clear();
                _messages.Clear();
            }
        }

        /// <summary>
        /// Handle connection status changes
        /// </summary>
        private void OnConnectionStatusChanged(object sender, string status)
        {
            OnStatusChanged(status);
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }

        protected virtual void OnConversationUpdated(ConversationUpdatedEventArgs e)
        {
            ConversationUpdated?.Invoke(this, e);
        }

        protected virtual void OnStatusChanged(string status)
        {
            StatusChanged?.Invoke(this, status);
        }

        #endregion

        #region IDisposable Implementation

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected new virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _hubConnection?.DisposeAsync();
                _disposed = true;
            }
        }

        #endregion
    }

    #region Event Args Classes

    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageModel Message { get; }
        public ConversationModel Conversation { get; }

        public MessageReceivedEventArgs(MessageModel message, ConversationModel conversation)
        {
            Message = message;
            Conversation = conversation;
        }
    }

    public class ConversationUpdatedEventArgs : EventArgs
    {
        public ConversationModel Conversation { get; }
        public string UpdateType { get; }

        public ConversationUpdatedEventArgs(ConversationModel conversation, string updateType)
        {
            Conversation = conversation;
            UpdateType = updateType;
        }
    }

    #endregion

    #region Data Models

    /// <summary>
    /// Conversation data model for UI binding
    /// </summary>
    public class ConversationModel
    {
        public int Id { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageTime { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public int? AssignedOperatorId { get; set; }
        public string AssignedOperatorName { get; set; }
        public int UnreadCount { get; set; }

        public string DisplayStatus => Status switch
        {
            "Open" => "Açık",
            "Closed" => "Kapalı",
            "Pending" => "Beklemede",
            _ => Status ?? "Bilinmiyor"
        };

        public string DisplayTime => LastMessageTime.ToString("HH:mm");
        public string DisplayDate => LastMessageTime.ToString("dd.MM.yyyy");
    }

    /// <summary>
    /// Message data model for UI binding
    /// </summary>
    public class MessageModel
    {
        public long Id { get; set; }
        public int ConversationId { get; set; }
        public string WhatsAppMessageId { get; set; }
        public string Direction { get; set; } // "In" or "Out"
        public string MessageType { get; set; } // "text", "image", etc.
        public string Body { get; set; }
        public string MediaUrl { get; set; }
        public int? OperatorId { get; set; }
        public string OperatorName { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsIncoming => Direction == "In";
        public bool IsOutgoing => Direction == "Out";
        public bool HasMedia => !string.IsNullOrEmpty(MediaUrl);
        public string DisplayTime => CreatedAt.ToString("HH:mm");
        public string DisplayDirection => IsIncoming ? "Gelen" : "Giden";

        public string DisplayType => MessageType switch
        {
            "text" => "Metin",
            "image" => "Resim",
            "document" => "Belge",
            "audio" => "Ses",
            "video" => "Video",
            _ => MessageType ?? "Bilinmiyor"
        };
    }

    #endregion
}
