using System;
using System.Collections.Generic;
using System.Linq;
using ScrewTurn.Wiki.PluginFramework;
using System.Reflection;
using Keeper.Garrett.ScrewTurn.Core;

namespace Keeper.Garrett.ScrewTurn.Utility
{
    public class FilePersister
    {
        private class FileDescription
        {
            public string Name { get; set; }
            public string Resource { get; set; }
        }

        private readonly string m_BaseDir = "Keeper.Garrett.Formatters";
        private string m_FormatterDir = "";

        private Dictionary<string, List<FileDescription>> m_Files = new Dictionary<string, List<FileDescription>>();

        public FilePersister(string _formatterDir)
        {
            m_FormatterDir = _formatterDir;
            m_Files.Add("/", new List<FileDescription>());
        }

        public void AddDir(string _directoryName)
        {
            m_Files.Add(_directoryName, new List<FileDescription>());
        }

        public void AddFile(string _directoryName, string _filename, string _resourceName)
        {
            m_Files[_directoryName].Add(new FileDescription() { Name = _filename, Resource = _resourceName });
        }

        public void StoreFiles(IHostV30 _host)
        {
            var provider = GetDefaultFilesStorageProvider(_host);

            CreateDirs(provider);

            CreateFiles(provider);
        }

        private IFilesStorageProviderV30 GetDefaultFilesStorageProvider(IHostV30 _host)
        {
            string defaultFilesStorageProviderName = _host.GetSettingValue(SettingName.DefaultFilesStorageProvider);

            var providers = _host.GetFilesStorageProviders(true);
            //Find matching provider, use default if none
            foreach (var prov in providers)
            {
                if (prov.Information.Name == defaultFilesStorageProviderName)
                {
                    return prov;
                }
            }

            return null;// _host.GetFilesStorageProviders(true).First(p => p.GetType().FullName == defaultFilesStorageProviderName);
        }

        private void CreateDirs(IFilesStorageProviderV30 _provider)
        {
            //Create Base if not allready there
            if (DirectoryExists(_provider, "/" ,m_BaseDir) == false)
            {
                _provider.CreateDirectory("/", m_BaseDir);
            }

            //Create Formatter dir if not allready there
            var baseDir = string.Format("/{0}/", m_BaseDir);
            if (DirectoryExists(_provider, baseDir, m_FormatterDir) == false)
            {
                _provider.CreateDirectory(baseDir, m_FormatterDir);
            }

            //Create all additional dirs
            var formatterDir = string.Format("/{0}/{1}", m_BaseDir, m_FormatterDir);
            foreach (var dir in m_Files)
            {
                if (dir.Key != "/")
                {
                    if (DirectoryExists(_provider, formatterDir, dir.Key) == false)
                    {
                        _provider.CreateDirectory(formatterDir, dir.Key);
                    }
                }
            }
        }

        private bool DirectoryExists(IFilesStorageProviderV30 _provider, string _searchDir, string _directoryName)
        {
            var directoryList = _provider.ListDirectories(_searchDir);
            foreach (string dir in directoryList)
            {
                if (dir.Trim('/').EndsWith(_directoryName) == true)
                {
                    return true;
                }
            }
            return false;
        }

        private void CreateFiles(IFilesStorageProviderV30 _provider)
        {
            var formatterDir = string.Format("/{0}/{1}", m_BaseDir, m_FormatterDir);

            //Create the files (overwrite always)
            foreach (var dir in m_Files)
            {
                //Use either root of formatter dir or use sub dir
                var dirName = (dir.Key != "/" ? string.Format("{0}/{1}", formatterDir, dir.Key) : formatterDir);

                foreach (var file in dir.Value)
                {
                    if (FileExists(_provider, dirName, file.Name) == false)
                    {
                        var fullPath = string.Format("{0}/{1}", dirName, file.Name);
                        _provider.StoreFile(fullPath, Assembly.GetExecutingAssembly().GetManifestResourceStream(file.Resource), true);
                    }
                }
            }
        }

        private bool FileExists(IFilesStorageProviderV30 filesStorageProvider, string directory, string fileName)
        {
            var filesList = filesStorageProvider.ListFiles(directory);
            var searchPath = string.Format("{0}/{1}", directory, fileName);
            foreach (string file in filesList)
            {
                if (file == searchPath) return true;
            }
            return false;
        }
    }
}
