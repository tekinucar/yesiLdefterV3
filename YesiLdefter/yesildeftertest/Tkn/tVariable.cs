using System;
using System.Collections.Generic;
using System.Data;

namespace yesildeftertest
{
    /// <summary>
    /// Tekin's variable pattern - tVariable (v static instance)
    /// Global değişkenler ve yapılandırmalar
    /// </summary>
    public class tVariable
    {
        #region tanımlar - Tekin's pattern

        // Tekin's naming convention
        public string ENTER = "\r\n";
        public string ENTER2 = "\r\n\r\n";

        // Application states - Tekin's pattern
        public bool SP_UserLOGIN = false;
        public bool SP_UserIN = false;
        public bool SP_ApplicationExit = false;
        public bool SP_Debug = false;

        // WhatsApp specific variables
        public bool SP_WhatsAppConnected = false;
        public bool SP_WhatsAppInitialized = false;

        // Registry path - Tekin's pattern
        public string registryPath = "Software\\Üstad\\YesiLdefter\\WhatsApp";

        // EXE arguments - Tekin's pattern
        public string EXE_Arguments = string.Empty;

        #endregion

        #region Database Types - Tekin's pattern

        public enum dBaseType
        {
            MSSQL = 1,
            MySQL = 2,
            SQLite = 3
        }

        public enum dBaseNo
        {
            UstadCrm = 1,
            UstadWhatsApp = 2,
            Local = 3
        }

        #endregion

        #region User Information - Tekin's pattern

        public class tUser
        {
            public int UserId = 0;
            public string UserEMail = string.Empty;
            public string UserKey = string.Empty;
            public string UserFullName = string.Empty;
            public string UserRole = string.Empty;
            public int FirmId = 0;
            public bool IsActive = false;
        }

        public tUser tUserInfo = new tUser();

        #endregion

        #region WhatsApp User Information

        public class tWhatsAppUser
        {
            public int OperatorId = 0;
            public string OperatorUserName = string.Empty;
            public string OperatorFullName = string.Empty;
            public string OperatorRole = string.Empty; // Admin, Agent
            public string JwtToken = string.Empty;
            public DateTime TokenExpiry = DateTime.MinValue;
            public int FirmId = 0;
            public bool IsConnected = false;
            public string TurnstileToken = string.Empty; // For Cloudflare Turnstile
            public bool IsAuthenticated = false;
            public string FirmGUID = string.Empty;
            public string UserGUID = string.Empty;
        }

        public tWhatsAppUser tWhatsAppOperator = new tWhatsAppUser();

        #endregion

        #region Registry Information - Tekin's pattern

        public class tUserRegister
        {
            public string UserLastLoginEMail = string.Empty;
            public string UserLastKey = string.Empty;
            public bool UserRemember = false;
            public int UserLastFirmId = 0;
            public List<string> eMailList = new List<string>();
            
            // WhatsApp specific
            public string WhatsAppApiUrl = "http://localhost:5000";
            public bool WhatsAppAutoConnect = false;
            public string WhatsAppLastOperator = string.Empty;
        }

        public tUserRegister userRegister = new tUserRegister();

        #endregion

        #region Database Connection - Tekin's pattern

        public class active_DB
        {
            public dBaseType projectDBType = dBaseType.MSSQL;
            public string managerDBName = string.Empty;
            public string projectDBName = string.Empty;
            public string managerMSSQLConn = string.Empty;
            public string projectMSSQLConn = string.Empty;
            
            // WhatsApp Database
            public string whatsAppDBName = "UstadWhatsApp";
            public string whatsAppMSSQLConn = string.Empty;
        }

        public active_DB activeDB = new active_DB();

        #endregion

        #region WhatsApp Configuration

        public class tWhatsAppConfig
        {
            public string ApiBaseUrl = "http://143.198.228.153:5000";
            public string WebhookUrl = string.Empty;
            public string PhoneNumberId = string.Empty;
            public string AccessToken = string.Empty;
            public string VerifyToken = string.Empty;
            public bool EnableNotifications = true;
            public bool EnableSounds = true;
            public int ReconnectInterval = 5000; // milliseconds
            public int MessageTimeout = 30000; // milliseconds
            public string TurnstileSiteKey = string.Empty; // Cloudflare Turnstile site key
        }

        public tWhatsAppConfig whatsAppConfig = new tWhatsAppConfig();

        #endregion

        #region Constructor

        public tVariable()
        {
            preparingDefaultValues();
        }

        #endregion

        #region Methods - Tekin's pattern

