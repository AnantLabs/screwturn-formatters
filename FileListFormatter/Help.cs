using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.Core;

namespace Keeper.Garrett.ScrewTurn.FileListFormatter
{
    public class Help
    {
        public static readonly Page[] HelpPages = new Page[]
        { 
            new Page() {
            Fullname = "FileListFormatterHelp",
            Title = "File List Help",
            Content = @"=='''File Lists'''==
Autogenerates a list/table of files in a directory chosen by the user, either for download or simple file overview.{BR}
You can verify that the formatter is working by [FileListFormatterTest|opening this test page (be patient, it may be slow)].{BR}
{TOC}
{BR}
=== Administrators ===
No special actions required.{BR}{BR}
=== Markup Usage ===
'''What can you do?'''{BR}
* Generate a list/table of files in a directory that match a certain pattern
* Specify specific FileStorageFormatter to find files in
* Enable/disable ability to download the files
* Sort the files asc/desc accoring to:
** Name
** Size
** Last modified date
** Download count
* Display details of the files:
** Size
** Last modified date
** Download count
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
'''{FileList('Pattern','Provider',OutputType,SortMethod,CreateLinks,DetailsToShow,TblHeading,Headers,TblStyle,HeadStyle,RowStyle)}'''{BR}{BR}
'''Where:''' {BR}
* ''Required:''
** '''Pattern''' - A valid path and file wildcard, must be encapsulated in ' '
*** All patterns must start with '/' ex.  /*.*, will give all files in the root dir of the provider
*** For sub directories:  /MyDir/*.* will work
**** Default dir if none specified is '/'
*** For file wildcards: *.* , Screw*Wiki.*xe, *.zip etc. will work
**** Default wildcard is *.* if none is specified
**** '''Note that the definition of the default dir might differ from provider to provider, ex. for Local Files Provider, the default root is /public/Upload'''
* ''Optional:''
** '''Provider''' - Fullname as seen in the wiki File management, ex. 'Local Files Provider' and 'SQL Server Files Storage Provider', must be encapsulated in ' '
*** Default value is the chosen wiki default file storage provider
*** '''Note that the definition of the default dir might differ from provider to provider, ex. for Local Files Provider, the default root is /public/Upload'''
** '''OutputType''' - Can be one of: '''*,# or 'table''''
*** '''""*""'''- Means unnumbered list
*** '''""#""'''- Means numbered list
*** '''""table""''' - Means use table instead of list
*** Default is *
** '''SortMethod''' - Can be anything from 0 - 7, where:
*** '''0'''- Filename Ascending
*** '''1'''- Filename Descending
*** '''2'''- Download count Ascending
*** '''3'''- Download count Descending
*** '''4'''- File Size Ascending
*** '''5'''- File Size Descending
*** '''6'''- Last Modified Date Ascending
*** '''7'''- Last Modified Date Descending
*** Default is 7
** '''CreateLink''' - Should filenames displayed as download links, true or false, default is false
** '''DetailsToShow''' - Can be anything from 0 - 7, where:
*** '''0'''- No extra details
*** '''1'''- Show download count
*** '''2'''- Show download count + file size
*** '''3'''- Show download count + last modified date
*** '''4'''- Show download count + file size + last modified date
*** '''5'''- Show file size
*** '''6'''- Show file size + last modified date
*** '''7'''- Show last modified date
*** Default is 0
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
{BR}
(((
'''Markup:'''{BR}{BR}
'''{FileList('/*.*',' ',,,,,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
* File1
* File2
* File3
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''{FileList('/*.*',' ',#,,,,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
1. File1
2. File2
3. File3
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''{FileList('/*.*',' ',*,true,,,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
* [File1|File Link1]
* [File2|File Link2]
* [File3|File Link3]
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''{FileList('/*.*',' ',*,7,true,0,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
* [File1|File Link1]
* [File2|File Link2]
* [File3|File Link3]
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''{FileList('/*.*',' ',*,7,true,4,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
* [File1|File Link1] (1-1-2010, 10 KB, 1 downloads)
* [File2|File Link2] (1-2-2010, 20 KB, 2 downloads)
* [File3|File Link3] (1-3-2010, 30 KB, 3 downloads)
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''{FileList('/*.*',' ',*,6,true,4,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
* [File3|File Link3] (1-3-2010, 30 KB, 3 downloads)
* [File2|File Link2] (1-2-2010, 20 KB, 2 downloads)
* [File1|File Link1] (1-1-2010, 10 KB, 1 downloads)
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''{FileList('/*.*',' ',*,2,true,4,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
* [File3|File Link3] (1-1-2010, 10 KB, 3 downloads)
* [File2|File Link2] (1-2-2010, 20 KB, 2 downloads)
* [File1|File Link1] (1-1-2010, 10 KB, 1 downloads)
)))
{BR}


==== Tables and Styling ====
'''Default style'''{BR}
Depends on your chosen theme. {BR}
(((
'''Markup:'''{BR}{BR}
'''{FileList('/*.*',' ',table,7,true,1,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|  
|+  
! Filename !! Downloads
|-  
| [File1|File Link1] || 3
|-  
| [File2|File Link2] || 2
|-  
| [File3|File Link3] || 1
|}
)))
{BR}

'''Predefined style: ''Black and White'' '''{BR}
(((
'''Markup:'''{BR}{BR}
'''{FileList('/*.*',' ',table,7,true,1,,,'bw','bw','bw') }''' {BR}{BR}
'''Result:'''{BR}{BR}
{| border=""0"" cellpadding=""2"" cellspacing=""0"" align=""center"" style=""background-color: #EEEEEE;""
|- align=""center"" style=""background-color: #000000; color: #FFFFFF; font-weight: bold;""  
| Filename || Downloads
|- align=""center"" style=""color: #000000;""
| [File1|File Link1] || 3
|- align=""center"" style=""color: #000000;""
| [File2|File Link2] || 2
|- align=""center"" style=""color: #000000;""
| [File3|File Link3] || 1
|}
)))
{BR}

'''Predefined style: ''Black and Grey'' '''{BR}
(((
'''Markup:'''{BR}{BR}
'''{FileList('/*.*',' ',table,7,true,1,,,'bg','bg','bg') }''' {BR}{BR}
'''Result:'''{BR}{BR}
{| border=""0"" cellpadding=""2"" cellspacing=""0"" align=""center"" style=""background-color: #EEEEEE;""
|- align=""center"" style=""background-color: #000000; color: #CCCCCC; font-weight: bold;""  
| Filename || Downloads
|- align=""center"" 
| [File1|File Link1] || 3
|- align=""center"" 
| [File2|File Link2] || 2
|- align=""center"" 
| [File3|File Link3] || 1
|}
)))
{BR}

'''Predefined style: ''Green and Black''' ''{BR}
(((
'''Markup:'''{BR}{BR}
'''{FileList('/*.*',' ',table,7,true,1,,,'gb','gb','gb') }''' {BR}{BR}
'''Result:'''{BR}{BR}
{| border=""0"" cellpadding=""2"" cellspacing=""0"" align=""center"" style=""background-color: #EEEEEE;""
|- align=""center"" style=""background-color: #88CC33; color: #000000; font-weight: bold;""   
| Filename || Downloads
|- align=""center"" style=""color: #000000;""
| [File1|File Link1] || 3
|- align=""center"" style=""color: #000000;""
| [File2|File Link2] || 2
|- align=""center"" style=""color: #000000;""
| [File3|File Link3] || 1
|}
)))
{BR}

'''Custom style:'''{BR}
(((
'''Markup:'''{BR}{BR}
''''{FileList('/*.*',' ',table,7,true,1,,,'cellspacing=""10"" style=""background-color: #88CC33; color: #000000;""','style=""color: #00AAAA;""','style=""color: #BBBB00;""') }''' {BR}{BR}
'''Result:'''{BR}{BR}
{| cellspacing=""10"" style=""background-color: #88CC33; color: #000000;""
|- style=""color: #00AAAA;""   
| Filename || Downloads
|- style=""color: #BBBB00;""
| [File1|File Link1] || 3
|- style=""color: #BBBB00;""
| [File2|File Link2] || 2
|- style=""color: #BBBB00;""
| [File3|File Link3] || 1
|}
)))
{BR}

'''Custom heading + headers:'''{BR}
(((
'''Markup:'''{BR}{BR}
''''{FileList('/*.*',' ',table,7,true,1,'My heading','Head1,Head2',,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|  
|+ My heading  
! Head1 !! Head2
|-  
| [File1|File Link1] || 3
|-  
| [File2|File Link2] || 2
|-  
| [File3|File Link3] || 1
|}
)))
{BR}
",
            Description = "FileListFormatter Help",
            Keywords = new string[] { "FileListFormatter", "Help", "Formatting", "Markup", "File", "List", "Table" }
        },

        new Page()
        {
            Fullname = "FileListFormatterTest",
            Title = "File List Test",
            Content = @"== Page for testing and verifying the FileListFormatter ==
You may need to adjust the path/pattern and provider for each test, to match existing files in your wiki.{BR}
{TOC}
{BR}
== Primitive Lists ==

===Default===
{BR}
{FileList('/*.*','',,,,,,,,,)}
{BR}

===Specific Provider===
{BR}
{FileList('/*.*','SQL Server Files Storage Provider',,,,,,,,,)}
{BR}

===With Links===
{BR}
{FileList('/*.*','',,,true,,,,,,)}
{BR}

===Primitive Unnumbered No Details===
{BR}
{FileList('/*.*','',*,7,true,0,,,,,)}
{BR}

===Primitive Unnumbered All Details===
{BR}
{FileList('/*.*','',*,7,true,4,,,,,)}
{BR}

===Primitive Numbered No Details===
{BR}
{FileList('/*.*','',#,7,true,0,,,,,)}
{BR}

===Primitive Numbered All Details===
{BR}
{FileList('/*.*','',#,7,true,4,,,,,)}
{BR}

===Primitive Sort Method 0===
{BR}
{FileList('/*.*','',#,0,true,4,,,,,)}
{BR}

===Primitive Sort Method 1===
{BR}
{FileList('/*.*','',#,1,true,4,,,,,)}
{BR}

===Primitive Sort Method 2===
{BR}
{FileList('/*.*','',#,2,true,4,,,,,)}
{BR}

===Primitive Sort Method 3===
{BR}
{FileList('/*.*','',#,3,true,4,,,,,)}
{BR}

===Primitive Sort Method 4===
{BR}
{FileList('/*.*','',#,4,true,4,,,,,)}
{BR}

===Primitive Sort Method 5===
{BR}
{FileList('/*.*','',#,5,true,4,,,,,)}
{BR}

===Primitive Sort Method 6===
{BR}
{FileList('/*.*','',#,6,true,4,,,,,)}
{BR}

===Primitive Sort Method 7===
{BR}
{FileList('/*.*','',#,7,true,4,,,,,)}
{BR}

===Primitive No Files===
{BR}
{FileList('/XXYYZZ/*.*','',#,7,true,4,,,,,)}
{BR}

===Primitive Default When Bad Params===
{BR}
{FileList('/XXYYZZ/*.*','XSD',X,X,X,X,,,,,)}
{BR}
{BR}
== Tables ==

===Default===
{BR}
{FileList('/*.*','',table,,,,,,,,)}
{BR}

===Specific Provider===
{BR}
{FileList('/*.*','SQL Server Files Storage Provider',table,,,,,,,,)}
{BR}

===With Links===
{BR}
{FileList('/*.*','',table,,true,,,,,,)}
{BR}

===Table No Details===
{BR}
{FileList('/*.*','',table,7,true,0,,,,,)}
{BR}

===Table All Details===
{BR}
{FileList('/*.*','',table,7,true,4,,,,,)}
{BR}

===Table Sort Method 0===
{BR}
{FileList('/*.*','',table,0,true,4,,,,,)}
{BR}

===Table Sort Method 1===
{BR}
{FileList('/*.*','',table,1,true,4,,,,,)}
{BR}

===Table Sort Method 2===
{BR}
{FileList('/*.*','',table,2,true,4,,,,,)}
{BR}

===Table Sort Method 3===
{BR}
{FileList('/*.*','',table,3,true,4,,,,,)}
{BR}

===Table Sort Method 4===
{BR}
{FileList('/*.*','',table,4,true,4,,,,,)}
{BR}

===Table Sort Method 5===
{BR}
{FileList('/*.*','',table,5,true,4,,,,,)}
{BR}

===Table Sort Method 6===
{BR}
{FileList('/*.*','',table,6,true,4,,,,,)}
{BR}

===Table Sort Method 7===
{BR}
{FileList('/*.*','',table,7,true,4,,,,,)}
{BR}

===Table No Files===
{BR}
{FileList('/XXYYZZ/*.*','',table,7,true,4,,,,,)}
{BR}

===Table Default When Bad Params===
{BR}
{FileList('XXYYZZ/*.*','AXC',table,X,X,X,,,,,)}
{BR}
{BR}

===Table All Details - Style BW===
{BR}
{FileList('/*.*','',table,7,true,4,,,'bw','bw','bw')}
{BR}

===Table All Details - Style BG===
{BR}
{FileList('/*.*','',table,7,true,4,,,'bg','bg','bg')}
{BR}

===Table All Details - Style GB===
{BR}
{FileList('/*.*','',table,7,true,4,,,'gb','gb','gb')}
{BR}

===Table Summary - Heading + 1 Header===
{BR}
{FileList('/*.*','',table,7,true,4,'My Table','MyHead',,,)}
{BR}

===Table Summary - Heading + 2 Headers===
{BR}
{FileList('/*.*','',table,7,true,4,'My Table','Head1,Head2',,,)}
{BR}

===Table Summary - Custom Style===
{BR}
{FileList('/*.*','',table,7,true,4,'My Table','Head1,Head2','cellspacing=""10"" style=""background-color: #88CC33; color: #000000;""','align=""center"" style=""color: #0000AA;""','align=""center"" style=""color: #BBBBBB;""')}
{BR}
",
            Description = "FileListFormatter Test",
            Keywords = new string[] { "FileListFormatter", "Test" }
        }
        };
    }
}
