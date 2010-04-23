using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.QueryTableFormatter;
using System.Diagnostics;
using Keeper.Garrett.ScrewTurn.EventLogFormatter;
using System.Linq;
using System.Collections;

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

                var dict = new SortedList<DateTime, string>(new ReverseDateComparer());

                dict.Add(new DateTime(2010, 1, 1), "1");
                dict.Add(new DateTime(2010, 2, 1), "2");
                dict.Add(new DateTime(2010, 3, 1), "3");
                dict.Add(new DateTime(2010, 4, 1), "4");

                //Sort to have latest first

//                var d = dict.Reverse().Cast<SortedList<DateTime, string>>().;

                var r = dict.Reverse().ToDictionary(x => x.Key, x => x.Value);
//                dict = dict.Reverse().ToDictionary;

                EventLog[] logs = EventLog.GetEventLogs(System.Environment.MachineName);
//                EventLog[] logs = EventLog.GetEventLogs("savannah");


                System.Console.WriteLine(DateTime.Now.ToUniversalTime());
                System.Console.WriteLine(DateTime.Now.ToShortTimeString());
                System.Console.WriteLine(DateTime.Now.ToShortDateString());
                System.Console.WriteLine(DateTime.Now.ToLongTimeString());
                System.Console.WriteLine(DateTime.Now.ToLongDateString());
                System.Console.WriteLine(DateTime.Now.ToLocalTime());
                System.Console.WriteLine(DateTime.Now.ToFileTimeUtc());
                System.Console.WriteLine(DateTime.Now.ToFileTime());
                System.Console.WriteLine(DateTime.Now.ToBinary());

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

        public class ReverseDateComparer : IComparer<DateTime>
        {
            public int Compare(DateTime x, DateTime y)
            {
                return -1 * DateTime.Compare(x, y);
            }
        }
    }
}
