using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using ScrewTurn.Wiki.PluginFramework;
using System.Web;
using Keeper.Garrett.ScrewTurn;
using Keeper.Garrett.ScrewTurn.EventLogFormatter;
using System.Diagnostics;

namespace UnitTest
{
    [TestFixture]
    public class EventLogFormatter_Test
    {
        [Test]
        public void VerifyPhaseSetup()
        {
            //Arrange
            var formatter = new EventLogFormatter();

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
        public void No_Filter()
        {
            //Arrange
            var formatter = new EventLogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                                    machine,log,filter,results,heading,cols,headers,tbl,head,row
            string input = "bla bla bla {EventLog(,'Application',,,,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("Information.png"));
        }

        [Test]
        public void Filter_Type_Error()
        {
            //Arrange
            var formatter = new EventLogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                         Filter options: "Id","Type","Date","Time","Source","Category","Event","User","Computer", "Description"
            //                                         machine,log,filter,results,heading,cols,headers,tbl,head,row
            string input = "bla bla bla {EventLog(,'Application','type=Error',,,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("Error.png"));
        }

        [Test]
        public void Filter_Type_Warning()
        {
            //Arrange
            var formatter = new EventLogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                         Filter options: "Id","Type","Date","Time","Source","Category","Event","User","Computer", "Description"
            //                                         machine,log,filter,results,heading,cols,headers,tbl,head,row
            string input = "bla bla bla {EventLog(,'Application','type=Warning',,,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("Warning.png"));
        }

        [Test]
        public void Filter_Type_Information()
        {
            //Arrange
            var formatter = new EventLogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                         Filter options: "Id","Type","Date","Time","Source","Category","Event","User","Computer", "Description"
            //                                         machine,log,filter,results,heading,cols,headers,tbl,head,row
            string input = "bla bla bla {EventLog(,'Application','type=Information',,,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("Information.png"));
        }

        [Test]
        public void Filter_Type_InfoAndWarn()
        {
            //Arrange
            var formatter = new EventLogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                         Filter options: "Id","Type","Date","Time","Source","Category","Event","User","Computer", "Description"
            //                                         machine,log,filter,results,heading,cols,headers,tbl,head,row
            string input = "bla bla bla {EventLog(,'Application','type=infoandwarn',,,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("Information.png"));
        }

        [Test]
        public void Filter_Type_InfoAndError()
        {
            //Arrange
            var formatter = new EventLogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                         Filter options: "Id","Type","Date","Time","Source","Category","Event","User","Computer", "Description"
            //                                         machine,log,filter,results,heading,cols,headers,tbl,head,row
            string input = "bla bla bla {EventLog(,'Application','type=infoanderror',50,,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("Information.png"));
        }

        [Test]
        public void Filter_Type_WarnAndError()
        {
            //Arrange
            var formatter = new EventLogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                         Filter options: "Id","Type","Date","Time","Source","Category","Event","User","Computer", "Description"
            //                                         machine,log,filter,results,heading,cols,headers,tbl,head,row
            string input = "bla bla bla {EventLog(,'Application','type=warnanderror',,,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("Warning.png"));
        }

        [Test]
        public void Filter_Date()
        {
            //Arrange
            var formatter = new EventLogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                         Filter options: "Id","Type","Date","Time","Source","Category","Event","User","Computer", "Description"
            //                                         machine,log,filter,results,heading,cols,headers,tbl,head,row
            string input = "bla bla bla {EventLog(,'Application','date=" + DateTime.Now.AddDays(-1) + "',,,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("Information.png"));
        }

        [Test]
        public void Filter_Date_Using_DaysBack()
        {
            //Arrange
            var formatter = new EventLogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                         Filter options: "Id","Type","Date","Time","Source","Category","Event","User","Computer", "Description"
            //                                         machine,log,filter,results,heading,cols,headers,tbl,head,row
            string input = "bla bla bla {EventLog(,'Application','date=-1',,,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("Information.png"));
        }

        [Test]
        public void Filter_Time()
        {
            //Arrange
            var formatter = new EventLogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                         Filter options: "Id","Type","Date","Time","Source","Category","Event","User","Computer", "Description"
            //                                         machine,log,filter,results,heading,cols,headers,tbl,head,row
            string input = "bla bla bla {EventLog(,'Application','time=" + DateTime.Now.AddDays(-1) + "',,,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("Information.png"));
        }

        [Test]
        public void Filter_Source()
        {
            //Arrange
            var formatter = new EventLogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                         Filter options: "Id","Type","Date","Time","Source","Category","Event","User","Computer", "Description"
            //                                         machine,log,filter,results,heading,cols,headers,tbl,head,row
            string input = "bla bla bla {EventLog(,'Application','source=EventSystem',,,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("Information.png"));
        }

        [Test]
        public void Filter_Description()
        {
            //Arrange
            var formatter = new EventLogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                         Filter options: "Id","Type","Date","Time","Source","Category","Event","User","Computer", "Description"
            //                                         machine,log,filter,results,heading,cols,headers,tbl,head,row
            string input = "bla bla bla {EventLog(,'Application','description=database',,,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("Information.png"));
        }

        [Test]
        public void Filter_Multiple_Type_Date_Source_Description()
        {
            //Arrange
            var formatter = new EventLogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                         Filter options: "Id","Type","Date","Time","Source","Category","Event","User","Computer", "Description"
            //                                         machine,log,filter,results,heading,cols,headers,tbl,head,row
            string input = "bla bla bla {EventLog(,'Application','type=information,date=" + DateTime.Now.AddDays(-1) + ",source=vss,description=idle',,,,,,,)} bla bla bla";            
            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("Information.png"));
        }

