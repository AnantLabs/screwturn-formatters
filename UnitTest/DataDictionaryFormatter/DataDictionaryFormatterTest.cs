using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using ScrewTurn.Wiki.PluginFramework;
using System.Web;
using Keeper.Garrett.ScrewTurn;
using Keeper.Garrett.ScrewTurn.DataDictionary;

namespace UnitTest
{
    [TestFixture]
    [Category("Formatter")]
    public class DataDictionaryFormatterTest
    {
        [TestFixtureSetUp]
        public void Setup()
        {
        }
        
        [Test]
        [Category("Namespace")]
        public void Format_Regex_Namespace_Root()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("DataDictionary", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "DataDictionary", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under {2}.\n----\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.AddPage(null, null, DateTime.Now)).IgnoreArguments().Return(pageInfo);
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "DataDictionary", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {Dict#An Entry#a Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{Dict#An Entry#a Summary}"));
            var args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("DataDictionary", (args[0][0] as PageInfo).FullName);
        }

        [Test]
        [Category("Namespace")]
        public void Format_Regex_Namespace_NotRoot()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyNsPage.DataDictionary", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyNsPage.DataDictionary", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under X.\n----\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.AddPage(null, null, DateTime.Now)).IgnoreArguments().Return(pageInfo);
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyNsPage.DataDictionary", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {Dict#An Entry#a Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{Dict#An Entry#a Summary}"));
            var args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyNsPage.DataDictionary", (args[0][0] as PageInfo).FullName);
        }

        [Test]
        [Category("Page")]
        public void Format_Regex_DefaultPage_NotExist()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("DataDictionary", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "DataDictionary", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under {2}.\n----\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(null);

            //Provider
            provider.Expect(x => x.AddPage(null, null, DateTime.Now)).IgnoreArguments().Return(pageInfo);
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "DataDictionary", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {Dict#An Entry#a Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            provider.AssertWasNotCalled(x => x.AddPage(null,null,DateTime.Now));
            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now,null,null,null,null,SaveMode.Normal));
            Assert.AreEqual(false, retval.Contains("{Dict#An Entry#a Summary}"));
            var args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("DataDictionary", (args[0][0] as PageInfo).FullName);
            //Test content 
            Assert.AreEqual("The complete list of DataDictionary entries filed under DataDictionary.\n----\n----", (args[0][5] as string));
        }

        [Test]
        [Category("Page")]
        public void Format_Regex_DefaultPage_Exist()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("DataDictionary", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "DataDictionary", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under {2}.\n----\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "DataDictionary", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {Dict#An Entry#a Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{Dict#An Entry#a Summary}"));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.AddPage(null, null, DateTime.Now));
            Assert.AreEqual(0, args.Count);

            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("DataDictionary", (args[0][0] as PageInfo).FullName);
        }

        [Test]
        [Category("Page")]
        public void Format_Regex_CustomPage_NotExist()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyDict", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(null);

            //Provider
            provider.Expect(x => x.AddPage(null, null, DateTime.Now)).IgnoreArguments().Return(pageInfo);
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {MyLink#An Entry#a Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            provider.AssertWasNotCalled(x => x.AddPage(null, null, DateTime.Now));
            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual(false, retval.Contains("{MyLink#An Entry#a Summary}"));
            var args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);
            //Test content 
            Assert.AreEqual("The complete list of DataDictionary entries filed under MyDict.\n----\n----", (args[0][5] as string));
        }

        [Test]
        [Category("Page")]
        public void Format_Regex_CustomPage_Exist()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyDict", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {MyLink#An Entry#a Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{MyDict#An Entry#a Summary}"));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.AddPage(null, null, DateTime.Now));
            Assert.AreEqual(0, args.Count);

            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);
        }

        [Test]
        [Category("Entry")]
        public void Format_Regex_FirstEntry_Summary()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyDict", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {MyLink#My First Entry#My First Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{MyDict#An Entry#a Summary}"));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.AddPage(null, null, DateTime.Now));
            Assert.AreEqual(0, args.Count);

            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);

            //Test content is there
            Assert.AreEqual(true, (args[0][5] as string).Contains("* '''My First Entry'''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyDict] - ''My First Summary''"));
            //Test content there only once
            Assert.AreEqual("The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyDict] - ''My First Summary''\n\n----", (args[0][5] as string));
        }

        [Test]
        [Category("Entry")]
        public void Format_Regex_FirstEntry_NoSummary()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyDict", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {MyLink#My First Entry#} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{MyDict#An Entry#}"));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.AddPage(null, null, DateTime.Now));
            Assert.AreEqual(0, args.Count);

            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);

            //Test content is there
            Assert.AreEqual(true, (args[0][5] as string).Contains("* '''My First Entry'''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyDict]"));
            //Test content there only once
            Assert.AreEqual("The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyDict]\n\n----", (args[0][5] as string));
        }

        [Test]
        [Category("Entry")]
        public void Format_Regex_FirstEntry_Null()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyDict", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {MyLink##My Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{MyDict##}"));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.AddPage(null, null, DateTime.Now));
            Assert.AreEqual(0, args.Count);

            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);

            //Test content is there
            Assert.AreEqual(true, (args[0][5] as string).Contains("* ''''''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyDict] - ''My Summary''"));
            //Test content there only once
            Assert.AreEqual("The complete list of DataDictionary entries filed under MyDict.\n----\n* ''''''\n** [MyDict] - ''My Summary''\n\n----", (args[0][5] as string));
        }

        [Test]
        [Category("Entry")]
        public void Format_Regex_FirstEntry_SecondTime_SameSummary()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyDict", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n\n** [MyDict] - ''My First Summary''\n\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);
            
            //Provider
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {MyLink#My First Entry#My First Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{MyLink#My First Entry#My First Summary}"));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.AddPage(null, null, DateTime.Now));
            Assert.AreEqual(0, args.Count);

            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);

            //Test content is there
            Assert.AreEqual(true, (args[0][5] as string).Contains("* '''My First Entry'''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyDict] - ''My First Summary''"));
            //Test content there only once
            Assert.AreEqual("The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyDict] - ''My First Summary''\n\n----", (args[0][5] as string));
        }

        [Test]
        [Category("Entry")]
        public void Format_Regex_FirstEntry_SecondTime_NewSummary()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyDict", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n\n** [MyDict] - ''My First Summary''\n\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {MyLink#My First Entry#My Second Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{MyLink#My First Entry#My Second Summary}"));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.AddPage(null, null, DateTime.Now));
            Assert.AreEqual(0, args.Count);

            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);

            //Test content is there
            Assert.AreEqual(true, (args[0][5] as string).Contains("* '''My First Entry'''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyDict] - ''My Second Summary''"));
            //Test content there only once
            Assert.AreEqual("The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyDict] - ''My Second Summary''\n\n----", (args[0][5] as string));
        }

        [Test]
        [Category("Entry")]
        public void Format_Regex_FirstEntry_NewSubEntry()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyDict", provider, DateTime.Now);
            var pageInfoContext = new PageInfo("SomeOtherPage", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n\n** [MyDict] - ''My First Summary''\n\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {MyLink#My First Entry#My Second Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfoContext, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{MyLink#My First Entry#My Second Summary}"));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.AddPage(null, null, DateTime.Now));
            Assert.AreEqual(0, args.Count);

            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);

            //Test content is there
            Assert.AreEqual(true, (args[0][5] as string).Contains("* '''My First Entry'''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyDict] - ''My First Summary''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [SomeOtherPage] - ''My Second Summary''"));
            //Test content there only once
            Assert.AreEqual("The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyDict] - ''My First Summary''\n** [SomeOtherPage] - ''My Second Summary''\n\n----", (args[0][5] as string));
        }

        [Test]
        [Category("Entry")]
        public void Format_Regex_FirstEntry_SameSubEntry_NewSummary()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyDict", provider, DateTime.Now);
            var pageInfoContext = new PageInfo("SomeOtherPage", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyDict] - ''My First Summary''\n** [SomeOtherPage] - ''My Second Summary''\n\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {MyLink#My First Entry#My Second Summary Modified} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfoContext, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{MyLink#My First Entry#My Second Summary Modified}"));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.AddPage(null, null, DateTime.Now));
            Assert.AreEqual(0, args.Count);

            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);

            //Test content is there
            Assert.AreEqual(true, (args[0][5] as string).Contains("* '''My First Entry'''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyDict] - ''My First Summary''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [SomeOtherPage] - ''My Second Summary Modified''"));
            //Test content there only once
            Assert.AreEqual("The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyDict] - ''My First Summary''\n** [SomeOtherPage] - ''My Second Summary Modified''\n\n----", (args[0][5] as string));
        }
        
        [Test]
        [Category("Entry")]
        public void Format_Regex_SecondEntry_Summary()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyDict", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n\n** [MyDict] - ''My First Summary''\n\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {MyLink#My Second Entry#My Second Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{MyLink#My Second Entry#My Second Summary}"));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.AddPage(null, null, DateTime.Now));
            Assert.AreEqual(0, args.Count);

            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);

            //Test content is there
            Assert.AreEqual(true, (args[0][5] as string).Contains("* '''My First Entry'''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyDict] - ''My First Summary''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("* '''My Second Entry'''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyDict] - ''My Second Summary''"));
            //Test content there only once
            Assert.AreEqual("The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyDict] - ''My First Summary''\n\n* '''My Second Entry'''\n** [MyDict] - ''My Second Summary''\n\n----", (args[0][5] as string));
        }

        [Test]
        [Category("Entry")]
        public void Format_Regex_SecondEntry_NewSummary()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyDict", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyDict] - ''My First Summary''\n\n* '''My Second Entry'''\n** [MyDict] - ''My Second Summary''\n\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {MyLink#My Second Entry#My Second Summary Modified} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{MyLink#My Second Entry#My Second Summary Modified}"));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.AddPage(null, null, DateTime.Now));
            Assert.AreEqual(0, args.Count);

            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);

            //Test content is there
            Assert.AreEqual(true, (args[0][5] as string).Contains("* '''My First Entry'''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyDict] - ''My First Summary''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("* '''My Second Entry'''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyDict] - ''My Second Summary Modified''"));
            //Test content there only once
            Assert.AreEqual("The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyDict] - ''My First Summary''\n\n* '''My Second Entry'''\n** [MyDict] - ''My Second Summary Modified''\n\n----", (args[0][5] as string));
        }

        [Test]
        [Category("Entry")]
        public void Format_Regex_Remove_FirstEntry_SecondSubEntry()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pIDict = new PageInfo("MyDict", provider, DateTime.Now);
            var pCDict = new PageContent(pIDict, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyRef1] - ''My First Summary''\n** [MyRef2] - ''My Second Summary''\n\n----", null, "");
            var pIRef1 = new PageInfo("MyRef1", provider, DateTime.Now);
            var pCRef1 = new PageContent(pIRef1, "MyRef1", "", DateTime.Now, "", "{MyLink#My First Entry#}", null, "");
            var pIRef2 = new PageInfo("MyRef2", provider, DateTime.Now);
            var pCRef2 = new PageContent(pIRef2, "MyRef2", "", DateTime.Now, "", "", null, "");

            //Setup to ensure creation
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null)).Repeat.Any();

            //Host+provider, chronologic calls
            host.Expect(x => x.FindPage("MyDict")).Return(pIDict); //Find Dict page
            provider.Expect(x => x.GetContent(pIDict)).Return(pCDict); //Dict content
            host.Expect(x => x.FindPage("MyRef1")).Return(pIRef1); //Find 1 refed page
            provider.Expect(x => x.GetContent(pIRef1)).Return(pCRef1); //Get 1 refed content
            host.Expect(x => x.FindPage("MyRef2")).Return(pIRef2); //Find 2 refed page
            provider.Expect(x => x.GetContent(pIRef2)).Return(pCRef2); //Get 2 refed content
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any(); //Modify ok
            string input = "bla bla {DataDictionary#ClEan} bla bla";

            //Dict page (dont matter when deleting as long as there's a Clean command on the page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pIDict, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);

            //Test content is there
            Assert.AreEqual(true,  (args[0][5] as string).Contains("* '''My First Entry'''"));
            Assert.AreEqual(true,  (args[0][5] as string).Contains("** [MyRef1] - ''My First Summary''"));
            Assert.AreEqual(false, (args[0][5] as string).Contains("** [MyRef2] - ''My Second Summary''"));
            //Test content there only once
            Assert.AreEqual("The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyRef1] - ''My First Summary''\n\n----", (args[0][5] as string));
        }


        

        [Test]
        [Category("Namespace Cached")]
        public void Format_Regex_Namespace_Root_Cached()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("DataDictionary", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "DataDictionary", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under {2}.\n----\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.AddPage(null, null, DateTime.Now)).IgnoreArguments().Return(pageInfo);
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "DataDictionary", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {Dict#An Entry#a Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "EnableCache=true");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{Dict#An Entry#a Summary}"));
            var args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("DataDictionary", (args[0][0] as PageInfo).FullName);
        }

        [Test]
        [Category("Namespace Cached")]
        public void Format_Regex_Namespace_NotRoot_Cached()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyNsPage.DataDictionary", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyNsPage.DataDictionary", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under X.\n----\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.AddPage(null, null, DateTime.Now)).IgnoreArguments().Return(pageInfo);
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyNsPage.DataDictionary", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {Dict#An Entry#a Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "EnableCache=true");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{Dict#An Entry#a Summary}"));
            var args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyNsPage.DataDictionary", (args[0][0] as PageInfo).FullName);
        }

        [Test]
        [Category("Page Cached")]
        public void Format_Regex_DefaultPage_NotExist_Cached()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("DataDictionary", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "DataDictionary", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under {2}.\n----\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(null);

            //Provider
            provider.Expect(x => x.AddPage(null, null, DateTime.Now)).IgnoreArguments().Return(pageInfo);
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "DataDictionary", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {Dict#An Entry#a Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "EnableCache=true");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            provider.AssertWasNotCalled(x => x.AddPage(null, null, DateTime.Now));
            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual(false, retval.Contains("{Dict#An Entry#a Summary}"));
            var args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("DataDictionary", (args[0][0] as PageInfo).FullName);
            //Test content 
            Assert.AreEqual("The complete list of DataDictionary entries filed under DataDictionary.\n----\n----", (args[0][5] as string));
        }

        [Test]
        [Category("Page Cached")]
        public void Format_Regex_DefaultPage_Exist_Cached()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("DataDictionary", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "DataDictionary", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under {2}.\n----\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "DataDictionary", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {Dict#An Entry#a Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "EnableCache=true");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{Dict#An Entry#a Summary}"));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.AddPage(null, null, DateTime.Now));
            Assert.AreEqual(0, args.Count);

            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("DataDictionary", (args[0][0] as PageInfo).FullName);
        }

        [Test]
        [Category("Page Cached")]
        public void Format_Regex_CustomPage_NotExist_Cached()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyDict", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(null);

            //Provider
            provider.Expect(x => x.AddPage(null, null, DateTime.Now)).IgnoreArguments().Return(pageInfo);
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {MyLink#An Entry#a Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict\nEnableCache=true");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            provider.AssertWasNotCalled(x => x.AddPage(null, null, DateTime.Now));
            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual(false, retval.Contains("{MyLink#An Entry#a Summary}"));
            var args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);
            //Test content 
            Assert.AreEqual("The complete list of DataDictionary entries filed under MyDict.\n----\n----", (args[0][5] as string));
        }

        [Test]
        [Category("Page Cached")]
        public void Format_Regex_CustomPage_Exist_Cached()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyDict", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {MyLink#An Entry#a Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict\nEnableCache=true");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{MyDict#An Entry#a Summary}"));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.AddPage(null, null, DateTime.Now));
            Assert.AreEqual(0, args.Count);

            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);
        }

        [Test]
        [Category("Entry Cached")]
        public void Format_Regex_FirstEntry_Summary_Cached()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyDict", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {MyLink#My First Entry#My First Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict\nEnableCache=true");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{MyDict#An Entry#a Summary}"));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.AddPage(null, null, DateTime.Now));
            Assert.AreEqual(0, args.Count);

            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);

            //Test content is there
            Assert.AreEqual(true, (args[0][5] as string).Contains("* '''My First Entry'''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyDict] - ''My First Summary''"));
            //Test content there only once
            Assert.AreEqual("The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyDict] - ''My First Summary''\n\n----", (args[0][5] as string));
        }

        [Test]
        [Category("Entry Cached")]
        public void Format_Regex_FirstEntry_NoSummary_Cached()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyDict", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {MyLink#My First Entry#} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict\nEnableCache=true");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{MyDict#An Entry#}"));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.AddPage(null, null, DateTime.Now));
            Assert.AreEqual(0, args.Count);

            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);

            //Test content is there
            Assert.AreEqual(true, (args[0][5] as string).Contains("* '''My First Entry'''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyDict]"));
            //Test content there only once
            Assert.AreEqual("The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyDict]\n\n----", (args[0][5] as string));
        }

        [Test]
        [Category("Entry Cached")]
        public void Format_Regex_FirstEntry_Null_Cached()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyDict", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {MyLink##My Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict\nEnableCache=true");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{MyDict##}"));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.AddPage(null, null, DateTime.Now));
            Assert.AreEqual(0, args.Count);

            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);

            //Test content is there
            Assert.AreEqual(true, (args[0][5] as string).Contains("* ''''''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyDict] - ''My Summary''"));
            //Test content there only once
            Assert.AreEqual("The complete list of DataDictionary entries filed under MyDict.\n----\n* ''''''\n** [MyDict] - ''My Summary''\n\n----", (args[0][5] as string));
        }

        [Test]
        [Category("Entry Cached")]
        public void Format_Regex_FirstEntry_SecondTime_SameSummary_Cached()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyDict", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n\n** [MyDict] - ''My First Summary''\n\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {MyLink#My First Entry#My First Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict\nEnableCache=true");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{MyLink#My First Entry#My First Summary}"));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.AddPage(null, null, DateTime.Now));
            Assert.AreEqual(0, args.Count);

            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);

            //Test content is there
            Assert.AreEqual(true, (args[0][5] as string).Contains("* '''My First Entry'''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyDict] - ''My First Summary''"));
            //Test content there only once
            Assert.AreEqual("The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyDict] - ''My First Summary''\n\n----", (args[0][5] as string));
        }

        [Test]
        [Category("Entry Cached")]
        public void Format_Regex_FirstEntry_SecondTime_NewSummary_Cached()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyDict", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n\n** [MyDict] - ''My First Summary''\n\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {MyLink#My First Entry#My Second Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict\nEnableCache=true");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{MyLink#My First Entry#My Second Summary}"));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.AddPage(null, null, DateTime.Now));
            Assert.AreEqual(0, args.Count);

            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);

            //Test content is there
            Assert.AreEqual(true, (args[0][5] as string).Contains("* '''My First Entry'''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyDict] - ''My Second Summary''"));
            //Test content there only once
            Assert.AreEqual("The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyDict] - ''My Second Summary''\n\n----", (args[0][5] as string));
        }

        [Test]
        [Category("Entry Cached")]
        public void Format_Regex_FirstEntry_NewSubEntry_Cached()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyDict", provider, DateTime.Now);
            var pageInfoContext = new PageInfo("SomeOtherPage", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n\n** [MyDict] - ''My First Summary''\n\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {MyLink#My First Entry#My Second Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfoContext, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict\nEnableCache=true");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{MyLink#My First Entry#My Second Summary}"));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.AddPage(null, null, DateTime.Now));
            Assert.AreEqual(0, args.Count);

            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);

            //Test content is there
            Assert.AreEqual(true, (args[0][5] as string).Contains("* '''My First Entry'''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyDict] - ''My First Summary''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [SomeOtherPage] - ''My Second Summary''"));
            //Test content there only once
            Assert.AreEqual("The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyDict] - ''My First Summary''\n** [SomeOtherPage] - ''My Second Summary''\n\n----", (args[0][5] as string));
        }

        [Test]
        [Category("Entry Cached")]
        public void Format_Regex_FirstEntry_SameSubEntry_NewSummary_Cached()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyDict", provider, DateTime.Now);
            var pageInfoContext = new PageInfo("SomeOtherPage", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyDict] - ''My First Summary''\n** [SomeOtherPage] - ''My Second Summary''\n\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {MyLink#My First Entry#My Second Summary Modified} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfoContext, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict\nEnableCache=true");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{MyLink#My First Entry#My Second Summary Modified}"));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.AddPage(null, null, DateTime.Now));
            Assert.AreEqual(0, args.Count);

            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);

            //Test content is there
            Assert.AreEqual(true, (args[0][5] as string).Contains("* '''My First Entry'''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyDict] - ''My First Summary''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [SomeOtherPage] - ''My Second Summary Modified''"));
            //Test content there only once
            Assert.AreEqual("The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyDict] - ''My First Summary''\n** [SomeOtherPage] - ''My Second Summary Modified''\n\n----", (args[0][5] as string));
        }

        [Test]
        [Category("Entry Cached")]
        public void Format_Regex_SecondEntry_Summary_Cached()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyDict", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n\n** [MyDict] - ''My First Summary''\n\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {MyLink#My Second Entry#My Second Summary} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict\nEnableCache=true");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{MyLink#My Second Entry#My Second Summary}"));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.AddPage(null, null, DateTime.Now));
            Assert.AreEqual(0, args.Count);

            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);

            //Test content is there
            Assert.AreEqual(true, (args[0][5] as string).Contains("* '''My First Entry'''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyDict] - ''My First Summary''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("* '''My Second Entry'''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyDict] - ''My Second Summary''"));
            //Test content there only once
            Assert.AreEqual("The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyDict] - ''My First Summary''\n\n* '''My Second Entry'''\n** [MyDict] - ''My Second Summary''\n\n----", (args[0][5] as string));
        }

        [Test]
        [Category("Entry Cached")]
        public void Format_Regex_SecondEntry_NewSummary_Cached()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pageInfo = new PageInfo("MyDict", provider, DateTime.Now);
            var pageContent = new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyDict] - ''My First Summary''\n\n* '''My Second Entry'''\n** [MyDict] - ''My Second Summary''\n\n----", null, "");

            //Host
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));
            host.Expect(x => x.FindPage(null)).IgnoreArguments().Return(pageInfo);

            //Provider
            provider.Expect(x => x.GetContent(null)).IgnoreArguments().Return(pageContent);//new PageContent(pageInfo, "MyDict", "", DateTime.Now, "", "", new string[] { }, ""));
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any();
            string input = "bla bla bla {MyLink#My Second Entry#My Second Summary Modified} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict\nEnableCache=true");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("{MyLink#My Second Entry#My Second Summary Modified}"));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.AddPage(null, null, DateTime.Now));
            Assert.AreEqual(0, args.Count);

            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);

            //Test content is there
            Assert.AreEqual(true, (args[0][5] as string).Contains("* '''My First Entry'''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyDict] - ''My First Summary''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("* '''My Second Entry'''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyDict] - ''My Second Summary Modified''"));
            //Test content there only once
            Assert.AreEqual("The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyDict] - ''My First Summary''\n\n* '''My Second Entry'''\n** [MyDict] - ''My Second Summary Modified''\n\n----", (args[0][5] as string));
        }

        [Test]
        [Category("Entry Cached")]
        public void Format_Regex_Remove_FirstEntry_SecondSubEntry_Cached()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var formatter = new DataDictionaryFormatter();
            var pIDict = new PageInfo("MyDict", provider, DateTime.Now);
            var pCDict = new PageContent(pIDict, "MyDict", "", DateTime.Now, "", "The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyRef1] - ''My First Summary''\n** [MyRef2] - ''My Second Summary''\n\n----", null, "");
            var pIRef1 = new PageInfo("MyRef1", provider, DateTime.Now);
            var pCRef1 = new PageContent(pIRef1, "MyRef1", "", DateTime.Now, "", "{MyLink#My First Entry#}", null, "");
            var pIRef2 = new PageInfo("MyRef2", provider, DateTime.Now);
            var pCRef2 = new PageContent(pIRef2, "MyRef2", "", DateTime.Now, "", "", null, "");

            //Setup to ensure creation
            host.Expect(x => x.GetPagesStorageProviders(true)).IgnoreArguments().Return(new IPagesStorageProviderV30[] { provider });
            host.Expect(x => x.GetCurrentUser()).Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null)).Repeat.Any();

            //Host+provider, chronologic calls
            host.Expect(x => x.FindPage("MyDict")).Return(pIDict); //Find Dict page
            provider.Expect(x => x.GetContent(pIDict)).Return(pCDict); //Dict content
            host.Expect(x => x.FindPage("MyRef1")).Return(pIRef1); //Find 1 refed page
            provider.Expect(x => x.GetContent(pIRef1)).Return(pCRef1); //Get 1 refed content
            host.Expect(x => x.FindPage("MyRef2")).Return(pIRef2); //Find 2 refed page
            provider.Expect(x => x.GetContent(pIRef2)).Return(pCRef2); //Get 2 refed content
            provider.Expect(x => x.ModifyPage(null, "", "", DateTime.Now, "", "", null, "", SaveMode.Normal)).IgnoreArguments().Return(true).Repeat.Any(); //Modify ok
            string input = "bla bla {DataDictionary#ClEan} bla bla";

            //Dict page (dont matter when deleting as long as there's a Clean command on the page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, pIDict, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "MyLink=MyDict\nEnableCache=true");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            provider.AssertWasNotCalled(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));

            var args = provider.GetArgumentsForCallsMadeOn(x => x.ModifyPage(null, null, null, DateTime.Now, null, null, null, null, SaveMode.Normal));
            Assert.AreEqual("MyDict", (args[0][0] as PageInfo).FullName);

            //Test content is there
            Assert.AreEqual(true, (args[0][5] as string).Contains("* '''My First Entry'''"));
            Assert.AreEqual(true, (args[0][5] as string).Contains("** [MyRef1] - ''My First Summary''"));
            Assert.AreEqual(false, (args[0][5] as string).Contains("** [MyRef2] - ''My Second Summary''"));
            //Test content there only once
            Assert.AreEqual("The complete list of DataDictionary entries filed under MyDict.\n----\n* '''My First Entry'''\n** [MyRef1] - ''My First Summary''\n\n----", (args[0][5] as string));
        }
    }
}
