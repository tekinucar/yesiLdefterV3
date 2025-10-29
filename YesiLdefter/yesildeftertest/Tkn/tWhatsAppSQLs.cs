using System;
using System.Data;

namespace yesildeftertest
{
    /// <summary>
    /// WhatsApp SQL sorgular - Tekin's pattern (tSQLs benzeri)
    /// Güvenli SQL sorgu hazırlama ve yönetimi
    /// </summary>
    public class tWhatsAppSQLs : tBase
    {
        #region tanımlar - Tekin's pattern

        private tToolBox t;

        #endregion

        #region constructor

        public tWhatsAppSQLs()
        {
            t = new tToolBox();
            preparingDefaultValues();
        }

        #endregion

        #region Authentication SQLs - Tekin's pattern

        /// <summary>
        /// Operatör login SQL hazırla - Tekin's pattern (preparingUstadUsersSql benzeri)
        /// </summary>
        public string preparingWhatsAppOperatorLoginSql(string userName, string password, int operatorId)
        {
            string sql = string.Empty;

            try
            {
                if (operatorId == 0 && isNotNull(userName))
                {
                    // Login kontrolü için SQL - Tekin's pattern
                    sql = $@"
                        EXEC [prc_WhatsApp_OperatorLogin] 
                            @UserName = '{userName.Replace("'", "''")}',
                            @Password = '{password.Replace("'", "''")}'";
                }
                else if (operatorId > 0)
                {
                    // Operatör bilgilerini getir - Tekin's pattern
                    sql = $@"
                        SELECT 
                            [Id], [FirmId], [UserName], [FullName], [UserEMail], 
                            [UserPhone], [UserRole], [IsActive], [CreatedAt]
                        FROM [WhatsApp_Operators] 
                        WHERE [Id] = {operatorId} AND [IsActive] = 1";
                }

                debugMessage($"WhatsApp Operator SQL prepared: {sql.Substring(0, Math.Min(100, sql.Length))}...");
            }
            catch (Exception ex)
            {
                errorMessage("SQL hazırlama hatası", ex);
            }

            return sql;
        }

        /// <summary>
        /// Şifre sıfırlama SQL - Tekin's pattern
        /// </summary>
        public string preparingPasswordResetSql(string operation, string userName = "", string token = "", int operatorId = 0)
        {
            string sql = string.Empty;

            try
            {
                switch (operation.ToUpper())
                {
                    case "START_RESET":
                        // Şifre sıfırlama başlat
                        sql = $@"
                            SELECT [Id], [UserEMail], [UserPhone] 
                            FROM [WhatsApp_Operators] 
                            WHERE [UserName] = '{userName.Replace("'", "''")}' 
                              AND [IsActive] = 1";
                        break;

                    case "VERIFY_TOKEN":
                        // Token doğrula
                        sql = $@"
                            SELECT rt.[OperatorId]
                            FROM [WhatsApp_PasswordResetTokens] rt
                            INNER JOIN [WhatsApp_Operators] o ON rt.[OperatorId] = o.[Id]
                            WHERE rt.[ResetToken] = '{token.Replace("'", "''")}' 
                              AND rt.[IsUsed] = 0 
                              AND rt.[TokenExpiry] > SYSUTCDATETIME()
                              AND o.[IsActive] = 1";
                        break;

                    case "UPDATE_PASSWORD":
                        // Şifre güncelle (bu kısım API tarafında PBKDF2 ile yapılacak)
                        sql = $@"
                            UPDATE [WhatsApp_Operators] 
                            SET [FailedCount] = 0, 
                                [LockoutEnd] = NULL, 
                                [UpdatedAt] = SYSUTCDATETIME()
                            WHERE [Id] = {operatorId}";
                        break;

                    case "MARK_TOKEN_USED":
                        // Token'ı kullanıldı olarak işaretle
                        sql = $@"
                            UPDATE [WhatsApp_PasswordResetTokens] 
                            SET [IsUsed] = 1 
                            WHERE [ResetToken] = '{token.Replace("'", "''")}'";
                        break;
                }

                debugMessage($"Password Reset SQL prepared: {operation}");
            }
            catch (Exception ex)
            {
                errorMessage("Password Reset SQL hazırlama hatası", ex);
            }

            return sql;
        }

