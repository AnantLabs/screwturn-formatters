using System;
using System.Collections.Generic;
using System.Text;

namespace Keeper.Garrett.ScrewTurn.QueryTableFormatter.Database
{
    public class DatabaseConfiguration
    {
        public DatabaseConfiguration(string _connStr, DatabaseType _type)
        {
            ConnectionString = _connStr;
            Type = _type;
        }

        public string ConnectionString { get; private set; }
        public DatabaseType Type { get; private set; }
    }
}
