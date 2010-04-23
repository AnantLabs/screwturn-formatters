using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.Core;
using System.Text.RegularExpressions;
using ScrewTurn.Wiki.PluginFramework;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Linq;
using Keeper.Garrett.ScrewTurn.Utility;

namespace Keeper.Garrett.ScrewTurn.BlogFormatter
{
    public class BlogFormatter : FormatterBase
    {
        public override bool PerformPhase1 { get { return true; } }
        public override bool PerformPhase2 { get { return false; } }
        public override bool PerformPhase3 { get { return false; } }

        //Tag format {BlogPosts(Category)}
        // Category,noPosts,useLastMod,showCloud,showArchive,about,bottom,style  
        private static readonly Regex TagRegex = new Regex(@"\{Blog\((?<blog>(.*?)),(?<noOfPostsToShow>(.*?)),(?<noOfRecentPostsToShow>(.*?)),(?<useLastModified>(.*?)),((?<showCloud>(.*?)))?,((?<showArchive>(.*?)))?,('(?<aboutPage>(.*?))')?,('(?<bottomPage>(.*?))')?,((?<stylesheet>(.*?)))?\)\}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        private static Random m_Random = new Random();

        public override void Init(IHostV30 _host, string _config)
        {
            base.Init(_host, _config, 55, Help.HelpPages);

            LogEntry("BlogFormatter - Init success", LogEntryType.General);
        }

        public override string Format(string raw, ContextInformation context, FormattingPhase phase)
        {
            try
            {
                if(    context.Context == FormattingContext.PageContent
                    && context.ForIndexing == false
                    && context.ForWysiwyg == false)
                {
                    switch (phase)
                    {
                        case FormattingPhase.Phase1:
                            var matches = TagRegex.Matches(raw);
                            if (matches != null && matches.Count > 0)
                            {
                                //Get current provider
                                var provider = context.Page.Provider;
                                
                                //Foreach blog 
                                foreach (Match match in matches)
                                {
                                    string blog = string.Empty;
                                    int noOfPostsToShow = 7;
                                    int noOfRecentPostsToShow = 15;
                                    bool useLastModified = false;
                                    bool showCloud = true;
                                    bool showArchive = true;
                                    string stylesheet = string.Empty;
                                    string about = string.Empty;
                                    string bottom = string.Empty;
                                    PageContent aboutPage = null;
                                    PageContent bottomPage = null;

                                    //Get params
                                    blog = (string.IsNullOrEmpty(match.Groups["blog"].Value) == true ? "" : match.Groups["blog"].Value);
                                    int.TryParse(match.Groups["noOfPostsToShow"].Value, out noOfPostsToShow);
                                    int.TryParse(match.Groups["noOfRecentPostsToShow"].Value, out noOfRecentPostsToShow);
                                    bool.TryParse(match.Groups["useLastModified"].Value, out useLastModified);
                                    bool.TryParse(match.Groups["showCloud"].Value, out showCloud);
                                    bool.TryParse(match.Groups["showArchive"].Value, out showArchive);
                                    about = (string.IsNullOrEmpty(match.Groups["aboutPage"].Value) == true ? null : match.Groups["aboutPage"].Value);
                                    bottom = (string.IsNullOrEmpty(match.Groups["bottomPage"].Value) == true ? null : match.Groups["bottomPage"].Value);

                                    //Get style
                                    stylesheet = (string.IsNullOrEmpty(match.Groups["stylesheet"].Value) == true ? "BlogDefault.css" : match.Groups["stylesheet"].Value);
                                    stylesheet = string.Format("<link type=\"text/css\" rel=\"stylesheet\" href=\"/Themes/Blog/{0}\"></link> ", stylesheet);

                                    //Security check that the page with the {Blog} tag itself do not have the category Blog.
                                    bool abortToAvoidSelfReferencing = false;
                                    var categories = provider.GetCategoriesForPage(context.Page);
                                    foreach (var category in categories)
                                    {
                                        if (category.FullName == blog)
                                        {
                                            abortToAvoidSelfReferencing = true;
                                        }
                                    }

                                    //Check additional pages are not the parent
                                    if (    context.Page.FullName == about
                                        ||  context.Page.FullName == bottom)
                                    {
                                        abortToAvoidSelfReferencing = true;
                                    }

                                    //Get posts
                                    var sortedPosts = new SortedList<DateTime, BlogPostInfo>(new ReverseDateComparer());

                                    if (string.IsNullOrEmpty(blog) == false 
                                        && provider != null 
                                        && abortToAvoidSelfReferencing == false)
                                    {
                                        var catInfo = provider.GetCategory(blog);
                                        if (catInfo != null)
                                        {
                                            foreach (var page in catInfo.Pages)
                                            {
                                                var pageInfo = m_Host.FindPage(page);
                                                if (pageInfo != null)
                                                {
                                                    var content = m_Host.GetPageContent(pageInfo);
                                                    
                                                    //Build dict
                                                    if (content != null)
                                                    {
                                                        var info = new BlogPostInfo() 
                                                        { 
                                                            Content = content, 
                                                            NoOfComments = provider.GetMessageCount(pageInfo),
                                                            UserName = content.User,
                                                            UserDisplayName = m_Host.FindUser(content.User).DisplayName
                                                        };

                                                        //Sort method
                                                        if (useLastModified == false)
                                                        {
                                                            sortedPosts.Add(content.PageInfo.CreationDateTime, info);
                                                        }
                                                        else
                                                        {
                                                            sortedPosts.Add(content.LastModified, info);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    //Get about if specified
                                    if (string.IsNullOrEmpty(about) == false && abortToAvoidSelfReferencing == false)
                                    {
                                        var pInfo = provider.GetPage(about);
                                        aboutPage = m_Host.GetPageContent(pInfo);
                                    }

                                    //Get bottom if specified
                                    if (string.IsNullOrEmpty(bottom) == false && abortToAvoidSelfReferencing == false)
                                    {
                                        var pInfo = provider.GetPage(bottom);
                                        bottomPage = m_Host.GetPageContent(pInfo);
                                    }

                                    
                                    //Create output
                                    var pageContent = GeneratePage(blog, sortedPosts, noOfPostsToShow, noOfRecentPostsToShow, showCloud, showArchive, aboutPage, bottomPage, stylesheet);

                                    //Add a final newline
                                    pageContent = string.Format("{0} \n", pageContent);

                                    //Insert list
                                    //Recall position as string may allready have been modified by other table match entry
                                    int pos = TagRegex.Match(raw).Index;
                                    int length = TagRegex.Match(raw).Length;
                                    raw = raw.Remove(pos, length);
                                    raw = raw.Insert(pos, pageContent);

                                }
                            }
                            break;
                        case FormattingPhase.Phase2:
                            break;
                        case FormattingPhase.Phase3:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                LogEntry(string.Format("BlogFormatter error: {0} {1}", e.Message, e.StackTrace), LogEntryType.Error);
            }

            return raw;
        }
        
        private string GeneratePage(string _blog, SortedList<DateTime, BlogPostInfo> _pages, int _noOfPostsToShow, int _noOfRecentPostsToShow, bool _showCloud, bool _showArchive, PageContent _aboutPage, PageContent _bottomPage, string _stylesheet)
        {
            //Create output
            string list = string.Empty;

            //Generate Posts
            list = GeneratePosts(_blog, _pages, _noOfPostsToShow, _stylesheet);

            list = string.Format("{0} \n <div id=\"blogsidebar\">",list);

            //Generate About
            if (_aboutPage != null)
            {
                list = string.Format("{0} \n {1} <br/><br/><br/>", list, GenerateAbout(_aboutPage, _stylesheet));
            }

            //Generate Keyword Cloud
            if (_showCloud == true)
            {
                list = string.Format("{0} \n {1} <br/><br/><br/>", list, GenerateCloud(_pages, _blog, _stylesheet));
            }

            //Generate Archive
            if (_showArchive == true)
            {
                list = string.Format("{0} \n {1} <br/><br/><br/>", list, GenerateArchive(_blog, _pages, _noOfRecentPostsToShow, _stylesheet));
            }

            //Generate bottom
            if (_bottomPage != null)
            {
                list = string.Format("{0} \n {1} <br/><br/><br/>", list, GenerateBottom(_bottomPage, _stylesheet));
            }

            list = string.Format("{0}</div>\n", list);

            return list;
        }

        private string GeneratePosts(string _blog, SortedList<DateTime, BlogPostInfo> _posts, int _noOfPostsToShow, string _stylesheet)
        {
            var retval = string.Empty;

            //Generate each posts
            if (_posts.Count > 0)
            {
                foreach (var entry in _posts)
                {
                    if (_noOfPostsToShow <= 0)
                    {
                        break;
                    }

                    retval = string.Format("{0} \n {1} ", retval, string.Format("<div class=\"blogpost\">\n"
                                                                                    + "<h1 class=\"blogtitle\"><a href=\"{0}.ashx\">{1}</a></h1>\n"
                                                                                    + "<p class=\"blogbyline\"><small>Posted on {2} by <a href=\"User.aspx?Username={3}\">{4}</a> | <a href=\"Edit.aspx?Page={5}\">Edit</a></small></p>\n"
                                                                                    + "<div class=\"blogentry\">\n"
                                                                                    + "{6}\n"
                                                                                    + "</div>\n"
                                                                                    + "<p class=\"blogmeta\"><a href=\"{7}.ashx\" class=\"blogmore\">Go to page</a> &nbsp;&nbsp;&nbsp; <a href=\"{8}.ashx?Discuss=1\" class=\"blogcomments\">Comments ({9})</a></p>\n"
                                                                                + "</div>\n"
                                                                            , entry.Value.Content.PageInfo.FullName             //Titlelink
                                                                            , entry.Value.Content.Title                         //Title
                                                                            , entry.Key.ToString("dd MMMM, yyyy")               //Date
                                                                            , entry.Value.UserName                              //UserName
                                                                            , entry.Value.UserDisplayName                       //UserDisplayname
                                                                            , entry.Value.Content.PageInfo.FullName             //Edit link
                                                                            , entry.Value.Content.Content                       //Content
                                                                            , entry.Value.Content.PageInfo.FullName             //Go to page
                                                                            , entry.Value.Content.PageInfo.FullName             //Discussion
                                                                            , entry.Value.NoOfComments                          //No of comments
                                                                            ));

                    _noOfPostsToShow--;
                }
            }

            //Generate default post if no posts yet
            if (string.IsNullOrEmpty(retval) == true)
            {
                retval = string.Format("<div class=\"blogpost\">\n"
                                + "<h1 class=\"blogtitle\">No posts yet!</h1>\n"
                                + "<p class=\"blogbyline\"><small>Posted on {0} by System</small></p>\n"
                                + "<div class=\"blogentry\">\nNo blog entries/pages was found for blog '{1}' or one of the pages (this,about,footer) also has the category '{1}' or they refer directly to this page.\nAvoid self referencing.\n\n Consult the [BlogFormatterHelp|help pages for more information].\n\n\n</div>\n"
                                + "<p class=\"blogmeta\"><a href=\"BlogFormatterHelp.ashx\" class=\"blogmore\">Read More</a></p>\n"
                                + "</div>\n", DateTime.Now.ToString("MMMM dd, yyyy"), _blog);
            }

            //Wrap all in stylesheet + div
            retval = string.Format("{0}{1}{2}{3}", "<div id=\"blogcontent\">\n", _stylesheet, retval, "</div>\n");

            return retval;
        }


        private string GenerateAbout(PageContent _page, string _stylesheet)
        {
            //Generate HTML
            var retval = string.Format("{0}\n"
                                + "<div id=\"blogaboutcontent\">\n"
                                    + "<h1 class=\"blogabouttitle\"><a href=\"{1}.ashx\">{2}</a></h1>\n"
                                    + "<div class=\"blogabout\">\n"
                                    + "{3}\n"
                                    + "</div>\n"
                                + "</div>\n"
                                , _stylesheet
                                , _page.PageInfo.FullName
                                , _page.Title
                                , _page.Content
                           );

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_pages"></param>
        /// <param name="_stylesheet"></param>
        /// <returns></returns>
        private string GenerateCloud(SortedList<DateTime, BlogPostInfo> _pages, string _blog, string _stylesheet)
        {
            //Create dict of unique keywords + ass. search url string
            var keywordsDict = new SortedDictionary<string, string>();
            foreach (var entry in _pages)
            {
                if (entry.Value.Content.Keywords != null)
                {
                    foreach (var word in entry.Value.Content.Keywords)
                    {
                        var key = word.ToLower();

                        if (keywordsDict.ContainsKey(key) == false)
                        {
                            keywordsDict.Add(key, BuildSearchUrl(key, _blog));
                        }
                    }
                }
            }

            //Generate HTML
            var words = "";
            foreach (var entry in keywordsDict)
            {
                words = string.Format("{0} {1}", words, entry.Value);
            }

            var retval = string.Format("{0}\n"
                                + "<div id=\"blogcloudcontent\">\n"
                                    + "<h1 class=\"blogcloudtitle\">keyword cloud</h1>\n"
                                    + "<div class=\"blogcloud\">\n"
                                    + "{1}\n"
                                    + "</div>\n"
                                + "</div>\n"
                                , _stylesheet
                                , (string.IsNullOrEmpty(words) == true ? "No keywords found, did you remember to add keywords to your posts?" : words)
                           );

            return retval;
        }

        /// <summary>
        /// Search url: Search.aspx?Query=ice&SearchUncategorized=0&Categories=Blog,&Mode=1&FilesAndAttachments=1
        /// </summary>
        /// <param name="_keyword"></param>
        /// <param name="_blog"></param>
        /// <returns></returns>
        private string BuildSearchUrl(string _keyword, string _blog)
        {
            return string.Format("<a href=\"Search.aspx?Query={0}&SearchUncategorized=0&Categories={1},&Mode=1&FilesAndAttachments=1\" style=\"font-size: {2}px;\">{3}</a>", _keyword, _blog, m_Random.Next(8, 30), _keyword);
        }

        private string GenerateArchive(string _blog, SortedList<DateTime, BlogPostInfo> _pages, int _noOfRecentPostsToShow, string _stylesheet)
        {
            string pageLinks = string.Empty;
            int noOfPosts = 0;
            foreach (var entry in _pages)
            {
                pageLinks = string.Format("{0} \n <p>&nbsp;&nbsp;&nbsp; <a href=\"{1}.ashx\">{2}</a></p>", pageLinks, entry.Value.Content.PageInfo.FullName, entry.Value.Content.Title);
                noOfPosts++;

                if (noOfPosts >= _noOfRecentPostsToShow)
                {
                    break;
                }
            }

            //Generate HTML
            var retval = string.Format("{0}\n"
                                + "<div id=\"blogarchivecontent\">\n"
                                    + "<h1 class=\"blogarchivetitle\"><a href=\"AllPages.aspx?SortBy=Creation&Reverse=1&Cat={1}&Page=0\">archive</a></h1>\n"
                                    + "<div class=\"blogarchive\"><h2><small>Most recent posts</small></h2>\n"
                                    + "{2}\n"
                                    + "</div>\n"
                                + "</div>\n"
                                , _stylesheet
                                , _blog
                                , pageLinks
                           );

            return retval;
        }

        private string GenerateBottom(PageContent _page, string _stylesheet)
        {
            //Generate HTML
            var retval = string.Format("{0}\n"
                                + "<div id=\"blogbottomcontent\">\n"
                                    + "<h1 class=\"blogbottomtitle\"><a href=\"{1}.ashx\">{2}</a></h1>\n"
                                    + "<div class=\"blogbottom\">\n"
                                    + "{3}\n"
                                    + "</div>\n"
                                + "</div>\n"
                                , _stylesheet
                                , _page.PageInfo.FullName
                                , _page.Title
                                , _page.Content
                           );

            return retval;
        }
    }
}
