-- =====================================================
-- USTAD WHATSAPP INTEGRATION - DATABASE SETUP
-- Tekin's SQL pattern ile uyumlu
-- =====================================================

USE master;
GO

-- Database oluştur
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'UstadWhatsApp')
BEGIN
    CREATE DATABASE [UstadWhatsApp]
    COLLATE Turkish_CI_AS;
    PRINT 'UstadWhatsApp database created successfully';
END
ELSE
BEGIN
    PRINT 'UstadWhatsApp database already exists';
END
GO

USE [UstadWhatsApp];
GO

-- =====================================================
-- 1. OPERATORS TABLE - Tekin's naming pattern
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='WhatsApp_Operators' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[WhatsApp_Operators] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [FirmId] INT NOT NULL DEFAULT 1, -- Multi-company support
        [UserName] NVARCHAR(50) NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [UserEMail] NVARCHAR(100) NULL, -- Tekin's naming: UserEMail
        [UserPhone] NVARCHAR(20) NULL, -- Tekin's naming: UserPhone
        [UserRole] NVARCHAR(20) NOT NULL DEFAULT 'Agent', -- Tekin's naming: UserRole
        [PasswordHash] VARBINARY(512) NOT NULL,
        [PasswordSalt] VARBINARY(128) NOT NULL, -- Tekin's naming: PasswordSalt
        [PasswordIterations] INT NOT NULL DEFAULT 200000,
        [FailedCount] INT NOT NULL DEFAULT 0,
        [LockoutEnd] DATETIME2 NULL,
        [IsActive] BIT NOT NULL DEFAULT 1, -- Tekin's pattern: IsActive
        [CreatedAt] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        
        CONSTRAINT [PK_WhatsApp_Operators] PRIMARY KEY ([Id]),
        CONSTRAINT [UQ_WhatsApp_Operators_UserName_Firm] UNIQUE([FirmId], [UserName])
    );
    
    PRINT 'WhatsApp_Operators table created';
END
GO

-- =====================================================
-- 2. PASSWORD RESET TOKENS - Tekin's pattern
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='WhatsApp_PasswordResetTokens' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[WhatsApp_PasswordResetTokens] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [OperatorId] INT NOT NULL,
        [ResetToken] NVARCHAR(128) NOT NULL, -- Tekin's naming
        [TokenExpiry] DATETIME2 NOT NULL, -- Tekin's naming
        [IsUsed] BIT NOT NULL DEFAULT 0, -- Tekin's pattern: IsUsed
        [CreatedAt] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        
        CONSTRAINT [PK_WhatsApp_PasswordResetTokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_WhatsApp_PasswordResetTokens_Operator] 
            FOREIGN KEY ([OperatorId]) REFERENCES [WhatsApp_Operators]([Id]),
        CONSTRAINT [UQ_WhatsApp_PasswordResetTokens_Token] UNIQUE([ResetToken])
    );
    
    PRINT 'WhatsApp_PasswordResetTokens table created';
END
GO

-- =====================================================
-- 3. CONVERSATIONS - Tekin's pattern
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='WhatsApp_Conversations' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[WhatsApp_Conversations] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [FirmId] INT NOT NULL DEFAULT 1,
        [CustomerNumber] NVARCHAR(32) NOT NULL, -- E.164 format
        [CustomerName] NVARCHAR(100) NULL,
        [AssignedOperatorId] INT NULL,
        [ConversationStatus] NVARCHAR(20) NOT NULL DEFAULT 'Open', -- Tekin's naming
        [ConversationPriority] NVARCHAR(10) NOT NULL DEFAULT 'Normal', -- Tekin's naming
        [LastMessageAt] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        [IsActive] BIT NOT NULL DEFAULT 1, -- Tekin's pattern
        [CreatedAt] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        
        CONSTRAINT [PK_WhatsApp_Conversations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_WhatsApp_Conversations_Operator] 
            FOREIGN KEY ([AssignedOperatorId]) REFERENCES [WhatsApp_Operators]([Id]),
        CONSTRAINT [UQ_WhatsApp_Conversations_Customer_Firm] UNIQUE([FirmId], [CustomerNumber])
    );
    
    PRINT 'WhatsApp_Conversations table created';
