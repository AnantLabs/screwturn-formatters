﻿using System;
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
using ScrewTurn.Wiki;

namespace Keeper.Garrett.ScrewTurn.BlogFormatter
{
    public class BlogFormatter : FormatterBase
    {
        public override bool PerformPhase1 { get { return true; } }
        public override bool PerformPhase2 { get { return false; } }
        public override bool PerformPhase3 { get { return false; } }

        //Tag format {BlogPosts(Category)}
        // Category,noPosts,useLastMod,showCloud,showArchive,about,bottom,style  
        private static readonly Regex TagRegex = new Regex(@"\{Blog(?<arguments>(.*?))\}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        private static Random m_Random = new Random();
        private static string m_DateTimeFormat = "";

        public override void Init(IHostV30 _host, string _config)
        {
            base.Init(_host, _config, Help.HelpPages);

            StoreFiles(_host);

            LogEntry("BlogFormatter - Init success", LogEntryType.General);
        }

        private void StoreFiles(IHostV30 _host)
        {
            try
            {
                var persister = new FilePersister("BlogFormatter");
                persister.AddDir("Images");
                persister.AddFile("/", "BlogStyle.css",               "Keeper.Garrett.ScrewTurn.BlogFormatter.Resources.BlogStyle.css");
                persister.AddFile("Images", "BlogPostBackground.jpg", "Keeper.Garrett.ScrewTurn.BlogFormatter.Resources.Images.BlogPostBackground.jpg");
                persister.AddFile("Images", "BlogPostComments.gif",   "Keeper.Garrett.ScrewTurn.BlogFormatter.Resources.Images.BlogPostComments.gif");
                persister.AddFile("Images", "BlogPostReadMore.gif",   "Keeper.Garrett.ScrewTurn.BlogFormatter.Resources.Images.BlogPostReadMore.gif");
                persister.StoreFiles(_host);
            }
            catch (Exception e)
            {
                LogEntry(string.Format("BlogFormatter - StoreFiles - Error: {0}", e.Message), LogEntryType.Error);
            }
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
                                    bool showAvatar = false;
                                    bool gravatarsEnabled = false;
                                    bool showCloud = false;
                                    bool showArchive = false;
                                    string stylesheet = string.Empty;
                                    string about = string.Empty;
                                    string bottom = string.Empty;
                                    PageContent aboutPage = null;
                                    PageContent bottomPage = null;

                                    var args = new ArgumentParser().Parse(match.Groups["arguments"].Value);

                                    //Get params 
                                    blog = (args.ContainsKey("cat") == true ? args["cat"] : "");

                                    var ns = (args.ContainsKey("ns") == true ? args["ns"] : "");

                                    int.TryParse( (args.ContainsKey("posts") == true ? args["posts"] : "7"), out noOfPostsToShow);
                                    noOfPostsToShow = (noOfPostsToShow <= 0 ? 7 : noOfPostsToShow);

                                    int.TryParse((args.ContainsKey("recent") == true ? args["recent"] : "15"), out noOfRecentPostsToShow);
                                    noOfRecentPostsToShow = (noOfRecentPostsToShow <= 0 ? 15 : noOfRecentPostsToShow);

                                    bool.TryParse((args.ContainsKey("usemod") == true ? args["usemod"] : "false"), out useLastModified);
                                    bool.TryParse((args.ContainsKey("cloud") == true ? args["cloud"] : "false"), out showCloud);
                                    bool.TryParse((args.ContainsKey("archive") == true ? args["archive"] : "false"), out showArchive);
                                    bool.TryParse((args.ContainsKey("avatar") == true ? args["avatar"] : "false"), out showAvatar);
                                    gravatarsEnabled = (m_Host.GetSettingsStorageProvider().GetSetting("DisplayGravatars").ToLower() == "yes" ? true : false);
                                    showAvatar = (gravatarsEnabled == true && showAvatar == true ? true : false);
                                    m_DateTimeFormat = m_Host.GetSettingValue(SettingName.DateTimeFormat); //Update datetime format

                                    about = (args.ContainsKey("about") == true ? args["about"] : null);
                                    bottom = (args.ContainsKey("bottom") == true ? args["bottom"] : null);

                                    //Get style
                                    stylesheet = (args.ContainsKey("css") == true ? args["css"] : "BlogStyle.css");
                                    stylesheet = string.Format("<link type=\"text/css\" rel=\"stylesheet\" href=\"GetFile.aspx?File=/Keeper.Garrett.Formatters/BlogFormatter/{0}\"></link> ", stylesheet);

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
                                        var currentNs = NameTools.GetNamespace(context.Page.FullName);
                                        var catInfos = CategoryTools.GetCategoryInformation(m_Host,provider, blog, currentNs, ns);

                                        foreach(var catInfo in catInfos)
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
                                                        //Find user
                                                        var postAuthor = new UserInfo(null, null, null, false, DateTime.Now, null);
                                                        string user = string.Empty;

                                                        if (useLastModified == false)
                                                        {
                                                            //Verify first version of page and user who created it
                                                            var cnt = provider.GetBackupContent(pageInfo, 0);
                                                            user = (cnt != null ? cnt.User : string.Empty); 

                                                        }
                                                        
                                                        //No user found? force last mod user
                                                        if(string.IsNullOrEmpty(user) == true)
                                                        {
                                                            user = content.User;
                                                        }

                                                        postAuthor = m_Host.FindUser(user);

                                                        //Create post to generate html from
                                                        var info = new BlogPostInfo() 
                                                        { 
                                                            Content = content, 
                                                            NoOfComments = provider.GetMessageCount(pageInfo),
                                                            UserName = content.User,
                                                            UserDisplayName = postAuthor.DisplayName,
                                                            UserGravatar = Avatar.GenerateAvatarLink(postAuthor.Email),
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
                                        if (pInfo != null)
                                        {
                                            aboutPage = m_Host.GetPageContent(pInfo);
                                        }
                                    }

                                    //Get bottom if specified
                                    if (string.IsNullOrEmpty(bottom) == false && abortToAvoidSelfReferencing == false)
                                    {
                                        var pInfo = provider.GetPage(bottom);
                                        if (pInfo != null)
                                        {
                                            bottomPage = m_Host.GetPageContent(pInfo);
                                        }
                                    }

                                    
                                    //Create output
                                    var pageContent = GeneratePage(blog, sortedPosts, noOfPostsToShow, noOfRecentPostsToShow, showAvatar, showCloud, showArchive, aboutPage, bottomPage, stylesheet);

                                    //Add a final newline
                                    pageContent = string.Format("{0} \n ", pageContent);

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

        private string GeneratePage(string _blog, SortedList<DateTime, BlogPostInfo> _pages, int _noOfPostsToShow, int _noOfRecentPostsToShow, bool _showGravatar, bool _showCloud, bool _showArchive, PageContent _aboutPage, PageContent _bottomPage, string _stylesheet)
        {
            //Create output
            string list = string.Empty;

            //Generate Posts
            list = GeneratePosts(_blog, _pages, _noOfPostsToShow, _showGravatar, _stylesheet);

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

        private string GeneratePosts(string _blog, SortedList<DateTime, BlogPostInfo> _posts, int _noOfPostsToShow, bool _showGravatar, string _stylesheet)
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

                    /*Generate title and gravatar*/
                    string title = "";
                    if (_showGravatar)
                    {
                        title = string.Format("<div class=\"blogavatar\"><a href=\"User.aspx?Username={0}\">{1}</a></div>\n", entry.Value.UserName, entry.Value.UserGravatar);
                        title = string.Format("{0}<h1 class=\"blogavatartitle\"><a href=\"{1}.ashx\">{2}</a></h1>\n", title, entry.Value.Content.PageInfo.FullName, entry.Value.Content.Title);
                    }
                    else
                    {
                        title = string.Format("<h1 class=\"blogtitle\"><a href=\"{0}.ashx\">{1}</a></h1>\n", entry.Value.Content.PageInfo.FullName, entry.Value.Content.Title);
                    }

                    retval = string.Format("{0} \n {1} ", retval, string.Format("<div class=\"blogpost\">\n"
                                                                                    + "{0}"
                                                                                    + "<p class=\"blogbyline\"><small>Posted on {1} by <a href=\"User.aspx?Username={2}\">{3}</a> | <a href=\"Edit.aspx?Page={4}\">Edit</a></small></p>\n"
                                                                                    + "<div class=\"blogentry\">\n"
                                                                                    + "{5}\n"
                                                                                    + "</div>\n"
                                                                                    + "<p class=\"blogmeta\"><a href=\"{6}.ashx\" class=\"blogmore\">Go to page</a> &nbsp;&nbsp;&nbsp; <a href=\"{6}.ashx?Discuss=1\" class=\"blogcomments\">Comments ({8})</a></p>\n"
                                                                                + "</div>\n"
                                                                            , title                                             //Gravatar + title + titlelink
                                                                            , entry.Key.ToString(m_DateTimeFormat)              //Date
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
                                    + "<h1 class=\"blogabouttitle\"><a href=\"Edit.aspx?Page={1}\">{2}</a></h1>"
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
                                    + "<h1 class=\"blogbottomtitle\"><a href=\"Edit.aspx?Page={1}\">{2}</a></h1>\n"
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
