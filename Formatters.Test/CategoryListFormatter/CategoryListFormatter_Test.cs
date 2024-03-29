﻿using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using ScrewTurn.Wiki.PluginFramework;
using System.Web;
using Keeper.Garrett.ScrewTurn;
using Keeper.Garrett.ScrewTurn.CategoryListFormatter;

namespace Formatters.Tests
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
            string input = "bla bla bla {CategoryList cat=MyCategory type=*} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [++MyPage1|Page 1] \n* [++MyPage2|Page 2] \n* [++MyPage3|Page 3] \n bla bla bla", retval);
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
            string input = "bla bla bla {CategoryList cat=MyCategory type=#} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n# [++MyPage1|Page 1] \n# [++MyPage2|Page 2] \n# [++MyPage3|Page 3] \n bla bla bla", retval);
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

            string input = "bla bla bla {CategoryList cat=MyCategory type=*} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [++MyPage0|aaa] \n* [++aaa|BBB] \n* [++MyPage3|Page 0] \n bla bla bla", retval);
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

            string input = "bla bla bla {CategoryList cat=MyCategory type=* cols='pagename,summary'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [++MyPage1|Page 1] - My Description 1 \n* [++MyPage2|Page 2] - My Description 2 \n* [++MyPage3|Page 3] - My Description 3 \n bla bla bla", retval);
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
            string input = "bla bla bla {CategoryList cat=MyCategory type=table} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            AssertTable.VerifyTable(retval, null, null, "", new List<string>() { "Page name" }, new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "[++MyPage1|Page 1]" }},
                {1, new List<string>() { "[++MyPage2|Page 2]" }},
                {2, new List<string>() { "[++MyPage3|Page 3]" }}
            });
        }

        [Test]
        public void Table_StraightOrder_ShowSummary_Test()
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
            string input = "bla bla bla {CategoryList cat=MyCategory type=table cols='pagename,summary'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert         
            AssertTable.VerifyTable(retval, null, null, "", new List<string>() { "Page name", "Summary" }, new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "[++MyPage1|Page 1]", "My Description 1" }},
                {1, new List<string>() { "[++MyPage2|Page 2]", "My Description 2" }},
                {2, new List<string>() { "[++MyPage3|Page 3]", "My Description 3" }}
            });
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
            string input = "bla bla bla {CategoryList cat=MyCategory type=table} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert    
            AssertTable.VerifyTable(retval, null, null, "", new List<string>() { "Page name" }, new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "[++MyPage0|aaa]" }},
                {1, new List<string>() { "[++aaa|BBB]" }},
                {2, new List<string>() { "[++MyPage3|Page 0]" }}
            });
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
            string input = "bla bla bla {CategoryList cat=MyCategory type=table cols='pagename,summary' head='My Products' colnames='Product Name,Summary'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            AssertTable.VerifyTable(retval, null, "My Products", "", new List<string>() { "Product Name", "Summary" }, new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "[++MyPage1|Page 1]", "My Description 1" }},
                {1, new List<string>() { "[++MyPage2|Page 2]", "My Description 2" }},
                {2, new List<string>() { "[++MyPage3|Page 3]", "My Description 3" }}
            }); 
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
            string input = "bla bla bla {CategoryList cat=MyCategory type=table cols=not head='My Products' colnames='Product Name,Summary' style='bw'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            AssertTable.VerifyTable(retval, "bw", "My Products", "", new List<string>() { "Product Name" }, new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "[++MyPage1|Page 1]" }},
                {1, new List<string>() { "[++MyPage2|Page 2]" }},
                {2, new List<string>() { "[++MyPage3|Page 3]" }}
            }); 
        }

        [Test]
        public void Table_StraightOrder_All_Columns_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo = MockRepository.GenerateStub<CategoryInfo>("MyCategory", provider);
            var pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, new DateTime(1980, 4, 27));
            var pageInfo2 = new PageInfo("MyPage2", provider, new DateTime(1980, 4, 27));
            var pageInfo3 = new PageInfo("MyPage3", provider, new DateTime(1980, 4, 27));
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "User", new DateTime(2010, 4, 27), "Comment 1", "Content 1", new string[] { "w1", "w2" }, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "User", new DateTime(2010, 4, 27), "Comment 2", "Content 2", new string[] { "w1", "w2" }, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "User", new DateTime(2010, 4, 27), "Comment 3", "Content 3", new string[] { "w1", "w2" }, "My Description 3");
            pageContent1.LinkedPages = new string[] { "PageLink1" };
            pageContent2.LinkedPages = new string[] { "PageLink1", "PageLink2" };
            pageContent3.LinkedPages = new string[] { "PageLink2", "PageLink3" };
            
            //Expect
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo);
            catInfo.Pages = pages;

            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            provider.Expect(x => x.GetBackupContent(pageInfo1, 0)).Return(pageContent1);
            provider.Expect(x => x.GetBackupContent(pageInfo2, 0)).Return(pageContent2);
            provider.Expect(x => x.GetBackupContent(pageInfo3, 0)).Return(pageContent3);

            host.Expect(x => x.FindUser("User")).Repeat.Any().Return(new UserInfo("User", "Garrett", "", true, DateTime.Now, null));

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {CategoryList cat=MyCategory type=table cols=all colnames=MyColHead head='My Products' style='generic'} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);
            
            //Assert
            AssertTable.VerifyTable(retval, "generic", "My Products", "", new List<string>() { "MyColHead", "Summary", "Keywords", "Last Modified", "Linked Pages", "Created", "Page name", "Last Modified By", "Created By" }, new Dictionary<int, List<string>>()
            {
                //                       "Comment",  "Content",  "Summary",         "Keywords", "Last Modified",      "Linked Pages", "Created",            "Page name",        "Last Modified By",                 "Created By" };
                {0, new List<string>() { "Comment 1","My Description 1","w1,w2",    "27-04-2010 00:00:00","[PageLink1]",             "27-04-1980 00:00:00","[++MyPage1|Page 1]", "[User.aspx?Username=User|Garrett]","[User.aspx?Username=User|Garrett]" }},
                {1, new List<string>() { "Comment 2","My Description 2","w1,w2",    "27-04-2010 00:00:00","[PageLink1],[PageLink2]", "27-04-1980 00:00:00","[++MyPage2|Page 2]", "[User.aspx?Username=User|Garrett]","[User.aspx?Username=User|Garrett]" }},
                {2, new List<string>() { "Comment 3","My Description 3","w1,w2",    "27-04-2010 00:00:00","[PageLink2],[PageLink3]", "27-04-1980 00:00:00","[++MyPage3|Page 3]", "[User.aspx?Username=User|Garrett]","[User.aspx?Username=User|Garrett]" }},
            });
        }

        [Test]
        public void PrimitiveList_StraightOrder_List_RootNS_Test()
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

            host.Expect(x => x.FindNamespace(null)).Return(null);
            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {CategoryList cat=MyCategory type=*} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [++MyPage1|Page 1] \n* [++MyPage2|Page 2] \n* [++MyPage3|Page 3] \n bla bla bla", retval);
        }

        [Test]
        public void PrimitiveList_StraightOrder_List_TestNS_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("Test.MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo1 = MockRepository.GenerateStub<CategoryInfo>("Test.MyCategory1", provider);
            var catInfo2 = MockRepository.GenerateStub<CategoryInfo>("Test.MyCategory2", provider);
            var pages = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pageInfo1 = new PageInfo("MyPage1", provider, DateTime.Now);
            var pageInfo2 = new PageInfo("MyPage2", provider, DateTime.Now);
            var pageInfo3 = new PageInfo("MyPage3", provider, DateTime.Now);
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "", DateTime.Now, "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "", DateTime.Now, "", "", null, "My Description 3");

            //Expect
            provider.Expect(x => x.GetCategories(null)).IgnoreArguments().Return(new CategoryInfo[2] { catInfo2, catInfo1 } );
            catInfo1.Pages = pages;
            catInfo2.Pages = pages;

            host.Expect(x => x.FindNamespace("Test")).Return(new NamespaceInfo("Test",provider,currentPageInfo));
            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1);
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2);
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3);
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1);
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2);
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3);

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {CategoryList cat=MyCategory1 type=*} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [++MyPage1|Page 1] \n* [++MyPage2|Page 2] \n* [++MyPage3|Page 3] \n bla bla bla", retval);
        }


        [Test]
        public void PrimitiveList_StraightOrder_List_RootNS_And_TestNS_FromRoot_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo1 = MockRepository.GenerateStub<CategoryInfo>("MyCategory1", provider);
            var catInfo2 = MockRepository.GenerateStub<CategoryInfo>("Test.MyCategory2", provider);
            var catInfo3 = MockRepository.GenerateStub<CategoryInfo>("Test.MyCategory1", provider);
            var pages1 = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pages2 = new string[] { "MyPage4", "MyPage5" };
            var pageInfo1 = new PageInfo("MyPage1", provider, DateTime.Now);
            var pageInfo2 = new PageInfo("MyPage2", provider, DateTime.Now);
            var pageInfo3 = new PageInfo("MyPage3", provider, DateTime.Now);
            var pageInfo4 = new PageInfo("Test.MyPage4", provider, DateTime.Now);
            var pageInfo5 = new PageInfo("Test.MyPage5", provider, DateTime.Now);
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "", DateTime.Now, "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "", DateTime.Now, "", "", null, "My Description 3");
            var pageContent4 = MockRepository.GenerateStub<PageContent>(pageInfo4, "Page 4", "", DateTime.Now, "", "", null, "My Description 4");
            var pageContent5 = MockRepository.GenerateStub<PageContent>(pageInfo5, "Page 5", "", DateTime.Now, "", "", null, "My Description 5");

            NamespaceInfo rootNs = null;
            NamespaceInfo testNs = new NamespaceInfo("Test", provider, currentPageInfo);

            //Expect
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo1);
            provider.Expect(x => x.GetCategories(testNs)).IgnoreArguments().Return(new CategoryInfo[2] { catInfo2, catInfo3 });
            catInfo1.Pages = pages1;
            catInfo2.Pages = pages2;
            catInfo3.Pages = pages2;

            host.Expect(x => x.FindNamespace(null)).Return(rootNs);
            host.Expect(x => x.FindNamespace("Test")).Return(testNs);
            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1).Repeat.Any();
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2).Repeat.Any();
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3).Repeat.Any();
            host.Expect(x => x.FindPage("MyPage4")).Return(pageInfo4).Repeat.Any();
            host.Expect(x => x.FindPage("MyPage5")).Return(pageInfo5).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo4)).Return(pageContent4).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo5)).Return(pageContent5).Repeat.Any();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {CategoryList ns='root,Test' cat=MyCategory1 type=*} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [++MyPage1|Page 1] \n* [++MyPage2|Page 2] \n* [++MyPage3|Page 3] \n* [++Test.MyPage4|Page 4] \n* [++Test.MyPage5|Page 5] \n bla bla bla", retval);
        }

        [Test]
        public void PrimitiveList_StraightOrder_List_RootNS_And_TestNS_FromTest_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("Test.MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo1 = MockRepository.GenerateStub<CategoryInfo>("MyCategory1", provider);
            var catInfo2 = MockRepository.GenerateStub<CategoryInfo>("Test.MyCategory2", provider);
            var catInfo3 = MockRepository.GenerateStub<CategoryInfo>("Test.MyCategory1", provider);
            var pages1 = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pages2 = new string[] { "MyPage4", "MyPage5" };
            var pageInfo1 = new PageInfo("MyPage1", provider, DateTime.Now);
            var pageInfo2 = new PageInfo("MyPage2", provider, DateTime.Now);
            var pageInfo3 = new PageInfo("MyPage3", provider, DateTime.Now);
            var pageInfo4 = new PageInfo("Test.MyPage4", provider, DateTime.Now);
            var pageInfo5 = new PageInfo("Test.MyPage5", provider, DateTime.Now);
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "", DateTime.Now, "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "", DateTime.Now, "", "", null, "My Description 3");
            var pageContent4 = MockRepository.GenerateStub<PageContent>(pageInfo4, "Page 4", "", DateTime.Now, "", "", null, "My Description 4");
            var pageContent5 = MockRepository.GenerateStub<PageContent>(pageInfo5, "Page 5", "", DateTime.Now, "", "", null, "My Description 5");

            NamespaceInfo rootNs = null;
            NamespaceInfo testNs = new NamespaceInfo("Test", provider, currentPageInfo);

            //Expect
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo1);
            provider.Expect(x => x.GetCategories(testNs)).IgnoreArguments().Return(new CategoryInfo[2] { catInfo2, catInfo3 });
            catInfo1.Pages = pages1;
            catInfo2.Pages = pages2;
            catInfo3.Pages = pages2;

            host.Expect(x => x.FindNamespace(null)).Return(rootNs);
            host.Expect(x => x.FindNamespace("Test")).Return(testNs);
            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1).Repeat.Any();
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2).Repeat.Any();
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3).Repeat.Any();
            host.Expect(x => x.FindPage("MyPage4")).Return(pageInfo4).Repeat.Any();
            host.Expect(x => x.FindPage("MyPage5")).Return(pageInfo5).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo4)).Return(pageContent4).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo5)).Return(pageContent5).Repeat.Any();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {CategoryList ns='root,Test' cat=MyCategory1 type=*} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [++MyPage1|Page 1] \n* [++MyPage2|Page 2] \n* [++MyPage3|Page 3] \n* [++Test.MyPage4|Page 4] \n* [++Test.MyPage5|Page 5] \n bla bla bla", retval);
        }

        [Test]
        public void PrimitiveList_StraightOrder_List_RootNS_And_TestNS_FromTest_Dub_Names_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("Test.MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo1 = MockRepository.GenerateStub<CategoryInfo>("MyCategory1", provider);
            var catInfo2 = MockRepository.GenerateStub<CategoryInfo>("Test.MyCategory2", provider);
            var catInfo3 = MockRepository.GenerateStub<CategoryInfo>("Test.MyCategory1", provider);
            var pages1 = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pages2 = new string[] { "Test.MyPage1", "Test.MyPage2" };
            var pageInfo1 = new PageInfo("MyPage1", provider, DateTime.Now);
            var pageInfo2 = new PageInfo("MyPage2", provider, DateTime.Now);
            var pageInfo3 = new PageInfo("MyPage3", provider, DateTime.Now);
            var pageInfo4 = new PageInfo("Test.MyPage1", provider, DateTime.Now);
            var pageInfo5 = new PageInfo("Test.MyPage2", provider, DateTime.Now);
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "", DateTime.Now, "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "", DateTime.Now, "", "", null, "My Description 3");
            var pageContent4 = MockRepository.GenerateStub<PageContent>(pageInfo4, "Page 1", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent5 = MockRepository.GenerateStub<PageContent>(pageInfo5, "Page 2", "", DateTime.Now, "", "", null, "My Description 2");

            NamespaceInfo rootNs = null;
            NamespaceInfo testNs = new NamespaceInfo("Test", provider, currentPageInfo);

            //Expect
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo1);
            provider.Expect(x => x.GetCategories(testNs)).IgnoreArguments().Return(new CategoryInfo[2] { catInfo2, catInfo3 });
            catInfo1.Pages = pages1;
            catInfo2.Pages = pages2;
            catInfo3.Pages = pages2;

            host.Expect(x => x.FindNamespace(null)).Return(rootNs);
            host.Expect(x => x.FindNamespace("Test")).Return(testNs);
            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1).Repeat.Any();
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2).Repeat.Any();
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3).Repeat.Any();
            host.Expect(x => x.FindPage("Test.MyPage1")).Return(pageInfo4).Repeat.Any();
            host.Expect(x => x.FindPage("Test.MyPage2")).Return(pageInfo5).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo4)).Return(pageContent4).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo5)).Return(pageContent5).Repeat.Any();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {CategoryList ns='root,Test' cat=MyCategory1 type=*} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [++MyPage1|Page 1] \n* [++Test.MyPage1|Page 1] \n* [++MyPage2|Page 2] \n* [++Test.MyPage2|Page 2] \n* [++MyPage3|Page 3] \n bla bla bla", retval);
        }

        [Test]
        public void PrimitiveList_StraightOrder_List_TestNS_Only_FromRoot_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo1 = MockRepository.GenerateStub<CategoryInfo>("MyCategory1", provider);
            var catInfo2 = MockRepository.GenerateStub<CategoryInfo>("Test.MyCategory2", provider);
            var catInfo3 = MockRepository.GenerateStub<CategoryInfo>("Test.MyCategory1", provider);
            var pages1 = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pages2 = new string[] { "MyPage4", "MyPage5" };
            var pageInfo1 = new PageInfo("MyPage1", provider, DateTime.Now);
            var pageInfo2 = new PageInfo("MyPage2", provider, DateTime.Now);
            var pageInfo3 = new PageInfo("MyPage3", provider, DateTime.Now);
            var pageInfo4 = new PageInfo("Test.MyPage4", provider, DateTime.Now);
            var pageInfo5 = new PageInfo("Test.MyPage5", provider, DateTime.Now);
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "", DateTime.Now, "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "", DateTime.Now, "", "", null, "My Description 3");
            var pageContent4 = MockRepository.GenerateStub<PageContent>(pageInfo4, "Page 4", "", DateTime.Now, "", "", null, "My Description 4");
            var pageContent5 = MockRepository.GenerateStub<PageContent>(pageInfo5, "Page 5", "", DateTime.Now, "", "", null, "My Description 5");

            NamespaceInfo rootNs = null;
            NamespaceInfo testNs = new NamespaceInfo("Test", provider, currentPageInfo);

            //Expect
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo1);
            provider.Expect(x => x.GetCategories(testNs)).IgnoreArguments().Return(new CategoryInfo[2] { catInfo2, catInfo3 });
            catInfo1.Pages = pages1;
            catInfo2.Pages = pages2;
            catInfo3.Pages = pages2;

            host.Expect(x => x.FindNamespace(null)).Return(rootNs);
            host.Expect(x => x.FindNamespace("Test")).Return(testNs);
            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1).Repeat.Any();
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2).Repeat.Any();
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3).Repeat.Any();
            host.Expect(x => x.FindPage("MyPage4")).Return(pageInfo4).Repeat.Any();
            host.Expect(x => x.FindPage("MyPage5")).Return(pageInfo5).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo4)).Return(pageContent4).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo5)).Return(pageContent5).Repeat.Any();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {CategoryList ns='Test' cat=MyCategory1 type=*} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [++Test.MyPage4|Page 4] \n* [++Test.MyPage5|Page 5] \n bla bla bla", retval);
        }

        [Test]
        public void PrimitiveList_StraightOrder_List_RootNS_Only_FromTest_Test()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();
            var currentPageInfo = new PageInfo("Test.MyPage", provider, DateTime.Now);
            var context = new ContextInformation(false, false, FormattingContext.PageContent, currentPageInfo, "", HttpContext.Current, "", new string[] { "" });  //MockRepository.GenerateStub<ContextInformation>();

            var catInfo1 = MockRepository.GenerateStub<CategoryInfo>("MyCategory1", provider);
            var catInfo2 = MockRepository.GenerateStub<CategoryInfo>("MyCategory2", provider);
            var catInfo3 = MockRepository.GenerateStub<CategoryInfo>("MyCategory1", provider);
            var pages1 = new string[] { "MyPage1", "MyPage2", "MyPage3" };
            var pages2 = new string[] { "MyPage4", "MyPage5" };
            var pageInfo1 = new PageInfo("MyPage1", provider, DateTime.Now);
            var pageInfo2 = new PageInfo("MyPage2", provider, DateTime.Now);
            var pageInfo3 = new PageInfo("MyPage3", provider, DateTime.Now);
            var pageInfo4 = new PageInfo("Test.MyPage4", provider, DateTime.Now);
            var pageInfo5 = new PageInfo("Test.MyPage5", provider, DateTime.Now);
            var pageContent1 = MockRepository.GenerateStub<PageContent>(pageInfo1, "Page 1", "", DateTime.Now, "", "", null, "My Description 1");
            var pageContent2 = MockRepository.GenerateStub<PageContent>(pageInfo2, "Page 2", "", DateTime.Now, "", "", null, "My Description 2");
            var pageContent3 = MockRepository.GenerateStub<PageContent>(pageInfo3, "Page 3", "", DateTime.Now, "", "", null, "My Description 3");
            var pageContent4 = MockRepository.GenerateStub<PageContent>(pageInfo4, "Page 4", "", DateTime.Now, "", "", null, "My Description 4");
            var pageContent5 = MockRepository.GenerateStub<PageContent>(pageInfo5, "Page 5", "", DateTime.Now, "", "", null, "My Description 5");

            NamespaceInfo rootNs = null;
            NamespaceInfo testNs = new NamespaceInfo("Test", provider, currentPageInfo);

            //Expect
            provider.Expect(x => x.GetCategory(null)).IgnoreArguments().Return(catInfo1);
            provider.Expect(x => x.GetCategories(testNs)).IgnoreArguments().Return(new CategoryInfo[2] { catInfo2, catInfo3 });
            catInfo1.Pages = pages1;
            catInfo2.Pages = pages2;
            catInfo3.Pages = pages2;

            host.Expect(x => x.FindNamespace(null)).Return(rootNs);
            host.Expect(x => x.FindNamespace("Test")).Return(testNs);
            host.Expect(x => x.FindPage("MyPage1")).Return(pageInfo1).Repeat.Any();
            host.Expect(x => x.FindPage("MyPage2")).Return(pageInfo2).Repeat.Any();
            host.Expect(x => x.FindPage("MyPage3")).Return(pageInfo3).Repeat.Any();
            host.Expect(x => x.FindPage("MyPage4")).Return(pageInfo4).Repeat.Any();
            host.Expect(x => x.FindPage("MyPage5")).Return(pageInfo5).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo1)).Return(pageContent1).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo2)).Return(pageContent2).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo3)).Return(pageContent3).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo4)).Return(pageContent4).Repeat.Any();
            host.Expect(x => x.GetPageContent(pageInfo5)).Return(pageContent5).Repeat.Any();

            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            // Category,output,include,head,headers,tbl,head,row
            string input = "bla bla bla {CategoryList ns='root' cat=MyCategory1 type=*} bla bla bla";

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual("bla bla bla  \n* [++MyPage1|Page 1] \n* [++MyPage2|Page 2] \n* [++MyPage3|Page 3] \n bla bla bla", retval);
        }
    }
}