END
GO

-- =====================================================
-- 4. MESSAGES - Tekin's pattern
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='WhatsApp_Messages' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[WhatsApp_Messages] (
        [Id] BIGINT IDENTITY(1,1) NOT NULL,
        [ConversationId] INT NOT NULL,
        [WhatsAppMessageId] NVARCHAR(100) NULL,
        [MessageDirection] NVARCHAR(10) NOT NULL, -- Tekin's naming: MessageDirection
        [MessageType] NVARCHAR(20) NOT NULL DEFAULT 'text', -- Tekin's naming: MessageType
        [MessageBody] NVARCHAR(MAX) NOT NULL, -- Tekin's naming: MessageBody
        [MediaUrl] NVARCHAR(500) NULL,
        [OperatorId] INT NULL,
        [IsRead] BIT NOT NULL DEFAULT 0, -- Tekin's pattern: IsRead
        [IsActive] BIT NOT NULL DEFAULT 1, -- Tekin's pattern: IsActive
        [CreatedAt] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        
        CONSTRAINT [PK_WhatsApp_Messages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_WhatsApp_Messages_Conversation] 
            FOREIGN KEY ([ConversationId]) REFERENCES [WhatsApp_Conversations]([Id]),
        CONSTRAINT [FK_WhatsApp_Messages_Operator] 
            FOREIGN KEY ([OperatorId]) REFERENCES [WhatsApp_Operators]([Id])
    );
    
    PRINT 'WhatsApp_Messages table created';
END
GO

-- =====================================================
-- 5. AUDIT LOGS - Tekin's pattern
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='WhatsApp_AuditLogs' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[WhatsApp_AuditLogs] (
        [Id] BIGINT IDENTITY(1,1) NOT NULL,
        [FirmId] INT NOT NULL DEFAULT 1,
        [OperatorId] INT NULL,
        [LogAction] NVARCHAR(100) NOT NULL, -- Tekin's naming: LogAction
        [LogEntityType] NVARCHAR(50) NULL, -- Tekin's naming: LogEntityType
        [LogEntityId] NVARCHAR(50) NULL, -- Tekin's naming: LogEntityId
        [LogDetails] NVARCHAR(1000) NULL, -- Tekin's naming: LogDetails
        [LogIpAddress] NVARCHAR(50) NULL, -- Tekin's naming: LogIpAddress
        [LogUserAgent] NVARCHAR(500) NULL, -- Tekin's naming: LogUserAgent
        [IsActive] BIT NOT NULL DEFAULT 1, -- Tekin's pattern: IsActive
        [CreatedAt] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        
        CONSTRAINT [PK_WhatsApp_AuditLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_WhatsApp_AuditLogs_Operator] 
            FOREIGN KEY ([OperatorId]) REFERENCES [WhatsApp_Operators]([Id])
    );
    
    PRINT 'WhatsApp_AuditLogs table created';
END
GO

-- =====================================================
-- 6. FIRM SETTINGS - Tekin's pattern
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='WhatsApp_FirmSettings' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[WhatsApp_FirmSettings] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [FirmId] INT NOT NULL,
        [FirmName] NVARCHAR(100) NOT NULL, -- Tekin's naming: FirmName
        [PhoneNumberId] NVARCHAR(50) NOT NULL,
        [AccessToken] NVARCHAR(500) NOT NULL,
        [WebhookVerifyToken] NVARCHAR(100) NOT NULL,
        [BusinessAccountId] NVARCHAR(50) NOT NULL,
        [ApiVersion] NVARCHAR(10) NOT NULL DEFAULT 'v20.0',
        [IsActive] BIT NOT NULL DEFAULT 1, -- Tekin's pattern: IsActive
        [CreatedAt] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        
        CONSTRAINT [PK_WhatsApp_FirmSettings] PRIMARY KEY ([Id]),
        CONSTRAINT [UQ_WhatsApp_FirmSettings_FirmId] UNIQUE([FirmId])
    );
    
    PRINT 'WhatsApp_FirmSettings table created';
