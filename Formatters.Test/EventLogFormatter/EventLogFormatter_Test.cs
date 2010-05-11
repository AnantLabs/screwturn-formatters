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

namespace Formatters.Tests
{
    [TestFixture]
    [Category("Formatter")]
    public class EventLogFormatter_Test
    {
        [Test]
        public void No_Machine()
        {
            //Arrange
            var formatter = new EventLogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                                    machine,log,filter,results,heading,cols,headers,tbl,head,row
            string input = "bla bla bla {EventLog log=Application machine=IDoNotExist} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla (((Error connection to IDoNotExist: Application, Filter: \r\nMessage: The network path was not found.\r\n))) bla bla bla", retval);
        }

        [Test]
        public void No_Results()
        {
            //Arrange
            var formatter = new EventLogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            //                                    machine,log,filter,results,heading,cols,headers,tbl,head,row
            string input = "bla bla bla {EventLog log='ForwardedEvents'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla (((<h2>No logs found for " + Environment.MachineName + ": ForwardedEvents, Filter: </h2>))) bla bla bla", retval);
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
            string input = "bla bla bla {EventLog log=Application} bla bla bla";

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
            string input = "bla bla bla {EventLog log=Application filter='type=Error'} bla bla bla";

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
            string input = "bla bla bla {EventLog log='Application' filter='type=Warning'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("Warning.png"));
        }

        [Test]
        public void Filter_Id_MsiInstaller()
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
            string input = "bla bla bla {EventLog log='Application' filter='id=1001'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("<td>[image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Information.png|Information] Information</td>"));
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
            string input = "bla bla bla {EventLog log=Application filter='type=Information'} bla bla bla";

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
            string input = "bla bla bla {EventLog log=Application filter='type=infoandwarn'} bla bla bla";

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
            string input = "bla bla bla {EventLog log=Application filter='type=infoanderror' results=50} bla bla bla";

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
            string input = "bla bla bla {EventLog log=Application filter='type=warnanderror'} bla bla bla";

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
            string input = "bla bla bla {EventLog log=Application filter='date=" + DateTime.Now.AddDays(-1) + "'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("Information.png"));
        }

        [Test]
        public void Filter_Time_Number()
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
            string input = "bla bla bla {EventLog log=Application filter='time=-24'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("Information.png"));
        }

        [Test]
        public void Filter_Event()
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
            string input = "bla bla bla {EventLog log=Application filter='event=1001'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("Information.png"));
        }

        [Test]
        public void Filter_Computer()
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
            string input = "bla bla bla {EventLog log=Application filter='computer=" + Environment.MachineName +"'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("Information.png"));
        }

        [Test]
        public void Filter_Category_Footer()
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
            string input = "bla bla bla {EventLog log=Application foot='My Footer' filter='category=General' cols='id'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("standard-foot"));
        }

        [Test]
        public void Filter_User_NoCols()
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
            string input = "bla bla bla {EventLog log=Application filter='user=NT AUTHORITY\\SYSTEM' cols='badcol'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains(">Type<"));
            Assert.AreEqual(true, retval.Contains(">Date<"));
            Assert.AreEqual(true, retval.Contains(">Description<"));
            Assert.AreEqual(true, retval.Contains(">Source<")); 
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
            string input = "bla bla bla {EventLog log=Application filter='date=-1'} bla bla bla";

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
            string input = "bla bla bla {EventLog log=Application filter='time=" + DateTime.Now.AddDays(-1) + "'} bla bla bla";

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
            string input = "bla bla bla {EventLog log=Application filter='source=EventSystem'} bla bla bla";

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
            string input = "bla bla bla {EventLog log=Application filter='description=database'} bla bla bla";

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
            string input = "bla bla bla {EventLog log=Application filter='type=information,date=" + DateTime.Now.AddDays(-2) + ",source=vss,description=idle'} bla bla bla";            
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
            string input = "bla bla bla {EventLog log=Application filter='Application' head='My Heading'} bla bla bla";

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
            string input = "bla bla bla {EventLog log=Application} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains(">Type<"));
            Assert.AreEqual(true, retval.Contains(">Date<"));
            Assert.AreEqual(true, retval.Contains(">Source<"));
            Assert.AreEqual(true, retval.Contains(">Description<"));
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
            string input = "bla bla bla {EventLog log=Application cols='all'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains(">Id<"));
            Assert.AreEqual(true, retval.Contains(">Type<"));
            Assert.AreEqual(true, retval.Contains(">Date<"));
            Assert.AreEqual(true, retval.Contains(">Time<"));
            Assert.AreEqual(true, retval.Contains(">Source<"));
            Assert.AreEqual(true, retval.Contains(">Category<"));
            Assert.AreEqual(true, retval.Contains(">Event<"));
            Assert.AreEqual(true, retval.Contains(">User<"));
            Assert.AreEqual(true, retval.Contains(">Computer<"));
            Assert.AreEqual(true, retval.Contains(">Description<"));
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
            string input = "bla bla bla {EventLog log=Application colnames='H1,H2,H3,H4'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains(">H1<"));
            Assert.AreEqual(true, retval.Contains(">H2<"));
            Assert.AreEqual(true, retval.Contains(">H3<"));
            Assert.AreEqual(true, retval.Contains(">H4<"));
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
            string input = "bla bla bla {EventLog log=Application cols='date,time,description,source'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains(">Date<"));
            Assert.AreEqual(true, retval.Contains(">Time<"));
            Assert.AreEqual(true, retval.Contains(">Description<"));
            Assert.AreEqual(true, retval.Contains(">Source<"));
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
            string input = "bla bla bla {EventLog log=Application cols='date,time,description,source' colnames='H1,H2,H3,H4'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains(">H1<"));
            Assert.AreEqual(true, retval.Contains(">H2<"));
            Assert.AreEqual(true, retval.Contains(">H3<"));
            Assert.AreEqual(true, retval.Contains(">H4<"));
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
            string input = "bla bla bla {EventLog log='Application' style='gb'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("<table id=\"gb\">"));
        }
    }
}
