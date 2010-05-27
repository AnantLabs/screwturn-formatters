using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using ScrewTurn.Wiki.PluginFramework;
using System.Web;
using Keeper.Garrett.ScrewTurn;
using Keeper.Garrett.ScrewTurn.BlogFormatter;
using ScrewTurn.Wiki;

namespace Formatters.Tests
{
    /// <summary>
    /// Category,noPosts,useLastMod,showCloud,showArchive,about,bottom,style  
    /// </summary>
    [TestFixture]
    [Category("Formatter")]
    public class BlogFormatter_Test
    {
        #region About

        [Test]
        public void GenerateAbout_MissingPage()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            catInfo[0].Pages = new string[] { };

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetPage(null)).IgnoreArguments().Return(null);

            host.Expect(x => x.GetPageContent(null)).IgnoreArguments().Return(null);
            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=3 recent=3 about='About'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true,retval.Contains("No blog entries/pages was found for blog 'MyBlog'"));
            Assert.AreEqual(false, retval.Contains("About Content"));
        }

        [Test]
        public void GenerateAbout()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            catInfo[0].Pages = new string[] { };

            var aboutPage = new PageInfo("About", provider, new DateTime(2010, 1, 4));
            var aboutContent = MockRepository.GenerateStub<PageContent>(aboutPage, "About Page", "", new DateTime(2010, 1, 4), "", "About Content", null, "My Description");

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetPage("About")).Return(aboutPage);

            host.Expect(x => x.GetPageContent(aboutPage)).Return(aboutContent);
            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=3 recent=3 about='About'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("About Content"));
        }


        [Test]
        public void GenerateAbout_SelfReference()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            catInfo[0].Pages = new string[] { };

            var aboutPage = new PageInfo("AboutPage", provider, new DateTime(2010, 1, 4));
            var aboutContent = MockRepository.GenerateStub<PageContent>(aboutPage, "About Page", "", new DateTime(2010, 1, 4), "", "About Content", null, "My Description");

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetPage("AboutPage")).Return(aboutPage);

            host.Expect(x => x.GetPageContent(aboutPage)).Return(aboutContent);
            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=3 recent=3 about='MyPage'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("No blog entries/pages was found for blog 'MyBlog'"));
            Assert.AreEqual(false, retval.Contains("About Content"));
        }

        #endregion

        [Test]
        public void StoreTableFiles_CreateAll()
        {
            //Arrange
            var formatter = new BlogFormatter();

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
            provider.Expect(x => x.ListFiles(string.Format("/Keeper.Garrett.Formatters/{0}/Images", formatter.GetType().Name))).Return(new string[] { "NoFilesAtAll" });

            //Act
            formatter.Init(host, "");

            //Assert
            provider.AssertWasCalled(x => x.CreateDirectory("/", "Keeper.Garrett.Formatters"));
            provider.AssertWasCalled(x => x.CreateDirectory("/Keeper.Garrett.Formatters/", formatter.GetType().Name));
            provider.AssertWasCalled(x => x.CreateDirectory(string.Format("/Keeper.Garrett.Formatters/{0}",formatter.GetType().Name), "Images"));
        }

        [Test]
        public void StoreTableFiles_CreateNone()
        {
            //Arrange
            var formatter = new BlogFormatter();

            //Arrange
            var host = MockRepository.GenerateMock<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            provider.Expect(x => x.Information).Return(new ComponentInformation("SomeProvider", "", "", "", ""));
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).IgnoreArguments().Return(provider.GetType().FullName);
            host.Expect(x => x.GetFilesStorageProviders(true)).IgnoreArguments().Return(new IFilesStorageProviderV30[] { provider });

            //Dirs
            provider.Expect(x => x.ListDirectories("/")).Return(new string[] { "Keeper.Garrett.Formatters" });
            provider.Expect(x => x.ListDirectories("/Keeper.Garrett.Formatters/")).Return(new string[] { formatter.GetType().Name });
            provider.Expect(x => x.ListDirectories(string.Format("/Keeper.Garrett.Formatters/{0}", formatter.GetType().Name))).Return(new string[] { "Images" });

            //Files
            provider.Expect(x => x.ListFiles(string.Format("/Keeper.Garrett.Formatters/{0}", formatter.GetType().Name))).Return(new string[] { "None" });
            provider.Expect(x => x.ListFiles(string.Format("/Keeper.Garrett.Formatters/{0}/Images", formatter.GetType().Name))).Return(new string[] { string.Format("/Keeper.Garrett.Formatters/{0}/Images/Error.png", formatter.GetType().Name) });

            //Act
            formatter.Init(host, "");

            //Assert
            provider.AssertWasNotCalled(x => x.CreateDirectory("/", "Keeper.Garrett.Formatters"));
            provider.AssertWasNotCalled(x => x.CreateDirectory("/Keeper.Garrett.Formatters/", formatter.GetType().Name));
            provider.AssertWasNotCalled(x => x.CreateDirectory(string.Format("/Keeper.Garrett.Formatters/{0}", formatter.GetType().Name), "Images"));
        }

        [Test]
        public void BlogPost_SelfReference()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("BlogPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("MyBlog", provider) };
            catInfo[0].Pages = new string[] { "MyPage1", "MyPage2", "MyPage3", "BlogPage" };
            var pageInfo1 = new PageInfo("MyPage1", provider, new DateTime(2010, 1, 1));
            var pageInfo2 = new PageInfo("MyPage2", provider, new DateTime(2010, 1, 2));
            var pageInfo3 = new PageInfo("MyPage3", provider, new DateTime(2010, 1, 3));
            var pageInfo4 = new PageInfo("BlogPage", provider, new DateTime(2010, 1, 3));
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "User", new DateTime(2010, 1, 1), "", "Content 1", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "User", new DateTime(2010, 1, 2), "", "Content 2", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "User", new DateTime(2010, 1, 3), "", "Content 3", null, "My Description 3");
            var pageContent4 = MockRepository.GenerateStub<PageContent>(pageInfo4, "BlogPage", "", new DateTime(2010, 1, 3), "", "Blog Page 3", null, "Blog Page");

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo[0]);
            provider.Expect(x => x.GetMessageCount(null)).IgnoreArguments().Return(3).Repeat.Any();

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.FindPage("BlogPage")).Return(pageInfo4);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);
            host.Expect(x => x.GetPageContent(pageInfo4)).Return(pageContent4);

            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=3 recent=3} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("No blog entries/pages was found for blog 'MyBlog'"));
            Assert.AreEqual(false, retval.Contains("MyPage"));
        }

        [Test]
        public void BlogPost_3_Posts_RootNs()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("BlogPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            catInfo[0].Pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, new DateTime(2010, 1, 1));
            var pageInfo2 = new PageInfo("MyPage2", provider, new DateTime(2010, 1, 2));
            var pageInfo3 = new PageInfo("MyPage3", provider, new DateTime(2010, 1, 3));
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "User", new DateTime(2010, 1, 1), "", "Content 1", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "User", new DateTime(2010, 1, 2), "", "Content 2", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "User", new DateTime(2010, 1, 3), "", "Content 3", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo[0]);
            provider.Expect(x => x.GetMessageCount(null)).IgnoreArguments().Return(3).Repeat.Any();

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");
            host.Expect(x => x.FindUser("User")).Repeat.Any().Return(new UserInfo("User", "Garrett", "", true, DateTime.Now, null));

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=3 recent=3} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"MyPage3.ashx\">Page 3</a></h1>"));
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"MyPage2.ashx\">Page 2</a></h1>"));
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"MyPage1.ashx\">Page 1</a></h1>"));
        }

        [Test]
        public void BlogPost_3_Posts_TestNS()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("Test.BlogPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            catInfo[0].Pages = new string[] { "Test.MyPage1", "Test.MyPage2", "Test.MyPage3" };
            var pageInfo1 = new PageInfo("Test.MyPage1", provider, new DateTime(2010, 1, 1));
            var pageInfo2 = new PageInfo("Test.MyPage2", provider, new DateTime(2010, 1, 2));
            var pageInfo3 = new PageInfo("Test.MyPage3", provider, new DateTime(2010, 1, 3));
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "User", new DateTime(2010, 1, 1), "", "Content 1", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "User", new DateTime(2010, 1, 2), "", "Content 2", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "User", new DateTime(2010, 1, 3), "", "Content 3", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo[0]);
            provider.Expect(x => x.GetMessageCount(null)).IgnoreArguments().Return(3).Repeat.Any();

            host.Expect(x => x.FindPage("Test.MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("Test.MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("Test.MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");
            host.Expect(x => x.FindUser("User")).Repeat.Any().Return(new UserInfo("User", "Garrett", "", true, DateTime.Now, null));

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=3 recent=3} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"Test.MyPage3.ashx\">Page 3</a></h1>"));
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"Test.MyPage2.ashx\">Page 2</a></h1>"));
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"Test.MyPage1.ashx\">Page 1</a></h1>"));
        }

  /*      [Test]
        public void BlogPost_3_Posts_TestNS_BlogRoot()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("Test.BlogPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            catInfo[0].Pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, new DateTime(2010, 1, 1));
            var pageInfo2 = new PageInfo("MyPage2", provider, new DateTime(2010, 1, 2));
            var pageInfo3 = new PageInfo("MyPage3", provider, new DateTime(2010, 1, 3));
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "User", new DateTime(2010, 1, 1), "", "Content 1", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "User", new DateTime(2010, 1, 2), "", "Content 2", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "User", new DateTime(2010, 1, 3), "", "Content 3", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo[0]);
            provider.Expect(x => x.GetMessageCount(null)).IgnoreArguments().Return(3).Repeat.Any();

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");
            host.Expect(x => x.FindUser("User")).Repeat.Any().Return(new UserInfo("User", "Garrett", "", true, DateTime.Now, null));

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=3 recent=3 ns=root} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"MyPage3.ashx\">Page 3</a></h1>"));
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"MyPage2.ashx\">Page 2</a></h1>"));
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"MyPage1.ashx\">Page 1</a></h1>"));
        }*/

        [Test]
        public void BlogPost_3_Posts_RootNS_BlogTest()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("BlogPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo1 = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            var catInfo2 = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("MyBlog", provider) };
            catInfo2[0].Pages = new string[] { "Test.MyPage1", "Test.MyPage2", "Test.MyPage3" };
            var pageInfo1 = new PageInfo("Test.MyPage1", provider, new DateTime(2010, 1, 1));
            var pageInfo2 = new PageInfo("Test.MyPage2", provider, new DateTime(2010, 1, 2));
            var pageInfo3 = new PageInfo("Test.MyPage3", provider, new DateTime(2010, 1, 3));
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "User", new DateTime(2010, 1, 1), "", "Content 1", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "User", new DateTime(2010, 1, 2), "", "Content 2", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "User", new DateTime(2010, 1, 3), "", "Content 3", null, "My Description 3");

            NamespaceInfo rootNs = null;
            NamespaceInfo testNs = new NamespaceInfo("Test", provider, currentPageInfo);

            host.Expect(x => x.FindNamespace(null)).Return(rootNs);
            host.Expect(x => x.FindNamespace("Test")).Return(testNs);

            //Expect
            provider.Expect(x => x.GetCategories(testNs)).Return(catInfo2);

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo1);
            provider.Expect(x => x.GetMessageCount(null)).IgnoreArguments().Return(3).Repeat.Any();

            host.Expect(x => x.FindPage("Test.MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("Test.MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("Test.MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");
            host.Expect(x => x.FindUser("User")).Repeat.Any().Return(new UserInfo("User", "Garrett", "", true, DateTime.Now, null));

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=3 recent=3 ns=Test} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"Test.MyPage3.ashx\">Page 3</a></h1>"));
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"Test.MyPage2.ashx\">Page 2</a></h1>"));
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"Test.MyPage1.ashx\">Page 1</a></h1>"));
        }

        [Test]
        public void BlogPost_3_Posts_TestNS_BlogRoot()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("Test.BlogPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo1 = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            var catInfo2 = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("MyBlog", provider) };
            catInfo2[0].Pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, new DateTime(2010, 1, 1));
            var pageInfo2 = new PageInfo("MyPage2", provider, new DateTime(2010, 1, 2));
            var pageInfo3 = new PageInfo("MyPage3", provider, new DateTime(2010, 1, 3));
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "User", new DateTime(2010, 1, 1), "", "Content 1", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "User", new DateTime(2010, 1, 2), "", "Content 2", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "User", new DateTime(2010, 1, 3), "", "Content 3", null, "My Description 3");

            NamespaceInfo rootNs = null;
            NamespaceInfo testNs = new NamespaceInfo("Test", provider, currentPageInfo);

            host.Expect(x => x.FindNamespace(null)).Return(rootNs);
            host.Expect(x => x.FindNamespace("Test")).Return(testNs);

            //Expect
            provider.Expect(x => x.GetCategory("MyBlog")).Return(catInfo2[0]);
            provider.Expect(x => x.GetCategories(testNs)).Return(catInfo2);

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo1);
            provider.Expect(x => x.GetMessageCount(null)).IgnoreArguments().Return(3).Repeat.Any();

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");
            host.Expect(x => x.FindUser("User")).Repeat.Any().Return(new UserInfo("User", "Garrett", "", true, DateTime.Now, null));

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=3 recent=3 ns=root} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"MyPage3.ashx\">Page 3</a></h1>"));
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"MyPage2.ashx\">Page 2</a></h1>"));
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"MyPage1.ashx\">Page 1</a></h1>"));
        }

        [Test]
        public void BlogPost_3_Posts_RootNS_BlogRootAndTest()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("BlogPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo1 = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };

            var catInfo2 = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("MyBlog", provider) };
            catInfo2[0].Pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, new DateTime(2010, 1, 1));
            var pageInfo2 = new PageInfo("MyPage2", provider, new DateTime(2010, 1, 2));
            var pageInfo3 = new PageInfo("MyPage3", provider, new DateTime(2010, 1, 3));
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "User", new DateTime(2010, 1, 1), "", "Content 1", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "User", new DateTime(2010, 1, 2), "", "Content 2", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "User", new DateTime(2010, 1, 3), "", "Content 3", null, "My Description 3");

            var catInfo3 = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("MyBlog", provider) };
            catInfo3[0].Pages = new string[] { "Test.MyPage1", "Test.MyPage2", "Test.MyPage3" };
            var pageInfoT1 = new PageInfo("Test.MyPage1", provider, new DateTime(2010, 1, 4));
            var pageInfoT2 = new PageInfo("Test.MyPage2", provider, new DateTime(2010, 1, 5));
            var pageInfoT3 = new PageInfo("Test.MyPage3", provider, new DateTime(2010, 1, 6));
            var pageContentT1 = MockRepository.GenerateStub<PageContent>(pageInfoT1, "Page 1", "User", new DateTime(2010, 1, 4), "", "Content 1", null, "My Description 1");
            var pageContentT2 = MockRepository.GenerateStub<PageContent>(pageInfoT2, "Page 2", "User", new DateTime(2010, 1, 5), "", "Content 2", null, "My Description 2");
            var pageContentT3 = MockRepository.GenerateStub<PageContent>(pageInfoT3, "Page 3", "User", new DateTime(2010, 1, 6), "", "Content 3", null, "My Description 3");

            NamespaceInfo rootNs = null;
            NamespaceInfo testNs = new NamespaceInfo("Test", provider, currentPageInfo);

            host.Expect(x => x.FindNamespace(null)).Return(rootNs);
            host.Expect(x => x.FindNamespace("Test")).Return(testNs);

            //Expect
            provider.Expect(x => x.GetCategory("MyBlog")).Return(catInfo2[0]);
            provider.Expect(x => x.GetCategories(testNs)).Return(catInfo3);

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo1);
            provider.Expect(x => x.GetMessageCount(null)).IgnoreArguments().Return(3).Repeat.Any();

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);
            host.Expect(x => x.FindPage("Test.MyPage1")).Return(pageInfoT1);
            host.Expect(x => x.FindPage("Test.MyPage2")).Return(pageInfoT2);
            host.Expect(x => x.FindPage("Test.MyPage3")).Return(pageInfoT3);
            host.Expect(x => x.GetPageContent(pageInfoT1)).Return(pageContentT1);
            host.Expect(x => x.GetPageContent(pageInfoT2)).Return(pageContentT2);
            host.Expect(x => x.GetPageContent(pageInfoT3)).Return(pageContentT3);

            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");
            host.Expect(x => x.FindUser("User")).Repeat.Any().Return(new UserInfo("User", "Garrett", "", true, DateTime.Now, null));

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=6 recent=6 ns='root,Test'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"Test.MyPage3.ashx\">Page 3</a></h1>"));
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"Test.MyPage2.ashx\">Page 2</a></h1>"));
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"Test.MyPage1.ashx\">Page 1</a></h1>"));
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"MyPage3.ashx\">Page 3</a></h1>"));
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"MyPage2.ashx\">Page 2</a></h1>"));
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"MyPage1.ashx\">Page 1</a></h1>"));
        }

        [Test]
        public void BlogPost_3_Posts_Max_2()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("BlogPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            catInfo[0].Pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, new DateTime(2010, 1, 1));
            var pageInfo2 = new PageInfo("MyPage2", provider, new DateTime(2010, 1, 2));
            var pageInfo3 = new PageInfo("MyPage3", provider, new DateTime(2010, 1, 3));
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "User", new DateTime(2010, 1, 1), "", "Content 1", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "User", new DateTime(2010, 1, 2), "", "Content 2", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "User", new DateTime(2010, 1, 3), "", "Content 3", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo[0]);
            provider.Expect(x => x.GetMessageCount(null)).IgnoreArguments().Return(3).Repeat.Any();

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");
            host.Expect(x => x.FindUser("User")).Repeat.Any().Return(new UserInfo("User", "Garrett", "", true, DateTime.Now, null));

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=2 recent=3} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"MyPage3.ashx\">Page 3</a></h1>"));
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"MyPage2.ashx\">Page 2</a></h1>"));
            Assert.AreEqual(false, retval.Contains("<h1 class=\"blogtitle\"><a href=\"MyPage1.ashx\">Page 1</a></h1>"));
        }

        [Test]
        public void BlogPost_No_Posts()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("BlogPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            catInfo[0].Pages = new string[] { };

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo[0]);
            provider.Expect(x => x.GetMessageCount(null)).IgnoreArguments().Return(3).Repeat.Any();

            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");
            host.Expect(x => x.FindUser("User")).Repeat.Any().Return(new UserInfo("User", "Garrett", "", true, DateTime.Now, null));

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=2 recent=3} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("No posts yet!"));
        }

        [Test]
        public void BlogPost_3_Posts_ShowGravatar()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("BlogPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            catInfo[0].Pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, new DateTime(2010, 1, 1));
            var pageInfo2 = new PageInfo("MyPage2", provider, new DateTime(2010, 1, 2));
            var pageInfo3 = new PageInfo("MyPage3", provider, new DateTime(2010, 1, 3));
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "User", new DateTime(2010, 1, 1), "", "Content 1", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "User", new DateTime(2010, 1, 2), "", "Content 2", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "User", new DateTime(2010, 1, 3), "", "Content 3", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo[0]);
            provider.Expect(x => x.GetMessageCount(null)).IgnoreArguments().Return(3).Repeat.Any();

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");
            host.Expect(x => x.FindUser("User")).Repeat.Any().Return(new UserInfo("User", "Garrett", "", true, DateTime.Now, null));

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=3 recent=3 avatar=true} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogavatartitle\"><a href=\"MyPage1.ashx\">Page 1</a></h1>\n"));
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogavatartitle\"><a href=\"MyPage2.ashx\">Page 2</a></h1>\n"));
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogavatartitle\"><a href=\"MyPage3.ashx\">Page 3</a></h1>\n"));
        }

        [Test]
        public void BlogPost_3_OrderByLastModified()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("BlogPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            catInfo[0].Pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, new DateTime(2010, 1, 1));
            var pageInfo2 = new PageInfo("MyPage2", provider, new DateTime(2010, 1, 2));
            var pageInfo3 = new PageInfo("MyPage3", provider, new DateTime(2010, 1, 3));
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "User", new DateTime(2010, 1, 1), "", "Content 1", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "User", new DateTime(2010, 1, 2), "", "Content 2", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "User", new DateTime(2010, 1, 3), "", "Content 3", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo[0]);
            provider.Expect(x => x.GetMessageCount(null)).IgnoreArguments().Return(3).Repeat.Any();
            provider.Expect(x => x.GetBackupContent(pageInfo1, 0)).Return(pageContent1);
            provider.Expect(x => x.GetBackupContent(pageInfo2, 0)).Return(pageContent2);
            provider.Expect(x => x.GetBackupContent(pageInfo3, 0)).Return(pageContent3);

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");
            host.Expect(x => x.FindUser("User")).Repeat.Any().Return(new UserInfo("User", "Garrett", "", true, DateTime.Now, null));

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=3 recent=3 useMod=true avatar=false cloud=false archive=false} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"MyPage3.ashx\">Page 3</a></h1>"));
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"MyPage2.ashx\">Page 2</a></h1>"));
            Assert.AreEqual(true, retval.Contains("<h1 class=\"blogtitle\"><a href=\"MyPage1.ashx\">Page 1</a></h1>"));
        }

        #region Archive

        [Test]
        public void GenerateArchive_NoPages()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            catInfo[0].Pages = new string[] { };

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetPage(null)).IgnoreArguments().Return(null);

            host.Expect(x => x.GetPageContent(null)).IgnoreArguments().Return(null);
            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=3 recent=3 archive=true} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("No blog entries/pages was found for blog 'MyBlog'"));
            Assert.AreEqual(true, retval.Contains("<div class=\"blogarchive\"><h2><small>Most recent posts</small></h2>\n\n</div>"));
        }

        [Test]
        public void GenerateArchive_1_Page()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("BlogPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            catInfo[0].Pages = new string[] { "MyPage1" };
            var pageInfo1 = new PageInfo("MyPage1", provider, new DateTime(2010, 1, 1));
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "User", new DateTime(2010, 1, 1), "", "Content 1", null, "My Description 1");

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo[0]);
            provider.Expect(x => x.GetMessageCount(null)).IgnoreArguments().Return(3).Repeat.Any();

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);

            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");
            host.Expect(x => x.FindUser("User")).Repeat.Any().Return(new UserInfo("User", "Garrett", "", true, DateTime.Now, null));

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=3 recent=3 archive=true} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("No blog entries/pages was found for blog 'MyBlog'"));
            Assert.AreEqual(true, retval.Contains("<div class=\"blogarchive\"><h2><small>Most recent posts</small></h2>\n \n <p>&nbsp;&nbsp;&nbsp; <a href=\"MyPage1.ashx\">Page 1</a></p>\n</div>"));
        }

        [Test]
        public void GenerateArchive_3_Pages_Max_2()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("BlogPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            catInfo[0].Pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, new DateTime(2010, 1, 1));
            var pageInfo2 = new PageInfo("MyPage2", provider, new DateTime(2010, 1, 2));
            var pageInfo3 = new PageInfo("MyPage3", provider, new DateTime(2010, 1, 3));
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "User", new DateTime(2010, 1, 1), "", "Content 1", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "User", new DateTime(2010, 1, 2), "", "Content 2", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "User", new DateTime(2010, 1, 3), "", "Content 3", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo[0]);
            provider.Expect(x => x.GetMessageCount(null)).IgnoreArguments().Return(3).Repeat.Any();

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");
            host.Expect(x => x.FindUser("User")).Repeat.Any().Return(new UserInfo("User", "Garrett", "", true, DateTime.Now, null));

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=3 recent=2 archive=true} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("No blog entries/pages was found for blog 'MyBlog'"));
            Assert.AreEqual(true, retval.Contains("<div class=\"blogarchive\"><h2><small>Most recent posts</small></h2>\n \n <p>&nbsp;&nbsp;&nbsp; <a href=\"MyPage3.ashx\">Page 3</a></p> \n <p>&nbsp;&nbsp;&nbsp; <a href=\"MyPage2.ashx\">Page 2</a></p>\n</div>"));
        }
        #endregion

        #region Bottom

        [Test]
        public void GenerateBottom_MissingPage()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            catInfo[0].Pages = new string[] { };

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetPage(null)).IgnoreArguments().Return(null);

            host.Expect(x => x.GetPageContent(null)).IgnoreArguments().Return(null);
            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=3 recent=3 bottom='Bottom'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("No blog entries/pages was found for blog 'MyBlog'"));
            Assert.AreEqual(false, retval.Contains("Bottom Content"));
        }
        
        [Test]
        public void GenerateBottom()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            catInfo[0].Pages = new string[] { };

            var aboutPage = new PageInfo("Bottom", provider, new DateTime(2010, 1, 4));
            var aboutContent = MockRepository.GenerateStub<PageContent>(aboutPage, "Bottom Page", "", new DateTime(2010, 1, 4), "", "Bottom Content", null, "My Description");

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetPage("Bottom")).Return(aboutPage);

            host.Expect(x => x.GetPageContent(aboutPage)).Return(aboutContent);
            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=3 recent=3 bottom='Bottom'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("Bottom Content"));
        }

        [Test]
        public void GenerateBottom_SelfReference()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            catInfo[0].Pages = new string[] { };

            var aboutPage = new PageInfo("BottomPage", provider, new DateTime(2010, 1, 4));
            var aboutContent = MockRepository.GenerateStub<PageContent>(aboutPage, "Bottom Page", "", new DateTime(2010, 1, 4), "", "Bottom Content", null, "My Description");

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetPage("BottomPage")).Return(aboutPage);

            host.Expect(x => x.GetPageContent(aboutPage)).Return(aboutContent);
            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=3 recent=3 bottom='MyPage'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("No blog entries/pages was found for blog 'MyBlog'"));
            Assert.AreEqual(false, retval.Contains("Bottom Content"));
        }

        #endregion

        #region Cloud

        [Test]
        public void GenerateCloud_NoKeywords_NoPages()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            catInfo[0].Pages = new string[] { };

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetPage(null)).IgnoreArguments().Return(null);

            host.Expect(x => x.GetPageContent(null)).IgnoreArguments().Return(null);
            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=3 recent=3 cloud=true} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(true, retval.Contains("No blog entries/pages was found for blog 'MyBlog'"));
            Assert.AreEqual(true, retval.Contains("<div class=\"blogcloud\">\nNo keywords found, did you remember to add keywords to your posts?\n</div>\n"));
        }

        [Test]
        public void GenerateCloud_NoKeywords_3_Pages()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            catInfo[0].Pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, new DateTime(2010, 1, 1));
            var pageInfo2 = new PageInfo("MyPage2", provider, new DateTime(2010, 1, 2));
            var pageInfo3 = new PageInfo("MyPage3", provider, new DateTime(2010, 1, 3));
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "User", new DateTime(2010, 1, 1), "", "Content 1", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "User", new DateTime(2010, 1, 2), "", "Content 2", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "User", new DateTime(2010, 1, 3), "", "Content 3", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo[0]);
            provider.Expect(x => x.GetMessageCount(null)).IgnoreArguments().Return(3).Repeat.Any();

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");
            host.Expect(x => x.FindUser("User")).Repeat.Any().Return(new UserInfo("User", "Garrett", "", true, DateTime.Now, null));

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=3 recent=3 cloud=true} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("No blog entries/pages was found for blog 'MyBlog'"));
            Assert.AreEqual(true, retval.Contains("<div class=\"blogcloud\">\nNo keywords found, did you remember to add keywords to your posts?\n</div>\n"));
        }

        [Test]
        public void GenerateCloud_1_Page_2_Words()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            catInfo[0].Pages = new string[] { "MyPage1" };
            var pageInfo1 = new PageInfo("MyPage1", provider, new DateTime(2010, 1, 1));
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "User", new DateTime(2010, 1, 1), "", "Content 1", new string[] { "Word1","Word2" }, "My Description 1");

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo[0]);
            provider.Expect(x => x.GetMessageCount(null)).IgnoreArguments().Return(3).Repeat.Any();

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);

            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");
            host.Expect(x => x.FindUser("User")).Repeat.Any().Return(new UserInfo("User", "Garrett", "", true, DateTime.Now, null));

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=3 recent=3 cloud=true} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("No blog entries/pages was found for blog 'MyBlog'"));
            Assert.AreEqual(true, retval.Contains("<div class=\"blogcloud\">"));
            Assert.AreEqual(true, retval.Contains(">word1<"));
            Assert.AreEqual(true, retval.Contains(">word2<"));
        }

        [Test]
        public void GenerateCloud_2_Pages_2_Identical_Keywords()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var settings = MockRepository.GenerateStub<ISettingsStorageProviderV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            catInfo[0].Pages = new string[] { "MyPage1", "MyPage2" };
            var pageInfo1 = new PageInfo("MyPage1", provider, new DateTime(2010, 1, 1));
            var pageInfo2 = new PageInfo("MyPage2", provider, new DateTime(2010, 1, 2));
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "User", new DateTime(2010, 1, 1), "", "Content 1", new string[] { "WORD1", "WORD2" }, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "User", new DateTime(2010, 1, 2), "", "Content 2", new string[] { "word1", "word2" }, "My Description 2");

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo[0]);
            provider.Expect(x => x.GetMessageCount(null)).IgnoreArguments().Return(3).Repeat.Any();

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);

            host.Expect(x => x.GetSettingsStorageProvider()).Return(settings);
            settings.Expect(x => x.GetSetting("DisplayGravatars")).Return("yes");
            host.Expect(x => x.FindUser("User")).Repeat.Any().Return(new UserInfo("User", "Garrett", "", true, DateTime.Now, null));

            // Blog, NoPosts, NoRecent, lastMod, avatar, cloud,archive,about,bottom,style
            string input = "bla bla bla {Blog cat=MyBlog posts=3 recent=3 cloud=true} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(false, retval.Contains("No blog entries/pages was found for blog 'MyBlog'"));
            Assert.AreEqual(true, retval.Contains("<div class=\"blogcloud\">"));
            Assert.AreEqual(true, retval.Contains(">word1<"));
            Assert.AreEqual(true, retval.Contains(">word2<"));
            Assert.AreEqual(false, retval.Contains(">WORD1<"));
            Assert.AreEqual(false, retval.Contains(">WORD2<"));
        }

        #endregion
    }
}