        #endregion

        #region Conversation SQLs - Tekin's pattern

        /// <summary>
        /// Konuşma SQL'leri hazırla - Tekin's pattern
        /// </summary>
        public string preparingConversationSql(string operation, int conversationId = 0, string customerNumber = "", int operatorId = 0, int firmId = 1)
        {
            string sql = string.Empty;

            try
            {
                switch (operation.ToUpper())
                {
                    case "GET_CONVERSATIONS":
                        // Operatör konuşmaları
                        sql = $@"
                            EXEC [prc_WhatsApp_GetOperatorConversations] 
                                @OperatorId = {operatorId},
                                @FirmId = {firmId}";
                        break;

                    case "GET_CONVERSATION_MESSAGES":
                        // Konuşma mesajları
                        sql = $@"
                            EXEC [prc_WhatsApp_GetConversationMessages] 
                                @ConversationId = {conversationId},
                                @OperatorId = {operatorId}";
                        break;

                    case "CREATE_CONVERSATION":
                        // Yeni konuşma oluştur
                        sql = $@"
                            IF NOT EXISTS (SELECT 1 FROM [WhatsApp_Conversations] 
                                          WHERE [CustomerNumber] = '{customerNumber.Replace("'", "''")}' 
                                            AND [FirmId] = {firmId})
                            BEGIN
                                INSERT INTO [WhatsApp_Conversations] (
                                    [FirmId], [CustomerNumber], [ConversationStatus], 
                                    [LastMessageAt], [IsActive], [CreatedAt], [UpdatedAt]
                                )
                                VALUES (
                                    {firmId}, '{customerNumber.Replace("'", "''")}', 'Open',
                                    SYSUTCDATETIME(), 1, SYSUTCDATETIME(), SYSUTCDATETIME()
                                )
                            END
                            
                            SELECT [Id] FROM [WhatsApp_Conversations] 
                            WHERE [CustomerNumber] = '{customerNumber.Replace("'", "''")}' 
                              AND [FirmId] = {firmId}";
                        break;

                    case "ASSIGN_CONVERSATION":
                        // Konuşma ata
                        sql = $@"
                            UPDATE [WhatsApp_Conversations] 
                            SET [AssignedOperatorId] = {(operatorId > 0 ? operatorId.ToString() : "NULL")}, 
                                [UpdatedAt] = SYSUTCDATETIME()
                            WHERE [Id] = {conversationId}";
                        break;

                    case "UPDATE_STATUS":
                        // Konuşma durumu güncelle
                        sql = $@"
                            UPDATE [WhatsApp_Conversations] 
                            SET [ConversationStatus] = 'Open', 
                                [UpdatedAt] = SYSUTCDATETIME()
                            WHERE [Id] = {conversationId}";
                        break;
                }

                debugMessage($"Conversation SQL prepared: {operation}");
            }
            catch (Exception ex)
            {
                errorMessage("Conversation SQL hazırlama hatası", ex);
            }

            return sql;
        }

        #endregion

        #region Message SQLs - Tekin's pattern