        /// <summary>
        /// Default değerleri hazırla - Tekin's pattern
        /// </summary>
        private void preparingDefaultValues()
        {
            // Default values
            SP_UserLOGIN = false;
            SP_UserIN = false;
            SP_ApplicationExit = false;
            SP_Debug = false;
            SP_WhatsAppConnected = false;
            SP_WhatsAppInitialized = false;

            // WhatsApp default config
            whatsAppConfig.ApiBaseUrl = "http://localhost:5000";
            whatsAppConfig.ReconnectInterval = 5000;
            whatsAppConfig.MessageTimeout = 30000;
            whatsAppConfig.EnableNotifications = true;
            whatsAppConfig.EnableSounds = true;

            // Registry defaults
            userRegister.eMailList = new List<string>();
            userRegister.WhatsAppApiUrl = "http://localhost:5000";
            userRegister.WhatsAppAutoConnect = false;
        }

        /// <summary>
        /// Kullanıcı bilgilerini temizle - Tekin's pattern
        /// </summary>
        public void clearUserInfo()
        {
            tUserInfo.UserId = 0;
            tUserInfo.UserEMail = string.Empty;
            tUserInfo.UserKey = string.Empty;
            tUserInfo.UserFullName = string.Empty;
            tUserInfo.UserRole = string.Empty;
            tUserInfo.FirmId = 0;
            tUserInfo.IsActive = false;

            SP_UserLOGIN = false;
            SP_UserIN = false;
        }

        /// <summary>
        /// WhatsApp operatör bilgilerini temizle
        /// </summary>
        public void clearWhatsAppOperatorInfo()
        {
            tWhatsAppOperator.OperatorId = 0;
            tWhatsAppOperator.OperatorUserName = string.Empty;
            tWhatsAppOperator.OperatorFullName = string.Empty;
            tWhatsAppOperator.OperatorRole = string.Empty;
            tWhatsAppOperator.JwtToken = string.Empty;
            tWhatsAppOperator.TokenExpiry = DateTime.MinValue;
            tWhatsAppOperator.FirmId = 0;
            tWhatsAppOperator.IsConnected = false;
            tWhatsAppOperator.TurnstileToken = string.Empty;

            SP_WhatsAppConnected = false;
        }

        /// <summary>
        /// Token geçerlilik kontrolü
        /// </summary>
        public bool isTokenValid()
        {
            return !string.IsNullOrEmpty(tWhatsAppOperator.JwtToken) && 
                   tWhatsAppOperator.TokenExpiry > DateTime.UtcNow.AddMinutes(5);
        }

        #endregion
    }

    /// <summary>
    /// Global variable instance - Tekin's pattern
    /// </summary>
    public static class GlobalVariables
    {
        public static tVariable Instance { get; } = new tVariable();

        // Static properties for easy access - Tekin's pattern
        public static string ENTER => Instance.ENTER;
        public static string ENTER2 => Instance.ENTER2;
        
        public static bool SP_UserLOGIN 
        { 
            get => Instance.SP_UserLOGIN; 
            set => Instance.SP_UserLOGIN = value; 
        }
        
        public static bool SP_UserIN 
        { 
            get => Instance.SP_UserIN; 
            set => Instance.SP_UserIN = value; 
        }
        
        public static bool SP_ApplicationExit 
        { 
            get => Instance.SP_ApplicationExit; 
            set => Instance.SP_ApplicationExit = value; 
        }
        
        public static bool SP_Debug 
        { 
            get => Instance.SP_Debug; 
            set => Instance.SP_Debug = value; 
        }
        
        public static bool SP_WhatsAppConnected 
        { 
            get => Instance.SP_WhatsAppConnected; 
            set => Instance.SP_WhatsAppConnected = value; 
        }
        
        public static bool SP_WhatsAppInitialized 
        { 
            get => Instance.SP_WhatsAppInitialized; 
            set => Instance.SP_WhatsAppInitialized = value; 
        }

        public static string registryPath => Instance.registryPath;
        public static string EXE_Arguments 
        { 
            get => Instance.EXE_Arguments; 
            set => Instance.EXE_Arguments = value; 
        }

        public static tVariable.dBaseNo dBaseNo => new tVariable.dBaseNo();
        public static tVariable.dBaseType dBaseType => new tVariable.dBaseType();
        public static tVariable.tUser tUser => Instance.tUserInfo;
        public static tVariable.tWhatsAppUser tWhatsAppOperator => Instance.tWhatsAppOperator;
        public static tVariable.tUserRegister tUserRegister => Instance.userRegister;
        public static tVariable.active_DB active_DB => Instance.activeDB;
        public static tVariable.tWhatsAppConfig tWhatsAppConfig => Instance.whatsAppConfig;

        // Methods
        public static void clearUserInfo() => Instance.clearUserInfo();
        public static void clearWhatsAppOperatorInfo() => Instance.clearWhatsAppOperatorInfo();
        public static bool isTokenValid() => Instance.isTokenValid();
    }
}
