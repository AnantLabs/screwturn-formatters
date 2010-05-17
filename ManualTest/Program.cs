using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.QueryTableFormatter;
using System.Diagnostics;
using Keeper.Garrett.ScrewTurn.EventLogFormatter;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Configuration.Install;
using System.Reflection;

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
                /*

                / Capture quoted string or non-quoted strings followed by whitespace
string exp = @"^(?:""([^""]*)""\s*|([^""\s]+)\s*)+";
Match m = Regex.Match(Environment.CommandLine, exp);

// Expect three Groups
// group[0] = entire match
// group[1] = matches from left capturing group
// group[2] = matches from right capturing group
if (m.Groups.Count < 3)
    throw new ArgumentException("A minimum of 2 arguments are required for this program");

// Sort the captures by their original postion
var captures = m.Groups[1].Captures.Cast<Capture>().Concat(
               m.Groups[2].Captures.Cast<Capture>()).
               OrderBy(x => x.Index).
               ToArray();

// captures[0] is the executable file
if (captures.Length < 3)
    throw new ArgumentException("A minimum of 2 arguments are required for this program")*/

//                var TagRegex = new Regex(@"\{Blog\((?<blog>(.*?)),(?<noOfPostsToShow>(.*?)),(?<noOfRecentPostsToShow>(.*?)),(?<useLastModified>(.*?)),(?<useCreateUserAsPostUser>(.*?)),(?<showGravatar>(.*?)),(?<showCloud>(.*?)),(?<showArchive>(.*?)),('(?<aboutPage>(.*?))')?,('(?<bottomPage>(.*?))')?,('(?<stylesheet>(.*?))')?\)\}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

//                var TagRegex = new Regex(@"\{Blog\(""(?<blog>(.*?)"") ((\/.*="".*"")*\)\}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);
//                var TagRegex = new Regex(@"\{Blog\(""(?<blog>(.*?)"") [\?&](?<name>[^&=]+)=(?<value>[^&=]+)\)\}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

                int ms = int.Parse(Math.Round(1234.0, 1).ToString());

                System.Console.WriteLine(ms);
                /*
                Trace.WriteLine(Assembly.GetExecutingAssembly().GetName().Version.ToString());


                Match m = Regex.Match(null, @"Version: (?<major>\d{1,3})\.(?<minor>\d{1,3})\.(?<build>\d{1,3})\.(?<revision>\d{1,3})");

                if (m.Success == true)
                {
                    int versionNo = int.Parse(m.Groups["major"].Value) * 1000;
                    versionNo += int.Parse(m.Groups["minor"].Value) * 100;
                    versionNo += int.Parse(m.Groups["build"].Value) * 10;
                    versionNo += int.Parse(m.Groups["revision"].Value);
                }

                foreach(var grp in m.Groups)
                {
                    Trace.WriteLine(grp);
                }

                string storeName = string.Format("/Keeper.Garrett.Formatters/{0}", "TestDir/Images");

                    var dirs = storeName.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    var lastDir = "";
                    foreach (var dir in dirs)
                    {
                        lastDir = string.Format("{0}/{1}", lastDir, dir);
                    }

                var TagRegex = new Regex(@"\{Blog (?<args>(.*?))\}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

                var parser = new CommandParser();


                var result1 = parser.Parse("fghfghfgh a='dfgdfg ækæ ' c='ffdgdfgklm' b='!#¤%&/()=?`'");

                var result2 = parser.Parse("a='dfgdfg ækæ ' c='ffdgdfgklm' b='!#¤%&/()=?`'");

                var result3 = parser.Parse(" a=' ' c=   b='!#¤%&/()=?`' ");

                var result4 = parser.Parse(" a='dfgdfg ækæ ' c='ffdgdfgklm' ='!#¤%&/()=?`'");

                var result5 = parser.Parse("a=dfgdfg ækæ  c='ffdgdfgklm' b=!#¤%&/()=?`");


/*                var ctx = new InstallContext();
                ctx.

                string argss = "a='' b='' c=''";

                argss.Split(' ');*/

                
//                var TagRegex = new Regex(@"\{Blog\(""(?<blog>(.*?)"") (?<argname>/\w+)=(?<argvalue>\w+)\)\}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);
                
                
               /* var matches = TagRegex.Matches("{Blog asd=\"asf asd\" a=test)}");

                var ret = ParseArgs(new string[] { "{Blog(\"MyBlog\" -a:test -b:test2)}" });

                foreach (Match match in matches)
                {
                    for (int i = 0; i < match.Groups.Count; i++ )
                        Trace.WriteLine(string.Format("Grp {0} - {1}", 1, match.Groups[i]));
                }


                
                var dict = new SortedList<DateTime, string>(new ReverseDateComparer());

                dict.Add(new DateTime(2010, 1, 1), "1");
                dict.Add(new DateTime(2010, 2, 1), "2");
                dict.Add(new DateTime(2010, 3, 1), "3");
                dict.Add(new DateTime(2010, 4, 1), "4");*/

                //Sort to have latest first

//                var d = dict.Reverse().Cast<SortedList<DateTime, string>>().;

               // var r = dict.Reverse().ToDictionary(x => x.Key, x => x.Value);
//                dict = dict.Reverse().ToDictionary;

         //       EventLog[] logs = EventLog.GetEventLogs(System.Environment.MachineName);
//                EventLog[] logs = EventLog.GetEventLogs("savannah");


            /*    System.Console.WriteLine(DateTime.Now.ToUniversalTime());
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
                }*/
            }
            catch (Exception e)
            {
                Trace.WriteLine("Error: {0}", e.Message);
            }

        }

        public static IDictionary<string, IList<string>> ParseArgs(string[] args)
        {
            const string ARGS_REGEX = @"-([a-zA-Z_][a-zA-Z_0-9]{0,}):(.{0,})";
            IDictionary<string, IList<string>> parsedArgs;
            string name, value;
            Match match;

            parsedArgs = new Dictionary<string, IList<string>>();

            if (args != null)
            {
                foreach (string arg in args)
                {
                    match = Regex.Match(arg, ARGS_REGEX, RegexOptions.IgnorePatternWhitespace);

                    if (match == null)
                        continue;

                    if (!match.Success)
                        continue;

                    if (match.Groups.Count != 3)
                        continue;

                    name = match.Groups[1].Value;
                    value = match.Groups[2].Value;

                    if (string.IsNullOrEmpty(name))
                        continue;

                    //if (DataType.IsTrimNullOrZeroLength(value))
                    //    continue;

                    name = name.ToLower();

                    if (!parsedArgs.ContainsKey(name))
                        parsedArgs.Add(name, new List<string>());

                    if (!parsedArgs[name].Contains(value))
                        parsedArgs[name].Add(value);
                }
            }

            return parsedArgs;
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