        [Test]
        public void Formatting_Heading()
        {
            //Arrange
            var formatter = new EventLogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                         Filter options: "Id","Type","Date","Time","Source","Category","Event","User","Computer", "Description"
            //                                         machine,log,filter,results,heading,cols,headers,tbl,head,row
            string input = "bla bla bla {EventLog(,'Application',,,'My Heading',,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("My Heading"));
        }

        [Test]
        public void Formatting_Columns_Default()
        {
            //Arrange
            var formatter = new EventLogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                         Filter options: "Id","Type","Date","Time","Source","Category","Event","User","Computer", "Description"
            //                                         machine,log,filter,results,heading,cols,headers,tbl,head,row
            string input = "bla bla bla {EventLog(,'Application',,,,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("! Type !! Date !! Source !! Description"));
        }

        [Test]
        public void Formatting_Columns_All()
        {
            //Arrange
            var formatter = new EventLogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                         Filter options: "Id","Type","Date","Time","Source","Category","Event","User","Computer", "Description"
            //                                         machine,log,filter,results,heading,cols,headers,tbl,head,row
            string input = "bla bla bla {EventLog(,'Application',,,,'all',,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("! Id !! Type !! Date !! Time !! Source !! Category !! Event !! User !! Computer !! Description"));
        }

        [Test]
        public void Formatting_Columns_Default_OtherHeads()
        {
            //Arrange
            var formatter = new EventLogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                         Filter options: "Id","Type","Date","Time","Source","Category","Event","User","Computer", "Description"
            //                                         machine,log,filter,results,heading,cols,headers,tbl,head,row
            string input = "bla bla bla {EventLog(,'Application',,,,,'H1,H2,H3,H4',,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("! H1 !! H2 !! H3 !! H4"));
        }

        [Test]
        public void Formatting_Columns_Default_Different_Order()
        {
            //Arrange
            var formatter = new EventLogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                         Filter options: "Id","Type","Date","Time","Source","Category","Event","User","Computer", "Description"
            //                                         machine,log,filter,results,heading,cols,headers,tbl,head,row
            string input = "bla bla bla {EventLog(,'Application',,,,'2,3,9,4',,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("! Date !! Time !! Description !! Source "));
        }

        [Test]
        public void Formatting_Columns_Default_Different_OrderAndHeadings()
        {
            //Arrange
            var formatter = new EventLogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                         Filter options: "Id","Type","Date","Time","Source","Category","Event","User","Computer", "Description"
            //                                         machine,log,filter,results,heading,cols,headers,tbl,head,row
            string input = "bla bla bla {EventLog(,'Application',,,,'2,3,9,4','H1,H2,H3,H4',,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("! H1 !! H2 !! H3 !! H4 "));
        }

        [Test]
        public void Formatting_Style()
        {
            //Arrange
            var formatter = new EventLogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                         Filter options: "Id","Type","Date","Time","Source","Category","Event","User","Computer", "Description"
            //                                         machine,log,filter,results,heading,cols,headers,tbl,head,row
            string input = "bla bla bla {EventLog(,'Application',,,,,,'gb','gb','gb')} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("style=\"background-color: #88CC33; color: #000000; font-weight: bold;\""));
        }
    }
}
