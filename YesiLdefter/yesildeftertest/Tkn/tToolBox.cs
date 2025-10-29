using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace yesildeftertest
{
    public class tToolBox : tBase
    {
        #region Variables and Helpers
        private HttpClient httpClient;

        #endregion

        #region Constructor

        public tToolBox()
        {
            preparingDefaultValues();
            httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        #endregion

        #region Database Methods
        public bool Db_Open(string connectionString)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    debugMessage($"Database connection opened: {connection.Database}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage("Database bağlantı hatası", ex);
                return false;
            }
        }

        public bool SQL_Read_Execute(tVariable.dBaseNo dbNo, DataSet ds, ref string sql, string tableName, string operation)
        {
            try
            {
                string connectionString = getConnectionString(dbNo);
                
                using (var connection = new SqlConnection(connectionString))
                using (var adapter = new SqlDataAdapter(sql, connection))
                {
                    connection.Open();
                    
                    if (ds.Tables.Contains(tableName))
                        ds.Tables.Remove(tableName);
                    
                    adapter.Fill(ds, tableName);
                    
                    debugMessage($"SQL executed: {operation} - Rows: {ds.Tables[tableName].Rows.Count}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage($"SQL hatası: {operation}", ex);
                return false;
            }
        }

        public bool SQL_Execute(tVariable.dBaseNo dbNo, string sql, string operation)
        {
            try
            {
                string connectionString = getConnectionString(dbNo);
                
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    
                    debugMessage($"SQL executed: {operation} - Affected rows: {affectedRows}");
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                errorMessage($"SQL execution hatası: {operation}", ex);
                return false;
            }
        }

        private string getConnectionString(tVariable.dBaseNo dbNo)
        {
            switch (dbNo)
            {
                case tVariable.dBaseNo.UstadCrm:
                    return GlobalVariables.Instance.activeDB.managerMSSQLConn;
                case tVariable.dBaseNo.UstadWhatsApp:
                    return GlobalVariables.Instance.activeDB.whatsAppMSSQLConn;
                case tVariable.dBaseNo.Local:
                    return GlobalVariables.Instance.activeDB.projectMSSQLConn;
                default:
                    return GlobalVariables.Instance.activeDB.managerMSSQLConn;
            }
        }

        #endregion

        #region Table Methods

        public void TableRemove(DataSet ds)
        {
            if (ds != null)
            {
                ds.Tables.Clear();
                debugMessage("DataSet tables cleared");
            }
        }

        public bool IsNotNull(DataSet ds)
        {
            return isNotNull(ds);
        }

        #endregion

        #region Form Methods
        public Control Find_Control(Form form, string controlName, string tableIPCode, string[] controls)
        {
            foreach (Control control in form.Controls)
            {
                if (control.Name == controlName)
                {
                    debugMessage($"Control found: {controlName}");
                    return control;
                }
                
                // Recursive search in child controls
                Control found = findControlRecursive(control, controlName);
                if (found != null)
                    return found;
            }
            
            debugMessage($"Control not found: {controlName}");
            return null;
        }

        private Control findControlRecursive(Control parent, string controlName)
        {
            foreach (Control child in parent.Controls)
            {
                if (child.Name == controlName)
                    return child;
                
                Control found = findControlRecursive(child, controlName);
                if (found != null)
                    return found;
            }
            return null;
        }

        public void SelectPage(Form form, string viewType, string pageName, int index)
        {
            debugMessage($"Page selection: {viewType} - {pageName} - {index}");
        }

        #endregion

        #region HTTP Methods - WhatsApp API için

        public async Task<string> HTTP_GET(string url, Dictionary<string, string>? headers = null)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }
                
                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                
                string content = await response.Content.ReadAsStringAsync();
                debugMessage($"HTTP GET success: {url}");
                return content;
            }
            catch (Exception ex)
            {
                errorMessage($"HTTP GET hatası: {url}", ex);
                return null;
            }
        }

        public async Task<string> HTTP_POST(string url, string jsonContent, Dictionary<string, string>? headers = null)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }
                
                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                
                string content = await response.Content.ReadAsStringAsync();
                debugMessage($"HTTP POST success: {url}");
                return content;
            }
            catch (Exception ex)
            {
                errorMessage($"HTTP POST hatası: {url}", ex);
                return null;
            }
        }

        #endregion

        #region Registry Methods
        public string Registry_Read(string keyPath, string valueName, string defaultValue = "")
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(keyPath))
                {
                    if (key != null)
                    {
                        object value = key.GetValue(valueName);
                        if (value != null)
                        {
                            debugMessage($"Registry read: {keyPath}\\{valueName}");
                            return value.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                debugMessage($"Registry read error: {ex.Message}");
            }
            
            return defaultValue;
        }

        public bool Registry_Write(string keyPath, string valueName, string value)
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(keyPath))
                {
                    if (key != null)
                    {
                        key.SetValue(valueName, value);
                        debugMessage($"Registry write: {keyPath}\\{valueName}");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage($"Registry write error: {keyPath}\\{valueName}", ex);
            }
            
            return false;
        }

        #endregion

        #region Utility Methods

        public DialogResult mySoru(string question)
        {
            return askQuestion(question);
        }

        public void WaitFormOpen(Form parentForm, string message)
        {
            // Implementation for wait form
            debugMessage($"Wait form: {message}");
        }

        public void WaitFormClose()
        {
            debugMessage("Wait form closed");
        }

        public Image Find_Glyph(string glyphName)
        {
            try
            {
                debugMessage($"Glyph requested: {glyphName}");
                return null; // Placeholder
            }
            catch (Exception ex)
            {
                debugMessage($"Glyph load error: {glyphName} - {ex.Message}");
                return null;
            }
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

        #region Dispose

        public void Dispose()
        {
            httpClient?.Dispose();
        }

        #endregion
    }
}
