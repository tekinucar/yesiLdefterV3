using System;
using System.Data;
using System.Threading.Tasks;
using System.Text.Json;
using yesildeftertest.Services;

namespace yesildeftertest.Integration
{
    /// <summary>
    /// Integration layer between existing Ustad modules and WhatsApp functionality
    /// Provides seamless integration with student management, exam planning, and payment tracking
    /// </summary>
    public class UstadModuleIntegration : tBase
    {
        #region tanımlar

        private readonly WhatsAppService _whatsAppService;
        private readonly tToolBox t;
        private readonly tWhatsAppSQLs sqls;

        #endregion

        #region constructor

        public UstadModuleIntegration(WhatsAppService whatsAppService)
        {
            _whatsAppService = whatsAppService ?? throw new ArgumentNullException(nameof(whatsAppService));
            t = new tToolBox();
            sqls = new tWhatsAppSQLs();
        }

        #endregion

        #region Student Management Integration

        /// <summary>
        /// Send WhatsApp notification for student application status
        /// Integrates with existing MtskAdayTalep procedures
        /// </summary>
        public async Task<bool> NotifyStudentApplicationStatus(int talepId, string status, string? customMessage = null)
        {
            try
            {
                // Get student info from existing Ustad tables
                var studentInfo = await GetStudentInfoByTalepId(talepId);
                if (studentInfo == null || string.IsNullOrEmpty(studentInfo.WhatsAppPhone))
                    return false;

                string message = customMessage ?? GenerateApplicationStatusMessage(status, studentInfo);
                
                // Send WhatsApp message
                bool success = await _whatsAppService.SendMessageAsync(studentInfo.WhatsAppPhone, message);
                
                if (success)
                {
                    // Log to existing Ustad audit system
                    await LogStudentNotification(talepId, "WhatsApp", message);
                }

                return success;
            }
            catch (Exception ex)
            {
                debugMessage($"Student notification error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Send document request via WhatsApp
        /// Integrates with existing MtskAdayBelgeler system
        /// </summary>
        public async Task<bool> RequestDocumentViaWhatsApp(int adayId, string documentType)
        {
            try
            {
                var student = await GetStudentInfoByAdayId(adayId);
                if (student == null) return false;

                string message = $"Merhaba {student.FullName},\n\n" +
                               $"Başvurunuzun tamamlanması için '{documentType}' belgesine ihtiyacımız var.\n\n" +
                               $"Belgeyi WhatsApp üzerinden gönderebilir veya okula getirebilirsiniz.\n\n" +
                               $"Teşekkürler,\nUstad Eğitim";

                return await _whatsAppService.SendMessageAsync(student.WhatsAppPhone, message);
            }
            catch (Exception ex)
            {
                debugMessage($"Document request error: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Exam Planning Integration

        /// <summary>
        /// Send exam schedule notifications via WhatsApp
        /// Integrates with existing exam planning procedures
        /// </summary>
        public async Task<bool> SendExamScheduleNotification(int sinavId)
        {
            try
            {
                // Get exam details from existing tables
                var examInfo = await GetExamInfo(sinavId);
                if (examInfo == null) return false;

                // Get enrolled students
                var students = await GetExamStudents(sinavId);
                
                foreach (var student in students)
                {
                    if (string.IsNullOrEmpty(student.WhatsAppPhone)) continue;

                    string message = $"Sınav Bildirimi\n\n" +
                                   $"Sınav: {examInfo.ExamName}\n" +
                                   $"Tarih: {examInfo.ExamDate:dd.MM.yyyy}\n" +
                                   $"Saat: {examInfo.ExamTime:HH:mm}\n" +
                                   $"Salon: {examInfo.ExamRoom}\n\n" +
                                   $"Sınavdan 30 dakika önce okulda bulununuz.\n\n" +
                                   $"Başarılar,\nUstad Eğitim";

                    await _whatsAppService.SendTemplateAsync(student.WhatsAppPhone, "exam_notification", "tr", 
                        new[] { examInfo.ExamName, examInfo.ExamDate.ToString("dd.MM.yyyy"), examInfo.ExamTime.ToString("HH:mm") });
                }

                return true;
            }
            catch (Exception ex)
            {
                debugMessage($"Exam notification error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Send exam results via WhatsApp
        /// </summary>
        public async Task<bool> SendExamResults(int adayId, int sinavId, bool isPassed, int score)
        {
            try
            {
                var student = await GetStudentInfoByAdayId(adayId);
                if (student == null) return false;

                var examInfo = await GetExamInfo(sinavId);
                if (examInfo == null) return false;

                string resultText = isPassed ? "GEÇTİNİZ" : "KALDI";
                string message = $"Sınav Sonucu\n\n" +
                               $"Öğrenci: {student.FullName}\n" +
                               $"Sınav: {examInfo.ExamName}\n" +
                               $"Puan: {score}\n" +
                               $"Sonuç: {resultText}\n\n" +
                               $"{(isPassed ? "Tebrikler! Başarıyla geçtiniz." : "Tekrar sınava girebilirsiniz.")}\n\n" +
                               $"Ustad Eğitim";

                return await _whatsAppService.SendMessageAsync(student.WhatsAppPhone, message);
            }
            catch (Exception ex)
            {
                debugMessage($"Exam results error: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Payment Integration

        /// <summary>
        /// Send payment reminders via WhatsApp
        /// Integrates with existing OnmOdemePlani system
        /// </summary>
        public async Task<bool> SendPaymentReminder(int adayId, decimal amount, DateTime dueDate)
        {
            try
            {
                var student = await GetStudentInfoByAdayId(adayId);
                if (student == null) return false;

                string message = $"Ödeme Hatırlatması\n\n" +
                               $"Sayın {student.FullName},\n\n" +
                               $"Ödeme Tutarı: {amount:C}\n" +
                               $"Vade Tarihi: {dueDate:dd.MM.yyyy}\n\n" +
                               $"Ödemenizi zamanında yapmanızı rica ederiz.\n\n" +
                               $"Ödeme için:\n" +
                               $"- Okul veznesine başvurabilirsiniz\n" +
                               $"- Banka havalesi yapabilirsiniz\n" +
                               $"- Online ödeme linkini kullanabilirsiniz\n\n" +
                               $"Teşekkürler,\nUstad Eğitim";

                return await _whatsAppService.SendTemplateAsync(student.WhatsAppPhone, "payment_reminder", "tr",
                    new[] { student.FullName, amount.ToString("C"), dueDate.ToString("dd.MM.yyyy") });
            }
            catch (Exception ex)
            {
                debugMessage($"Payment reminder error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Send payment confirmation via WhatsApp
        /// </summary>
        public async Task<bool> SendPaymentConfirmation(int adayId, decimal amount, string paymentMethod)
        {
            try
            {
                var student = await GetStudentInfoByAdayId(adayId);
                if (student == null) return false;

                string message = $"Ödeme Onayı\n\n" +
                               $"Sayın {student.FullName},\n\n" +
                               $"Ödemeniz başarıyla alınmıştır.\n\n" +
                               $"Ödeme Tutarı: {amount:C}\n" +
                               $"Ödeme Yöntemi: {paymentMethod}\n" +
                               $"Tarih: {DateTime.Now:dd.MM.yyyy HH:mm}\n\n" +
                               $"Makbuzunuz e-posta adresinize gönderilmiştir.\n\n" +
                               $"Teşekkürler,\nUstad Eğitim";

                return await _whatsAppService.SendMessageAsync(student.WhatsAppPhone, message);
            }
            catch (Exception ex)
            {
                debugMessage($"Payment confirmation error: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Organizational Integration

        /// <summary>
        /// Send organizational announcements via WhatsApp
        /// Integrates with existing organizational management
        /// </summary>
        public async Task<bool> SendOrganizationalAnnouncement(int firmId, string announcement, string? targetRole = null)
        {
            try
            {
                // Get target audience based on role and firm
                var recipients = await GetAnnouncementRecipients(firmId, targetRole);
                
                foreach (var recipient in recipients)
                {
                    if (string.IsNullOrEmpty(recipient.WhatsAppPhone)) continue;

                    string message = $"Duyuru\n\n" +
                                   $"Sayın {recipient.FullName},\n\n" +
                                   $"{announcement}\n\n" +
                                   $"Ustad Eğitim Yönetimi";

                    await _whatsAppService.SendMessageAsync(recipient.WhatsAppPhone, message);
                }

                return true;
            }
            catch (Exception ex)
            {
                debugMessage($"Organizational announcement error: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Missing Methods

        /// <summary>
        /// Get enrolled students for exam
        /// </summary>
        private async Task<List<StudentInfo>> GetExamStudents(int sinavId)
        {
            try
            {
                string sql = $@"
                    SELECT DISTINCT
                        a.Id, a.AdayAdi + ' ' + a.AdaySoyadi AS FullName,
                        a.AdayTelefon, a.AdayWhatsApp AS WhatsAppPhone,
                        a.AdayEMail, a.TcNo
                    FROM MtskSinavKayit sk
                    INNER JOIN MtskAday a ON sk.AdayId = a.Id
                    WHERE sk.SinavId = {sinavId} AND sk.IsActive = 1 AND a.IsActive = 1";

                DataSet ds = new DataSet();
                bool success = t.SQL_Read_Execute(tVariable.dBaseNo.UstadCrm, ds, ref sql, "ExamStudents", "GetExamStudents");
                
                var students = new List<StudentInfo>();
                if (success && t.IsNotNull(ds))
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        students.Add(new StudentInfo
                        {
                            Id = t.myInt32(row["Id"].ToString()),
                            FullName = row["FullName"].ToString(),
                            Phone = row["AdayTelefon"].ToString(),
                            WhatsAppPhone = row["WhatsAppPhone"].ToString(),
                            Email = row["AdayEMail"].ToString(),
                            TcNo = row["TcNo"].ToString()
                        });
                    }
                }

                return students;
            }
            catch (Exception ex)
            {
                debugMessage($"Get exam students error: {ex.Message}");
                return new List<StudentInfo>();
            }
        }

        /// <summary>
        /// Get announcement recipients
        /// </summary>
        private async Task<List<StudentInfo>> GetAnnouncementRecipients(int firmId, string? targetRole = null)
        {
            try
            {
                string sql = $@"
                    SELECT DISTINCT
                        a.Id, a.AdayAdi + ' ' + a.AdaySoyadi AS FullName,
                        a.AdayTelefon, a.AdayWhatsApp AS WhatsAppPhone,
                        a.AdayEMail, a.TcNo
                    FROM MtskAday a
                    WHERE a.FirmId = {firmId} 
                      AND a.IsActive = 1
                      AND a.AdayWhatsApp IS NOT NULL
                      AND a.AdayWhatsApp != ''";

                DataSet ds = new DataSet();
                bool success = t.SQL_Read_Execute(tVariable.dBaseNo.UstadCrm, ds, ref sql, "AnnouncementRecipients", "GetAnnouncementRecipients");
                
                var recipients = new List<StudentInfo>();
                if (success && t.IsNotNull(ds))
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        recipients.Add(new StudentInfo
                        {
                            Id = t.myInt32(row["Id"].ToString()),
                            FullName = row["FullName"].ToString(),
                            Phone = row["AdayTelefon"].ToString(),
                            WhatsAppPhone = row["WhatsAppPhone"].ToString(),
                            Email = row["AdayEMail"].ToString(),
                            TcNo = row["TcNo"].ToString()
                        });
                    }
                }

                return recipients;
            }
            catch (Exception ex)
            {
                debugMessage($"Get announcement recipients error: {ex.Message}");
                return new List<StudentInfo>();
            }
        }

        #endregion

        #region Data Access Methods

        /// <summary>
        /// Get student information from existing MtskAday table
        /// </summary>
        private async Task<StudentInfo> GetStudentInfoByAdayId(int adayId)
        {
            try
            {
                string sql = $@"
                    SELECT 
                        a.Id, a.AdayAdi + ' ' + a.AdaySoyadi AS FullName,
                        a.AdayTelefon, a.AdayWhatsApp AS WhatsAppPhone,
                        a.AdayEMail, a.TcNo
                    FROM MtskAday a 
                    WHERE a.Id = {adayId} AND a.IsActive = 1";

                DataSet ds = new DataSet();
                bool success = t.SQL_Read_Execute(tVariable.dBaseNo.UstadCrm, ds, ref sql, "StudentInfo", "GetStudent");
                
                if (success && t.IsNotNull(ds))
                {
                    var row = ds.Tables[0].Rows[0];
                    return new StudentInfo
                    {
                        Id = t.myInt32(row["Id"].ToString()),
                        FullName = row["FullName"].ToString(),
                        Phone = row["AdayTelefon"].ToString(),
                        WhatsAppPhone = row["WhatsAppPhone"].ToString(),
                        Email = row["AdayEMail"].ToString(),
                        TcNo = row["TcNo"].ToString()
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                debugMessage($"Get student info error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get student information by TalepId
        /// </summary>
        private async Task<StudentInfo> GetStudentInfoByTalepId(int talepId)
        {
            try
            {
                string sql = $@"
                    SELECT 
                        a.Id, a.AdayAdi + ' ' + a.AdaySoyadi AS FullName,
                        a.AdayTelefon, a.AdayWhatsApp AS WhatsAppPhone,
                        a.AdayEMail, a.TcNo
                    FROM MtskAdayTalep t
                    INNER JOIN MtskAday a ON t.AdayId = a.Id
                    WHERE t.Id = {talepId} AND t.IsActive = 1 AND a.IsActive = 1";

                DataSet ds = new DataSet();
                bool success = t.SQL_Read_Execute(tVariable.dBaseNo.UstadCrm, ds, ref sql, "StudentInfo", "GetStudentByTalep");
                
                if (success && t.IsNotNull(ds))
                {
                    var row = ds.Tables[0].Rows[0];
                    return new StudentInfo
                    {
                        Id = t.myInt32(row["Id"].ToString()),
                        FullName = row["FullName"].ToString(),
                        Phone = row["AdayTelefon"].ToString(),
                        WhatsAppPhone = row["WhatsAppPhone"].ToString(),
                        Email = row["AdayEMail"].ToString(),
                        TcNo = row["TcNo"].ToString()
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                debugMessage($"Get student info by talep error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get exam information from existing exam tables
        /// </summary>
        private async Task<ExamInfo> GetExamInfo(int sinavId)
        {
            try
            {
                string sql = $@"
                    SELECT 
                        s.Id, s.SinavAdi AS ExamName,
                        s.SinavTarihi AS ExamDate, s.SinavSaati AS ExamTime,
                        s.SinavSalonu AS ExamRoom, s.SinavTuru AS ExamType
                    FROM MtskSinavETeorik s
                    WHERE s.Id = {sinavId} AND s.IsActive = 1
                    
                    UNION ALL
                    
                    SELECT 
                        u.Id, u.UygulamaAdi AS ExamName,
                        u.UygulamaTarihi AS ExamDate, u.UygulamaSaati AS ExamTime,
                        u.UygulamaSalonu AS ExamRoom, 'Uygulama' AS ExamType
                    FROM MtskSinavUygulama u
                    WHERE u.Id = {sinavId} AND u.IsActive = 1";

                DataSet ds = new DataSet();
                bool success = t.SQL_Read_Execute(tVariable.dBaseNo.UstadCrm, ds, ref sql, "ExamInfo", "GetExam");
                
                if (success && t.IsNotNull(ds))
                {
                    var row = ds.Tables[0].Rows[0];
                    return new ExamInfo
                    {
                        Id = t.myInt32(row["Id"].ToString()),
                        ExamName = row["ExamName"].ToString(),
                        ExamDate = t.myDateTime(row["ExamDate"].ToString()),
                        ExamTime = TimeSpan.Parse(row["ExamTime"].ToString()),
                        ExamRoom = row["ExamRoom"].ToString(),
                        ExamType = row["ExamType"].ToString()
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                debugMessage($"Get exam info error: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Payment Integration

        /// <summary>
        /// Process payment notifications with WhatsApp integration
        /// Integrates with existing OnmOdemePlani system
        /// </summary>
        public async Task<bool> ProcessPaymentNotifications()
        {
            try
            {
                // Get overdue payments from existing system
                string sql = @"
                    SELECT DISTINCT
                        a.Id AS AdayId, a.AdayAdi + ' ' + a.AdaySoyadi AS FullName,
                        a.AdayWhatsApp AS WhatsAppPhone,
                        op.OdemeTutari AS Amount, op.VadeTarihi AS DueDate,
                        DATEDIFF(DAY, op.VadeTarihi, GETDATE()) AS DaysOverdue
                    FROM OnmOdemePlani op
                    INNER JOIN MtskAdayTalep t ON op.TalepId = t.Id
                    INNER JOIN MtskAday a ON t.AdayId = a.Id
                    WHERE op.OdemeDurumu = 'Beklemede'
                      AND op.VadeTarihi < GETDATE()
                      AND a.AdayWhatsApp IS NOT NULL
                      AND a.AdayWhatsApp != ''
                      AND a.IsActive = 1";

                DataSet ds = new DataSet();
                bool success = t.SQL_Read_Execute(tVariable.dBaseNo.UstadCrm, ds, ref sql, "OverduePayments", "GetOverduePayments");

                if (success && t.IsNotNull(ds))
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var student = new StudentInfo
                        {
                            Id = t.myInt32(row["AdayId"].ToString()),
                            FullName = row["FullName"].ToString(),
                            WhatsAppPhone = row["WhatsAppPhone"].ToString()
                        };

                        decimal amount = Convert.ToDecimal(row["Amount"]);
                        DateTime dueDate = t.myDateTime(row["DueDate"].ToString());
                        int daysOverdue = t.myInt32(row["DaysOverdue"].ToString());

                        await SendPaymentReminder(student, amount, dueDate, daysOverdue);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                debugMessage($"Payment notifications error: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Cross-Platform Integration

        /// <summary>
        /// Sync conversation data with web and mobile platforms
        /// </summary>
        public async Task<bool> SyncConversationData(int conversationId)
        {
            try
            {
                // This method would sync conversation data across platforms
                // Implementation would depend on your specific web/mobile API structure
                
                var conversationData = new
                {
                    ConversationId = conversationId,
                    Platform = "Desktop",
                    SyncTimestamp = DateTime.UtcNow,
                    OperatorId = GlobalVariables.tWhatsAppOperator.OperatorId
                };

                // Send sync request to web/mobile APIs
                string syncJson = JsonSerializer.Serialize(conversationData);
                string response = await t.HTTP_POST($"{GlobalVariables.tWhatsAppConfig.ApiBaseUrl}/sync/conversation", syncJson);

                return !string.IsNullOrEmpty(response);
            }
            catch (Exception ex)
            {
                debugMessage($"Conversation sync error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Generate QR code for cross-platform authentication
        /// </summary>
        public async Task<string> GenerateQRCodeForCrossPlatformAuth()
        {
            try
            {
                var qrData = new
                {
                    OperatorId = GlobalVariables.tWhatsAppOperator.OperatorId,
                    FirmId = GlobalVariables.tWhatsAppOperator.FirmId,
                    Platform = "Desktop",
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5)
                };

                string qrJson = JsonSerializer.Serialize(qrData);
                string response = await t.HTTP_POST($"{GlobalVariables.tWhatsAppConfig.ApiBaseUrl}/auth/qr-generate", qrJson);

                if (!string.IsNullOrEmpty(response))
                {
                    var qrResponse = JsonSerializer.Deserialize<QRCodeResponse>(response);
                    return qrResponse?.QRCode;
                }

                return null;
            }
            catch (Exception ex)
            {
                debugMessage($"QR code generation error: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Generate application status message
        /// </summary>
        private string GenerateApplicationStatusMessage(string status, StudentInfo student)
        {
            return status switch
            {
                "Approved" => $"Merhaba {student.FullName},\n\nBaşvurunuz ONAYLANDI! Eğitim programınız başlıyor.\n\nDetaylar için okula başvurunuz.\n\nUstad Eğitim",
                "Rejected" => $"Merhaba {student.FullName},\n\nBaşvurunuz değerlendirme aşamasında. Ek bilgi gerekebilir.\n\nLütfen okula başvurunuz.\n\nUstad Eğitim",
                "Pending" => $"Merhaba {student.FullName},\n\nBaşvurunuz inceleniyor. Sonuç kısa sürede bildirilecek.\n\nUstad Eğitim",
                _ => $"Merhaba {student.FullName},\n\nBaşvuru durumunuz: {status}\n\nDetaylar için okula başvurunuz.\n\nUstad Eğitim"
            };
        }

        /// <summary>
        /// Log student notification to existing audit system
        /// </summary>
        private async Task LogStudentNotification(int talepId, string channel, string message)
        {
            try
            {
                string logSql = $@"
                    INSERT INTO MtskAdayTakip (
                        TalepId, TakipTarihi, TakipNotu, TakipTuru, 
                        UserId, IsActive, CreatedAt
                    )
                    VALUES (
                        {talepId}, GETDATE(), 
                        'WhatsApp Bildirimi: {message.Replace("'", "''")}',
                        'WhatsApp', {GlobalVariables.tUser.UserId}, 1, GETDATE()
                    )";

                t.SQL_Execute(tVariable.dBaseNo.UstadCrm, logSql, "LogWhatsAppNotification");
            }
            catch (Exception ex)
            {
                debugMessage($"Log notification error: {ex.Message}");
            }
        }

        /// <summary>
        /// Send payment reminder with overdue calculation
        /// </summary>
        private async Task<bool> SendPaymentReminder(StudentInfo student, decimal amount, DateTime dueDate, int daysOverdue)
        {
            string urgencyText = daysOverdue switch
            {
                <= 0 => "Ödeme vade tarihiniz yaklaşıyor.",
                <= 7 => "Ödemeniz gecikmiştir.",
                <= 30 => "Ödemeniz önemli ölçüde gecikmiştir.",
                _ => "Ödemeniz kritik seviyede gecikmiştir. Acil ödeme gereklidir."
            };

            string message = $"Ödeme Hatırlatması\n\n" +
                           $"Sayın {student.FullName},\n\n" +
                           $"{urgencyText}\n\n" +
                           $"Tutar: {amount:C}\n" +
                           $"Vade: {dueDate:dd.MM.yyyy}\n" +
                           $"Gecikme: {Math.Max(0, daysOverdue)} gün\n\n" +
                           $"Lütfen en kısa sürede ödemenizi yapınız.\n\n" +
                           $"Ustad Eğitim";

            return await _whatsAppService.SendMessageAsync(student.WhatsAppPhone, message);
        }

        #endregion

        #region Data Models

        public class StudentInfo
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Phone { get; set; }
            public string WhatsAppPhone { get; set; }
            public string Email { get; set; }
            public string TcNo { get; set; }
        }

        public class ExamInfo
        {
            public int Id { get; set; }
            public string ExamName { get; set; }
            public DateTime ExamDate { get; set; }
            public TimeSpan ExamTime { get; set; }
            public string ExamRoom { get; set; }
            public string ExamType { get; set; }
        }

        public class QRCodeResponse
        {
            public string QRCode { get; set; }
            public DateTime ExpiresAt { get; set; }
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            t?.Dispose();
            sqls?.Dispose();
        }

        #endregion
    }
}
