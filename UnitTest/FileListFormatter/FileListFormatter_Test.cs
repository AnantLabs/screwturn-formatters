using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using ScrewTurn.Wiki.PluginFramework;
using System.Web;
using Keeper.Garrett.ScrewTurn;
using Keeper.Garrett.ScrewTurn.FileListFormatter;

namespace UnitTest
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
        public void VerifyPhaseSetup()
        {
            //Arrange
            var formatter = new FileListFormatter();

            //Act
            var phase1 = formatter.PerformPhase1;
            var phase2 = formatter.PerformPhase2;
            var phase3 = formatter.PerformPhase3;

            //Assert
            Assert.AreEqual(true, phase1);
            Assert.AreEqual(false, phase2);
            Assert.AreEqual(false, phase3);
        }

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
            string input = "bla bla bla {FileList('*.*','Local Storage Provider',*,7,true,true,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla {FileList('*.*','Local Storage Provider',*,7,true,true,,,,,)} bla bla bla", retval);

            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(3, errorCount);
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
            string input = "bla bla bla {FileList('*.*',,*,7,true,1,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [file3.zip|file3.zip] (30 downloads) \n* [file2.txt|file2.txt] (20 downloads) \n* [file1.exe|file1.exe] (10 downloads) \n bla bla bla", retval);
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
            string input = "bla bla bla {FileList('*.*','Local Storage Provider 2',*,7,true,1,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [file3.zip|file3.zip] (30 downloads) \n* [file2.txt|file2.txt] (20 downloads) \n* [file1.exe|file1.exe] (10 downloads) \n bla bla bla", retval);
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
            string input = "bla bla bla {FileList('','',,,,,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert         
            Assert.AreEqual("bla bla bla  \n* file1.exe  \n* file2.txt  \n* file3.zip  \n bla bla bla", retval);
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
            string input = "bla bla bla {FileList('*.*',,*,0,true,1,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [file1.exe|file1.exe] (300 downloads) \n* [file2.txt|file2.txt] (200 downloads) \n* [file3.zip|file3.zip] (100 downloads) \n bla bla bla", retval);
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
            string input = "bla bla bla {FileList('*.*',,*,1,true,1,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [file3.zip|file3.zip] (100 downloads) \n* [file2.txt|file2.txt] (200 downloads) \n* [file1.exe|file1.exe] (300 downloads) \n bla bla bla", retval);
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
            string input = "bla bla bla {FileList('*.*',,*,2,true,1,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [file3.zip|file3.zip] (100 downloads) \n* [file2.txt|file2.txt] (200 downloads) \n* [file1.exe|file1.exe] (300 downloads) \n bla bla bla", retval);
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
            string input = "bla bla bla {FileList('*.*',,*,3,true,1,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [file1.exe|file1.exe] (300 downloads) \n* [file2.txt|file2.txt] (200 downloads) \n* [file3.zip|file3.zip] (100 downloads) \n bla bla bla", retval);
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
            string input = "bla bla bla {FileList('*.*',,*,4,true,1,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [file2.txt|file2.txt] (200 downloads) \n* [file3.zip|file3.zip] (100 downloads) \n* [file1.exe|file1.exe] (300 downloads) \n bla bla bla", retval);
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
            string input = "bla bla bla {FileList('*.*',,*,5,true,1,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [file2.txt|file2.txt] (200 downloads) \n* [file1.exe|file1.exe] (300 downloads) \n* [file3.zip|file3.zip] (100 downloads) \n bla bla bla", retval);
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
            string input = "bla bla bla {FileList('*.*',,*,6,true,1,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [file3.zip|file3.zip] (100 downloads) \n* [file1.exe|file1.exe] (300 downloads) \n* [file2.txt|file2.txt] (200 downloads) \n bla bla bla", retval);
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
            string input = "bla bla bla {FileList('*.*',,*,7,true,1,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [file2.txt|file2.txt] (200 downloads) \n* [file1.exe|file1.exe] (300 downloads) \n* [file3.zip|file3.zip] (100 downloads) \n bla bla bla", retval);
        }
        #endregion

        #region Primitive Details

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
            string input = "bla bla bla {FileList('*.*',,*,0,true,0,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [file1.exe|file1.exe]  \n* [file2.txt|file2.txt]  \n* [file3.zip|file3.zip]  \n bla bla bla", retval);
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
            string input = "bla bla bla {FileList('*.*',,*,0,true,1,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [file1.exe|file1.exe] (300 downloads) \n* [file2.txt|file2.txt] (200 downloads) \n* [file3.zip|file3.zip] (100 downloads) \n bla bla bla", retval);
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
            string input = "bla bla bla {FileList('*.*',,*,0,true,2,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [file1.exe|file1.exe] (300 kb, 300 downloads) \n* [file2.txt|file2.txt] (200 kb, 200 downloads) \n* [file3.zip|file3.zip] (100 kb, 100 downloads) \n bla bla bla", retval);
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
            string input = "bla bla bla {FileList('*.*',,*,0,true,3,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [file1.exe|file1.exe] (02-01-2010 00:00:00, 300 downloads) \n* [file2.txt|file2.txt] (03-01-2010 00:00:00, 200 downloads) \n* [file3.zip|file3.zip] (01-01-2010 00:00:00, 100 downloads) \n bla bla bla", retval);
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
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(200, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList('*.*',,*,0,true,4,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [file1.exe|file1.exe] (02-01-2010 00:00:00, 300 kb, 300 downloads) \n* [file2.txt|file2.txt] (03-01-2010 00:00:00, 200 kb, 200 downloads) \n* [file3.zip|file3.zip] (01-01-2010 00:00:00, 100 kb, 100 downloads) \n bla bla bla", retval);
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
            string input = "bla bla bla {FileList('*.*',,*,0,true,5,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [file1.exe|file1.exe] (300 kb) \n* [file2.txt|file2.txt] (200 kb) \n* [file3.zip|file3.zip] (100 kb) \n bla bla bla", retval);
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
            provider.Expect(x => x.GetFileDetails("file1.exe")).Return(new FileDetails(300, new DateTime(2010, 1, 2), 300));
            provider.Expect(x => x.GetFileDetails("file2.txt")).Return(new FileDetails(200, new DateTime(2010, 1, 3), 200));
            provider.Expect(x => x.GetFileDetails("file3.zip")).Return(new FileDetails(100, new DateTime(2010, 1, 1), 100));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileList('*.*',,*,0,true,6,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [file1.exe|file1.exe] (02-01-2010 00:00:00, 300 kb) \n* [file2.txt|file2.txt] (03-01-2010 00:00:00, 200 kb) \n* [file3.zip|file3.zip] (01-01-2010 00:00:00, 100 kb) \n bla bla bla", retval);
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
            string input = "bla bla bla {FileList('*.*',,*,0,true,7,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [file1.exe|file1.exe] (02-01-2010 00:00:00) \n* [file2.txt|file2.txt] (03-01-2010 00:00:00) \n* [file3.zip|file3.zip] (01-01-2010 00:00:00) \n bla bla bla", retval);
        }
        #endregion

        [Test]
        public void PrimitiveList_StraightOrder_List1_Test()
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
            string input = "bla bla bla {FileList('*.*','Local Storage Provider',*,7,true,1,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [file3.zip|file3.zip] (30 downloads) \n* [file2.txt|file2.txt] (20 downloads) \n* [file1.exe|file1.exe] (10 downloads) \n bla bla bla", retval);
        }
