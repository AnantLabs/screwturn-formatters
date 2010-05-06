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

namespace UnitTest.General
{
    class General_Test
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
            }

            return retval;
        }

        private string GetExpected(int _formatter)
        {
            string retval = null;

            switch (_formatter)
            {
                case 1:
                    retval = "bla bla bla {Blog(MyBlog,3,3,false,false,false,false,'About',,)} bla bla bla";
                    break;
                case 2:
                    retval = "bla bla bla {CategoryList(MyCategory,*,false,,,,,)} bla bla bla";
                    break;
                case 3:
                    retval = "bla bla bla {EventLog(,'Application',,,,,,,,)} bla bla bla";
                    break;
                case 4:
                    retval = "bla bla bla {FileList('*.*','Local Storage Provider',*,7,true,true,,,,,)} bla bla bla";
                    break;
                case 5:
                    retval = "bla bla bla {QTable(MyLink,'select * from schedule',,,,,,)} bla bla bla";
                    break;
                case 6:
                    retval = "bla bla bla {FileCont file='/*.*')} bla bla bla";
                    break;
            }

            return retval;
        }

        [Test]
        public void VerifyPhaseSetup([Values(1, 2, 3, 4, 5, 6)] int _formatter)
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
        public void ForcePhase2([Values(1, 2, 3, 4, 5, 6)] int _formatter)
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
        public void ForcePhase3([Values(1, 2, 3, 4, 5, 6)] int _formatter)
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
        public void ForcePhaseX([Values(1, 2, 3, 4, 5, 6)] int _formatter)
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
        public void ForceException([Values(1, 2, 4, 5, 6)] int _formatter)
        {
            //Arrange
            var formatter = GetFormatter(_formatter);

            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Throw(new Exception("An Error Occoured"));

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(GetExpected(_formatter), context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(GetExpected(_formatter), retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            Assert.AreEqual(4, args.Count);
        }
    }
}