        /// <summary>
        /// Mesaj SQL'leri hazırla - Tekin's pattern
        /// </summary>
        public string preparingMessageSql(string operation, int conversationId = 0, string messageBody = "", 
            string messageDirection = "Out", string messageType = "text", int operatorId = 0, string whatsAppMessageId = "")
        {
            string sql = string.Empty;

            try
            {
                switch (operation.ToUpper())
                {
                    case "INSERT_MESSAGE":
                        // Yeni mesaj ekle
                        sql = $@"
                            INSERT INTO [WhatsApp_Messages] (
                                [ConversationId], [WhatsAppMessageId], [MessageDirection], 
                                [MessageType], [MessageBody], [OperatorId], [IsRead], 
                                [IsActive], [CreatedAt]
                            )
                            VALUES (
                                {conversationId}, 
                                {(isNotNull(whatsAppMessageId) ? $"'{whatsAppMessageId.Replace("'", "''")}'" : "NULL")},
                                '{messageDirection}', 
                                '{messageType}', 
                                '{messageBody.Replace("'", "''")}', 
                                {(operatorId > 0 ? operatorId.ToString() : "NULL")}, 
                                {(messageDirection == "Out" ? "1" : "0")}, 
                                1, 
                                SYSUTCDATETIME()
                            )";
                        break;

                    case "MARK_AS_READ":
                        // Mesajları okundu işaretle
                        sql = $@"
                            UPDATE [WhatsApp_Messages] 
                            SET [IsRead] = 1 
                            WHERE [ConversationId] = {conversationId} 
                              AND [MessageDirection] = 'In' 
                              AND [IsRead] = 0 
                              AND [IsActive] = 1";
                        break;

                    case "GET_UNREAD_COUNT":
                        // Okunmamış mesaj sayısı
                        sql = $@"
                            SELECT COUNT(*) AS [UnreadCount]
                            FROM [WhatsApp_Messages] 
                            WHERE [ConversationId] = {conversationId} 
                              AND [MessageDirection] = 'In' 
                              AND [IsRead] = 0 
                              AND [IsActive] = 1";
                        break;

                    case "UPDATE_CONVERSATION_TIME":
                        // Konuşma son mesaj zamanını güncelle
                        sql = $@"
                            UPDATE [WhatsApp_Conversations] 
                            SET [LastMessageAt] = SYSUTCDATETIME(),
                                [UpdatedAt] = SYSUTCDATETIME()
                            WHERE [Id] = {conversationId}";
                        break;
                }

                debugMessage($"Message SQL prepared: {operation}");
            }
            catch (Exception ex)
            {
                errorMessage("Message SQL hazırlama hatası", ex);
            }

            return sql;
        }

        #endregion

        #region Audit Log SQLs - Tekin's pattern

        /// <summary>
        /// Audit log SQL hazırla - Tekin's pattern
        /// </summary>
        public string preparingAuditLogSql(int firmId, int operatorId, string action, string details = "", 
            string entityType = "", string entityId = "", string ipAddress = "", string userAgent = "")
        {
            string sql = string.Empty;

            try
            {
                sql = $@"
                    INSERT INTO [WhatsApp_AuditLogs] (
                        [FirmId], [OperatorId], [LogAction], [LogEntityType], 
                        [LogEntityId], [LogDetails], [LogIpAddress], [LogUserAgent], 
                        [IsActive], [CreatedAt]
                    )
                    VALUES (
                        {firmId}, 
                        {operatorId}, 
                        '{action.Replace("'", "''")}', 
                        {(isNotNull(entityType) ? $"'{entityType.Replace("'", "''")}'" : "NULL")},
                        {(isNotNull(entityId) ? $"'{entityId.Replace("'", "''")}'" : "NULL")},
                        {(isNotNull(details) ? $"'{details.Replace("'", "''")}'" : "NULL")},
                        {(isNotNull(ipAddress) ? $"'{ipAddress.Replace("'", "''")}'" : "NULL")},
                        {(isNotNull(userAgent) ? $"'{userAgent.Replace("'", "''")}'" : "NULL")},
                        1, 
                        SYSUTCDATETIME()
                    )";

                debugMessage($"Audit Log SQL prepared: {action}");
            }
            catch (Exception ex)
            {
                errorMessage("Audit Log SQL hazırlama hatası", ex);
            }

            return sql;
        }

        /// <summary>
        /// Audit log listesi SQL - Tekin's pattern
        /// </summary>
        public string preparingAuditLogListSql(int firmId, int operatorId = 0, string action = "", int pageSize = 100, int pageOffset = 0)
        {
            string sql = string.Empty;

            try
            {
                sql = $@"
                    SELECT 
                        al.[Id],
                        al.[FirmId],
                        al.[OperatorId],
                        o.[FullName] AS [OperatorName],
                        al.[LogAction],
                        al.[LogEntityType],
                        al.[LogEntityId],
                        al.[LogDetails],
                        al.[LogIpAddress],
                        al.[CreatedAt]
                    FROM [WhatsApp_AuditLogs] al
                    LEFT JOIN [WhatsApp_Operators] o ON al.[OperatorId] = o.[Id]
                    WHERE al.[FirmId] = {firmId}
                      AND al.[IsActive] = 1";

                // Filtreler - Tekin's pattern
                if (operatorId > 0)
                    sql += $" AND al.[OperatorId] = {operatorId}";

                if (isNotNull(action))
                    sql += $" AND al.[LogAction] = '{action.Replace("'", "''")}'";

                sql += $@"
                    ORDER BY al.[CreatedAt] DESC
                    OFFSET {pageOffset} ROWS FETCH NEXT {pageSize} ROWS ONLY";

                debugMessage($"Audit Log List SQL prepared");
            }
            catch (Exception ex)
            {
                errorMessage("Audit Log List SQL hazırlama hatası", ex);
            }

            return sql;
        }

        #endregion

        #region Operator Management SQLs - Tekin's pattern

        /// <summary>
        /// Operatör yönetimi SQL - Tekin's pattern
        /// </summary>
        public string preparingOperatorManagementSql(string operation, int operatorId = 0, int firmId = 1, 
            string userName = "", string fullName = "", string userRole = "Agent", bool isActive = true)
        {
            string sql = string.Empty;

            try
            {
                switch (operation.ToUpper())
                {
                    case "GET_OPERATORS":
                        // Operatör listesi
                        sql = $@"
                            SELECT 
                                [Id], [UserName], [FullName], [UserEMail], [UserPhone],
                                [UserRole], [FailedCount], [LockoutEnd], [IsActive], 
                                [CreatedAt], [UpdatedAt]
                            FROM [WhatsApp_Operators] 
                            WHERE [FirmId] = {firmId}
                            ORDER BY [FullName]";
                        break;

                    case "UPDATE_ROLE":
                        // Rol güncelle
                        sql = $@"
                            UPDATE [WhatsApp_Operators] 
                            SET [UserRole] = '{userRole}', 
                                [UpdatedAt] = SYSUTCDATETIME()
                            WHERE [Id] = {operatorId}";
                        break;

                    case "UPDATE_ACTIVE":
                        // Aktiflik durumu güncelle
                        sql = $@"
                            UPDATE [WhatsApp_Operators] 
                            SET [IsActive] = {(isActive ? "1" : "0")}, 
                                [UpdatedAt] = SYSUTCDATETIME()
                            WHERE [Id] = {operatorId}";
                        break;

                    case "UNLOCK_OPERATOR":
                        // Operatör kilidini aç
                        sql = $@"
                            UPDATE [WhatsApp_Operators] 
                            SET [FailedCount] = 0, 
                                [LockoutEnd] = NULL, 
                                [UpdatedAt] = SYSUTCDATETIME()
                            WHERE [Id] = {operatorId}";
                        break;

                    case "INCREMENT_FAILED":
                        // Başarısız giriş sayısını artır
                        sql = $@"
                            UPDATE [WhatsApp_Operators] 
                            SET [FailedCount] = [FailedCount] + 1,
                                [LockoutEnd] = CASE 
                                    WHEN [FailedCount] + 1 >= 5 THEN DATEADD(MINUTE, 15, SYSUTCDATETIME())
                                    ELSE [LockoutEnd] 
                                END,
                                [UpdatedAt] = SYSUTCDATETIME()
                            WHERE [Id] = {operatorId}";
                        break;
                }

                debugMessage($"Operator Management SQL prepared: {operation}");
            }
            catch (Exception ex)
            {
                errorMessage("Operator Management SQL hazırlama hatası", ex);
            }

            return sql;
        }

        #endregion

        #region Statistics SQLs - Tekin's pattern

        /// <summary>
        /// İstatistik SQL'leri - Tekin's pattern
        /// </summary>
        public string preparingStatisticsSql(string operation, int firmId = 1, int operatorId = 0, DateTime? startDate = null, DateTime? endDate = null)
        {
            string sql = string.Empty;

            try
            {
                // Tarih filtreleri - Tekin's pattern
                string dateFilter = "";
                if (startDate.HasValue && endDate.HasValue)
                {
                    dateFilter = $" AND [CreatedAt] BETWEEN '{startDate.Value:yyyy-MM-dd}' AND '{endDate.Value:yyyy-MM-dd 23:59:59}'";
                }

                switch (operation.ToUpper())
                {
                    case "DAILY_MESSAGES":
                        // Günlük mesaj istatistikleri
                        sql = $@"
                            SELECT 
                                CAST([CreatedAt] AS DATE) AS [MessageDate],
                                COUNT(*) AS [MessageCount],
                                SUM(CASE WHEN [MessageDirection] = 'In' THEN 1 ELSE 0 END) AS [IncomingCount],
                                SUM(CASE WHEN [MessageDirection] = 'Out' THEN 1 ELSE 0 END) AS [OutgoingCount]
                            FROM [WhatsApp_Messages] m
                            INNER JOIN [WhatsApp_Conversations] c ON m.[ConversationId] = c.[Id]
                            WHERE c.[FirmId] = {firmId}
                              AND m.[IsActive] = 1
                              {dateFilter}
                            GROUP BY CAST([CreatedAt] AS DATE)
                            ORDER BY [MessageDate] DESC";
                        break;

                    case "OPERATOR_PERFORMANCE":
                        // Operatör performansı
                        sql = $@"
                            SELECT 
                                o.[Id],
                                o.[FullName],
                                COUNT(m.[Id]) AS [MessagesSent],
                                COUNT(DISTINCT c.[Id]) AS [ConversationsHandled],
                                AVG(DATEDIFF(MINUTE, m.[CreatedAt], 
                                    (SELECT MIN(m2.[CreatedAt]) 
                                     FROM [WhatsApp_Messages] m2 
                                     WHERE m2.[ConversationId] = m.[ConversationId] 
                                       AND m2.[MessageDirection] = 'Out' 
                                       AND m2.[CreatedAt] > m.[CreatedAt]))) AS [AvgResponseTime]
                            FROM [WhatsApp_Operators] o
                            LEFT JOIN [WhatsApp_Messages] m ON o.[Id] = m.[OperatorId] AND m.[MessageDirection] = 'Out'
                            LEFT JOIN [WhatsApp_Conversations] c ON m.[ConversationId] = c.[Id]
                            WHERE o.[FirmId] = {firmId}
                              AND o.[IsActive] = 1
                              {dateFilter}
                            GROUP BY o.[Id], o.[FullName]
                            ORDER BY [MessagesSent] DESC";
                        break;

                    case "CONVERSATION_SUMMARY":
                        // Konuşma özeti
                        sql = $@"
                            SELECT 
                                COUNT(*) AS [TotalConversations],
                                SUM(CASE WHEN [ConversationStatus] = 'Open' THEN 1 ELSE 0 END) AS [OpenConversations],
                                SUM(CASE WHEN [ConversationStatus] = 'Closed' THEN 1 ELSE 0 END) AS [ClosedConversations],
                                SUM(CASE WHEN [AssignedOperatorId] IS NULL THEN 1 ELSE 0 END) AS [UnassignedConversations]
                            FROM [WhatsApp_Conversations]
                            WHERE [FirmId] = {firmId}
                              AND [IsActive] = 1
                              {dateFilter}";
                        break;
                }

                debugMessage($"Statistics SQL prepared: {operation}");
            }
            catch (Exception ex)
            {
                errorMessage("Statistics SQL hazırlama hatası", ex);
            }

            return sql;
        }

        #endregion

        #region Utility Methods - Tekin's pattern

        /// <summary>
        /// SQL injection koruması - Tekin's pattern
        /// </summary>
        public string escapeSqlString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return input.Replace("'", "''")
                       .Replace("--", "")
                       .Replace(";", "")
                       .Replace("/*", "")
                       .Replace("*/", "")
                       .Replace("xp_", "")
                       .Replace("sp_", "");
        }

        /// <summary>
        /// Phone number formatla - E.164 format
        /// </summary>
        public string formatPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return "";

            // Sadece rakamları al
            string cleaned = new string(phoneNumber.Where(char.IsDigit).ToArray());

            // Türkiye numarası kontrolü
            if (cleaned.StartsWith("0") && cleaned.Length == 11)
            {
                cleaned = "90" + cleaned.Substring(1);
            }
            else if (!cleaned.StartsWith("90") && cleaned.Length == 10)
            {
                cleaned = "90" + cleaned;
            }

            return cleaned;
        }

        /// <summary>
        /// DateTime SQL formatı - Tekin's pattern
        /// </summary>
        public string formatDateTimeForSql(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            t?.Dispose();
        }

        #endregion
    }
}
