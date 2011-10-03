using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Keeper.Garrett.ScrewTurn.BlogFormatter;
using Keeper.Garrett.ScrewTurn.FileListFormatter;
using Keeper.Garrett.ScrewTurn.CategoryListFormatter;
using Keeper.Garrett.ScrewTurn.EventLogFormatter;
using Keeper.Garrett.ScrewTurn.QueryTableFormatter;
using Keeper.Garrett.ScrewTurn.Core;
using ScrewTurn.Wiki.PluginFramework;
using Rhino.Mocks;
using System.Web;
using Keeper.Garrett.ScrewTurn.FileContentFormatter;
using System.Reflection;
using Keeper.Garrett.ScrewTurn.MessageFormatter;

namespace Formatters.Tests
{
    [TestFixture]
    public class General_Test
    {

        #region Helper methods

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
        
        private IFormatterProviderV30 GetFormatter(int _formatter)
        {
            IFormatterProviderV30 retval = null;

            switch (_formatter)
            {
                case 1:
                    retval = new BlogFormatter();
                    break;
                case 2:
                    retval = new CategoryListFormatter();
                    break;
                case 3:
                    retval = new EventLogFormatter();
                    break;
                case 4:
                    retval = new FileListFormatter();
                    break;
                case 5:
                    retval = new QueryTableFormatter();
                    break;
                case 6:
                    retval = new FileContentFormatter();
                    break;
                case 7:
                    retval = new MessageFormatter();
                    break;
            }

            return retval;
        }

        private string GetExpected(int _formatter)
        {
            string retval = null;

            switch (_formatter)
            {
                case 1:
                    retval = "bla bla bla {Blog cat=MyBlog} bla bla bla";
                    break;
                case 2:
                    retval = "bla bla bla {CategoryList cat=MyCategory} bla bla bla";
                    break;
                case 3:
                    retval = "bla bla bla {EventLog log='Application'} bla bla bla";
                    break;
                case 4:
                    retval = "bla bla bla {FileList file='*.*' prov='Local Storage Provider'} bla bla bla";
                    break;
                case 5:
                    retval = "bla bla bla {QTable conn=MyLink query='select * from schedule'} bla bla bla";
                    break;
                case 6:
                    retval = "bla bla bla {FileCont file='/*.*'} bla bla bla";
                    break;
                case 7:
                    retval = "bla bla bla <msg>Test Message</msg> bla bla bla";
                    break;
            }

            return retval;
        }

