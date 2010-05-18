using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using ScrewTurn.Wiki.PluginFramework;
using System.Web;
using Keeper.Garrett.ScrewTurn;
using Keeper.Garrett.ScrewTurn.FileListFormatter;
using Keeper.Garrett.ScrewTurn.FileContentFormatter;
using System.IO;

namespace Formatters.Tests
{
    [TestFixture]
    [Category("Formatter")]
    public class FileContentFormatter_Test
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
            var formatter = new FileContentFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { });

            string input = "bla bla bla {FileCont file='*.*' prov='Local Storage Provider'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla {FileCont file='*.*' prov='Local Storage Provider'} bla bla bla", retval);

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
            var formatter = new FileContentFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe" });

            provider.Expect(x => x.RetrieveFile("", null, false)).IgnoreArguments().Return(true).WhenCalled(y => (y.Arguments[1] as MemoryStream).Write(new byte[] { 49, 50, 51 }, 0, 3));

            string input = "bla bla bla {FileCont file='*.*'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla 123 bla bla bla", retval);
        }

        [Test]
        public void Different_Provider()
        {
            //Arrange
            var formatter = new FileContentFormatter();
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

            provider2.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe" });

            provider2.Expect(x => x.RetrieveFile("", null, false)).IgnoreArguments().Return(true).WhenCalled(y => (y.Arguments[1] as MemoryStream).Write(new byte[] { 49, 50, 51 }, 0, 3));

            string input = "bla bla bla {FileCont file='*.*' prov='Local Storage Provider 2'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla 123 bla bla bla", retval);
        }

        [Test]
        public void Default_Values()
        {
            //Arrange
            var formatter = new FileContentFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe" });

            provider.Expect(x => x.RetrieveFile("", null, false)).IgnoreArguments().Return(true).WhenCalled(y => (y.Arguments[1] as MemoryStream).Write(new byte[] { 49, 50, 51 }, 0, 3));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileCont} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert         
            Assert.AreEqual("bla bla bla 123 bla bla bla", retval);
        }

        [Test]
        public void NoFilesInDir()
        {
            //Arrange
            var formatter = new FileContentFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { });

            provider.Expect(x => x.RetrieveFile("", null, false)).IgnoreArguments().Return(true).WhenCalled(y => (y.Arguments[1] as MemoryStream).Write(new byte[] { 49, 50, 51 }, 0, 3));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileCont file='/test/*.txt'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert         
            Assert.AreEqual("bla bla bla (No files found matching \"*.txt\".) bla bla bla", retval);
        }


        [Test]
        public void DirectoryError()
        {
            //Arrange
            var formatter = new FileContentFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(null);

            provider.Expect(x => x.RetrieveFile("", null, false)).IgnoreArguments().Return(true).WhenCalled(y => (y.Arguments[1] as MemoryStream).Write(new byte[] { 49, 50, 51 }, 0, 3));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileCont file='/test/*.*'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert         
            Assert.AreEqual("bla bla bla (No directory found matching \"/test/\".) bla bla bla", retval);
        }

        [Test]
        public void Default_Values_3_Files_Match_2()
        {
            //Arrange
            var formatter = new FileContentFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "file1.exe","file2.exe","file1.txt" });

            provider.Expect(x => x.RetrieveFile("", null, false)).IgnoreArguments().Return(true).WhenCalled(y => (y.Arguments[1] as MemoryStream).Write(new byte[] { 49, 50, 51 }, 0, 3));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileCont file='/*.exe'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert         
            Assert.AreEqual("bla bla bla 123 <br></br> 123 bla bla bla", retval);
        }

        [Test]
        public void Find_1_File_Embedding_Height()
        {
            //Arrange
            var formatter = new FileContentFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "/file1.exe", "/file2.exe", "/file1.txt" });

            provider.Expect(x => x.RetrieveFile("", null, false)).IgnoreArguments().Return(true).WhenCalled(y => (y.Arguments[1] as MemoryStream).Write(new byte[] { 49, 50, 51 }, 0, 3));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileCont file='/*.txt' height=400} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert                   
            Assert.AreEqual("bla bla bla <object data=\"GetFile.aspx?File=/file1.txt\" type=\"text/html\" height=\"400\" ><embed src=\"GetFile.aspx?File=/file1.txt\" type=\"text/html\" height=\"400\"></embed></object> bla bla bla", retval);
        }

        [Test]
        public void Find_1_File_Embedding_Width()
        {
            //Arrange
            var formatter = new FileContentFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "/file1.exe", "/file2.exe", "/file1.txt" });

            provider.Expect(x => x.RetrieveFile("", null, false)).IgnoreArguments().Return(true).WhenCalled(y => (y.Arguments[1] as MemoryStream).Write(new byte[] { 49, 50, 51 }, 0, 3));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileCont file='/*.txt' width=400} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert         
            Assert.AreEqual("bla bla bla <object data=\"GetFile.aspx?File=/file1.txt\" type=\"text/html\" width=\"400\" ><embed src=\"GetFile.aspx?File=/file1.txt\" type=\"text/html\" width=\"400\"></embed></object> bla bla bla", retval);
        }

        [Test]
        public void Find_1_File_Embedding_Both()
        {
            //Arrange
            var formatter = new FileContentFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "/file1.exe", "/file2.exe", "/file1.txt" });

            provider.Expect(x => x.RetrieveFile("", null, false)).IgnoreArguments().Return(true).WhenCalled(y => (y.Arguments[1] as MemoryStream).Write(new byte[] { 49, 50, 51 }, 0, 3));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileCont file='/*.txt' height=200 width=400} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert         
            Assert.AreEqual("bla bla bla <object data=\"GetFile.aspx?File=/file1.txt\" type=\"text/html\" height=\"200\" width=\"400\" ><embed src=\"GetFile.aspx?File=/file1.txt\" type=\"text/html\" height=\"200\" width=\"400\"></embed></object> bla bla bla", retval);
        }

        [Test]
        public void Find_2_Files_Embedding_Both()
        {
            //Arrange
            var formatter = new FileContentFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "/file1.exe", "/file2.exe", "/file1.txt" });

            provider.Expect(x => x.RetrieveFile("", null, false)).IgnoreArguments().Return(true).WhenCalled(y => (y.Arguments[1] as MemoryStream).Write(new byte[] { 49, 50, 51 }, 0, 3));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileCont file='/*.exe' height=200 width=400} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert         
            Assert.AreEqual("bla bla bla <object data=\"GetFile.aspx?File=/file1.exe\" type=\"text/html\" height=\"200\" width=\"400\" ><embed src=\"GetFile.aspx?File=/file1.exe\" type=\"text/html\" height=\"200\" width=\"400\"></embed></object> <br></br> <object data=\"GetFile.aspx?File=/file2.exe\" type=\"text/html\" height=\"200\" width=\"400\" ><embed src=\"GetFile.aspx?File=/file2.exe\" type=\"text/html\" height=\"200\" width=\"400\"></embed></object> bla bla bla", retval);
        }

        [Test]
        public void Error_Reading_File()
        {
            //Arrange
            var formatter = new FileContentFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "/file1.exe", "/file2.exe", "/file1.txt" });

            provider.Expect(x => x.RetrieveFile("", null, false)).IgnoreArguments().Throw(new Exception("Tilt"));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {FileCont file='/*.txt'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert         
            Assert.AreEqual("bla bla bla (No files found matching \"*.txt\".) bla bla bla", retval);

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
        public void Find_1_File_Using_TagsAround()
        {
            //Arrange
            var formatter = new FileContentFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { provider });
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).Return("Local Storage Provider");
            provider.Expect(x => x.Information).Return(new ComponentInformation("Local Storage Provider", "", "", "", ""));

            provider.Expect(x => x.ListFiles(null)).IgnoreArguments().Return(new string[] { "/file1.exe", "/file2.exe", "/file1.txt" });

            provider.Expect(x => x.RetrieveFile("", null, false)).IgnoreArguments().Return(true).WhenCalled(y => (y.Arguments[1] as MemoryStream).Write(new byte[] { 49, 50, 51 }, 0, 3));

            // Category,output,include,head,headers,tbl,head,row
            // {FileList('filePattern','storageProvider',outputType,sortMethod,asLinks,showDownloadCount,'heading'?,'headers'?,'tblFormat'?,'headFormat'?,'rowFormat'? )
            string input = "bla bla bla {{{FileCont file='/*.txt'}}} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert         
            Assert.AreEqual("bla bla bla {{123}} bla bla bla", retval);
        }
    }
}
