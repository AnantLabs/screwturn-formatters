using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.QueryTableFormatter.Database;
using Oracle.DataAccess.Client;
using System.Data;

namespace Keeper.Garrett.ScrewTurn.QueryTableFormatter.Database
{
    public class Oracle : DatabaseBase
    {
        public Oracle(string _connectionString)
            : base(_connectionString)
        {
        }

        protected override IDbConnection  GetConnection()
        {
            return new OracleConnection();
        }

        protected override IDbCommand GetCommand()
        {
            return new OracleCommand();
        }
    }
}