END
GO

-- =====================================================
-- INDEXES - Tekin's pattern ile performans
-- =====================================================

-- Operators indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_WhatsApp_Operators_FirmId')
    CREATE INDEX [IX_WhatsApp_Operators_FirmId] ON [WhatsApp_Operators]([FirmId]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_WhatsApp_Operators_UserName')
    CREATE INDEX [IX_WhatsApp_Operators_UserName] ON [WhatsApp_Operators]([UserName]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_WhatsApp_Operators_UserEMail')
    CREATE INDEX [IX_WhatsApp_Operators_UserEMail] ON [WhatsApp_Operators]([UserEMail]);

-- Conversations indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_WhatsApp_Conversations_CustomerNumber')
    CREATE INDEX [IX_WhatsApp_Conversations_CustomerNumber] ON [WhatsApp_Conversations]([CustomerNumber]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_WhatsApp_Conversations_LastMessageAt')
    CREATE INDEX [IX_WhatsApp_Conversations_LastMessageAt] ON [WhatsApp_Conversations]([LastMessageAt] DESC);

-- Messages indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_WhatsApp_Messages_ConversationId')
    CREATE INDEX [IX_WhatsApp_Messages_ConversationId] ON [WhatsApp_Messages]([ConversationId]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_WhatsApp_Messages_CreatedAt')
    CREATE INDEX [IX_WhatsApp_Messages_CreatedAt] ON [WhatsApp_Messages]([CreatedAt] DESC);

PRINT 'Indexes created successfully';
GO

-- =====================================================
-- STORED PROCEDURES - Tekin's pattern
-- =====================================================

-- Operatör oluştur - Tekin's pattern (prc_ prefix)
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'prc_WhatsApp_CreateOperator')
    DROP PROCEDURE [prc_WhatsApp_CreateOperator];
GO

CREATE PROCEDURE [prc_WhatsApp_CreateOperator]
    @FirmId INT,
    @UserName NVARCHAR(50),
    @FullName NVARCHAR(100),
    @UserEMail NVARCHAR(100) = NULL,
    @UserPhone NVARCHAR(20) = NULL,
    @UserRole NVARCHAR(20) = 'Agent',
    @Password NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Salt VARBINARY(128);
    DECLARE @Hash VARBINARY(512);
    DECLARE @Iterations INT = 200000;
    
    -- Generate salt (simplified - in real implementation, use crypto functions)
    SET @Salt = CONVERT(VARBINARY(128), NEWID());
    
    -- Generate hash (simplified - in real implementation, use PBKDF2)
    SET @Hash = HASHBYTES('SHA2_256', @Password + CONVERT(NVARCHAR(128), @Salt));
    
    INSERT INTO [WhatsApp_Operators] (
        [FirmId], [UserName], [FullName], [UserEMail], [UserPhone], 
        [UserRole], [PasswordHash], [PasswordSalt], [PasswordIterations],
        [IsActive], [CreatedAt], [UpdatedAt]
    )
    VALUES (
        @FirmId, @UserName, @FullName, @UserEMail, @UserPhone,
        @UserRole, @Hash, @Salt, @Iterations,
        1, SYSUTCDATETIME(), SYSUTCDATETIME()
    );
    
    PRINT 'Operator created: ' + @UserName;
END
GO

-- Operatör girişi kontrol - Tekin's pattern
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'prc_WhatsApp_OperatorLogin')
    DROP PROCEDURE [prc_WhatsApp_OperatorLogin];
GO

CREATE PROCEDURE [prc_WhatsApp_OperatorLogin]
    @UserName NVARCHAR(50),
    @Password NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        [Id],
        [FirmId],
        [UserName],
        [FullName],
        [UserEMail],
        [UserPhone],
        [UserRole],
        [PasswordHash],
        [PasswordSalt],
        [PasswordIterations],
        [FailedCount],
        [LockoutEnd],
        [IsActive]
    FROM [WhatsApp_Operators]
    WHERE [UserName] = @UserName
      AND [IsActive] = 1;
END
GO

-- Operatör konuşmaları - Tekin's pattern
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'prc_WhatsApp_GetOperatorConversations')
    DROP PROCEDURE [prc_WhatsApp_GetOperatorConversations];
GO

CREATE PROCEDURE [prc_WhatsApp_GetOperatorConversations]
    @OperatorId INT,
    @FirmId INT,
    @ConversationStatus NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.[Id],
        c.[CustomerNumber],
        c.[CustomerName],
        c.[ConversationStatus],
        c.[ConversationPriority],
        c.[LastMessageAt],
        o.[FullName] AS [AssignedOperatorName],
        (SELECT COUNT(*) 
         FROM [WhatsApp_Messages] m 
         WHERE m.[ConversationId] = c.[Id] 
           AND m.[MessageDirection] = 'In' 
           AND m.[IsRead] = 0
           AND m.[IsActive] = 1) AS [UnreadCount],
        (SELECT TOP 1 m.[MessageBody] 
         FROM [WhatsApp_Messages] m 
         WHERE m.[ConversationId] = c.[Id] 
           AND m.[IsActive] = 1
         ORDER BY m.[CreatedAt] DESC) AS [LastMessage]
    FROM [WhatsApp_Conversations] c
    LEFT JOIN [WhatsApp_Operators] o ON c.[AssignedOperatorId] = o.[Id]
    WHERE c.[FirmId] = @FirmId
      AND c.[IsActive] = 1
      AND (@ConversationStatus IS NULL OR c.[ConversationStatus] = @ConversationStatus)
      AND (c.[AssignedOperatorId] = @OperatorId OR EXISTS(
          SELECT 1 FROM [WhatsApp_Operators] op 
          WHERE op.[Id] = @OperatorId AND op.[UserRole] = 'Admin'
      ))
    ORDER BY c.[LastMessageAt] DESC;
END
GO

-- Konuşma mesajları - Tekin's pattern  
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'prc_WhatsApp_GetConversationMessages')
    DROP PROCEDURE [prc_WhatsApp_GetConversationMessages];
