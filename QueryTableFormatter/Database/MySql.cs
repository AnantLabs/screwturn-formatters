using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.QueryTableFormatter.Database;
using MySql.Data.MySqlClient;
using System.Data;

namespace Keeper.Garrett.ScrewTurn.QueryTableFormatter.Database
{
    public class MySql : DatabaseBase
    {
        public MySql(string _connectionString)
            : base(_connectionString)
        {
        }

        protected override IDbConnection GetConnection()
        {
            return new MySqlConnection();
        }

        protected override IDbCommand GetCommand()
        {
            return new MySqlCommand();
        }
    }
}
