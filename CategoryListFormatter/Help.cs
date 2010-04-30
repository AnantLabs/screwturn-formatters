using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.Core;

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
* Choose wether to include a summary of the pages in the list items
* Use tables instead of lists
** Add a table heading
** Override column headers and make them more ''user friendly'' 
** Change table style
** Change columns header style
** Change row style
** Use one of 3 predefined styles '''bw,bg,gb'''
** Combine several of the above options
{BR}

(((
'''Usage:'''{BR}{BR}
'''{CategoryList(Category,output,showSummary,TblHeading,Headers,TblStyle,HeadStyle,RowStyle)}'''{BR}{BR}
'''Where:''' {BR}
* ''Required:''
** '''Category''' - Name of a valid category
** '''output''' - Can be 1 of 3, '''*,# or nothing'''
*** '''""*""'''- Means unnumbered list
*** '''""#""'''- Means numbered list
*** '''""""''' - Means use table instead of list
** '''showSummary''' - Will add a page summary to each item in the output (if present). Must be ""true"" or ""false""
* ''Optional (applies only for tables as output):''
** '''TblHeading''' - Heading of the table, must be encapsulated in ' ' '''ex. 'My Heading' '''
** '''Headers''' - Columnheaders will override default naming, '''must be encapsulated in ' ' ex. 'Head1,Head2' '''
** '''TblStyle''' - Style format, '''must be encapsulated in ' ' ex. 'align=""center"" style=""color: #000000;""' '''
** '''HeadFStyle''' - Style format, '''must be encapsulated in ' ' ex. 'align=""center"" style=""color: #000000;""' '''
** '''RowStyle''' - Style format,'''must be encapsulated in ' ' ex. 'align=""center"" style=""color: #000000;""' '''
{BR}
* All ""''','''"" '''must''' always be included in the tag, at all times, even if the content is blank.
)))
{BR}

