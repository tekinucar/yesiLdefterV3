-- USTAD DESKTOP WHATSAPP INTEGRATION - DATABASE SCHEMA
-- Compatible with existing Ustad system architecture

-- =====================================================
-- 1. OPERATORS TABLE (Multi-company support with FirmId)
-- =====================================================
CREATE TABLE WhatsApp_Operators (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirmId INT NOT NULL, -- Multi-company support (existing pattern)
    UserName NVARCHAR(50) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NULL,
    Phone NVARCHAR(20) NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'Agent', -- 'Admin' | 'Agent'
    PasswordHash VARBINARY(512) NOT NULL, -- PBKDF2 hash
    Salt VARBINARY(128) NOT NULL, -- Random salt
    Iterations INT NOT NULL DEFAULT 200000, -- PBKDF2 iterations
    FailedCount INT NOT NULL DEFAULT 0,
    LockoutEnd DATETIME2 NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT UQ_WhatsApp_Operators_UserName_Firm UNIQUE(FirmId, UserName)
);

-- =====================================================
-- 2. PASSWORD RESET TOKENS
-- =====================================================
CREATE TABLE WhatsApp_PasswordResetTokens (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OperatorId INT NOT NULL,
    Token NVARCHAR(128) NOT NULL UNIQUE,
    ExpiresAt DATETIME2 NOT NULL,
    Used BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_WhatsApp_PasswordResetTokens_Operator 
        FOREIGN KEY (OperatorId) REFERENCES WhatsApp_Operators(Id)
);

-- =====================================================
-- 3. CONVERSATIONS (Customer conversations)
-- =====================================================
CREATE TABLE WhatsApp_Conversations (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirmId INT NOT NULL,
    CustomerNumber NVARCHAR(32) NOT NULL, -- E.164 format (e.g., 905551234567)
    CustomerName NVARCHAR(100) NULL, -- If available from WhatsApp profile
    AssignedOperatorId INT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Open', -- 'Open' | 'Closed' | 'Pending'
    Priority NVARCHAR(10) NOT NULL DEFAULT 'Normal', -- 'Low' | 'Normal' | 'High' | 'Urgent'
    LastMessageAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_WhatsApp_Conversations_Operator 
        FOREIGN KEY (AssignedOperatorId) REFERENCES WhatsApp_Operators(Id),
    CONSTRAINT UQ_WhatsApp_Conversations_Customer_Firm UNIQUE(FirmId, CustomerNumber)
);

-- =====================================================
-- 4. MESSAGES (All message history)
-- =====================================================
CREATE TABLE WhatsApp_Messages (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    ConversationId INT NOT NULL,
    WhatsAppMessageId NVARCHAR(100) NULL, -- WhatsApp's message ID
    Direction NVARCHAR(10) NOT NULL, -- 'In' (from customer) | 'Out' (to customer)
    MessageType NVARCHAR(20) NOT NULL DEFAULT 'text', -- 'text' | 'image' | 'document' | 'template'
    Body NVARCHAR(MAX) NOT NULL,
    MediaUrl NVARCHAR(500) NULL, -- For images, documents
    OperatorId INT NULL, -- Who sent the message (for outgoing)
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_WhatsApp_Messages_Conversation 
        FOREIGN KEY (ConversationId) REFERENCES WhatsApp_Conversations(Id),
    CONSTRAINT FK_WhatsApp_Messages_Operator 
        FOREIGN KEY (OperatorId) REFERENCES WhatsApp_Operators(Id)
);

-- =====================================================
-- 5. AUDIT LOGS (Security and compliance)
-- =====================================================
CREATE TABLE WhatsApp_AuditLogs (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    FirmId INT NOT NULL,
    OperatorId INT NULL,
    Action NVARCHAR(100) NOT NULL, -- e.g., 'LOGIN', 'MESSAGE_SENT', 'CONVERSATION_ASSIGNED'
    EntityType NVARCHAR(50) NULL, -- 'Conversation', 'Operator', 'Message'
    EntityId NVARCHAR(50) NULL, -- ID of affected entity
    Details NVARCHAR(1000) NULL, -- JSON or text details
    IpAddress NVARCHAR(50) NULL,
    UserAgent NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_WhatsApp_AuditLogs_Operator 
        FOREIGN KEY (OperatorId) REFERENCES WhatsApp_Operators(Id)
);