GO

CREATE PROCEDURE [prc_WhatsApp_GetConversationMessages]
    @ConversationId INT,
    @OperatorId INT,
    @PageSize INT = 50,
    @PageOffset INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Yetki kontrolü - Tekin's pattern
    IF NOT EXISTS (
        SELECT 1 FROM [WhatsApp_Conversations] c
        INNER JOIN [WhatsApp_Operators] o ON o.[Id] = @OperatorId
        WHERE c.[Id] = @ConversationId 
          AND c.[IsActive] = 1
          AND (c.[AssignedOperatorId] = @OperatorId OR o.[UserRole] = 'Admin')
    )
    BEGIN
        RAISERROR('Access denied to conversation', 16, 1);
        RETURN;
    END

    SELECT 
        m.[Id],
        m.[MessageDirection],
        m.[MessageType],
        m.[MessageBody],
        m.[MediaUrl],
        m.[IsRead],
        m.[CreatedAt],
        o.[FullName] AS [OperatorName]
    FROM [WhatsApp_Messages] m
    LEFT JOIN [WhatsApp_Operators] o ON m.[OperatorId] = o.[Id]
    WHERE m.[ConversationId] = @ConversationId
      AND m.[IsActive] = 1
    ORDER BY m.[CreatedAt] DESC
    OFFSET @PageOffset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

-- =====================================================
-- TRIGGERS - Tekin's pattern (trg_ prefix)
-- =====================================================

