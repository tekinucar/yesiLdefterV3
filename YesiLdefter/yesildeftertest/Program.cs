using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace yesildeftertest
{
    /// <summary>
    /// Ana program sınıfı - Tekin's pattern ile uyumlu
    /// WhatsApp entegrasyonu ile güvenli başlangıç
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Uygulamanın ana girdi noktası - Tekin's pattern
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // DPI farkındalığını sistem seviyesinde ayarla - Tekin's pattern
            SetProcessDPIAware();
            
            // Modern DPI yaklaşımı (Windows 10 ve üzeri) - Tekin's pattern
            try
            {
                SetProcessDpiAwarenessContext((int)DpiAwarenessContext.PerMonitorAwareV2);
            }
            catch
            {
                // Eski Windows sürümleri için fallback
                SetProcessDPIAware();
            }

            // Application configuration - Tekin's pattern
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Global exception handling - Tekin's pattern
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                // Start main form directly - production mode only
                Application.Run(new Form1(args));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Uygulama başlatılırken hata oluştu:\n\n{ex.Message}",
                    "Kritik Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        #region Exception Handling - Tekin's pattern

        /// <summary>
        /// Thread exception handler - Tekin's pattern
        /// </summary>
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            LogException(e.Exception);
            
            var result = MessageBox.Show(
                $"Beklenmeyen bir hata oluştu:\n\n{e.Exception.Message}\n\nUygulama kapatılsın mı?",
                "Hata",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Error
            );

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// Unhandled exception handler - Tekin's pattern
        /// </summary>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                LogException(ex);
                
                MessageBox.Show(
                    $"Kritik hata oluştu:\n\n{ex.Message}\n\nUygulama kapatılacak.",
                    "Kritik Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop
                );
            }
            
            Application.Exit();
        }

        /// <summary>
        /// Exception'ı logla - Tekin's pattern
        /// </summary>
        private static void LogException(Exception ex)
        {
            try
            {
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - EXCEPTION: {ex.Message}\nStack Trace: {ex.StackTrace}\n";
                
                // Debug output - Tekin's pattern
                System.Diagnostics.Debug.WriteLine(logMessage);
                
                // TODO: File veya database'e log yazma - Tekin's logging pattern
            }
            catch
            {
                // Log yazma hatası - görmezden gel
            }
        }

        #endregion

        #region DPI Awareness - Tekin's pattern

        // DPI farkındalığı için P/Invoke tanımları - Tekin's pattern
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [DllImport("user32.dll")]
        private static extern bool SetProcessDpiAwarenessContext(int dpiFlag);

        private enum DpiAwarenessContext
        {
            Unaware = -1,
            SystemAware = -2,
            PerMonitorAware = -3,
            PerMonitorAwareV2 = -4
        }

        #endregion
    }
}