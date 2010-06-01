using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using ScrewTurn.Wiki.PluginFramework;
using System.Web;
using Keeper.Garrett.ScrewTurn;
using Keeper.Garrett.ScrewTurn.FileListFormatter;

namespace Formatters.Tests
{
    [TestFixture]
    [Category("Formatter")]
    public class FileListFormatter_Test
    {
        #region Helpers
        private void GetLogCounts(out int _warn, out int _err, out int _gen, IList<Object[]> _args)
        {
            _warn = _err = _gen = 0;

            foreach (var arg in _args)
            {
                switch (arg[1].ToString())
                {
                    case "Warning":
                        _warn++;
                        break;
                    case "Error":
                        _err++;
                        break;
                    case "General":
                        _gen++;
                        break;
                }
            }
        }
        #endregion

        [Test]
        public void Missing_Provider()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { });

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' prov='Local Storage Provider' type=* dwnl=true details=downloads} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla {FileList file='*.*' prov='Local Storage Provider' type=* dwnl=true details=downloads} bla bla bla", retval);

            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(4, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        public void Default_Provider()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 10));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(100, new DateTime(2010, 1, 2), 20));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 3), 30));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' type=* details='downloads' dwnl=true} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [GetFile.aspx?File=file3.zip|file3.zip] (30 downloads) \n* [GetFile.aspx?File=file2.txt|file2.txt] (20 downloads) \n* [GetFile.aspx?File=file1.exe|file1.exe] (10 downloads) \n bla bla bla", retval);
        }

        [Test]
        public void Different_Provider()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider1 = MockRepository.GenerateStub<IFilesStorageProviderV30>();
            var provider2 = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider1, provider2 });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider 1");
            provider1.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider 1", "", "", "", ""));
            provider2.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider 2", "", "", "", ""));

            provider2.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider2.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 10));
            provider2.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(100, new DateTime(2010, 1, 2), 20));
            provider2.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 3), 30));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' prov='Local Storage Provider 2' type=* dwnl=true details=downloads} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [GetFile.aspx?File=file3.zip|file3.zip] (30 downloads) \n* [GetFile.aspx?File=file2.txt|file2.txt] (20 downloads) \n* [GetFile.aspx?File=file1.exe|file1.exe] (10 downloads) \n bla bla bla", retval);
        }

        [Test]
        public void Default_Values()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 10));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(100, new DateTime(2010, 1, 2), 20));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 3), 30));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert         
            Assert.AreEqual("bla bla bla  \n* [GetFile.aspx?File=file3.zip|file3.zip]  \n* [GetFile.aspx?File=file2.txt|file2.txt]  \n* [GetFile.aspx?File=file1.exe|file1.exe]  \n bla bla bla", retval);
        }

        [Test]
        public void No_Directory()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Throw(new Exception("Directory does not exist"));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileListfile='/Test/*.*' sort='name,asc' details='size'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla (No directory found matching \"/Test/\".) \n bla bla bla", retval);
        }

        #region SortOrder

        [Test]
        public void SortOrder_Filename_Asc()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(200, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' sort='name,asc' details=downloads} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [GetFile.aspx?File=file1.exe|file1.exe] (300 downloads) \n* [GetFile.aspx?File=file2.txt|file2.txt] (200 downloads) \n* [GetFile.aspx?File=file3.zip|file3.zip] (100 downloads) \n bla bla bla", retval);
        }

        [Test]
        public void SortOrder_Filename_Desc()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(200, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' sort='name,desc' details=downloads} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [GetFile.aspx?File=file3.zip|file3.zip] (100 downloads) \n* [GetFile.aspx?File=file2.txt|file2.txt] (200 downloads) \n* [GetFile.aspx?File=file1.exe|file1.exe] (300 downloads) \n bla bla bla", retval);
        }

        [Test]
        public void SortOrder_Downloads_Asc()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(200, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' sort='downloads,asc' details=downloads} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [GetFile.aspx?File=file3.zip|file3.zip] (100 downloads) \n* [GetFile.aspx?File=file2.txt|file2.txt] (200 downloads) \n* [GetFile.aspx?File=file1.exe|file1.exe] (300 downloads) \n bla bla bla", retval);
        }

        [Test]
        public void SortOrder_Downloads_Desc()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(200, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' sort='downloads,desc' details=downloads} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [GetFile.aspx?File=file1.exe|file1.exe] (300 downloads) \n* [GetFile.aspx?File=file2.txt|file2.txt] (200 downloads) \n* [GetFile.aspx?File=file3.zip|file3.zip] (100 downloads) \n bla bla bla", retval);
        }

        [Test]
        public void SortOrder_FileSize_Asc()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(100, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(200, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' sort='size,asc' details=downloads} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [GetFile.aspx?File=file2.txt|file2.txt] (200 downloads) \n* [GetFile.aspx?File=file3.zip|file3.zip] (100 downloads) \n* [GetFile.aspx?File=file1.exe|file1.exe] (300 downloads) \n bla bla bla", retval);
        }

        [Test]
        public void SortOrder_FileSize_Desc()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(500, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' sort='size,desc' details=downloads} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [GetFile.aspx?File=file2.txt|file2.txt] (200 downloads) \n* [GetFile.aspx?File=file1.exe|file1.exe] (300 downloads) \n* [GetFile.aspx?File=file3.zip|file3.zip] (100 downloads) \n bla bla bla", retval);
        }

        [Test]
        public void SortOrder_ModDate_Asc()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(100, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(200, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' sort='date,asc' details=downloads} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [GetFile.aspx?File=file3.zip|file3.zip] (100 downloads) \n* [GetFile.aspx?File=file1.exe|file1.exe] (300 downloads) \n* [GetFile.aspx?File=file2.txt|file2.txt] (200 downloads) \n bla bla bla", retval);
        }

        [Test]
        public void SortOrder_ModDate_Desc()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(500, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' sort='date,desc' details=downloads} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [GetFile.aspx?File=file2.txt|file2.txt] (200 downloads) \n* [GetFile.aspx?File=file1.exe|file1.exe] (300 downloads) \n* [GetFile.aspx?File=file3.zip|file3.zip] (100 downloads) \n bla bla bla", retval);
        }

        [Test]
        public void SortOrder_ToMany()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(500, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' sort='date,desc,date,desc,date,desc' details=downloads} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [GetFile.aspx?File=file2.txt|file2.txt] (200 downloads) \n* [GetFile.aspx?File=file1.exe|file1.exe] (300 downloads) \n* [GetFile.aspx?File=file3.zip|file3.zip] (100 downloads) \n bla bla bla", retval);
        }
        #endregion

        #region Primitive Details

        [Test]
        public void Primitive_Details_None_Numbered()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(200, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' type=# sort='name,asc'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n# [GetFile.aspx?File=file1.exe|file1.exe]  \n# [GetFile.aspx?File=file2.txt|file2.txt]  \n# [GetFile.aspx?File=file3.zip|file3.zip]  \n bla bla bla", retval);
        }

        [Test]
        public void Primitive_Details_None()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(200, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' type=* sort='name,asc'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [GetFile.aspx?File=file1.exe|file1.exe]  \n* [GetFile.aspx?File=file2.txt|file2.txt]  \n* [GetFile.aspx?File=file3.zip|file3.zip]  \n bla bla bla", retval);
        }

        [Test]
        public void Primitive_Details_Downloads()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(200, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' sort='name,asc' details=downloads} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [GetFile.aspx?File=file1.exe|file1.exe] (300 downloads) \n* [GetFile.aspx?File=file2.txt|file2.txt] (200 downloads) \n* [GetFile.aspx?File=file3.zip|file3.zip] (100 downloads) \n bla bla bla", retval);
        }

        [Test]
        public void Primitive_Details_DownloadsAndSize()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(200, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' sort='name,asc' details='downloads,size'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [GetFile.aspx?File=file1.exe|file1.exe] (300 B, 300 downloads) \n* [GetFile.aspx?File=file2.txt|file2.txt] (200 B, 200 downloads) \n* [GetFile.aspx?File=file3.zip|file3.zip] (100 B, 100 downloads) \n bla bla bla", retval);
        }

        [Test]
        public void Primitive_Details_DownloadsAndModDate()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(200, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' sort='name,asc' details='downloads,date'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [GetFile.aspx?File=file1.exe|file1.exe] (02-01-2010 00:00:00, 300 downloads) \n* [GetFile.aspx?File=file2.txt|file2.txt] (03-01-2010 00:00:00, 200 downloads) \n* [GetFile.aspx?File=file3.zip|file3.zip] (01-01-2010 00:00:00, 100 downloads) \n bla bla bla", retval);
        }

        [Test]
        public void Primitive_Details_DownloadsAndSizeAndModDate()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(3000, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(2000, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(1000, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' sort='name,asc' details='downloads,size,date'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [GetFile.aspx?File=file1.exe|file1.exe] (02-01-2010 00:00:00, 3 KB, 300 downloads) \n* [GetFile.aspx?File=file2.txt|file2.txt] (03-01-2010 00:00:00, 2 KB, 200 downloads) \n* [GetFile.aspx?File=file3.zip|file3.zip] (01-01-2010 00:00:00, 1 KB, 100 downloads) \n bla bla bla", retval);
        }

        [Test]
        public void Primitive_Details_ToMany()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(200, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileListfile='*.*' sort='name,asc' details='size,size,size,size,size'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [GetFile.aspx?File=file1.exe|file1.exe]  \n* [GetFile.aspx?File=file2.txt|file2.txt]  \n* [GetFile.aspx?File=file3.zip|file3.zip]  \n bla bla bla", retval);
        }

        [Test]
        public void Primitive_Details_Size()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(200, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileListfile='*.*' sort='name,asc' details='size'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [GetFile.aspx?File=file1.exe|file1.exe] (300 B) \n* [GetFile.aspx?File=file2.txt|file2.txt] (200 B) \n* [GetFile.aspx?File=file3.zip|file3.zip] (100 B) \n bla bla bla", retval);
        }

        [Test]
        public void Primitive_Details_SizeAndModDate()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(30000, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(20000, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(10000, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileListfile='*.*' sort='name,asc' details='size,date'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [GetFile.aspx?File=file1.exe|file1.exe] (02-01-2010 00:00:00, 30 KB) \n* [GetFile.aspx?File=file2.txt|file2.txt] (03-01-2010 00:00:00, 20 KB) \n* [GetFile.aspx?File=file3.zip|file3.zip] (01-01-2010 00:00:00, 10 KB) \n bla bla bla", retval);
        }

        [Test]
        public void Primitive_Details_ModDate()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(200, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' sort='name,asc' details='date'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [GetFile.aspx?File=file1.exe|file1.exe] (02-01-2010 00:00:00) \n* [GetFile.aspx?File=file2.txt|file2.txt] (03-01-2010 00:00:00) \n* [GetFile.aspx?File=file3.zip|file3.zip] (01-01-2010 00:00:00) \n bla bla bla", retval);
        }
        #endregion

        #region Table Details

        [Test]
        public void Table_Details_None_Heading()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(200, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' type=table sort='name,asc' head='MyHeading'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert         
            AssertTable.VerifyTable(retval, null, "MyHeading", "", new List<string>() { "Name" }, new Dictionary<int, List<string>>()
            {
                { 0, new List<string>() { "[GetFile.aspx?File=file1.exe|file1.exe]" } },
                { 1, new List<string>() { "[GetFile.aspx?File=file2.txt|file2.txt]" } },
                { 2, new List<string>() { "[GetFile.aspx?File=file3.zip|file3.zip]" } }
            });
        }

        [Test]
        public void Table_Details_None_Header()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(200, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList( file='*.*' type=table sort='name,asc' head='MyHeading' colnames='MyHeader'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);


            //Assert        
            AssertTable.VerifyTable(retval, null, "MyHeading", "", new List<string>() { "MyHeader" }, new Dictionary<int, List<string>>()
            {
                { 0, new List<string>() { "[GetFile.aspx?File=file1.exe|file1.exe]" } },
                { 1, new List<string>() { "[GetFile.aspx?File=file2.txt|file2.txt]" } },
                { 2, new List<string>() { "[GetFile.aspx?File=file3.zip|file3.zip]" } }
            });
        }

        [Test]
        public void Table_Details_None()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(200, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' type=table sort='name,asc'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);


            //Assert         
            AssertTable.VerifyTable(retval, null, null, "", new List<string>() { "Name" }, new Dictionary<int, List<string>>()
            {
                { 0, new List<string>() { "[GetFile.aspx?File=file1.exe|file1.exe]" } },
                { 1, new List<string>() { "[GetFile.aspx?File=file2.txt|file2.txt]" } },
                { 2, new List<string>() { "[GetFile.aspx?File=file3.zip|file3.zip]" } }
            });
        }

        [Test]
        public void Table_Details_Downloads()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(200, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' type=table sort='name,asc' cols='downloads'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            AssertTable.VerifyTable(retval, null, null, "", new List<string>() { "Name", "Downloads" }, new Dictionary<int, List<string>>()
            {
                { 0, new List<string>() { "[GetFile.aspx?File=file1.exe|file1.exe]", "300" } },
                { 1, new List<string>() { "[GetFile.aspx?File=file2.txt|file2.txt]", "200" } },
                { 2, new List<string>() { "[GetFile.aspx?File=file3.zip|file3.zip]", "100" } }
            });
        }

        [Test]
        public void Table_Details_DownloadsAndSize()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(200, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' type=table sort='name,asc' cols='downloads,size'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert         
            AssertTable.VerifyTable(retval, null, null, "", new List<string>() { "Name", "Downloads", "Size" }, new Dictionary<int, List<string>>()
            {
                { 0, new List<string>() { "[GetFile.aspx?File=file1.exe|file1.exe]", "300", "300 B" } },
                { 1, new List<string>() { "[GetFile.aspx?File=file2.txt|file2.txt]", "200", "200 B" } },
                { 2, new List<string>() { "[GetFile.aspx?File=file3.zip|file3.zip]", "100", "100 B" } }
            });
        }

        [Test]
        public void Table_Details_DownloadsAndModDate()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(200, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' type=table sort='name,asc' cols='name,downloads,date'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert         
            AssertTable.VerifyTable(retval, null, null, "", new List<string>() { "Name", "Downloads", "Last Modified" }, new Dictionary<int, List<string>>()
            {
                { 0, new List<string>() { "[GetFile.aspx?File=file1.exe|file1.exe]","300", "02-01-2010 00:00:00" } },
                { 1, new List<string>() { "[GetFile.aspx?File=file2.txt|file2.txt]","200", "03-01-2010 00:00:00" } },
                { 2, new List<string>() { "[GetFile.aspx?File=file3.zip|file3.zip]","100", "01-01-2010 00:00:00" } }
            });
        }

        [Test]
        public void Table_Details_DownloadsAndSizeAndModDate()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(3000, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(2000, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(1000, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' type=table sort='name,asc' cols='downloads,size,date'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert         
            AssertTable.VerifyTable(retval, null, null, "", new List<string>() { "Name", "Downloads", "Size", "Last Modified"  }, new Dictionary<int, List<string>>()
            {
                { 0, new List<string>() { "[GetFile.aspx?File=file1.exe|file1.exe]", "300", "3 KB", "02-01-2010 00:00:00" } },
                { 1, new List<string>() { "[GetFile.aspx?File=file2.txt|file2.txt]", "200", "2 KB", "03-01-2010 00:00:00" } },
                { 2, new List<string>() { "[GetFile.aspx?File=file3.zip|file3.zip]", "100", "1 KB", "01-01-2010 00:00:00" } }
            });
        }

        [Test]
        public void Table_Details_All()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(3000, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(2000, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(1000, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' type=table sort='name,asc' cols='all' style=generic} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert         
            AssertTable.VerifyTable(retval, "generic", null, "", new List<string>() { "Name", "Downloads", "Size", "Last Modified" }, new Dictionary<int, List<string>>()
            {
                { 0, new List<string>() { "[GetFile.aspx?File=file1.exe|file1.exe]", "300", "3 KB", "02-01-2010 00:00:00" } },
                { 1, new List<string>() { "[GetFile.aspx?File=file2.txt|file2.txt]", "200", "2 KB", "03-01-2010 00:00:00" } },
                { 2, new List<string>() { "[GetFile.aspx?File=file3.zip|file3.zip]", "100", "1 KB", "01-01-2010 00:00:00" } }
            });
        }

        [Test]
        public void Table_Details_Size()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(200, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' type=table sort='name,asc' cols='size'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert         
            AssertTable.VerifyTable(retval, null, null, "", new List<string>() { "Name", "Size" }, new Dictionary<int, List<string>>()
            {
                { 0, new List<string>() { "[GetFile.aspx?File=file1.exe|file1.exe]", "300 B" } },
                { 1, new List<string>() { "[GetFile.aspx?File=file2.txt|file2.txt]", "200 B" } },
                { 2, new List<string>() { "[GetFile.aspx?File=file3.zip|file3.zip]", "100 B" } }
            });
        }

        [Test]
        public void Table_Details_SizeAndModDate()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(30000, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(20000, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(10000, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' type=table sort='name,asc' cols='date,size'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert         
            AssertTable.VerifyTable(retval, null, null, "", new List<string>() { "Name", "Last Modified", "Size"  }, new Dictionary<int, List<string>>()
            {
                { 0, new List<string>() { "[GetFile.aspx?File=file1.exe|file1.exe]", "02-01-2010 00:00:00", "30 KB" } },
                { 1, new List<string>() { "[GetFile.aspx?File=file2.txt|file2.txt]", "03-01-2010 00:00:00", "20 KB" } },
                { 2, new List<string>() { "[GetFile.aspx?File=file3.zip|file3.zip]", "01-01-2010 00:00:00", "10 KB" } }
            }); 
        }

        [Test]
        public void Table_Details_ModDate()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(200, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' type=table sort='name,asc' cols='date'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert         
            AssertTable.VerifyTable(retval, null, null, "", new List<string>() { "Name", "Last Modified" }, new Dictionary<int, List<string>>()
            {
                { 0, new List<string>() { "[GetFile.aspx?File=file1.exe|file1.exe]", "02-01-2010 00:00:00" } },
                { 1, new List<string>() { "[GetFile.aspx?File=file2.txt|file2.txt]", "03-01-2010 00:00:00" } },
                { 2, new List<string>() { "[GetFile.aspx?File=file3.zip|file3.zip]", "01-01-2010 00:00:00" } }
            }); 
        }

        [Test]
        public void Table_Cols_Bad()
        {
            //Arrange
            var formatter = new FileListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe", "file2.txt", "file3.zip" });
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(3000, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(2000, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(1000, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList file='*.*' type=table sort='name,asc' cols='XXyyZZ'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert         
            AssertTable.VerifyTable(retval, null, null, "", new List<string>() { "Name" }, new Dictionary<int, List<string>>()
            {
                { 0, new List<string>() { "[GetFile.aspx?File=file1.exe|file1.exe]" } },
                { 1, new List<string>() { "[GetFile.aspx?File=file2.txt|file2.txt]" } },
                { 2, new List<string>() { "[GetFile.aspx?File=file3.zip|file3.zip]" } }
            });
        }

        #endregion
    }
}
