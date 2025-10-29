using System.Data;

namespace yesildeftertest
{
    public class tBase
    {
        #region Variables and Helpers

        protected string tSql = string.Empty;
        protected string tableIPCode = string.Empty;
        protected bool isDebugMode = false;

        #endregion

        #region Constructor

        public tBase()
        {
            preparingDefaultValues();
        }

        #endregion

        #region Methods

        protected virtual void preparingDefaultValues()
        {
            isDebugMode = false;
        }

        public void debugMessage(string message)
        {
            if (isDebugMode)
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] {DateTime.Now:HH:mm:ss} - {message}");
            }
        }

        public void errorMessage(string message, Exception? ex = null)
        {
            string fullMessage = message;
            if (ex != null)
            {
                fullMessage += $"\n\nDetay: {ex.Message}";
            }
            
            MessageBox.Show(fullMessage, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            debugMessage($"ERROR: {fullMessage}");
        }

        public void infoMessage(string message)
        {
            MessageBox.Show(message, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            debugMessage($"INFO: {message}");
        }

        protected DialogResult askQuestion(string question)
        {
            return MessageBox.Show(question, "Soru", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        protected bool isNotNull(string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        /// Check if DataSet contains data
        protected bool isNotNull(DataSet ds)
        {
            return ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0;
        }

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Convert string to int32 safely
        /// </summary>
        public int myInt32(string value)
        {
            if (int.TryParse(value, out int result))
                return result;
            return 0;
        }

        /// <summary>
        /// Convert string to DateTime safely
        /// </summary>
        public DateTime myDateTime(string value)
        {
            if (DateTime.TryParse(value, out DateTime result))
                return result;
            return DateTime.MinValue;
        }

        /// <summary>
        /// Convert string to double safely
        /// </summary>
        public double myDouble(string value)
        {
            if (double.TryParse(value, out double result))
                return result;
            return 0.0;
        }

        /// <summary>
        /// Convert string to bool safely
        /// </summary>
        public bool myBool(string value)
        {
            if (bool.TryParse(value, out bool result))
                return result;
            return false;
        }

        #endregion
    }
}
