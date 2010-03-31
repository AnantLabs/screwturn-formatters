using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.QueryTableFormatter.Database;
using System.Data.SQLite;
using System.Data;

namespace Keeper.Garrett.ScrewTurn.QueryTableFormatter.Database
{
    public class SqLite : DatabaseBase
    {
        public SqLite(string _connectionString)
            : base(_connectionString)
        {
        }

        protected override IDbConnection GetConnection()
        {
            return new SQLiteConnection();
        }

        protected override IDbCommand GetCommand()
        {
            return new SQLiteCommand();
        }
    }
}
