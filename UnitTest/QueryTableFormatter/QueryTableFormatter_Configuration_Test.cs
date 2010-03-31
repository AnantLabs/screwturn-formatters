using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using ScrewTurn.Wiki.PluginFramework;
using System.Web;
using Keeper.Garrett.ScrewTurn;
using Keeper.Garrett.ScrewTurn.QueryTableFormatter;

namespace UnitTest
{
    [TestFixture]
    [Category("Formatter")]
    public class QueryTableFormatter_Configuration_Test
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

        private IHostV30 GenerateMockedHost()
        {
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider1 = MockRepository.GenerateStub<IPagesStorageProviderV30>();

            host.Expect(x => x.GetDefaultProvider(null)).IgnoreArguments().Return("IPagesStorageProviderV30");
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider1 });

            return host;
        }

        #endregion

        #region Configuration

        [Test]
        public void Configuration_No_DBLinkEntries()
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            string input = "bla bla bla {QTable(DBLink,'select * from schedule',,,,,,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(retval, input);
        }

        [Test]
        public void Configuration_DBLinkEntries_BadKey()
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            string input = "bla bla bla {QTable(BADLink,'select * from schedule',,,,,,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "{MyLink=Oracle,someconnectionstring}");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(retval, "bla bla bla No DBLinkKey found for BADLink bla bla bla");
        }

        [Test]
        public void Configuration_Two_DBLinkEntries_BadKey()
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            string input = "bla bla bla {QTable(BADLink,'select * from schedule',,,,,,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "{MyLink1=Oracle,someconnectionstring}\n{MyLink2=MySql,someconnectionstring}");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(retval, "bla bla bla No DBLinkKey found for BADLink bla bla bla");
        }

        [Test]
        public void Configuration_ConnectionString_Warning()
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "{MyLink=Oracle,User Id=wiki;Password=myretuefest;Data Source=(DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = 192.168.1.100)(PORT = 0)) )(CONNECT_DATA = (SERVER = DEDICATED) (SERVICE_NAME = XE)));}");

            //Assert
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(1, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        #endregion

        #region DB Support

        [Test]
        [Category("Database Support")]
        public void Configuration_Oracle_Supported()
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            string input = "bla bla bla {QTable(BADLink,'select * from schedule',,,,,,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "{MyLink1=Oracle,someconnectionstring}");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(1, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }


        [Test]
        [Category("Database Support")]
        public void Configuration_MsSql_Supported()
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            string input = "bla bla bla {QTable(BADLink,'select * from schedule',,,,,,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "{MyLink1=MSSql,someconnectionstring}");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(1, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        [Category("Database Support")]
        public void Configuration_MySql_Supported()
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            string input = "bla bla bla {QTable(BADLink,'select * from schedule',,,,,,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "{MyLink1=MySql,someconnectionstring}");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(1, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        [Category("Database Support")]
        public void Configuration_SQLite_Supported()
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            string input = "bla bla bla {QTable(BADLink,'select * from schedule',,,,,,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "{MyLink1=SQLite,someconnectionstring}");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(1, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        #endregion
    }
}
