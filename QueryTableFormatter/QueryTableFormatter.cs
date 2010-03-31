using System;
using System.Collections.Generic;
using System.Text;
using ScrewTurn.Wiki.PluginFramework;
using System.Text.RegularExpressions;
using Keeper.Garrett.ScrewTurn.Core;
using Keeper.Garrett.ScrewTurn.QueryTableFormatter.Database;
using System.Reflection;

namespace Keeper.Garrett.ScrewTurn.QueryTableFormatter
{
    public class QueryTableFormatter : FormatterBase
    {
        public override bool PerformPhase1 { get { return true; } }
        public override bool PerformPhase2 { get { return false; } }
        public override bool PerformPhase3 { get { return false; } }
                                                                
        private static readonly Regex ConfigRegex = new Regex(@"\{(?<dblink>(.*?))=(?<dbtype>(.*?)),(?<connstr>(.*?))\}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        // Key#Query#Heading#ColumnOrder#Headers#TdblFormat#HeadFormat#RowFormat
        private static readonly Regex TagRegex = new Regex(@"\{QTable\((?<key>(.*?)),('(?<query>(.*?))')?,('(?<heading>(.*?))')?,('(?<columns>(.*?))')?,('(?<headers>(.*?))')?,('(?<tblFormat>(.*?))')?,('(?<headFormat>(.*?))')?,('(?<rowFormat>(.*?))')?\)\}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        private static Dictionary<string, DatabaseConfiguration> m_Connections = new Dictionary<string, DatabaseConfiguration>();

        public override void Init(IHostV30 _host, string _config)
        {
            string logMessage = "";
            LogEntryType logType = LogEntryType.General;

            try
            {
                base.Init(_host, _config, Help.HelpPages);

                ParseConfigurationString(_config);

                VerifyConnections();

                logMessage = string.Format("QueryTableFormatter - Init success ({0} links verified)", m_Connections.Count);
            }
            catch (Exception e)
            {
                logMessage = string.Format("QueryTableFormatter - Init failure: {0}", string.Format("{0}\r\n{1}", e.Message, e.StackTrace));
                logType = LogEntryType.Error;
            }

            LogEntry(logMessage, logType);
        }

        private void ParseConfigurationString(string _config)
        {
            m_Config = null;

            //Find links
            m_Connections.Clear();

            var matches = ConfigRegex.Matches(_config);
            if (matches != null && matches.Count > 0)
            {
                //Foreach db entry
                foreach (Match match in matches)
                {
                    if (match.Groups.Count == 4)
                    {
                        string key = match.Groups["dblink"].Value;
                        string dbType = match.Groups["dbtype"].Value;
                        string connStr = match.Groups["connstr"].Value;

                        if (string.IsNullOrEmpty(key) == false &&
                             string.IsNullOrEmpty(dbType) == false &&
                             string.IsNullOrEmpty(connStr) == false)
                        {
                            switch (dbType.ToLower())
                            {
                                case "oracle":
                                    m_Connections.Add(key, new DatabaseConfiguration(connStr, DatabaseType.Oracle));
                                    break;
                                case "mssql":
                                    m_Connections.Add(key, new DatabaseConfiguration(connStr, DatabaseType.MsSql));
                                    break;
                                case "mysql":
                                    m_Connections.Add(key, new DatabaseConfiguration(connStr, DatabaseType.MySql));
                                    break;
                                case "sqlite":
                                    m_Connections.Add(key, new DatabaseConfiguration(connStr, DatabaseType.SqLite));
                                    break;
                                default:
                                    break;
                            };

                        }
                    }

                }
            }
        }

        private void VerifyConnections()
        {
            foreach (var conn in m_Connections)
            {
                try
                {
                    IDatabase connection = GetConnection(conn.Key);
                    connection.Connect();
                    connection.Disconnet();
                }
                catch (Exception e)
                {
                    string msg = string.Format("Connection string error\r\nDBLinkKey: {0}\r\nMessage: {1}\r\nStackTrace: {2}", conn.Key, e.Message, e.StackTrace);
                    LogEntry(string.Format("QueryTableFormatter - Init warning: {0}", msg), LogEntryType.Warning);
                }
            }
        }

        private IDatabase GetConnection(string _connectionKey)
        {
            IDatabase retval = null;
            if (m_Connections.ContainsKey(_connectionKey))
            {
                switch (m_Connections[_connectionKey].Type)
                {
                    case DatabaseType.Oracle:
                        retval = new Database.Oracle(m_Connections[_connectionKey].ConnectionString);
                        break;
                    case DatabaseType.MsSql:
                        retval = new Database.MsSql(m_Connections[_connectionKey].ConnectionString);
                        break;
                    case DatabaseType.MySql:
                        retval = new Database.MySql(m_Connections[_connectionKey].ConnectionString);
                        break;
                    case DatabaseType.SqLite:
                        retval = new Database.SqLite(m_Connections[_connectionKey].ConnectionString);
                        break;
                    default:
                        break;
                };
            }

            return retval;
        }

        public override string ConfigHelpHtml
        {
            get
            {
                return string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}",
                    "<p>WARNING: NEVER use a connectionstring which can access Produciton environment in this formatter!</p>",
                    "<p>WARNING: Ensure that you ONLY use DB users that have READONLY access! AND NO ACCESS TO PRODUCTION!</p>",
                    "<p>The plugin WILL check for common keywords such as DELETE/UPDATE/CREATE/GRANT/etc (125 in all) but it is YOUR responsability to ensure that the DB user cannot harm the DB.</p>",
                    "<p>WARNING: Use at your own responsability! Users MAY try to create harmfull queries!</p>",
                    "<p>WARNING: Only use this plugin in environments where you can trust the users, ex an intranet, NOT the WWW!</p>",
                    "<p></p>",
                    "<p>Usage: {DBLinkKey=DBType,DBConnectionString}</p>",
                    "<p>Every link must be included in {..}, Every link must start on a newline.",
                    "<p>Where:</p>",
                    "<p> - DBLinkKey, the key you write in your tag to tell the formatter which db to query. Keys MUST be unique.</p>",
                    "<p> - DBType, must be one of: Oracle, MySql, MsSql, SqLite.</p>",
                    "<p> - DBConnectionString, the connection string used to connect to the db. May NOT include newlines.</p>",
                    "<p>Examples:</p>",
                    "<p> - {MyLink1=Oracle,User Id=MyUser;Password=MyPass;Data Source=TnsName;} </p>",
                    "<p> - {MyLink2=MsSql,Data Source=MyServer\\SQLEXPRESS;Initial Catalog=MyCatalog;User ID=MyUser;Password=MyPass;} </p>",
                    "<p> - {MyLink3=MySql,Database=MyDB;Data Source=MyServer;User Id=MyUser;Password=MyPass;} </p>",
                    "<p> - {MyLink4=SqLite,Data Source=MyDB.sqlite;Version=3;}"
                    );
            }
        }

