using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using ScrewTurn.Wiki.PluginFramework;
using System.Web;
using Keeper.Garrett.ScrewTurn;
using Keeper.Garrett.ScrewTurn.CategoryListFormatter;

namespace UnitTest
{
    [TestFixture]
    [Category("Formatter")]
    public class CategoryListFormatter_Test
    {
        [Test]
        public void PrimitiveList_StraightOrder_List1_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo = MockRepository.GenerateStub<CategoryInfo>("MyCategory", provider);
            var pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, DateTime.Now);
            var pageInfo2 = new PageInfo("MyPage2", provider, DateTime.Now);
            var pageInfo3 = new PageInfo("MyPage3", provider, DateTime.Now);
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "", DateTime.Now, "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "", DateTime.Now, "", "", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo);
            catInfo.Pages = pages;

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {CategoryList(MyCategory,*,false,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [MyPage1|Page 1] \n* [MyPage2|Page 2] \n* [MyPage3|Page 3] \n bla bla bla", retval);
        }

        [Test]
        public void PrimitiveList_StraightOrder_List2_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo = MockRepository.GenerateStub<CategoryInfo>("MyCategory", provider);
            var pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, DateTime.Now);
            var pageInfo2 = new PageInfo("MyPage2", provider, DateTime.Now);
            var pageInfo3 = new PageInfo("MyPage3", provider, DateTime.Now);
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "", DateTime.Now, "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "", DateTime.Now, "", "", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo);
            catInfo.Pages = pages;

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {CategoryList(MyCategory,#,false,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n# [MyPage1|Page 1] \n# [MyPage2|Page 2] \n# [MyPage3|Page 3] \n bla bla bla", retval);
        }

        [Test]
        public void PrimitiveList_MessyOrder_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo = MockRepository.GenerateStub<CategoryInfo>("MyCategory", provider);
            var pages = new string[] { "aaa", "MyPage3", "MyPage0" };
            var pageInfo1 = new PageInfo("aaa", provider, DateTime.Now);
            var pageInfo2 = new PageInfo("MyPage3", provider, DateTime.Now);
            var pageInfo3 = new PageInfo("MyPage0", provider, DateTime.Now);
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "BBB", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 0", "", DateTime.Now, "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "aaa", "", DateTime.Now, "", "", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo);
            catInfo.Pages = pages;

            host.Expect(x => x.FindPage("aaa")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage0")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            string input = "bla bla bla {CategoryList(MyCategory,*,false,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [MyPage0|aaa] \n* [aaa|BBB] \n* [MyPage3|Page 0] \n bla bla bla", retval);
        }

        [Test]
        public void PrimitiveList_StraightOrder_WithDescription_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo = MockRepository.GenerateStub<CategoryInfo>("MyCategory", provider);
            var pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, DateTime.Now);
            var pageInfo2 = new PageInfo("MyPage2", provider, DateTime.Now);
            var pageInfo3 = new PageInfo("MyPage3", provider, DateTime.Now);
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "", DateTime.Now, "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "", DateTime.Now, "", "", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo);
            catInfo.Pages = pages;

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            string input = "bla bla bla {CategoryList(MyCategory,*,true,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [MyPage1|Page 1] - My Description 1 \n* [MyPage2|Page 2] - My Description 2 \n* [MyPage3|Page 3] - My Description 3 \n bla bla bla", retval);
        }

        [Test]
        public void Table_StraightOrder_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo = MockRepository.GenerateStub<CategoryInfo>("MyCategory", provider);
            var pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, DateTime.Now);
            var pageInfo2 = new PageInfo("MyPage2", provider, DateTime.Now);
            var pageInfo3 = new PageInfo("MyPage3", provider, DateTime.Now);
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "", DateTime.Now, "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "", DateTime.Now, "", "", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo);
            catInfo.Pages = pages;

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {CategoryList(MyCategory,,false,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla {|  \n|+  \n! Page name \n|-  \n| [MyPage1|Page 1] \n|-  \n| [MyPage2|Page 2] \n|-  \n| [MyPage3|Page 3] \n|} \n bla bla bla", retval);
        }

        [Test]
        public void Table_MessyOrder_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo = MockRepository.GenerateStub<CategoryInfo>("MyCategory", provider);
            var pages = new string[] { "aaa", "MyPage3", "MyPage0" };
            var pageInfo1 = new PageInfo("aaa", provider, DateTime.Now);
            var pageInfo2 = new PageInfo("MyPage3", provider, DateTime.Now);
            var pageInfo3 = new PageInfo("MyPage0", provider, DateTime.Now);
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "BBB", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 0", "", DateTime.Now, "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "aaa", "", DateTime.Now, "", "", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo);
            catInfo.Pages = pages;

            host.Expect(x => x.FindPage("aaa")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage0")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {CategoryList(MyCategory,,false,,,,,)} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert    
            Assert.AreEqual("bla bla bla {|  \n|+  \n! Page name \n|-  \n| [MyPage0|aaa] \n|-  \n| [aaa|BBB] \n|-  \n| [MyPage3|Page 0] \n|} \n bla bla bla", retval);
        }

        [Test]
        public void Table_StraightOrder_AllFormats_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo = MockRepository.GenerateStub<CategoryInfo>("MyCategory", provider);
            var pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, DateTime.Now);
            var pageInfo2 = new PageInfo("MyPage2", provider, DateTime.Now);
            var pageInfo3 = new PageInfo("MyPage3", provider, DateTime.Now);
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "", DateTime.Now, "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "", DateTime.Now, "", "", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo);
            catInfo.Pages = pages;

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {CategoryList(MyCategory,,false,'My Products','Product Name,Summary','My Table Format','My Head Format','My Row Format')} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla {| My Table Format \n|+ My Products \n|- My Head Format \n| Product Name \n|- My Row Format \n| [MyPage1|Page 1] \n|- My Row Format \n| [MyPage2|Page 2] \n|- My Row Format \n| [MyPage3|Page 3] \n|} \n bla bla bla", retval);
        }

        [Test]
        public void Table_StraightOrder_Style_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo = MockRepository.GenerateStub<CategoryInfo>("MyCategory", provider);
            var pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, DateTime.Now);
            var pageInfo2 = new PageInfo("MyPage2", provider, DateTime.Now);
            var pageInfo3 = new PageInfo("MyPage3", provider, DateTime.Now);
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "", DateTime.Now, "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "", DateTime.Now, "", "", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo);
            catInfo.Pages = pages;

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {CategoryList(MyCategory,,false,'My Products','Product Name,Summary','bw','bw','bw')} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla {| border=\"0\" cellpadding=\"2\" cellspacing=\"0\" align=\"center\" style=\"background-color: #EEEEEE;\" \n|+ My Products \n|- align=\"center\" style=\"background-color: #000000; color: #FFFFFF; font-weight: bold;\" \n| Product Name \n|- align=\"center\" style=\"color: #000000;\" \n| [MyPage1|Page 1] \n|- align=\"center\" style=\"color: #000000;\" \n| [MyPage2|Page 2] \n|- align=\"center\" style=\"color: #000000;\" \n| [MyPage3|Page 3] \n|} \n bla bla bla", retval);
        }
    }
}
