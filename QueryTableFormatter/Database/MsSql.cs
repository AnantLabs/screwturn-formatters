using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Keeper.Garrett.ScrewTurn.QueryTableFormatter.Database;
using System.Data;

namespace Keeper.Garrett.ScrewTurn.QueryTableFormatter.Database
{
    public class MsSql : DatabaseBase
    {
        public MsSql(string _connectionString)
            : base(_connectionString)
        {
        }

        protected override IDbConnection GetConnection()
        {
            return new SqlConnection();
        }

        protected override IDbCommand GetCommand()
        {
            return new SqlCommand();
        }
    }
}
