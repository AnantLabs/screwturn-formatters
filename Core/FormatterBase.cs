using System;
using System.Collections.Generic;
using System.Text;
using ScrewTurn.Wiki.PluginFramework;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Keeper.Garrett.ScrewTurn.Core
{
    [NoCoverage]
    public abstract class FormatterBase : IFormatterProviderV30
    {
        protected IHostV30 m_Host;
        protected string m_Config;

        public string Configuration { get { return m_Config; } }

        private static readonly Regex VersionRegex = new Regex(@"Version: (?<major>\d{1,3})\.(?<minor>\d{1,3})\.(?<build>\d{1,5})\.(?<revision>\d{1,5})", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        #region IFormatterProviderV30 Members

        private int m_ExecutionPriority = 50;

        public virtual int ExecutionPriority
        {
            get { return m_ExecutionPriority; }
        }

        public abstract string Format(string raw, ContextInformation context, FormattingPhase phase);

        public virtual bool PerformPhase1
        {
            get { return true; }
        }

        public virtual bool PerformPhase2
        {
            get { return true; }
        }

        public virtual bool PerformPhase3
        {
            get { return true; }
        }

        public virtual string PrepareTitle(string title, ContextInformation context)
        {
            return title;
        }

        #endregion

        #region IProviderV30 Members

        public virtual string ConfigHelpHtml
        {
            get { return "No configuration needed for this formatter."; }
        }

        public virtual ComponentInformation Information
        {
            get { return new ComponentInformation(this.GetType().Name, "Christian Hollerup Mikkelsen", string.Format("{0}", this.GetType().Assembly.GetName().Version ), "http://keeper.endoftheinternet.org/", null); }
        }

        public virtual void Init(IHostV30 host, string config)
        {
            this.m_Host = host;
            this.m_Config = config;
        }

        public virtual void Init(IHostV30 host, string config, int priority)
        {
            this.m_Host = host;
            this.m_Config = config;
            this.m_ExecutionPriority = (priority < 0 || priority > 100 ? 50 : priority);
        }

        public virtual void Init(IHostV30 host, string config, Page[] _pagesToCreate)
        {
            this.m_Host = host;
            this.m_Config = config;

            //Create pages if not allready there
            if (_pagesToCreate != null && _pagesToCreate.Length > 0)
            {
                CreatePages(_pagesToCreate);
            }
        }

        public virtual void Init(IHostV30 host, string config, int priority, Page[] _pagesToCreate)
        {
            this.m_Host = host;
            this.m_Config = config;
            this.m_ExecutionPriority = (priority < 0 || priority > 100 ? 50 : priority);

            //Create pages if not allready there
            if (_pagesToCreate != null && _pagesToCreate.Length > 0)
            {
                CreatePages(_pagesToCreate);
            }
        }

        public virtual void Shutdown()
        {
            //Cleanup not needed
        }

        private void CreatePages(Page[] _pagesToCreate)
        {
            foreach (var page in _pagesToCreate)
            {
                try
                {
                    var pInfo = m_Host.FindPage(page.Fullname);

                    var provider = GetDefaultPageStorageProvider(m_Host);

                    var version = Assembly.GetExecutingAssembly().GetName().Version;
                    int versionNo =   (version.Major * 1000) 
                                    + (version.Minor * 100) 
                                    + (version.Build * 10) 
                                    + (version.Revision);

                    if (provider != null)
                    {
                        //Page created yet?
                        if (pInfo == null)
                        {
                            pInfo = provider.AddPage(null, page.Fullname, DateTime.Now);

                            var pageContent = new PageContent(pInfo, page.Title, this.GetType().Name, DateTime.Now, string.Format("Version: {0}", version.ToString()), page.Content, page.Keywords, page.Description);
                            var result = provider.ModifyPage(pInfo, page.Title, this.GetType().Name, DateTime.Now, string.Format("Version: {0}", version.ToString()), page.Content, page.Keywords, page.Description, SaveMode.Normal);

                            m_Host.LogEntry(string.Format("{0} - {1} page {2}, {3}"
                                                , this.GetType().Name
                                                , (result == true ? "Created" : "Unable to create")
                                                , page.Fullname
                                                , string.Format("Version: {0}", version.ToString())
                                                )
                                            , LogEntryType.General, this.GetType().Name, this);
                        }
                        else
                        {
                            var existingPageContent = m_Host.GetPageContent(pInfo);

                            //Retreive version no
                            var matchVersion = VersionRegex.Match((string.IsNullOrEmpty(existingPageContent.Comment) == false ? existingPageContent.Comment : "Version: 0.0.0.0"));

                            int pageVersionNo = 0;
                            if (matchVersion.Success == true)
                            {
                                pageVersionNo += int.Parse(matchVersion.Groups["major"].Value) * 1000;
                                pageVersionNo += int.Parse(matchVersion.Groups["minor"].Value) * 100;
                                pageVersionNo += int.Parse(matchVersion.Groups["build"].Value) * 10;
                                pageVersionNo += int.Parse(matchVersion.Groups["revision"].Value);
                            }

                            //Overwrite
                            if (pageVersionNo < versionNo)
                            {
                                //Save the page and overwrite the old one
                                var result = provider.ModifyPage(pInfo
                                    , page.Title
                                    , this.GetType().Name
                                    , DateTime.Now
                                    , string.Format("Version: {0}", version.ToString()) //Set new version
                                    , page.Content
                                    , page.Keywords
                                    , page.Description
                                    , SaveMode.Normal);

                                m_Host.LogEntry(string.Format("{0} - {1} page {2} from {3} to {4}"
                                                    , this.GetType().Name
                                                    , (result == true ? "Updated" : "Unable to update")
                                                    , page.Fullname
                                                    , matchVersion.Groups[0]
                                                    , string.Format("Version: {0}", version.ToString())
                                                    )
                                                , LogEntryType.General, this.GetType().Name, this);
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    m_Host.LogEntry(string.Format("{0} - Init warning: Error creating page {1}\r\n{2}", this.GetType().Name, page.Fullname , e.Message), LogEntryType.Error, this.GetType().Name, this);
                }
            }
        }

        private IPagesStorageProviderV30 GetDefaultPageStorageProvider(IHostV30 _host)
        {
            string defaultPageStorageProviderName = _host.GetSettingValue(SettingName.DefaultPagesStorageProvider);

            var providers = _host.GetPagesStorageProviders(true);
            //Find matching provider, use default if none
            IPagesStorageProviderV30 retval = null;
            foreach (var prov in providers)
            {
                if (prov.GetType().FullName == defaultPageStorageProviderName)
                {
                    retval = prov;
                    break;
                }
            }
            return retval;
        }

        private string GetUserName()
        {
            string retval = this.GetType().Name;
            if(m_Host != null && m_Host.GetCurrentUser() != null)
            {
                retval = m_Host.GetCurrentUser().Username;
            }
            return retval;
        }

        protected void LogEntry(string _message, LogEntryType _type)
        {
            m_Host.LogEntry(_message, _type, GetUserName(), this);
        }

        #endregion
    }
}
