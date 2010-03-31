using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.QueryTableFormatter;
using System.Diagnostics;
using Keeper.Garrett.ScrewTurn.EventLogFormatter;

namespace ManualTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Machine/IP - Log - Filter - Begrænsning
            try
            {
/*                var db = new Keeper.Garrett.ScrewTurn.QueryTableFormatter.Database.Oracle("User Id=wiki;Password=myretuefest;Data Source=(DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = 192.168.1.100)(PORT = 1521)) )(CONNECT_DATA = (SERVER = DEDICATED) (SERVICE_NAME = XE)));");
                db.Connect();
                var result = db.Query("select * from wiki.schedule");*/



                EventLog[] logs = EventLog.GetEventLogs(System.Environment.MachineName);
//                EventLog[] logs = EventLog.GetEventLogs("savannah");


                Trace.WriteLine("Number of logs on computer: " + logs.Length);

                foreach (EventLog log in logs)
                {
                    Trace.WriteLine("LogName: " + log.Log);

                    foreach (EventLogEntry entry in log.Entries)
                    {
                        
                        Trace.WriteLine("LogEntries: " + entry.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Error: {0}", e.Message);
            }

        }
    }
}