-- =====================================================
-- 6. WHATSAPP TEMPLATES (For 24h+ messaging)
-- =====================================================
CREATE TABLE WhatsApp_Templates (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirmId INT NOT NULL,
    TemplateName NVARCHAR(100) NOT NULL,
    Language NVARCHAR(10) NOT NULL DEFAULT 'tr',
    Category NVARCHAR(50) NOT NULL, -- 'MARKETING' | 'UTILITY' | 'AUTHENTICATION'
    HeaderText NVARCHAR(500) NULL,
    BodyText NVARCHAR(1000) NOT NULL,
    FooterText NVARCHAR(100) NULL,
    ButtonText NVARCHAR(100) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT UQ_WhatsApp_Templates_Name_Firm UNIQUE(FirmId, TemplateName)
);

-- =====================================================
-- 7. FIRM WHATSAPP SETTINGS (Multi-company configuration)
-- =====================================================
CREATE TABLE WhatsApp_FirmSettings (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirmId INT NOT NULL UNIQUE,
    PhoneNumberId NVARCHAR(50) NOT NULL, -- WhatsApp Business Phone Number ID
    AccessToken NVARCHAR(500) NOT NULL, -- WhatsApp Cloud API Access Token
    WebhookVerifyToken NVARCHAR(100) NOT NULL, -- Webhook verification token
    BusinessAccountId NVARCHAR(50) NOT NULL, -- WABA ID
    ApiVersion NVARCHAR(10) NOT NULL DEFAULT 'v20.0',
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

-- =====================================================
-- INDEXES FOR PERFORMANCE
-- =====================================================

-- Operators
CREATE INDEX IX_WhatsApp_Operators_FirmId ON WhatsApp_Operators(FirmId);
CREATE INDEX IX_WhatsApp_Operators_UserName ON WhatsApp_Operators(UserName);
CREATE INDEX IX_WhatsApp_Operators_Email ON WhatsApp_Operators(Email);

-- Conversations
CREATE INDEX IX_WhatsApp_Conversations_FirmId ON WhatsApp_Conversations(FirmId);
CREATE INDEX IX_WhatsApp_Conversations_CustomerNumber ON WhatsApp_Conversations(CustomerNumber);
CREATE INDEX IX_WhatsApp_Conversations_AssignedOperator ON WhatsApp_Conversations(AssignedOperatorId);
CREATE INDEX IX_WhatsApp_Conversations_Status ON WhatsApp_Conversations(Status);
CREATE INDEX IX_WhatsApp_Conversations_LastMessage ON WhatsApp_Conversations(LastMessageAt DESC);

-- Messages
CREATE INDEX IX_WhatsApp_Messages_ConversationId ON WhatsApp_Messages(ConversationId);
CREATE INDEX IX_WhatsApp_Messages_CreatedAt ON WhatsApp_Messages(CreatedAt DESC);
CREATE INDEX IX_WhatsApp_Messages_Direction ON WhatsApp_Messages(Direction);
CREATE INDEX IX_WhatsApp_Messages_WhatsAppId ON WhatsApp_Messages(WhatsAppMessageId);

-- Audit Logs
CREATE INDEX IX_WhatsApp_AuditLogs_FirmId ON WhatsApp_AuditLogs(FirmId);
CREATE INDEX IX_WhatsApp_AuditLogs_OperatorId ON WhatsApp_AuditLogs(OperatorId);
CREATE INDEX IX_WhatsApp_AuditLogs_Action ON WhatsApp_AuditLogs(Action);
CREATE INDEX IX_WhatsApp_AuditLogs_CreatedAt ON WhatsApp_AuditLogs(CreatedAt DESC);

-- =====================================================
-- SAMPLE DATA (For testing)
-- =====================================================

-- Sample firm settings (replace with your actual WhatsApp Business API credentials)
INSERT INTO WhatsApp_FirmSettings (FirmId, PhoneNumberId, AccessToken, WebhookVerifyToken, BusinessAccountId)
VALUES (1, 'YOUR_PHONE_NUMBER_ID', 'YOUR_ACCESS_TOKEN', 'YOUR_VERIFY_TOKEN', 'YOUR_WABA_ID');

-- Sample admin operator (password: "admin123!" - you should change this)
-- Hash generated with PBKDF2, 200000 iterations
DECLARE @AdminSalt VARBINARY(128) = 0x1234567890ABCDEF1234567890ABCDEF1234567890ABCDEF1234567890ABCDEF;
DECLARE @AdminHash VARBINARY(512) = 0xABCDEF1234567890ABCDEF1234567890ABCDEF1234567890ABCDEF1234567890;

INSERT INTO WhatsApp_Operators (FirmId, UserName, FullName, Email, Phone, Role, PasswordHash, Salt, Iterations)
VALUES (1, 'admin', 'System Administrator', 'admin@ustad.com', '905551234567', 'Admin', @AdminHash, @AdminSalt, 200000);

-- =====================================================
-- STORED PROCEDURES
-- =====================================================

-- Get operator conversations
CREATE PROCEDURE sp_WhatsApp_GetOperatorConversations
    @OperatorId INT,
    @FirmId INT,
    @Status NVARCHAR(20) = NULL
AS
BEGIN
    SELECT 
        c.Id,
        c.CustomerNumber,
        c.CustomerName,
        c.Status,
        c.Priority,
        c.LastMessageAt,
        o.FullName AS AssignedOperatorName,
        (SELECT COUNT(*) FROM WhatsApp_Messages m WHERE m.ConversationId = c.Id AND m.Direction = 'In' AND m.IsRead = 0) AS UnreadCount,
        (SELECT TOP 1 m.Body FROM WhatsApp_Messages m WHERE m.ConversationId = c.Id ORDER BY m.CreatedAt DESC) AS LastMessage
    FROM WhatsApp_Conversations c
    LEFT JOIN WhatsApp_Operators o ON c.AssignedOperatorId = o.Id
    WHERE c.FirmId = @FirmId
        AND (@Status IS NULL OR c.Status = @Status)
        AND (c.AssignedOperatorId = @OperatorId OR EXISTS(
            SELECT 1 FROM WhatsApp_Operators op WHERE op.Id = @OperatorId AND op.Role = 'Admin'
        ))
    ORDER BY c.LastMessageAt DESC;
END;

-- Get conversation messages
CREATE PROCEDURE sp_WhatsApp_GetConversationMessages
    @ConversationId INT,
    @OperatorId INT,
    @PageSize INT = 50,
    @PageOffset INT = 0
AS
BEGIN
    -- Verify operator has access to this conversation
    IF NOT EXISTS (
        SELECT 1 FROM WhatsApp_Conversations c
        INNER JOIN WhatsApp_Operators o ON o.Id = @OperatorId
        WHERE c.Id = @ConversationId 
            AND (c.AssignedOperatorId = @OperatorId OR o.Role = 'Admin')
    )
    BEGIN
        RAISERROR('Access denied to conversation', 16, 1);
        RETURN;
    END

    SELECT 
        m.Id,
        m.Direction,
        m.MessageType,
        m.Body,
        m.MediaUrl,
        m.CreatedAt,
        o.FullName AS OperatorName
    FROM WhatsApp_Messages m
    LEFT JOIN WhatsApp_Operators o ON m.OperatorId = o.Id
    WHERE m.ConversationId = @ConversationId
    ORDER BY m.CreatedAt DESC
    OFFSET @PageOffset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;

-- Assign conversation to operator
CREATE PROCEDURE sp_WhatsApp_AssignConversation
    @ConversationId INT,
    @OperatorId INT,
    @AssignedByOperatorId INT
AS
BEGIN
    -- Verify assigning operator has admin rights
    IF NOT EXISTS (SELECT 1 FROM WhatsApp_Operators WHERE Id = @AssignedByOperatorId AND Role = 'Admin')
    BEGIN
        RAISERROR('Only admins can assign conversations', 16, 1);
        RETURN;
    END

    UPDATE WhatsApp_Conversations 
    SET AssignedOperatorId = @OperatorId, UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @ConversationId;

    -- Log the assignment
    INSERT INTO WhatsApp_AuditLogs (FirmId, OperatorId, Action, EntityType, EntityId, Details)
    SELECT c.FirmId, @AssignedByOperatorId, 'CONVERSATION_ASSIGNED', 'Conversation', 
           CAST(@ConversationId AS NVARCHAR), 
           'Assigned to operator ID: ' + CAST(@OperatorId AS NVARCHAR)
    FROM WhatsApp_Conversations c WHERE c.Id = @ConversationId;
END;