-- Operators trigger - Tekin's pattern (trg_users benzeri)
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'TR' AND name = 'trg_WhatsApp_Operators')
    DROP TRIGGER [trg_WhatsApp_Operators];
GO

CREATE TRIGGER [trg_WhatsApp_Operators] ON [dbo].[WhatsApp_Operators]
AFTER INSERT, UPDATE
AS 
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @OperatorFullName NVARCHAR(100);
    DECLARE @OperatorUserName NVARCHAR(50);
    
    DECLARE operatorsCursor CURSOR FOR 
    SELECT [Id] FROM INSERTED;
    
    DECLARE @curId INT;
    
    OPEN operatorsCursor;
    FETCH NEXT FROM operatorsCursor INTO @curId;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN 
        -- Update timestamp - Tekin's pattern
        UPDATE [WhatsApp_Operators] 
        SET [UpdatedAt] = SYSUTCDATETIME()
        WHERE [Id] = @curId;
        
        FETCH NEXT FROM operatorsCursor INTO @curId;
    END
    
    CLOSE operatorsCursor;
    DEALLOCATE operatorsCursor;
END
GO

-- =====================================================
-- SAMPLE DATA - Test için
-- =====================================================

-- Test firması
IF NOT EXISTS (SELECT * FROM [WhatsApp_FirmSettings] WHERE [FirmId] = 1)
BEGIN
    INSERT INTO [WhatsApp_FirmSettings] (
        [FirmId], [FirmName], [PhoneNumberId], [AccessToken], 
        [WebhookVerifyToken], [BusinessAccountId], [IsActive]
    )
    VALUES (
        1, 'Test Firma', 'TEST_PHONE_ID', 'TEST_ACCESS_TOKEN', 
        'test_verify_token_123', 'TEST_WABA_ID', 1
    );
    
    PRINT 'Test firm settings created';
END

-- Test admin operatör
-- Şifre: admin123! (Bu şifre production'da değiştirilmelidir)
IF NOT EXISTS (SELECT * FROM [WhatsApp_Operators] WHERE [UserName] = 'admin')
BEGIN
    EXEC [prc_WhatsApp_CreateOperator] 
        @FirmId = 1,
        @UserName = 'admin',
        @FullName = 'Test Administrator',
        @UserEMail = 'admin@test.com',
        @UserPhone = '905551234567',
        @UserRole = 'Admin',
        @Password = 'admin123!';
    
    PRINT 'Test admin operator created - Username: admin, Password: admin123!';
END

-- Test agent operatör
-- Şifre: agent123!
IF NOT EXISTS (SELECT * FROM [WhatsApp_Operators] WHERE [UserName] = 'agent')
BEGIN
    EXEC [prc_WhatsApp_CreateOperator] 
        @FirmId = 1,
        @UserName = 'agent',
        @FullName = 'Test Agent',
        @UserEMail = 'agent@test.com',
        @UserPhone = '905551234568',
        @UserRole = 'Agent',
        @Password = 'agent123!';
    
    PRINT 'Test agent operator created - Username: agent, Password: agent123!';
END

-- =====================================================
-- FINAL MESSAGE
-- =====================================================

PRINT '==============================================';
PRINT 'USTAD WHATSAPP INTEGRATION SETUP COMPLETED';
PRINT '==============================================';
PRINT 'Database: UstadWhatsApp';
PRINT 'Tables: 6 tables created';
PRINT 'Procedures: 3 procedures created';
PRINT 'Triggers: 1 trigger created';
PRINT 'Test Data: 2 operators created';
PRINT '';
PRINT 'TEST ACCOUNTS:';
PRINT '  Admin: admin / admin123!';
PRINT '  Agent: agent / agent123!';
PRINT '';
PRINT 'NEXT STEPS:';
PRINT '1. Update WhatsApp API credentials in WhatsApp_FirmSettings';
PRINT '2. Start WhatsApp Webhook API service';
PRINT '3. Test login with Windows Forms client';
PRINT '==============================================';

GO