        [Test]
        public void VerifyPhaseSetup([Values(1, 2, 3, 4, 5, 6,7)] int _formatter)
        {
            //Arrange
            var formatter = GetFormatter(_formatter);

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
        public void ForcePhase2([Values(1, 2, 3, 4, 5, 6,7)] int _formatter)
        {
            //Arrange
            var formatter = GetFormatter(_formatter);

            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(GetExpected(_formatter), context, FormattingPhase.Phase2);

            //Assert
            Assert.AreEqual(GetExpected(_formatter), retval);
        }

        [Test]
        public void ForcePhase3([Values(1, 2, 3, 4, 5, 6,7)] int _formatter)
        {
            //Arrange
            var formatter = GetFormatter(_formatter);

            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(GetExpected(_formatter), context, FormattingPhase.Phase3);

            //Assert
            Assert.AreEqual(GetExpected(_formatter), retval);
        }

        [Test]
        public void ForcePhaseX([Values(1, 2, 3, 4, 5, 6,7)] int _formatter)
        {
            //Arrange
            var formatter = GetFormatter(_formatter);

            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(GetExpected(_formatter), context, (FormattingPhase) 4);

            //Assert
            Assert.AreEqual(GetExpected(_formatter), retval);
        }

        [Test]
        public void ForceException([Values(1, 2, 3, 4, 5, 6)] int _formatter)
        {
            //Arrange
            var formatter = GetFormatter(_formatter);

            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Throw(new Exception("An Error Occoured"));
            host.Expect(x => x.GetSettingValue(SettingName.AccountActivationMode)).IgnoreArguments().Throw(new Exception("An Error Occoured"));

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(GetExpected(_formatter), context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(GetExpected(_formatter), retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            Assert.GreaterOrEqual(args.Count,4);
        }

        [Test]
        public void StoreTableFiles_CreateAll([Values(2, 3, 4, 5)] int _formatter)
        {
            //Arrange
            var formatter = GetFormatter(_formatter);

            //Arrange
            var host = MockRepository.GenerateMock<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            provider.Expect(x => x.Information).Return(new ComponentInformation("SomeProvider", "", "", "", ""));
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).IgnoreArguments().Return(provider.GetType().FullName);
            host.Expect(x => x.GetFilesStorageProviders(true)).IgnoreArguments().Return(new IFilesStorageProviderV30[] { provider });

            //Dirs
            provider.Expect(x => x.ListDirectories("/")).Return(new string[] { "NoPresent1" });
            provider.Expect(x => x.ListDirectories("/Keeper.Garrett.Formatters/")).Return(new string[] { "NoPresent2" });
            provider.Expect(x => x.ListDirectories("/Keeper.Garrett.Formatters/Tables")).Return(new string[] { "NoPresent3" });

            provider.Expect(x => x.ListDirectories(string.Format("/Keeper.Garrett.Formatters/{0}",formatter.GetType().Name))).Return(new string[] { "NoPresent3" });

            //Files
            provider.Expect(x => x.ListFiles("/Keeper.Garrett.Formatters/Tables")).Return(new string[] { "NoFilesAtAll" });
            provider.Expect(x => x.ListFiles("/Keeper.Garrett.Formatters/Tables/Images")).Return(new string[] { "NoFilesAtAll" });

            provider.Expect(x => x.ListFiles(string.Format("/Keeper.Garrett.Formatters/{0}", formatter.GetType().Name))).Return(new string[] { "NoFilesAtAll" });
            provider.Expect(x => x.ListFiles(string.Format("/Keeper.Garrett.Formatters/{0}/Images", formatter.GetType().Name))).Return(new string[] { "NoFilesAtAll" });

            //Act
            formatter.Init(host, "");

            //Assert
            provider.AssertWasCalled(x => x.CreateDirectory("/", "Keeper.Garrett.Formatters"));
            provider.AssertWasCalled(x => x.CreateDirectory("/Keeper.Garrett.Formatters/", "Tables"));
            provider.AssertWasCalled(x => x.CreateDirectory("/Keeper.Garrett.Formatters/Tables", "Images"));
        }

        [Test]
        public void StoreTableFiles_CreateNone([Values( 2, 3, 4, 5)] int _formatter)
        {
            //Arrange
            var formatter = GetFormatter(_formatter);

            //Arrange
            var host = MockRepository.GenerateMock<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            provider.Expect(x => x.Information).Return(new ComponentInformation("SomeProvider", "", "", "", ""));
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).IgnoreArguments().Return(provider.GetType().FullName);
            host.Expect(x => x.GetFilesStorageProviders(true)).IgnoreArguments().Return(new IFilesStorageProviderV30[] { provider });

            //Dirs
            provider.Expect(x => x.ListDirectories("/")).Return(new string[] { "Keeper.Garrett.Formatters" });
            provider.Expect(x => x.ListDirectories("/Keeper.Garrett.Formatters/")).Return(new string[] { "Tables" });
            provider.Expect(x => x.ListDirectories("/Keeper.Garrett.Formatters/Tables")).Return(new string[] { "Images" });

            provider.Expect(x => x.ListDirectories(string.Format("/Keeper.Garrett.Formatters/{0}", formatter.GetType().Name))).Return(new string[] { "Images" });

            //Files
            provider.Expect(x => x.ListFiles("/Keeper.Garrett.Formatters/Tables")).Return(new string[] { "/Keeper.Garrett.Formatters/Tables/TableStyle.css" });
            provider.Expect(x => x.ListFiles("/Keeper.Garrett.Formatters/Tables/Images")).Return(new string[] { "/Keeper.Garrett.Formatters/Tables/Images/back.png" });

            provider.Expect(x => x.ListFiles(string.Format("/Keeper.Garrett.Formatters/{0}", formatter.GetType().Name))).Return(new string[] { "None" });
            provider.Expect(x => x.ListFiles(string.Format("/Keeper.Garrett.Formatters/{0}/Images", formatter.GetType().Name))).Return(new string[] { string.Format("/Keeper.Garrett.Formatters/{0}/Images/Error.png", formatter.GetType().Name) });

            //Act
            formatter.Init(host, "");

            //Assert
            provider.AssertWasNotCalled(x => x.CreateDirectory("/", "Keeper.Garrett.Formatters"));
            provider.AssertWasNotCalled(x => x.CreateDirectory("/Keeper.Garrett.Formatters/", "Tables"));
            provider.AssertWasNotCalled(x => x.CreateDirectory("/Keeper.Garrett.Formatters/Tables", "Images"));
        }
    }
}
