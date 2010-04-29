using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;

namespace Keeper.Garrett.ScrewTurn.QueryTableFormatter.Database
{
    public abstract class DatabaseBase : IDatabase
    {
        #region Illegal Keywords

        private readonly List<string> m_OracleKeywords = new List<string>()
        {
            "ADD",
            "ALTER",
            "AUDIT",
            "CALL",
            "COMMENT",
            "COMPRESS",
            "CREATE",
            "DELETE",
            "DROP",
            "GRANT",
            "INCREMENT",
            "INDEX",
            "INSERT",
            "INTO",
            "LOCK",
            "MODIFY",
            "PRIVILEGES",
            "RENAME",
            "REVOKE",
            "SET",
            "START",
            "TRIGGER",
            "TRUNCATE",
            "UPDATE"
        };

        private readonly List<string> m_MSSqlKeywords = new List<string>()
        {
            "ADD",
            "ALTER",
            "CASCADE",
            "COMMIT",
            "COMPUTE",
            "CREATE",
            "DELETE",
            "DENY",
            "DROP",
            "DUMP",
            "EXEC",
            "EXECUTE",
            "GRANT",
            "HOLDLOCK",
            "IDENTITY_INSERT",
            "INSERT",
            "INTO",
            "KILL",
            "PROC",
            "PROCEDURE",
            "RECONFIGURE",
            "REFERENCES",
            "REPLICATION",
            "RESTORE",
            "RESTRICT",
            "REVOKE",
            "ROLLBACK",
            "RULE",
            "SAVE",
            "SET",
            "SETUSER",
            "TRANSACTION",
            "TRIGGER",
            "TRUNCATE",
            "USE"
        };

        private readonly List<string> m_MySqlKeywords = new List<string>()
        {
            "ADD",
            "ALTER",
            "CALL",
            "CASCADE",
            "CHANGE",
            "CREATE",
            "DEC",
            "DECLARE",
            "DECLARE",
            "DELETE",
            "DROP",
            "EXIT",
            "FETCH",
            "FORCE",
            "GRANT",
            "INSERT",
            "INTO",
            "KILL",
            "LOCK",
            "LOOP",
            "PROCEDURE",
            "PURGE",
            "REFERENCES",
            "RELEASE",
            "RENAME",
            "REPLACE",
            "REQUIRE",
            "RESTRICT",
            "REVOKE",
            "SET",
            "TRIGGER",
            "UNLOCK",
            "UPDATE",
            "UPGRADE",
            "WRITE"
        };

        private readonly List<string> m_SQLiteKeywords = new List<string>()
        {
            "ABORT",
            "ACTION",
            "ADD",
            "ALTER",
            "AUTOINCREMENT",
            "CASCADE",
            "COMMIT",
            "CREATE",
            "DELETE",
            "DROP",
            "END",
            "INSERT",
            "INSTEAD",
            "INTO",
            "REFERENCES",
            "RELEASE",
            "RENAME",
            "REPLACE",
            "RESTRICT",
            "ROLLBACK",
            "SAVEPOINT",
            "SET",
            "TRANSACTION",
            "TRIGGER",
            "UPDATE",
            "VACUUM"
        };

        #endregion

        protected bool m_IsConnected = false;
        protected string m_ConnectionString = null;

        protected Dictionary<int, string> m_IndexColumnNameDictionary = new Dictionary<int, string>();
        protected Dictionary<string, int> m_ColumnNameIndexDictionary = new Dictionary<string, int>();

        protected System.Data.IDbConnection m_Connection = null;

        public DatabaseBase(string _connectionString)
        {
            m_ConnectionString = _connectionString;
            m_IsConnected = false;
        }

        private bool SecurityScanQuery(string _rawQueryString)
        {
            string query = _rawQueryString.ToUpper();

            foreach (var keyword in m_OracleKeywords)
            {
                if (Regex.IsMatch(query,string.Format("\\b{0}\\b", keyword)) == true)  return false;//query.Contains(keyword) == true) return false;
            }

            foreach (var keyword in m_MSSqlKeywords)
            {
                if (Regex.IsMatch(query, string.Format("\\b{0}\\b", keyword)) == true) return false;
            }

            foreach (var keyword in m_MySqlKeywords)
            {
                if (Regex.IsMatch(query, string.Format("\\b{0}\\b", keyword)) == true) return false;
            }

            foreach (var keyword in m_SQLiteKeywords)
            {
                if (Regex.IsMatch(query, string.Format("\\b{0}\\b", keyword)) == true) return false;
            }

            return true;
        }

        public bool IsConnected()
        {
            return m_IsConnected;
        }

        public bool Connect()
        {
            if (m_IsConnected == true)
            {
                return m_IsConnected;
            }

            m_Connection = GetConnection();
            m_Connection.ConnectionString = m_ConnectionString;
            m_Connection.Open();

            return (m_IsConnected = true);
        }

        public bool Disconnet()
        {
            if (m_IsConnected == true)
            {
                m_Connection.Close();
                m_Connection.Dispose();
                m_IsConnected = false;
            }

            return m_IsConnected;
        }

        public Dictionary<int, List<string>> Query(string _queryString)
        {
            //Clear dicts, before new query, query MUST update dicts
            ClearColumnDictionaries();

            if (SecurityScanQuery(_queryString) == true)
            {
                return PerformQuery(_queryString);
            }
            else
            {
                throw new AccessViolationException(string.Format("Query failed security check. Use only SELECT statements.\r\nQuery was: {0}", _queryString));
            }
        }

       /* public Dictionary<int, string> Get_Index_Column_Name_Dictionary_FromLatestQuery()
        {
            return m_IndexColumnNameDictionary;
        }

        public Dictionary<string, int> Get_Column_Name_Index_Dictionary_FromLatestQuery()
        {
            return m_ColumnNameIndexDictionary;
        }*/

        public List<string> Get_Column_Name_List_FromLatestQuery()
        {
            var retval = new List<string>();
            foreach(var column in m_ColumnNameIndexDictionary.Keys)
            {
                retval.Add(column);
            }
            return retval;
        }

        private void ClearColumnDictionaries()
        {
            m_ColumnNameIndexDictionary.Clear();
            m_IndexColumnNameDictionary.Clear();
        }

        private void AddColumnName(int _index, string _name)
        {
            if (m_IndexColumnNameDictionary.ContainsKey(_index) == false)
            {
                m_IndexColumnNameDictionary.Add(_index, _name);
            }

            if (m_ColumnNameIndexDictionary.ContainsKey(_name) == false)
            {
                m_ColumnNameIndexDictionary.Add(_name, _index);
            }
        }

        private Dictionary<int, List<string>> PerformQuery(string _queryString)
        {
            var retval = new Dictionary<int, List<string>>();

            //Build query command
            IDbCommand cmd = GetCommand();
            cmd.Connection = m_Connection;
            cmd.CommandText = _queryString;
            cmd.CommandType = System.Data.CommandType.Text;

            //Execute
            IDataReader reader = cmd.ExecuteReader();

            //Handle result

            //Get Field names
            for (int i = 0; i < reader.FieldCount; i++)
            {
                AddColumnName(i, reader.GetName(i));
            }

            int rowId = 0;
            while (reader.Read())
            {
                var rowValue = new List<string>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    rowValue.Add(reader[i].ToString());
                }

                retval.Add(rowId++, rowValue);
            }

            reader.Close();

            return retval;
        }

        protected abstract IDbConnection GetConnection();

        protected abstract IDbCommand GetCommand();
    }
}