==== Minimum ====
The combinations of lists.{BR}
(((
'''Markup:'''{BR}{BR}
'''{CategoryList(MyCat,*,false,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
* [Page1|Page Link1]
* [Page2|Page Link2]
* [Page3|Page Link3]
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''{CategoryList(MyCat,#,false,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
1. [Page1|Page Link1]
2. [Page2|Page Link2]
3. [Page3|Page Link3]
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''{CategoryList(MyCat,*,true,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
* [Page1|Page Link1] - Page Summary/Description 1
* [Page2|Page Link2] - Page Summary/Description 2
* [Page3|Page Link3] - Page Summary/Description 3
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''{CategoryList(MyCat,#,true,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
1. [Page1|Page Link1] - Page Summary/Description 1
2. [Page2|Page Link2] - Page Summary/Description 2
3. [Page3|Page Link3] - Page Summary/Description 3
)))
{BR}

==== Tables and Styling ====
'''Default style'''{BR}
Depends on your chosen theme. {BR}
(((
'''Markup:'''{BR}{BR}
'''{CategoryList(MyCat,,true,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|  
|+  
! Page name !! Description
|-  
| [Page1|Page Link1] || Page Summary/Description 1
|-  
| [Page2|Page Link2] || Page Summary/Description 2
|-  
| [Page3|Page Link3] || Page Summary/Description 3
|}
)))
{BR}

'''Predefined style: ''Black and White'' '''{BR}
(((
'''Markup:'''{BR}{BR}
'''{CategoryList(MyCat,,true,,,'bw','bw','bw') }''' {BR}{BR}
'''Result:'''{BR}{BR}
{| border=""0"" cellpadding=""2"" cellspacing=""0"" align=""center"" style=""background-color: #EEEEEE;""
|- align=""center"" style=""background-color: #000000; color: #FFFFFF; font-weight: bold;""  
| Page name || Description
|- align=""center"" style=""color: #000000;""
| [Page1|Page Link1] || Page Summary/Description 1
|- align=""center"" style=""color: #000000;""
| [Page2|Page Link2] || Page Summary/Description 2
|- align=""center"" style=""color: #000000;""
| [Page3|Page Link3] || Page Summary/Description 3
|}
)))
{BR}

'''Predefined style: ''Black and Grey'' '''{BR}
(((
'''Markup:'''{BR}{BR}
'''{CategoryList(MyCat,,true,,,'bg','bg','bg') }''' {BR}{BR}
'''Result:'''{BR}{BR}
{| border=""0"" cellpadding=""2"" cellspacing=""0"" align=""center"" style=""background-color: #EEEEEE;""
|- align=""center"" style=""background-color: #000000; color: #CCCCCC; font-weight: bold;""  
| Page name || Description
|- align=""center"" 
| [Page1|Page Link1] || Page Summary/Description 1
|- align=""center"" 
| [Page2|Page Link2] || Page Summary/Description 2
|- align=""center"" 
| [Page3|Page Link3] || Page Summary/Description 3
|}
)))
{BR}

'''Predefined style: ''Green and Black''' ''{BR}
(((
'''Markup:'''{BR}{BR}
'''{CategoryList(MyCat,,true,,,'gb','gb','gb') }''' {BR}{BR}
'''Result:'''{BR}{BR}
{| border=""0"" cellpadding=""2"" cellspacing=""0"" align=""center"" style=""background-color: #EEEEEE;""
|- align=""center"" style=""background-color: #88CC33; color: #000000; font-weight: bold;""   
| Page name || Description
|- align=""center"" style=""color: #000000;""
| [Page1|Page Link1] || Page Summary/Description 1
|- align=""center"" style=""color: #000000;""
| [Page2|Page Link2] || Page Summary/Description 2
|- align=""center"" style=""color: #000000;""
| [Page3|Page Link3] || Page Summary/Description 3
|}
)))
{BR}

'''Custom style:'''{BR}
(((
'''Markup:'''{BR}{BR}
'''{CategoryList(MyCat,,true,,,'cellspacing=""10"" style=""background-color: #88CC33; color: #000000;""','style=""color: #00AAAA;""','style=""color: #BBBB00;""') }''' {BR}{BR}
'''Result:'''{BR}{BR}
{| cellspacing=""10"" style=""background-color: #88CC33; color: #000000;""
|- style=""color: #00AAAA;""   
| Page name || Description
|- style=""color: #BBBB00;""
| [Page1|Page Link1] || Page Summary/Description 1
|- style=""color: #BBBB00;""
| [Page2|Page Link2] || Page Summary/Description 2
|- style=""color: #BBBB00;""
| [Page3|Page Link3] || Page Summary/Description 3
|}
)))
{BR}

'''Custom heading + headers:'''{BR}
(((
'''Markup:'''{BR}{BR}
'''{CategoryList(MyCat,,true,'My heading','Head1,Head2',,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|  
|+ My heading  
! Head1 !! Head2
|-  
| [Page1|Page Link1] || Page Summary/Description 1
|-  
| [Page2|Page Link2] || Page Summary/Description 2
|-  
| [Page3|Page Link3] || Page Summary/Description 3
|}
)))
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
{CategoryList(Help,*,false,,,,,)}
{BR}

===Primitive Unnumbered  Summary===
{BR}
{CategoryList(Help,*,true,,,,,)}
{BR}

===Primitive Numbered No Summary===
{BR}
{CategoryList(Help,#,true,,,,,)}
{BR}

===Primitive Numbered  Summary===
{BR}
{CategoryList(Help,#,true,,,,,)}
{BR}

===Primitive Default When Bad Param No Summary===
{BR}
{CategoryList(Help,X,false,,,,,)}
{BR}

===Primitive Default When Bad Param Summary===
{BR}
{CategoryList(Help,X,true,,,,,)}
{BR}
{BR}
== Tables ==

===Table No Summary===
{BR}
{CategoryList(Help,,false,,,,,)}
{BR}

===Table Summary===
{BR}
{CategoryList(Help,,true,,,,,)}
{BR}

===Table No Summary - Style BW===
{BR}
{CategoryList(Help,,false,,,'bw','bw','bw')}
{BR}

===Table Summary - Style BW===
{BR}
{CategoryList(Help,,true,,,'bw','bw','bw')}
{BR}

===Table No Summary - Style BG===
{BR}
{CategoryList(Help,,false,,,'bg','bg','bg')}
{BR}

===Table Summary - Style BG===
{BR}
{CategoryList(Help,,true,,,'bg','bg','bg')}
{BR}

===Table No Summary - Style GB===
{BR}
{CategoryList(Help,,false,,,'gb','gb','gb')}
{BR}

===Table Summary - Style GB===
{BR}
{CategoryList(Help,,true,,,'gb','gb','gb')}
{BR}

===Table Summary - Heading + 1 Header===
{BR}
{CategoryList(Help,,true,'My Table','MyHead',,,)}
{BR}

===Table Summary - Heading + 2 Headers===
{BR}
{CategoryList(Help,,true,'My Table','Head1,Head2',,,)}
{BR}

===Table Summary - Custom Style===
{BR}
{CategoryList(Help,,true,'My Table','Head1,Head2','cellspacing=""10"" style=""background-color: #88CC33; color: #000000;""','align=""center"" style=""color: #0000AA;""','align=""center"" style=""color: #BBBBBB;""')}
{BR}
",
            Description = "CategoryListFormatter Test",
            Keywords = new string[] { "CategoryListFormatter", "Test" }
        }
        };
    }
}
