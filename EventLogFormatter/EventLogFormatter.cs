using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.Core;
using System.Text.RegularExpressions;
using ScrewTurn.Wiki.PluginFramework;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Reflection;
using Keeper.Garrett.ScrewTurn.Utility;

namespace Keeper.Garrett.ScrewTurn.EventLogFormatter
{
    public class EventLogFormatter : FormatterBase
    {
        public override bool PerformPhase1 { get { return true; } }
        public override bool PerformPhase2 { get { return false; } }
        public override bool PerformPhase3 { get { return false; } }

        //  { "Id","Type","Date","Time","Source","Category","Event","User","Computer", "Description" },
        private Dictionary<string, int> m_ColumnDictionary = new Dictionary<string, int>()
        {
            { "id", 0 },
            { "type", 1 },
            { "date", 2 },
            { "time", 3 },
            { "source", 4 },
            { "category", 5 },
            { "event", 6 },
            { "user", 7 },
            { "computer", 8 },
            { "description", 9 }
        };

        private List<string> m_ColumnNames = new List<string>()
        {
            "Id",
            "Type",
            "Date",
            "Time",
            "Source",
            "Category",
            "Event",
            "User",
            "Computer",
            "Description"
        };

        //                                                              machine,log,filter,results,heading,cols,headers,tbl,head,row
//        private static readonly Regex TagRegex = new Regex(@"\{EventLog\((?<machine>(.*?)),('(?<logname>(.*?))'),('(?<filter>(.*?))')?,(?<results>(.*?)),('(?<heading>(.*?))')?,('(?<columns>(.*?))')?,('(?<headers>(.*?))')?,('(?<tblFormat>(.*?))')?,('(?<headFormat>(.*?))')?,('(?<rowFormat>(.*?))')?\)\}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);
        private static readonly Regex TagRegex = new Regex(@"\{EventLog(?<arguments>(.*?))\}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);


        public override void Init(IHostV30 _host, string _config)
        {
            base.Init(_host, _config, Help.HelpPages);
            StoreIcons();
            LogEntry("EventLogFormatter - Init success", LogEntryType.General);
        }

        private void StoreIcons()
        {
            try
            {
                var files = new List<string>() { "Information.png", "Error.png", "Warning.png" };

                var fileDictionary = new Dictionary<string, MemoryStream>();

                foreach (var file in files)
                {

                    Assembly myAssembly = Assembly.GetExecutingAssembly();
                    Stream stream = myAssembly.GetManifestResourceStream(
                        string.Format("Keeper.Garrett.ScrewTurn.EventLogFormatter.Images.{0}",file));
                    Bitmap image = new Bitmap(stream);
                    
                    var memoryStream = new MemoryStream();
                    image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    memoryStream.Position = 0;
                    fileDictionary.Add(file, memoryStream);
                }

                StoreFiles("EventLogFormatter", fileDictionary);
            }
            catch (Exception e)
            {
                LogEntry(string.Format("Error creating default icon images during init.\r\n{0}", e.Message), LogEntryType.Error);
            }
        }

        public override string Format(string raw, ContextInformation context, FormattingPhase phase)
        {
            try
            {
                if (context.Context == FormattingContext.PageContent
                    && context.ForIndexing == false
                    && context.ForWysiwyg == false)
                {
                    switch (phase)
                    {
                        case FormattingPhase.Phase1:
                            var matches = TagRegex.Matches(raw);
                            if (matches != null && matches.Count > 0)
                            {
                                //Foreach query
                                foreach (Match match in matches)
                                {
                                    var args = new ArgumentParser().Parse(match.Groups["arguments"].Value);

                                    var machine = (args.ContainsKey("machine") == true ? args["machine"] : System.Environment.MachineName);
                                    var logname = (args.ContainsKey("log") == true ? args["log"] : "");
                                    var filter = (args.ContainsKey("filter") == true ? args["filter"] : ""); 

                                    int results = 0;
                                    int.TryParse( (args.ContainsKey("results") == true ? args["results"] : "15"),out results);

                                    #region Formatting
                                    var head = (args.ContainsKey("head") == true ? args["head"] : "");
                                    var foot = (args.ContainsKey("foot") == true ? args["foot"] : "");
                                    var style = (args.ContainsKey("style") == true ? args["style"] : "");
                                    
                                    var columns = new List<int>();
                                    var headers = m_ColumnNames;

                                    //Parse columns to show
                                    if (args.ContainsKey("cols") == true)
                                    {
                                        var value = args["cols"];

                                        switch (value.ToLower())
                                        {
                                            case "all":
                                                columns.Add(0);
                                                columns.Add(1);
                                                columns.Add(2);
                                                columns.Add(3);
                                                columns.Add(4);
                                                columns.Add(5);
                                                columns.Add(6);
                                                columns.Add(7);
                                                columns.Add(8);
                                                columns.Add(9);
                                                break;
                                            default:
                                                var tmpColumns = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                                foreach (var str in tmpColumns)
                                                {
                                                    var key = str.ToLower();
                                                    if (m_ColumnDictionary.ContainsKey(key) == true)
                                                    {
                                                        columns.Add(m_ColumnDictionary[key]);
                                                    }
                                                }
                                                break;
                                        };
                                    }
                                    else
                                    {
                                        columns.Add(m_ColumnDictionary["type"]);//Type
                                        columns.Add(m_ColumnDictionary["date"]);//Create time
                                        columns.Add(m_ColumnDictionary["source"]);//Source
                                        columns.Add(m_ColumnDictionary["description"]);//Description 
                                    }

                                    //Parse custom header names to use
                                    if (args.ContainsKey("colnames") == true)
                                    {
                                        var tmpColumns = args["colnames"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                        for (int i = 0; i < tmpColumns.Length && i < columns.Count ; i++)
                                        {
                                            headers[columns[i]] = tmpColumns[i];
                                        }
                                    }

                                    #endregion


                                    //var machine = (string.IsNullOrEmpty(match.Groups["machine"].Value) == false ? match.Groups["machine"].Value.Trim() : System.Environment.MachineName);
                                    //var logname = (string.IsNullOrEmpty(match.Groups["logname"].Value) == false ? match.Groups["logname"].Value.Trim() : "");
                                    //var filter = (string.IsNullOrEmpty(match.Groups["filter"].Value) == false ? match.Groups["filter"].Value.Trim() : "");
                                    //int results = 15;
                                    //int.TryParse((string.IsNullOrEmpty(match.Groups["results"].Value) == false ? match.Groups["results"].Value.Trim() : "15"), out results);

                                    //#region Formatting
                                    ////Get formatting
                                    //var columns = new List<int>();
                                    //var headers = new List<string>();

                                    ////Parse columns to show
                                    //if (string.IsNullOrEmpty(match.Groups["columns"].Value) == false)
                                    //{
                                    //    var value = match.Groups["columns"].Value;

                                    //    switch (value.ToLower())
                                    //    {
                                    //        case "all":
                                    //            //Handled by header setup
                                    //            break;
                                    //        default:
                                    //            var tmpColumns = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    //            foreach (var str in tmpColumns)
                                    //            {
                                    //                int columnIndex;
                                    //                if (int.TryParse(str, out columnIndex) == true)
                                    //                {
                                    //                    columns.Add(columnIndex);
                                    //                }
                                    //            }
                                    //            break;
                                    //    };
                                    //}
                                    //else
                                    //{
                                    //    columns.Add(1);//Type
                                    //    columns.Add(2);//Create time
                                    //    columns.Add(4);//Source
                                    //    columns.Add(9);//Description
                                    //}

                                    ////Parse custom headers to show
                                    //if (string.IsNullOrEmpty(match.Groups["headers"].Value) == false)
                                    //{
                                    //    var tmpColumns = match.Groups["headers"].Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    //    foreach (var str in tmpColumns)
                                    //    {
                                    //        headers.Add(str);
                                    //    }
                                    //}

                                    ////Setup table formatting, default to system/wiki theme if no override present
                                    //var heading = (string.IsNullOrEmpty(match.Groups["heading"].Value) == false ? match.Groups["heading"].Value.Trim() : "");
                                    //var tblFormat = (string.IsNullOrEmpty(match.Groups["tblFormat"].Value) == false ? match.Groups["tblFormat"].Value.Trim() : "");
                                    //var headFormat = (string.IsNullOrEmpty(match.Groups["headFormat"].Value) == false ? match.Groups["headFormat"].Value.Trim() : "");
                                    //var rowFormat = (string.IsNullOrEmpty(match.Groups["rowFormat"].Value) == false ? match.Groups["rowFormat"].Value.Trim() : "");

                                    //#endregion

                                    //Fetch logs
                                    var logRows = GetEventLogEntries(machine, logname, filter, columns, results);

                                    //Prepare resulting table
                                    var resultTable = string.Format("No logs found for {0}: {1}, Filter: {2}", machine,logname,filter);

                                    if (logRows.Count > 0)
                                    {
                                        //Generate table
                                        resultTable = Utility.XHtmlTableGenerator.GenerateTable(logRows,
                                                                    head,
                                                                    foot,
                                                                    columns,
                                                                    headers,
                                                                    new List<string>() { "Id", "Type", "Date", "Time", "Source", "Category", "Event", "User", "Computer", "Description" },
                                                                    style);
                                    }

                                    //Insert table
                                    //Recall position as string may allready have been modified by other table match entry
                                    int pos = TagRegex.Match(raw).Index;
                                    int length = TagRegex.Match(raw).Length;
                                    raw = raw.Remove(pos, length);
                                    raw = raw.Insert(pos, resultTable);
                                }
                            }
                            break;
                        case FormattingPhase.Phase2:
                            break;
                        case FormattingPhase.Phase3:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
               LogEntry(string.Format("EventLogFormatter error: {0} {1}", e.Message, e.StackTrace), LogEntryType.Error);
            }

            return raw;
        }

        private Dictionary<int, List<string>> GetEventLogEntries(string _machine, string _logName, string _filter, List<int> _columns, int results )
        {
            var retval = new Dictionary<int, List<string>>();

            try
            {
                var eventLogs = EventLog.GetEventLogs(_machine);
                
                var filterValues = ParseFilter(_filter);

                foreach (EventLog log in eventLogs)
                {
                    //Find the correct log
                    if (log.Log == _logName)
                    {
                        int index = 0;

                        foreach (EventLogEntry entry in log.Entries)
                        {
                            if (MatchFilter(entry, filterValues) == true)
                            {
                                retval.Add( index++,
                                            //  { "Id","Type","Date","Time","Source","Category","Event","User","Computer", "Description" },
                                            new List<string>() 
                                            {
                                                entry.Index.ToString(),
                                                GenerateTypeEntry(entry),
                                                entry.TimeGenerated.ToString(),
                                                entry.TimeWritten.ToString(),
                                                entry.Source,
                                                entry.Category,
                                                entry.EventID.ToString(),
                                                entry.UserName,
                                                entry.MachineName,
                                                entry.Message
                                            }
                                    );

                                if (index > results - 1)
                                {
                                    /*string msg = string.Format("Maximum results retrieved ({0})", results);
                                    retval.Add(index++, new List<string>() { msg, msg, msg, msg, msg, msg, msg, msg, msg, msg });*/
                                    break;
                                }
                            }
                        }

                        break;//Since it was only this log that where requested
                    }
                }

                //No results?
                if (retval.Count <= 0)
                {
                    retval.Add(int.MinValue, new List<string>() { string.Format("align=\"center\" colspan=\"{0}\" | <h2>No logs found for {0}: {1}, Filter: {2}</h2>",
                        (_columns.Count > 0 ? _columns.Count : 10),
                        _machine,
                        _logName,
                        _filter ) 
                    });
                }
            }
            catch (Exception e)
            {
                retval.Add(int.MinValue + 1, new List<string>() { string.Format("Error connection to {0}: {1}, Filter: {2}\r\nMessage: {3}", _machine, _logName, _filter, e.Message) });

                //Stacktrace ONLY to log
                LogEntry(string.Format("EventLogFormatter error: {0}\r\nStackTrace: {1}", retval[int.MinValue + 1][0], e.StackTrace), LogEntryType.Error);
            }

            return retval;
        }

        private Dictionary<string,string> ParseFilter(string _filter)
        {
            var values = new Dictionary<string, string>();

            //Split into sub filters
            if (string.IsNullOrEmpty(_filter) == false)
            {
                var subFilters = _filter.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var entry in subFilters)
                {
                    var value = entry.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (value.Length == 2)
                    {
                        values.Add(value[0], value[1]);
                    }
                }
            }

            return values;
        }

        private bool MatchFilter(EventLogEntry _log, Dictionary<string, string> _filterValues)
        {
            bool retval = true;

            //Are there a filter to use
            if (_filterValues.Count > 0)
            {
                retval = false;

                int matchCount = 0;

                foreach (var entry in _filterValues)
                {
                    switch(entry.Key.ToLower())
                    {
                        case "id":
                            if (_log.Index.ToString().ToLower().Trim() == entry.Value.ToLower().Trim()) matchCount++; 
                            break;
                        case "type":
                            switch (entry.Value.ToLower().Trim())
                            {
                                case "infoandwarn":
                                    if (_log.EntryType.ToString().ToLower() == "information"
                                        || _log.EntryType.ToString().ToLower() == "warning")
                                    {
                                        matchCount++;
                                    }
                                    break;
                                case "infoanderror":
                                    if (_log.EntryType.ToString().ToLower() == "information"
                                        || _log.EntryType.ToString().ToLower() == "error")
                                    {
                                        matchCount++;
                                    }
                                    break;
                                case "warnanderror":
                                    if (    _log.EntryType.ToString().ToLower() == "warning"
                                        ||  _log.EntryType.ToString().ToLower() == "error")
                                    {
                                        matchCount++;
                                    }
                                    break;
                                default:
                                    if (_log.EntryType.ToString().ToLower() == entry.Value.ToLower()) matchCount++;
                                    break;
                            }
                            break;
                        case "date":
                            DateTime filterVal1 = new DateTime();
                            int dayFilter1 = 0;
                            if (DateTime.TryParse(entry.Value, out filterVal1) == true)
                            {
                                if (_log.TimeGenerated >= filterVal1)
                                {
                                    matchCount++;
                                }
                            }
                            else if (int.TryParse(entry.Value, out dayFilter1) == true)
                            {
                                if (_log.TimeGenerated >= DateTime.Now.AddDays(dayFilter1))
                                {
                                    matchCount++;
                                }
                            }
                            break;
                        case "time":
                            DateTime filterVal2 = new DateTime();
                            int dayFilter2 = 0;
                            if (DateTime.TryParse(entry.Value, out filterVal2) == true)
                            {
                                if (_log.TimeWritten >= filterVal2)
                                {
                                    matchCount++;
                                }
                            }
                            else if (int.TryParse(entry.Value, out dayFilter2) == true)
                            {
                                if (_log.TimeWritten >= DateTime.Now.AddDays(dayFilter2))
                                {
                                    matchCount++;
                                }
                            }
                            break;
                        case "source":
                            if (_log.Source.ToString().ToLower().Trim() == entry.Value.ToLower().Trim()) matchCount++;
                            break;
                        case "category":
                            if (_log.Category.ToString().ToLower().Trim() == entry.Value.ToLower().Trim()) matchCount++;
                            break;
                        case "event":
                            if (_log.InstanceId.ToString().ToLower().Trim() == entry.Value.ToLower().Trim()) matchCount++;
                            break;
                        case "user":
                            if (_log.UserName.ToString().ToLower().Trim() == entry.Value.ToLower().Trim()) matchCount++;
                            break;
                        case "computer":
                            if (_log.MachineName.ToString().ToLower().Trim() == entry.Value.ToLower().Trim()) matchCount++;
                            break;
                        case "description":
                            if (_log.Message.ToLower().Contains(entry.Value.ToLower().Trim()) == true) matchCount++;
                            break;
                    };

                    if (matchCount >= _filterValues.Count)
                    {
                        retval = true;
                        break;
                    }
                }
            }

            return retval;
        }

        private string GenerateTypeEntry(EventLogEntry _log)
        {
            string retval = "";
            string type = _log.EntryType.ToString();

            switch(type)
            {
                case "Warning":
                    retval = string.Format("[image||{{UP}}/Keeper.Garrett.Formatters/EventLogFormatter/Warning.png|Warning] {0}", type);
                    break;
                case "Error":
                    retval = string.Format("[image||{{UP}}/Keeper.Garrett.Formatters/EventLogFormatter/Error.png|Error] {0}", type);
                    break;
                case "Information":
                default:
                    retval = string.Format("[image||{{UP}}/Keeper.Garrett.Formatters/EventLogFormatter/Information.png|Information] {0}", type);
                    break;
            }
            return retval;
        }
    }
}
