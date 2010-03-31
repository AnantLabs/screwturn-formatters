using System;
using System.Collections.Generic;
using System.Text;

namespace Keeper.Garrett.ScrewTurn.QueryTableFormatter.Database
{
    interface IDatabase
    {
        bool Connect();
        bool Disconnet();
        bool IsConnected();

        Dictionary<int, List<string>> Query(string _queryString);

        Dictionary<int, string> Get_Index_Column_Name_Dictionary_FromLatestQuery();
        Dictionary<string, int> Get_Column_Name_Index_Dictionary_FromLatestQuery();
        List<string> Get_Column_Name_List_FromLatestQuery();
    }
}
