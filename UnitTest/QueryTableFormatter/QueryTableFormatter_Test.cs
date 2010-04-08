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
    public class QueryTableFormatter_Test
    {
        private const string Oralce = "{MyLink=Oracle,User Id=wiki;Password=myretuefest;Data Source=SERVER;}";
        private const string MsSql = "{MyLink=MsSql,Data Source=Server\\SQLEXPRESS;Initial Catalog=test;User ID=test;Password=myretuefest;}";
        private const string MySql = "{MyLink=MySql,Database=wiki;Data Source=192.168.1.100;User Id=wiki;Password=myretuefest;}";
        private const string SqLite = "{MyLink=SqLite,Data Source=\\\\Server\\SQLite\\Wiki.sqlite;Version=3;}";

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


        #region Query

        [Test]
        public void Query_Fail_All_SecurityChecks([Values(Oralce,MsSql,MySql,SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            string input =
                  "bla bla bla {QTable(MyLink,'delete * from schedule t where t.id=0 and t.reason='asd'',,,,,,)} bla bla bla"
                + "bla bla bla {QTable(MyLink,'alter table schedule set',,,,,,)} bla bla bla"
                + "bla bla bla {QTable(MyLink,'drop schedule',,,,,,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
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
        public void Query_Fail_2_Pass_1_SecurityChecks([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            string input =
                  "bla bla bla {QTable(MyLink,'delete * from schedule',,,,,,)} bla bla bla"
                + "bla bla bla {QTable(MyLink,'select * from schedule',,,,,,)} bla bla bla"
                + "bla bla bla {QTable(MyLink,'drop * from schedule',,,,,,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(2, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        public void Query_Pass_Default([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            string input = "bla bla bla {QTable(MyLink,'select * from schedule',,,,,,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla {|  \n|+  \n! ID !! RUNTIME !! REASON !! UPDAT \n|-  \n| 1 || 01-01-2010 00:00:00 || dsadasdasd || 01-01-2010 00:00:00 \n|} bla bla bla", retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        public void Query_Pass_Default_With_Where_Clauses([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            string input = "bla bla bla {QTable(MyLink,'select * from schedule t where t.id=1 and t.reason='dsadasdasd'',,,,,,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla {|  \n|+  \n! ID !! RUNTIME !! REASON !! UPDAT \n|-  \n| 1 || 01-01-2010 00:00:00 || dsadasdasd || 01-01-2010 00:00:00 \n|} bla bla bla", retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        public void Query_Fail_Default_With_Where_Clauses([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            string input = "bla bla bla {QTable(MyLink,'select * from schedule t where t.id=1 and t.reason='badcomparison'',,,,,,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla {|  \n|+  \n! ID !! RUNTIME !! REASON !! UPDAT \n|-  \n| align=\"center\" colspan=\"4\" | <h2>Query revealed no results, table/view is empty.</h2> \n|} bla bla bla", retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }


        [Test]
        public void Query_EmptyTable([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            string input = "bla bla bla {QTable(MyLink,'select * from emptytable',,,,,,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);


            //Assert  
            Assert.AreEqual("bla bla bla {|  \n|+  \n! ID !! REASON \n|-  \n| align=\"center\" colspan=\"2\" | <h2>Query revealed no results, table/view is empty.</h2> \n|} bla bla bla", retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        public void Query_Pass_Format_No_Of_Columns_To_Show([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            //                           Key ,Query ,Heading ,ColumnOrder ,Headers ,TdblFormat ,HeadFormat ,RowFormat
            string input = "bla bla bla {QTable(MyLink,'select * from schedule',,'1,2,3',,,,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);


            //Assert        
            Assert.AreEqual("bla bla bla {|  \n|+  \n! RUNTIME !! REASON !! UPDAT \n|-  \n| 01-01-2010 00:00:00 || dsadasdasd || 01-01-2010 00:00:00 \n|} bla bla bla", retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        public void Query_Pass_Format_Column_Order([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            //                           Key ,Query ,Heading ,ColumnOrder ,Headers ,TdblFormat ,HeadFormat ,RowFormat
            string input = "bla bla bla {QTable(MyLink,'select * from schedule',,'3,1,2',,,,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);


            //Assert        
            Assert.AreEqual("bla bla bla {|  \n|+  \n! UPDAT !! RUNTIME !! REASON \n|-  \n| 01-01-2010 00:00:00 || 01-01-2010 00:00:00 || dsadasdasd \n|} bla bla bla", retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        public void Query_Pass_Format_Column_Order_And_Headers_Match([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            //                           Key ,Query ,Heading ,ColumnOrder ,Headers ,TdblFormat ,HeadFormat ,RowFormat
            string input = "bla bla bla {QTable(MyLink,'select * from schedule',,'3,1,2','Head3,Head1,Head2',,,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);


            //Assert        
            Assert.AreEqual("bla bla bla {|  \n|+  \n! Head3 !! Head1 !! Head2 \n|-  \n| 01-01-2010 00:00:00 || 01-01-2010 00:00:00 || dsadasdasd \n|} bla bla bla", retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        public void Query_Pass_Format_Column_Order_And_ToFew_Headers([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            //                           Key ,Query ,Heading ,ColumnOrder ,Headers ,TdblFormat ,HeadFormat ,RowFormat
            string input = "bla bla bla {QTable(MyLink,'select * from schedule',,'3,1,2','Head3,Head1',,,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);


            //Assert        
            Assert.AreEqual("bla bla bla {|  \n|+  \n! Head3 !! Head1 !! REASON \n|-  \n| 01-01-2010 00:00:00 || 01-01-2010 00:00:00 || dsadasdasd \n|} bla bla bla", retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }


        [Test]
        public void Query_Pass_Format_Column_Order_And_ToFew_Headers_ToMany_Columns([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            //                           Key ,Query ,Heading ,ColumnOrder ,Headers ,TdblFormat ,HeadFormat ,RowFormat
            string input = "bla bla bla {QTable(MyLink,'select * from schedule',,'3,1,2,4,5,6','Head3,Head1',,,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);


            //Assert        
            Assert.AreEqual("bla bla bla {|  \n|+  \n! Head3 !! Head1 !! REASON !! ?Missing Header? !! ?Missing Header? !! ?Missing Header? \n|-  \n| 01-01-2010 00:00:00 || 01-01-2010 00:00:00 || dsadasdasd \n|} bla bla bla", retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        public void Query_Pass_Format_Heading([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                           Key ,Query ,Heading ,ColumnOrder ,Headers ,TdblFormat ,HeadFormat ,RowFormat
            string input = "bla bla bla {QTable(MyLink,'select * from schedule','My Heading',,,,,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);


            //Assert       
            Assert.AreEqual("bla bla bla {|  \n|+ My Heading \n! ID !! RUNTIME !! REASON !! UPDAT \n|-  \n| 1 || 01-01-2010 00:00:00 || dsadasdasd || 01-01-2010 00:00:00 \n|} bla bla bla", retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        public void Query_Pass_Format_TableStyle([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                           Key ,Query ,Heading ,ColumnOrder ,Headers ,TdblFormat ,HeadFormat ,RowFormat
            string input = "bla bla bla {QTable(MyLink,'select * from schedule',,,,'My Table Format',,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);


            //Assert        
            Assert.AreEqual("bla bla bla {| My Table Format \n|+  \n! ID !! RUNTIME !! REASON !! UPDAT \n|-  \n| 1 || 01-01-2010 00:00:00 || dsadasdasd || 01-01-2010 00:00:00 \n|} bla bla bla", retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        public void Query_Pass_Format_TableHeaders([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                           Key ,Query ,Heading ,ColumnOrder ,Headers ,TdblFormat ,HeadFormat ,RowFormat
            string input = "bla bla bla {QTable(MyLink,'select * from schedule',,,'MyHead1,MyHead2,MyHead3,MyHead4',,,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);


            //Assert        
            Assert.AreEqual("bla bla bla {|  \n|+  \n! MyHead1 !! MyHead2 !! MyHead3 !! MyHead4 \n|-  \n| 1 || 01-01-2010 00:00:00 || dsadasdasd || 01-01-2010 00:00:00 \n|} bla bla bla", retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        public void Query_Pass_Format_TableHeaders_ToFew([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                           Key ,Query ,Heading ,ColumnOrder ,Headers ,TdblFormat ,HeadFormat ,RowFormat
            string input = "bla bla bla {QTable(MyLink,'select * from schedule',,,'MyHead1,MyHead2,MyHead3',,,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);


            //Assert        
            Assert.AreEqual("bla bla bla {|  \n|+  \n! MyHead1 !! MyHead2 !! MyHead3 !! UPDAT \n|-  \n| 1 || 01-01-2010 00:00:00 || dsadasdasd || 01-01-2010 00:00:00 \n|} bla bla bla", retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        public void Query_Pass_Format_TableRowFormat([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                           Key ,Query ,Heading ,ColumnOrder ,Headers ,TdblFormat ,HeadFormat ,RowFormat
            string input = "bla bla bla {QTable(MyLink,'select * from schedule',,,,,,'My Row Format')} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);


            //Assert  
            Assert.AreEqual("bla bla bla {|  \n|+  \n! ID !! RUNTIME !! REASON !! UPDAT \n|- My Row Format \n| 1 || 01-01-2010 00:00:00 || dsadasdasd || 01-01-2010 00:00:00 \n|} bla bla bla", retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        public void Query_Pass_Format_AllFormatsActive([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
                                            //  Key   ,Query                      ,Heading ,ColumnOrder,Headers            ,TdblFormat     ,HeadFormat    ,RowFormat
            string input = "bla bla bla {QTable(MyLink,'select * from schedule','My Table','1,2,3','MyHead1,MyHead2,MyHead3','My Table format','My Head Format','My Row Format')} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);


            //Assert       
            Assert.AreEqual("bla bla bla {| My Table format \n|+ My Table \n|- My Head Format \n| MyHead1 || MyHead2 || MyHead3 \n|- My Row Format \n| 01-01-2010 00:00:00 || dsadasdasd || 01-01-2010 00:00:00 \n|} bla bla bla", retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        public void Query_Pass_Format_AllFormatsActive_2_Tables([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                           Key ,Query ,Heading ,ColumnOrder ,Headers ,TdblFormat ,HeadFormat ,RowFormat
            string input = "bla bla bla {QTable(MyLink,'select * from schedule','My Table','1,2,3','MyHead1,MyHead2,MyHead3','My Table format','My Head Format','My Row Format')} bla bla bla {QTable(MyLink,'select * from schedule','My Table2','1,2,3','MyHead1,MyHead2,MyHead3','My Table format2','My Head Format2','My Row Format2')}";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);


            //Assert        
            Assert.AreEqual("bla bla bla {| My Table format \n|+ My Table \n|- My Head Format \n| MyHead1 || MyHead2 || MyHead3 \n|- My Row Format \n| 01-01-2010 00:00:00 || dsadasdasd || 01-01-2010 00:00:00 \n|} bla bla bla {| My Table format2 \n|+ My Table2 \n|- My Head Format2 \n| MyHead1 || MyHead2 || MyHead3 \n|- My Row Format2 \n| 01-01-2010 00:00:00 || dsadasdasd || 01-01-2010 00:00:00 \n|}", retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        public void Query_Pass_Format_Style_BlackWhite([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                           Key ,Query ,Heading ,ColumnOrder ,Headers ,TdblFormat ,HeadFormat ,RowFormat
            string input = "bla bla bla {QTable(MyLink,'select * from schedule',,,,'bw','bw','bw')} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);


            //Assert  
            Assert.AreEqual("bla bla bla {| border=\"0\" cellpadding=\"2\" cellspacing=\"0\" align=\"center\" style=\"background-color: #EEEEEE;\" \n|+  \n|- align=\"center\" style=\"background-color: #000000; color: #FFFFFF; font-weight: bold;\" \n| ID || RUNTIME || REASON || UPDAT \n|- align=\"center\" style=\"color: #000000;\" \n| 1 || 01-01-2010 00:00:00 || dsadasdasd || 01-01-2010 00:00:00 \n|} bla bla bla", retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        public void Query_Pass_Format_Style_BlackGrey([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                           Key ,Query ,Heading ,ColumnOrder ,Headers ,TdblFormat ,HeadFormat ,RowFormat
            string input = "bla bla bla {QTable(MyLink,'select * from schedule',,,,'bg','bg','bg')} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);


            //Assert  
            Assert.AreEqual("bla bla bla {| border=\"0\" cellpadding=\"2\" cellspacing=\"0\" align=\"center\" style=\"background-color: #EEEEEE;\" \n|+  \n|- align=\"center\" style=\"background-color: #000000; color: #CCCCCC; font-weight: bold;\" \n| ID || RUNTIME || REASON || UPDAT \n|- align=\"center\" \n| 1 || 01-01-2010 00:00:00 || dsadasdasd || 01-01-2010 00:00:00 \n|} bla bla bla", retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        public void Query_Pass_Format_Style_GreenBlack([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                           Key ,Query ,Heading ,ColumnOrder ,Headers ,TdblFormat ,HeadFormat ,RowFormat
            string input = "bla bla bla {QTable(MyLink,'select * from schedule',,,,'gb','gb','gb')} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);


            //Assert  
            Assert.AreEqual("bla bla bla {| border=\"0\" cellpadding=\"2\" cellspacing=\"0\" align=\"center\" style=\"background-color: #EEEEEE;\" \n|+  \n|- align=\"center\" style=\"background-color: #88CC33; color: #000000; font-weight: bold;\" \n| ID || RUNTIME || REASON || UPDAT \n|- align=\"center\" style=\"color: #000000;\" \n| 1 || 01-01-2010 00:00:00 || dsadasdasd || 01-01-2010 00:00:00 \n|} bla bla bla", retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        public void Query_Pass_Format_Custom_Tabel_Style([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                           Key ,Query ,Heading ,ColumnOrder ,Headers ,TdblFormat ,HeadFormat ,RowFormat
            string input = "bla bla bla {QTable(MyLink,'select * from schedule',,,,'border=\"0\" cellpadding=\"2\" cellspacing=\"1\" align=\"center\" style=\"background-color: #EEEEEE;\"',,)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);


            //Assert  
            Assert.AreEqual("bla bla bla {| border=\"0\" cellpadding=\"2\" cellspacing=\"1\" align=\"center\" style=\"background-color: #EEEEEE;\" \n|+  \n! ID !! RUNTIME !! REASON !! UPDAT \n|-  \n| 1 || 01-01-2010 00:00:00 || dsadasdasd || 01-01-2010 00:00:00 \n|} bla bla bla", retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        public void Query_Pass_Format_Custom_Head_Style([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                           Key ,Query ,Heading ,ColumnOrder ,Headers ,TdblFormat ,HeadFormat ,RowFormat
            string input = "bla bla bla {QTable(MyLink,'select * from schedule',,,,,'border=\"0\" cellpadding=\"2\" cellspacing=\"1\" align=\"center\" style=\"background-color: #EEEEEE;\"',)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);


            //Assert  
            Assert.AreEqual("bla bla bla {|  \n|+  \n|- border=\"0\" cellpadding=\"2\" cellspacing=\"1\" align=\"center\" style=\"background-color: #EEEEEE;\" \n| ID || RUNTIME || REASON || UPDAT \n|-  \n| 1 || 01-01-2010 00:00:00 || dsadasdasd || 01-01-2010 00:00:00 \n|} bla bla bla", retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        [Test]
        public void Query_Pass_Format_Custom_Row_Style([Values(Oralce, MsSql, MySql, SqLite)] string _connectionString)
        {
            //Arrange
            var host = GenerateMockedHost();
            var formatter = new QueryTableFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                           Key ,Query ,Heading ,ColumnOrder ,Headers ,TdblFormat ,HeadFormat ,RowFormat
            string input = "bla bla bla {QTable(MyLink,'select * from schedule',,,,,,'border=\"0\" cellpadding=\"2\" cellspacing=\"1\" align=\"center\" style=\"background-color: #EEEEEE;\"')} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, _connectionString);
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);


            //Assert  
            Assert.AreEqual("bla bla bla {|  \n|+  \n! ID !! RUNTIME !! REASON !! UPDAT \n|- border=\"0\" cellpadding=\"2\" cellspacing=\"1\" align=\"center\" style=\"background-color: #EEEEEE;\" \n| 1 || 01-01-2010 00:00:00 || dsadasdasd || 01-01-2010 00:00:00 \n|} bla bla bla", retval);
            var args = host.GetArgumentsForCallsMadeOn(x => x.LogEntry("", LogEntryType.Error, "", null));
            int warnCount = 0;
            int errorCount = 0;
            int generalCount = 0;
            GetLogCounts(out warnCount, out errorCount, out generalCount, args);
            Assert.AreEqual(0, warnCount);
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(1, generalCount);
        }

        #endregion
    }
}
