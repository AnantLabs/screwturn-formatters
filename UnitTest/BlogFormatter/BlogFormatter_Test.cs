using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using ScrewTurn.Wiki.PluginFramework;
using System.Web;
using Keeper.Garrett.ScrewTurn;
using Keeper.Garrett.ScrewTurn.BlogFormatter;

namespace UnitTest
{
    /// <summary>
    /// Category,noPosts,useLastMod,showCloud,showArchive,about,bottom,style  
    /// </summary>
    [TestFixture]
    [Category("Formatter")]
    public class BlogFormatter_Test
    {
        [Test]
        public void GenerateAbout_MissingPage()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            catInfo[0].Pages = new string[] { };

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetPage(null)).IgnoreArguments().Return(null);

            host.Expect(x => x.GetPageContent(null)).IgnoreArguments().Return(null);

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {Blog(MyBlog,3,false,false,false,'About',,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla <div id=\"blogcontent\">\n<link type=\"text/css\" rel=\"stylesheet\" href=\"/Themes/Blog/BlogDefault.css\"></link> <div class=\"blogpost\">\n<h1 class=\"blogtitle\">No posts yet!</h1>\n<p class=\"blogbyline\"><small>Posted on " + DateTime.Now.ToString("MMMM dd, yyyy") + " by System | <a href=\"#\">Edit</a></small></p>\n<div class=\"blogentry\">\nNo blog entries/pages was found for blog 'MyBlog' or one of the pages (this,about,footer) also has the category 'MyBlog' or they refer directly to this page.\nAvoid self referencing.\n\n Consult the [BlogFormatterHelp|help pages for more information].\n\n\n</div>\n<p class=\"blogmeta\"><a href=\"BlogFormatterHelp.ashx\" class=\"blogmore\">Read More</a></p>\n</div>\n</div>\n \n <div id=\"blogsidebar\"></div>\n \n bla bla bla", retval);
        }

        [Test]
        public void GenerateAbout()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            catInfo[0].Pages = new string[] { };

            var aboutPage = new PageInfo("About", provider, new DateTime(2010, 1, 4));
            var aboutContent = MockRepository.GenerateStub<PageContent>(aboutPage, "About Page", "", new DateTime(2010, 1, 4), "", "", null, "My Description");

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetPage("About")).Return(aboutPage);

            host.Expect(x => x.GetPageContent(aboutPage)).Return(aboutContent);

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {Blog(MyBlog,3,false,false,false,'About',,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla <div id=\"blogcontent\">\n<link type=\"text/css\" rel=\"stylesheet\" href=\"/Themes/Blog/BlogDefault.css\"></link> <div class=\"blogpost\">\n<h1 class=\"blogtitle\">No posts yet!</h1>\n<p class=\"blogbyline\"><small>Posted on " + DateTime.Now.ToString("MMMM dd, yyyy") + " by System | <a href=\"#\">Edit</a></small></p>\n<div class=\"blogentry\">\nNo blog entries/pages was found for blog 'MyBlog' or one of the pages (this,about,footer) also has the category 'MyBlog' or they refer directly to this page.\nAvoid self referencing.\n\n Consult the [BlogFormatterHelp|help pages for more information].\n\n\n</div>\n<p class=\"blogmeta\"><a href=\"BlogFormatterHelp.ashx\" class=\"blogmore\">Read More</a></p>\n</div>\n</div>\n \n <div id=\"blogsidebar\"> \n <link type=\"text/css\" rel=\"stylesheet\" href=\"/Themes/Blog/BlogDefault.css\"></link> \n<div id=\"blogaboutcontent\">\n<h1 class=\"blogabouttitle\"><a href=\"About.ashx\">About Page</a></h1>\n<div class=\"blogabout\">\n\n</div>\n</div>\n <br/><br/><br/></div>\n \n bla bla bla", retval);
        }


    /*    [Test]
        public void GenerateAbout()
        {
            //Arrange
            var formatter = new BlogFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });

            var catInfo = new CategoryInfo[1] { MockRepository.GenerateStub<CategoryInfo>("Blog", provider) };
            var pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, new DateTime(2010, 1, 1));
            var pageInfo2 = new PageInfo("MyPage2", provider, new DateTime(2010, 1, 2));
            var pageInfo3 = new PageInfo("MyPage3", provider, new DateTime(2010, 1, 3));
            var pageInfo4 = new PageInfo("About", provider, new DateTime(2010, 1, 4));
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "", new DateTime(2010, 1, 1), "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "", new DateTime(2010, 1, 2), "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "", new DateTime(2010, 1, 3), "", "", null, "My Description 3");
            var pageContent4 = MockRepository.GenerateStub<PageContent>(pageInfo4, "Page 4", "", new DateTime(2010, 1, 4), "", "", null, "My Description 4");
            catInfo[0].Pages = pages;

            //Expect
            provider.Expect(x => x.GetCategoriesForPage(null)).IgnoreArguments().Return(catInfo);
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo[0]);
            provider.Expect(x => x.GetMessageCount(null)).IgnoreArguments().Return(3).Repeat.Any();
            provider.Expect(x => x.GetPage("About")).Return(pageInfo4);

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);
            host.Expect(x => x.GetPageContent(pageInfo4)).Return(pageContent4);

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {Blog(MyBlog,3,false,false,false,'About',,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [MyPage1|Page 1] \n* [MyPage2|Page 2] \n* [MyPage3|Page 3] \n bla bla bla", retval);
        }*/
    }
}