/*
        [Test]
        public void PrimitiveList_StraightOrder_List2_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo = MockRepository.GenerateStub<CategoryInfo>("MyCategory", provider);
            var pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, DateTime.Now);
            var pageInfo2 = new PageInfo("MyPage2", provider, DateTime.Now);
            var pageInfo3 = new PageInfo("MyPage3", provider, DateTime.Now);
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "", DateTime.Now, "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "", DateTime.Now, "", "", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo);
            catInfo.Pages = pages;

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {CategoryList(MyCategory,#,false,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n# [MyPage1|Page 1] \n# [MyPage2|Page 2] \n# [MyPage3|Page 3] \n bla bla bla", retval);
        }

        [Test]
        public void PrimitiveList_MessyOrder_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo = MockRepository.GenerateStub<CategoryInfo>("MyCategory", provider);
            var pages = new string[] { "aaa", "MyPage3", "MyPage0" };
            var pageInfo1 = new PageInfo("aaa", provider, DateTime.Now);
            var pageInfo2 = new PageInfo("MyPage3", provider, DateTime.Now);
            var pageInfo3 = new PageInfo("MyPage0", provider, DateTime.Now);
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "BBB", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 0", "", DateTime.Now, "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "aaa", "", DateTime.Now, "", "", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo);
            catInfo.Pages = pages;

            host.Expect(x => x.FindPage("aaa")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage0")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            string input = "bla bla bla {CategoryList(MyCategory,*,false,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [MyPage0|aaa] \n* [aaa|BBB] \n* [MyPage3|Page 0] \n bla bla bla", retval);
        }

        [Test]
        public void PrimitiveList_StraightOrder_WithDescription_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo = MockRepository.GenerateStub<CategoryInfo>("MyCategory", provider);
            var pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, DateTime.Now);
            var pageInfo2 = new PageInfo("MyPage2", provider, DateTime.Now);
            var pageInfo3 = new PageInfo("MyPage3", provider, DateTime.Now);
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "", DateTime.Now, "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "", DateTime.Now, "", "", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo);
            catInfo.Pages = pages;

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            string input = "bla bla bla {CategoryList(MyCategory,*,true,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [MyPage1|Page 1] - My Description 1 \n* [MyPage2|Page 2] - My Description 2 \n* [MyPage3|Page 3] - My Description 3 \n bla bla bla", retval);
        }

        [Test]
        public void Table_StraightOrder_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo = MockRepository.GenerateStub<CategoryInfo>("MyCategory", provider);
            var pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, DateTime.Now);
            var pageInfo2 = new PageInfo("MyPage2", provider, DateTime.Now);
            var pageInfo3 = new PageInfo("MyPage3", provider, DateTime.Now);
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "", DateTime.Now, "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "", DateTime.Now, "", "", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo);
            catInfo.Pages = pages;

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {CategoryList(MyCategory,,false,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla {|  \n|+  \n! Page name \n|-  \n| [MyPage1|Page 1] \n|-  \n| [MyPage2|Page 2] \n|-  \n| [MyPage3|Page 3] \n|} \n bla bla bla", retval);
        }

        [Test]
        public void Table_StraightOrder_ShowSummary_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo = MockRepository.GenerateStub<CategoryInfo>("MyCategory", provider);
            var pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, DateTime.Now);
            var pageInfo2 = new PageInfo("MyPage2", provider, DateTime.Now);
            var pageInfo3 = new PageInfo("MyPage3", provider, DateTime.Now);
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "", DateTime.Now, "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "", DateTime.Now, "", "", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo);
            catInfo.Pages = pages;

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {CategoryList(MyCategory,,true,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert         
            Assert.AreEqual("bla bla bla {|  \n|+  \n! Page name !! Description \n|-  \n| [MyPage1|Page 1] || My Description 1 \n|-  \n| [MyPage2|Page 2] || My Description 2 \n|-  \n| [MyPage3|Page 3] || My Description 3 \n|} \n bla bla bla", retval);
        }

        [Test]
        public void Table_MessyOrder_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo = MockRepository.GenerateStub<CategoryInfo>("MyCategory", provider);
            var pages = new string[] { "aaa", "MyPage3", "MyPage0" };
            var pageInfo1 = new PageInfo("aaa", provider, DateTime.Now);
            var pageInfo2 = new PageInfo("MyPage3", provider, DateTime.Now);
            var pageInfo3 = new PageInfo("MyPage0", provider, DateTime.Now);
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "BBB", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 0", "", DateTime.Now, "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "aaa", "", DateTime.Now, "", "", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo);
            catInfo.Pages = pages;

            host.Expect(x => x.FindPage("aaa")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage0")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {CategoryList(MyCategory,,false,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert    
            Assert.AreEqual("bla bla bla {|  \n|+  \n! Page name \n|-  \n| [MyPage0|aaa] \n|-  \n| [aaa|BBB] \n|-  \n| [MyPage3|Page 0] \n|} \n bla bla bla", retval);
        }

        [Test]
        public void Table_StraightOrder_AllFormats_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo = MockRepository.GenerateStub<CategoryInfo>("MyCategory", provider);
            var pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, DateTime.Now);
            var pageInfo2 = new PageInfo("MyPage2", provider, DateTime.Now);
            var pageInfo3 = new PageInfo("MyPage3", provider, DateTime.Now);
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "", DateTime.Now, "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "", DateTime.Now, "", "", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo);
            catInfo.Pages = pages;

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {CategoryList(MyCategory,,false,'My Products','Product Name,Summary','My Table Format','My Head Format','My Row Format')} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla {| My Table Format \n|+ My Products \n|- My Head Format \n| Product Name \n|- My Row Format \n| [MyPage1|Page 1] \n|- My Row Format \n| [MyPage2|Page 2] \n|- My Row Format \n| [MyPage3|Page 3] \n|} \n bla bla bla", retval);
        }

        [Test]
        public void Table_StraightOrder_Style_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo = MockRepository.GenerateStub<CategoryInfo>("MyCategory", provider);
            var pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, DateTime.Now);
            var pageInfo2 = new PageInfo("MyPage2", provider, DateTime.Now);
            var pageInfo3 = new PageInfo("MyPage3", provider, DateTime.Now);
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "", DateTime.Now, "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "", DateTime.Now, "", "", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo);
            catInfo.Pages = pages;

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {CategoryList(MyCategory,,false,'My Products','Product Name,Summary','bw','bw','bw')} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla {| border=\"0\" cellpadding=\"2\" cellspacing=\"0\" align=\"center\" style=\"background-color: #EEEEEE;\" \n|+ My Products \n|- align=\"center\" style=\"background-color: #000000; color: #FFFFFF; font-weight: bold;\" \n| Product Name \n|- align=\"center\" style=\"color: #000000;\" \n| [MyPage1|Page 1] \n|- align=\"center\" style=\"color: #000000;\" \n| [MyPage2|Page 2] \n|- align=\"center\" style=\"color: #000000;\" \n| [MyPage3|Page 3] \n|} \n bla bla bla", retval);
        }*/
    }
}
