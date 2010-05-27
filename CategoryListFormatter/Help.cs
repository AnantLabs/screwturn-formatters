using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.Core;
using Keeper.Garrett.ScrewTurn.Utility;

namespace Keeper.Garrett.ScrewTurn.CategoryListFormatter
{
    public class Help
    {
        public static readonly Page[] HelpPages = new Page[]
        { 
            new Page() {
            Fullname = "CategoryListFormatterHelp",
            Title = "Category List Help",
            Content = @"=='''Category Lists'''==
Autogenerates a list/table of pages which are members of a category chosen by the user.{BR}
You can verify that the formatter is working by [CategoryListFormatterTest|opening this test page (be patient, it may be slow)].{BR}
{TOC}
{BR}
=== Administrators ===
No special actions required.{BR}{BR}
=== Markup Usage ===
'''What can you do?'''{BR}
* Generate a list/table of pages that match a certain category
* Use numbered/unnumbered lists (# or *)
* Choose wether among the following page properties to display:
** comment
** summary
** keywords
** lastmodified
** linkedpages
** createtime
** pagename
** user
** creator
* Choose one or more Namespaces to search in, the namespace must of course contain the same category
* [Keeper-Garrett-Table-Styles|Use tables instead of lists]
{BR}

(((
'''Usage:'''{BR}{BR}
'''{ CategoryList cat= type= }'''{BR}{BR}
'''Where:''' {BR}
* ''Required:''
** '''cat''' - Name of a valid category
* ''Optional:''
** '''ns''' - Namespace(s) to search for pages in, ex. ns=root or ns='root,ProjectX'.
*** Default the current Namespace is used
** '''type''' - Can be 1 of 4, '''*,#, or table'''
*** '''""*""'''- Means unnumbered list
*** '''""#""'''- Means numbered list
*** '''""table""''' - Means use table instead of list
*** Default is *
* To display additional columns use the [Keeper-Garrett-Table-Styles|table tags found here]. Simply use the values described above in the 'cols' argument.
{BR}
'' All args which have a value that contains whitespaces, must be encapsulated in ' ', ex. 'My Category'.  ''
)))
{BR}

==== Minimum ====
The combinations of lists.{BR}
(((
'''Markup:'''{BR}{BR}
'''{ CategoryList cat=MyCat type=* }''' {BR}{BR}
'''Result:'''{BR}

* [Page1|Page Link1]
* [Page2|Page Link2]
* [Page3|Page Link3]
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''{ CategoryList cat=MyCat type=# }''' {BR}{BR}
'''Result:'''{BR}

1. [Page1|Page Link1]
2. [Page2|Page Link2]
3. [Page3|Page Link3]
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''{ CategoryList cat=MyCat type=* cols=summary }''' {BR}{BR}
'''Result:'''{BR}

* [Page1|Page Link1] - Page Summary
* [Page2|Page Link2] - Page Summary
* [Page3|Page Link3] - Page Summary
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''{ CategoryList cat=MyCat type=# cols='summary,user' }''' {BR}{BR}
'''Result:'''{BR}

1. [Page1|Page Link1] - Page Summary - Username{BR}
2. [Page2|Page Link2] - Page Summary - Username{BR}
3. [Page3|Page Link3] - Page Summary - Username{BR}
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''{ CategoryList cat=MyCat ns=ProjectX type=# cols='summary,user' }''' {BR}{BR}
'''Result:'''{BR}

1. [ProjectX.Page1|Page Link1] - Page Summary - Username{BR}
2. [ProjectX.Page2|Page Link2] - Page Summary - Username{BR}
3. [ProjectX.Page3|Page Link3] - Page Summary - Username{BR}
)))
{BR}


==== Tables and Styling ====
To use [Keeper-Garrett-Table-Styles|tables look here]. 
{BR}
",
            Description = "CategoryListFormatter Help",
            Keywords = new string[] { "CategoryListFormatter", "Help", "Formatting", "Markup", "Category", "List", "Table" }
        },

        new Page()
        {
            Fullname = "CategoryListFormatterTest",
            Title = "Category List Test",
            Content = @"== Page for testing and verifying the CategoryListFormatter ==
You may need to adjust the category for each test, to match an existing category on your wiki.{BR}
{TOC}
{BR}
== Primitive Lists ==

===Primitive Unnumbered No Summary===
{BR}
{CategoryList cat=Help type=*}
{BR}

===Primitive Unnumbered  Summary===
{BR}
{CategoryList cat=Help type=* cols=summary}
{BR}

===Primitive Numbered No Summary===
{BR}
{CategoryList cat=Help type=#}
{BR}

===Primitive Numbered  Summary===
{BR}
{CategoryList cat=Help type=# cols=summary}
{BR}
",
            Description = "CategoryListFormatter Test",
            Keywords = new string[] { "CategoryListFormatter", "Test" }
        },

        XHtmlTableGenerator.HelpPage
        };
    }
}