        public override string Format(string raw, ContextInformation context, FormattingPhase phase)
        {
            try
            {
                if (m_Connections.Count > 0 
                    && context.Context == FormattingContext.PageContent
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
                                    var dbKey = match.Groups["key"].Value;
                                    var query = match.Groups["query"].Value;

                                    #region Formatting of table

                                    var columns = new List<int>();
                                    var headers = new List<string>();


                                    //Parse columns to show
                                    if (string.IsNullOrEmpty(match.Groups["columns"].Value) == false)
                                    {
                                        var tmpColumns = match.Groups["columns"].Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                        foreach (var str in tmpColumns)
                                        {
                                            int columnIndex;
                                            if (int.TryParse(str, out columnIndex) == true)
                                            {
                                                columns.Add(columnIndex);
                                            }
                                        }
                                    }


                                    //Parse custom headers to show
                                    if (string.IsNullOrEmpty(match.Groups["headers"].Value) == false)
                                    {
                                        var tmpColumns = match.Groups["headers"].Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                        foreach (var str in tmpColumns)
                                        {
                                            headers.Add(str);
                                        }
                                    }


                                    //Setup table formatting, default to system/wiki theme if no override present
                                    var heading =   (string.IsNullOrEmpty(match.Groups["heading"].Value) == false       ? match.Groups["heading"].Value.Trim() : "");
                                    var tblFormat = (string.IsNullOrEmpty(match.Groups["tblFormat"].Value) == false     ? match.Groups["tblFormat"].Value.Trim() : "");
                                    var headFormat =(string.IsNullOrEmpty(match.Groups["headFormat"].Value) == false    ? match.Groups["headFormat"].Value.Trim() : "");
                                    var rowFormat = (string.IsNullOrEmpty(match.Groups["rowFormat"].Value) == false     ? match.Groups["rowFormat"].Value.Trim() : "");

                                    #endregion

                                    //Prepare resulting table
                                    var resultTable = string.Format("No DBLinkKey found for {0}", dbKey);

                                    if (m_Connections.ContainsKey(dbKey) == true)
                                    {
                                        var actualHeaders = new List<string>();

                                        //Perform query
                                        var result = PerformQuery(dbKey, query, ref actualHeaders);

                                        //Generate table
                                        resultTable = Utility.TableGenerator.GenerateTable(result, 
                                                                    heading,
                                                                    columns,
                                                                    headers,
                                                                    actualHeaders,
                                                                    tblFormat,
                                                                    headFormat,
                                                                    rowFormat);
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
                LogEntry(string.Format("QueryTableFormatter error: {0} {1}", e.Message, e.StackTrace), LogEntryType.Error);
            }

            return raw;
        }

        private Dictionary<int,List<string>> PerformQuery(string _key, string _query, ref List<string> _actualHeaders)
        {
            var retval = new Dictionary<int, List<string>>();

            try
            {
                IDatabase connection = GetConnection(_key);

                if (connection.Connect() == true)
                {
                    retval = connection.Query(_query);
                }

                //Update headers
                _actualHeaders = connection.Get_Column_Name_List_FromLatestQuery();

                //No results?
                if(retval.Count <= 0)
                {
                    retval.Add(int.MinValue, new List<string>() { string.Format("align=\"center\" colspan=\"{0}\" | <h2>Query revealed no results, table/view is empty.</h2>", _actualHeaders.Count) });
                }

                connection.Disconnet();
            }
            catch (Exception e)
            {
                retval.Add(int.MinValue + 1, new List<string>() { string.Format("Error connection to DB or Querying.\r\nDBLinkKey: {0}\r\nQuery: {1}\r\nMessage: {2}", _key, _query, e.Message) });

                //Stacktrace ONLY to log
                LogEntry(string.Format("QueryTableFormatter error: {0}\r\nStackTrace: {1}", retval[int.MinValue + 1][0], e.StackTrace), LogEntryType.Error);
            }

            return retval;
        }
    }
}
