using System;
using System.Data;

namespace yesildeftertest
{
    /// <summary>
    /// DataNavigator - Tekin's pattern for data navigation
    /// Provides navigation functionality for DataSet operations
    /// </summary>
    public class DataNavigator : tBase
    {
        #region tanımlar - Tekin's pattern

        private DataSet _dataSet;
        private string _tableName;
        private int _currentIndex = -1;
        private int _recordCount = 0;

        #endregion

        #region Properties

        public DataSet DataSet => _dataSet;
        public string TableName => _tableName;
        public int CurrentIndex => _currentIndex;
        public int RecordCount => _recordCount;
        public bool HasData => _recordCount > 0;
        public bool IsFirst => _currentIndex == 0;
        public bool IsLast => _currentIndex == _recordCount - 1;

        #endregion

        #region Constructor

        public DataNavigator()
        {
            preparingDefaultValues();
        }

        public DataNavigator(DataSet dataSet, string tableName)
        {
            preparingDefaultValues();
            SetDataSource(dataSet, tableName);
        }

        #endregion

        #region Methods - Tekin's pattern

        /// <summary>
        /// Set data source - Tekin's pattern
        /// </summary>
        public void SetDataSource(DataSet dataSet, string tableName)
        {
            _dataSet = dataSet;
            _tableName = tableName;
            
            if (_dataSet != null && _dataSet.Tables.Contains(_tableName))
            {
                _recordCount = _dataSet.Tables[_tableName].Rows.Count;
                _currentIndex = _recordCount > 0 ? 0 : -1;
            }
            else
            {
                _recordCount = 0;
                _currentIndex = -1;
            }
        }

        /// <summary>
        /// Move to first record - Tekin's pattern
        /// </summary>
        public bool MoveFirst()
        {
            if (_recordCount > 0)
            {
                _currentIndex = 0;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Move to last record - Tekin's pattern
        /// </summary>
        public bool MoveLast()
        {
            if (_recordCount > 0)
            {
                _currentIndex = _recordCount - 1;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Move to next record - Tekin's pattern
        /// </summary>
        public bool MoveNext()
        {
            if (_currentIndex < _recordCount - 1)
            {
                _currentIndex++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Move to previous record - Tekin's pattern
        /// </summary>
        public bool MovePrevious()
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get current row - Tekin's pattern
        /// </summary>
        public DataRow GetCurrentRow()
        {
            if (_dataSet != null && _dataSet.Tables.Contains(_tableName) && 
                _currentIndex >= 0 && _currentIndex < _recordCount)
            {
                return _dataSet.Tables[_tableName].Rows[_currentIndex];
            }
            return null;
        }

        /// <summary>
        /// Get field value - Tekin's pattern
        /// </summary>
        public object GetField(string fieldName)
        {
            var row = GetCurrentRow();
            if (row != null && row.Table.Columns.Contains(fieldName))
            {
                return row[fieldName];
            }
            return null;
        }

        /// <summary>
        /// Get field value as string - Tekin's pattern
        /// </summary>
        public string GetFieldString(string fieldName)
        {
            var value = GetField(fieldName);
            return value?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Get field value as int - Tekin's pattern
        /// </summary>
        public int GetFieldInt(string fieldName)
        {
            return myInt32(GetFieldString(fieldName));
        }

        /// <summary>
        /// Get field value as DateTime - Tekin's pattern
        /// </summary>
        public DateTime GetFieldDateTime(string fieldName)
        {
            return myDateTime(GetFieldString(fieldName));
        }

        /// <summary>
        /// Find record by field value - Tekin's pattern
        /// </summary>
        public bool FindRecord(string fieldName, object value)
        {
            if (_dataSet != null && _dataSet.Tables.Contains(_tableName))
            {
                var table = _dataSet.Tables[_tableName];
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    if (table.Rows[i][fieldName].Equals(value))
                    {
                        _currentIndex = i;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Clear data - Tekin's pattern
        /// </summary>
        public void Clear()
        {
            _dataSet = null;
            _tableName = string.Empty;
            _currentIndex = -1;
            _recordCount = 0;
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            Clear();
        }

        #endregion
    }
}