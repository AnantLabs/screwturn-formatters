using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using ScrewTurn.Wiki.PluginFramework;
using System.Web;
using Keeper.Garrett.ScrewTurn;
using Keeper.Garrett.ScrewTurn.MessageFormatter;

namespace Formatters.Tests
{
    [TestFixture]
    [Category("Formatter")]
    public class MessageFormatter_Test
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
        public void Default_Type()
        {
            //Arrange
            var formatter = new MessageFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { });

            string input = "bla bla bla <msg type=>Message</msg> bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla <link type=\"text/css\" rel=\"stylesheet\" href=\"GetFile.aspx?File=/Keeper.Garrett.Formatters/MessageFormatter/MessageStyle.css\"></link>\n<table class=\"information\"><tbody>\n<tr><td class=\"image-col\"></td><td class=\"head-col\">Information</td</tr>\n<tr><td></td><td class=\"content\">Message</td></tr></tbody></table> bla bla bla", retval);
        }

        [Test]
        public void Default_NoType()
        {
            //Arrange
            var formatter = new MessageFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { });

            string input = "bla bla bla <msg>Message</msg> bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla <link type=\"text/css\" rel=\"stylesheet\" href=\"GetFile.aspx?File=/Keeper.Garrett.Formatters/MessageFormatter/MessageStyle.css\"></link>\n<table class=\"information\"><tbody>\n<tr><td class=\"image-col\"></td><td class=\"head-col\">Information</td</tr>\n<tr><td></td><td class=\"content\">Message</td></tr></tbody></table> bla bla bla", retval);
        }

        [Test]
        public void Specific_Type()
        {
            //Arrange
            var formatter = new MessageFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { });

            string input = "bla bla bla <msg type=Tip>Message</msg> bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla <link type=\"text/css\" rel=\"stylesheet\" href=\"GetFile.aspx?File=/Keeper.Garrett.Formatters/MessageFormatter/MessageStyle.css\"></link>\n<table class=\"tip\"><tbody>\n<tr><td class=\"image-col\"></td><td class=\"head-col\">Tip</td</tr>\n<tr><td></td><td class=\"content\">Message</td></tr></tbody></table> bla bla bla", retval);
        }

        [Test]
        public void Specific_Type_With_Own_Heading()
        {
            //Arrange
            var formatter = new MessageFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { });

            string input = "bla bla bla <msg type=Tip head=MyHead>Message</msg> bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla <link type=\"text/css\" rel=\"stylesheet\" href=\"GetFile.aspx?File=/Keeper.Garrett.Formatters/MessageFormatter/MessageStyle.css\"></link>\n<table class=\"tip\"><tbody>\n<tr><td class=\"image-col\"></td><td class=\"head-col\">MyHead</td</tr>\n<tr><td></td><td class=\"content\">Message</td></tr></tbody></table> bla bla bla", retval);
        }

        [Test]
        public void Specific_Type_With_Default_Heading()
        {
            //Arrange
            var formatter = new MessageFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { });

            string input = "bla bla bla <msg type=Tip head=>Message</msg> bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla <link type=\"text/css\" rel=\"stylesheet\" href=\"GetFile.aspx?File=/Keeper.Garrett.Formatters/MessageFormatter/MessageStyle.css\"></link>\n<table class=\"tip\"><tbody>\n<tr><td class=\"image-col\"></td><td class=\"head-col\">Tip</td</tr>\n<tr><td></td><td class=\"content\">Message</td></tr></tbody></table> bla bla bla", retval);
        }

        [Test]
        public void Specific_Type_No_Message()
        {
            //Arrange
            var formatter = new MessageFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var pageProvider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", pageProvider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            //Expect
            host.Expect(x => x.GetFilesStorageProviders(true)).Return(new IFilesStorageProviderV30[] { });

            string input = "bla bla bla <msg type=Tip></msg> bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla <link type=\"text/css\" rel=\"stylesheet\" href=\"GetFile.aspx?File=/Keeper.Garrett.Formatters/MessageFormatter/MessageStyle.css\"></link>\n<table class=\"tip\"><tbody>\n<tr><td class=\"image-col\"></td><td class=\"head-col\">Tip</td</tr></tbody></table> bla bla bla", retval);
        }

        [Test]
        public void StoreFiles_CreateAll()
        {
            //Arrange
            var formatter = new MessageFormatter();

            //Arrange
            var host = MockRepository.GenerateMock<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            provider.Expect(x => x.Information).Return(new ComponentInformation("SomeProvider", "", "", "", ""));
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).IgnoreArguments().Return(provider.GetType().FullName);
            host.Expect(x => x.GetFilesStorageProviders(true)).IgnoreArguments().Return(new IFilesStorageProviderV30[] { provider });

            //Dirs
            provider.Expect(x => x.ListDirectories("/")).Return(new string[] { "NoPresent1" });
            provider.Expect(x => x.ListDirectories("/Keeper.Garrett.Formatters/")).Return(new string[] { "NoPresent2" });
            provider.Expect(x => x.ListDirectories(string.Format("/Keeper.Garrett.Formatters/{0}", formatter.GetType().Name))).Return(new string[] { "NoPresent3" });

            //Files
            provider.Expect(x => x.ListFiles(string.Format("/Keeper.Garrett.Formatters/{0}", formatter.GetType().Name))).Return(new string[] { "NoFilesAtAll" });
            provider.Expect(x => x.ListFiles(string.Format("/Keeper.Garrett.Formatters/{0}/Images-24x24", formatter.GetType().Name))).Return(new string[] { "NoFilesAtAll" });
            provider.Expect(x => x.ListFiles(string.Format("/Keeper.Garrett.Formatters/{0}/Images-32x32", formatter.GetType().Name))).Return(new string[] { "NoFilesAtAll" });

            //Act
            formatter.Init(host, "");

            //Assert
            provider.AssertWasCalled(x => x.CreateDirectory("/", "Keeper.Garrett.Formatters"));
            provider.AssertWasCalled(x => x.CreateDirectory("/Keeper.Garrett.Formatters/", "MessageFormatter"));
            provider.AssertWasCalled(x => x.CreateDirectory("/Keeper.Garrett.Formatters/MessageFormatter", "Images-24x24"));
            provider.AssertWasCalled(x => x.CreateDirectory("/Keeper.Garrett.Formatters/MessageFormatter", "Images-32x32"));
        }
    }
}
