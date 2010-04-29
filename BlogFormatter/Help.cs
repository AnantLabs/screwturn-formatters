using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.Core;

namespace Keeper.Garrett.ScrewTurn.BlogFormatter
{
    public class Help
    {
        public static readonly Page[] HelpPages = new Page[]
        { 
            new Page() {
            Fullname = "BlogFormatterHelp",
            Title = "Blog Help",
            Content = @"=='''Blog'''==
Autogenerates a blog consisting of pages with a specific category.{BR}
You can that the formatter is working by [BlogFormatterTest|opening this test page].{BR}
{TOC}
{BR}
=== Administrators ===
'''Installation'''
* Unzip the BlogFormatter.zip
* Place the folder 'Blog' in the '...\Themes\' folder.
* Load the plugin into the wiki using the Administrator interface as with most other plugins.
{BR}

=== Markup Usage ===
'''What can you do?'''{BR}
* Generate a blog with posts for a certain category
* Set the number of posts to display
* Choose wether to use create date or last modified for post order
* Choose to display an about section
* Choose to display a cloud based on page keywords
* Choose to display an archive section
** Choose how many recent posts to list
* Choose to display a bottom section
* Choose custom layout using stylesheet and images
* 1 predefined stylesupplied in ...Themes\Blog\BlogDefault.css
{BR}

'''How does it work?'''{BR}
* Create a new page called ex. MyBlog. 
** '''Do not makes this page, the about page or the bottom page, a member of the MyBlogCategory, if you do you will create a self refering loop, which the BlogFormatter will detect, display a warning and deny blog generation.'''{BR}
* Insert the Blog tag {Blog(MyBlogCategory,,,,)} and all posts shown in the blog will be pages marked with the category MyBlogCategory.{BR}
* The keyword cloud is generated based on the ''keywords'' attached to each wiki page.{BR}
That's it! :).{BR}
The rest is purely customization of the look.
{BR}

'''Usage:'''{BR}{BR}
(((
'''{Blog(Category,noOfPosts,noOfRecent,useLastModified,showGravatars,showCloud,showArchive,'aboutPage','bottomPage','stylesheet')}'''{BR}{BR}
'''Where:''' {BR}
* ''Required:''
** '''Category''' - Name of a valid category, to generate Blog from
* ''Optional (applies only for tables as output):''
** '''noOfPosts''' - No of posts to show, default 7
** '''noOfRecent'''- No of most recent posts to show, default 15
** '''useLastModified'''- Use last modified date instead of create date as post ordering, default false
*** If false create date and create user is used for post, if true latest mod date and mod user is displayed
** '''showGravatars''' - Show gravatars for post creator, only works when DisplayGravatars have been enabled in the wiki configuration, default false
*** This feature only really make any meaning when used with Blogs where multiple users can create posts
** '''showCloud''' - Show keyword cloud, default false
** '''showArchive''' - Show archive (most recent blogs), default false
** '''aboutPage''' - Name of page to include and display as the about section, must be encapsulated in ' ' '''ex. 'MyBlogAboutMe' '''
*** If the about page is not being shown it is most likelly because you did not supply the correct, case-sentive page name in ' '
** '''bottomPage''' - Name of page to include and display as the bottom section, must be encapsulated in ' ' '''ex. 'MyBlogBottom' '''
*** If the bottom page is not being shown it is most likelly because you did not supply the correct, case-sentive page name in ' '
** '''stylesheet''' - Name of stylesheet to use other than the default, must be encapsulated in ' ' '''ex. 'MyBlogCustomStylesheet.css' '''
*** The stylesheet must be placed in at the ...Themes\Blog\ folder.
{BR}
* All ""''','''"" '''must''' always be included in the tag, at all times, even if the content is blank.
)))
{BR}

==== Examples ====
Any of the following examples can be combined to generate your desired look. {BR}
A typical Blog would have the following setup:

===== Standard =====
(((
'''Markup:'''{BR}{BR}
'''{Blog(MyBlogCat,7,15,false,false,true,true,'MyAboutPage','MyBottomPage',) }''' {BR}{BR}
'''Which will yeild:'''{BR}{BR}
* At most 7 posts displayed
* At most 15 recent posts displayed
* Posts are displayed and ordered using create date
* No Gravatars
* Cloud shown
* Archive shown
* About shown
* Bottom shown
* No custom stylesheet
)))

===== Default =====
(((
'''Markup:'''{BR}{BR}
'''{Blog(MyBlogCategory,,,,,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
* At most 7 posts displayed
* No recent posts displayed
* Posts are displayed and ordered using create date
* No Gravatars
* No Cloud
* No Archive
* No About
* No Bottom
* No custom stylesheet
)))
{BR}

===== Max No Of Posts To Show =====
(((
'''Markup:'''{BR}{BR}
'''{Blog(MyBlogCategory,3,,,,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
* '''At most 3 posts displayed'''
* No recent posts displayed
* Posts are displayed and ordered using create date
* No Gravatars
* No Cloud
* No Archive
* No About
* No Bottom
* No custom stylesheet
)))
{BR}

===== Max No Of Most Recent Posts To Show =====
(((
'''Markup:'''{BR}{BR}
'''{Blog(MyBlogCategory,,3,,,,true,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
* At most 7 posts displayed
* '''At most 3 posts displayed'''
* Posts are displayed and ordered using create date
* No Gravatars
* No Cloud
* '''Archive shown'''
* No About
* No Bottom
* No custom stylesheet
)))
{BR}

===== Use Modified Date+User To Display And Order Posts =====
(((
'''Markup:'''{BR}{BR}
'''{Blog(MyBlogCategory,,,true,,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
* At most 7 posts displayed
* No recent posts displayed
* '''Posts are displayed and ordered using modified date + last mod user'''
* No Gravatars
* No Cloud
* No Archive 
* No About
* No Bottom
* No custom stylesheet
)))
{BR}

===== Show Gravatars =====
(((
'''Markup:'''{BR}{BR}
'''{Blog(MyBlogCategory,,,,true,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
* At most 7 posts displayed
* No recent posts displayed
* Posts are displayed and ordered using create date
* '''Gravatars are shown'''
* No Cloud
* No Archive 
* No About
* No Bottom
* No custom stylesheet
)))
{BR}

===== Show Keyword Cloud =====
(((
'''Markup:'''{BR}{BR}
'''{Blog(MyBlogCategory,,,,true,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
* At most 7 posts displayed
* No recent posts displayed
* Posts are displayed and ordered  using create date
* No Gravatars
* '''Cloud shown'''
* No Archive
* No About
* No Bottom
* No custom stylesheet
)))
{BR}

===== Show Archive =====
(((
'''Markup:'''{BR}{BR}
'''{Blog(MyBlogCategory,,,,,,true,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
* At most 7 posts displayed
* '''At most 15 posts displayed'''
* Posts are displayed and ordered using create date
* No Gravatars
* No Cloud
* '''Archive shown'''
* No About
* No Bottom
* No custom stylesheet
)))
{BR}

===== Show About Section =====
(((
'''Markup:'''{BR}{BR}
'''{Blog(MyBlogCategory,,,,,,,'AboutPage',,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
* At most 7 posts displayed
* No recent posts displayed
* Posts are displayed and ordered  using create date
* No Gravatars
* No Cloud
* No Archive
* '''About shown using the wiki page 'AboutPage' '''
* No Bottom
* No custom stylesheet
)))
{BR}

===== Show Bottom Section =====
(((
'''Markup:'''{BR}{BR}
'''{Blog(MyBlogCategory,,,,,,,,'BottomPage',) }''' {BR}{BR}
'''Result:'''{BR}{BR}
* At most 7 posts displayed
* No recent posts displayed
* Posts are displayed and ordered  using create date
* No Gravatars
* No Cloud
* No Archive
* No About
* '''Bottom shown using the wiki page 'BottomPage' '''
* No custom stylesheet
)))
{BR}

===== Use Custom Stylesheet =====
(((
'''Markup:'''{BR}{BR}
'''{Blog(MyBlogCategory,,,,,,,,,'MyStyleSheet.css') }''' {BR}{BR}
'''Result:'''{BR}{BR}
* At most 7 posts displayed
* No recent posts displayed
* Posts are displayed and ordered  using create date
* No Gravatars
* No Cloud
* No Archive
* No About
* No Bottom
* '''Custom stylesheet 'MyStyleSheet.css' placed in .../Themes/Blog is used for layout and design of the blog'''
)))
{BR}

==== About styling ====
To create a custom stylesheet examine the supplied BlogDefault.css stylesheet placed in .../Themes/Blog.
This file contains all the nessasary div's tags etc to create your own style."
,
            Description = "BlogFormatter Help",
            Keywords = new string[] { "BlogFormatter", "Help", "Formatting", "Markup", "Blog", "Posts", "Archive" }
        },

        new Page()
        {
            Fullname = "BlogFormatterTest",
            Title = "Blog Test",
            Content = @"== Page for testing and verifying the BlogtFormatter ==
You may need to adjust the category for the test to get a view of the entire blog, as there is no default blog category supplied with the formatter.{BR}
{BR}
== Max 3 posts, max 3 recent, use create date, show gravatars, show cloud, show archive, show about, show bottom, no custom stylesheet==
{Blog(MyBlog,3,3,false,true,true,true,'About','Bottom',)}
",
            Description = "BlogFormatter Test",
            Keywords = new string[] { "BlogFormatter", "Test" }
        }
        }; 
    }
}
