using System;
using System.Collections.Generic;
using System.Text;
using ScrewTurn.Wiki.PluginFramework;
using System.Reflection;
using System.IO;

namespace Keeper.Garrett.ScrewTurn.Core
{
    [NoCoverage]
    public abstract class FormatterBase : IFormatterProviderV30
    {
        protected IHostV30 m_Host;
        protected string m_Config;

        public string Configuration { get { return m_Config; } }

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

                    //Page created yet?
                    if (pInfo == null)
                    {
                        //Find the default provider and create pages
                        var defaultProvider = m_Host.GetDefaultProvider(typeof(IPagesStorageProviderV30));
                        var providers = m_Host.GetPagesStorageProviders(true);
                        IPagesStorageProviderV30 provider = null;
                        foreach(var prov in providers)
                        {
                            if(prov.GetType().FullName == defaultProvider)
                            {
                                provider = prov;
                            }
                        }

                        if (provider != null)
                        {
                            pInfo = provider.AddPage(null, page.Fullname, DateTime.Now);

                            var pageContent = new PageContent(pInfo, page.Title, this.GetType().Name, DateTime.Now, "", page.Content, page.Keywords, page.Description);
                            var result = provider.ModifyPage(pInfo, page.Title, this.GetType().Name, DateTime.Now, "", page.Content, page.Keywords, page.Description, SaveMode.Normal);

                            m_Host.LogEntry(string.Format("{0} - {1} page {2}", this.GetType().Name, (result == true ? "Created" : "Unable to create"), page.Fullname), LogEntryType.General, this.GetType().Name, this);
                        }
                    }
                }
                catch(Exception e)
                {
                    m_Host.LogEntry(string.Format("{0} - Init warning: Error creating page {1}\r\n{2}", this.GetType().Name, page.Fullname , e.Message), LogEntryType.Error, this.GetType().Name, this);
                }
            }
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

        protected void StoreFiles(string _dirName, Dictionary<string, MemoryStream> _filesToSave)
        {
            if (m_Host != null)
            {
                string storeName = "Keeper.Garrett.Formatters";
                var providers = m_Host.GetFilesStorageProviders(true);
                if (providers != null && providers.Length > 0)
                {
                    var provider = providers[0];

                    var rootDirs = provider.ListDirectories("/");
                    bool foundDirectory = false;
                    foreach(var dir in rootDirs)
                    {
                        if (dir.Contains(storeName) == true)
                        {
                            foundDirectory = true;
                            break;
                        }
                    }
                    if (foundDirectory == false)
                    {
                        provider.CreateDirectory("/", storeName);
                    }


                    var formatterDirs = provider.ListDirectories(string.Format("/{0}",storeName));
                    foundDirectory = false;
                    foreach (var dir in formatterDirs)
                    {
                        if (dir.Contains(_dirName) == true)
                        {
                            foundDirectory = true;
                            break;
                        }
                    }
                    if (foundDirectory == false)
                    {
                        provider.CreateDirectory(string.Format("/{0}/", storeName), _dirName);
                    }

                    foreach(var fileEntry in _filesToSave)
                    {
                        provider.StoreFile(string.Format("/{0}/{1}/{2}",storeName,_dirName, fileEntry.Key), fileEntry.Value, true);
                    }
                }
            }
        }

        #endregion
    }
}
