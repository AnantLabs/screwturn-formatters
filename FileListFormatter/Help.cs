using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.Core;
using Keeper.Garrett.ScrewTurn.Utility;

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
* Specify specific File Storage Provider to find files in
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
{BR}

(((
'''Usage:'''{BR}{BR}
'''{ FileList file= prov= type= dwnl= details= sort= }'''{BR}{BR}
'''Where:''' {BR}
* '''file''' - A valid path and file wildcard
** All patterns must start with '/' ex.  /*.*, will give all files in the root dir of the provider
** For sub directories:  /MyDir/*.* will work
*** Default dir if none specified is '/'
** For file wildcards: *.* , Screw*Wiki.*xe, *.zip etc. will work
** Default wildcard is *.* if none is specified
** '''Note that the definition of the default dir might differ from provider to provider, ex. for Local Files Provider, the default root is /public/Upload'''
* '''prov''' - Fullname as seen in the wiki File management, ex. 'Local Files Provider' and 'SQL Server Files Storage Provider', must be encapsulated in ' '
** Default value is the chosen wiki default file storage provider
** '''Note that the definition of the default dir might differ from provider to provider, ex. for Local Files Provider, the default root is /public/Upload'''
* '''type''' - Can be one of: '''*,# or 'table''''
** '''""*""'''- Means unnumbered list
** '''""#""'''- Means numbered list
** '''""table""''' - Means use table instead of list
** Default is *
* '''dwnl''' - Should filenames displayed as download links, true or false, default is false
* '''sort''' - Can be anything of the following 4, but must be follow by either ''asc'' or ''desc'' for direction and seperated by ','
** '''name'''- Filename
** '''downloads'''- Download count
** '''size'''- File size
** '''date'''- Last modified date
** Default is date,desc
* '''details''' - Details to show about each file. Can be any combination of the following 4 seperated by ','
** '''name'''- Filename
** '''downloads'''- Download count
** '''size'''- File size
** '''date'''- Last modified date
** Default is none 

{BR}
'' All args which have a value that contains whitespaces, must be encapsulated in ' ', ex. 'Local Storage Provider'.  '')))
{BR}

==== Minimum ====
{BR}
(((
'''Markup:'''{BR}{BR}
'''{ FileList file='/*.*'}''' {BR}{BR}
'''Result:'''{BR}

* File1
* File2
* File3
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''{ FileList file='/*.*' type=# }''' {BR}{BR}
'''Result:'''{BR}

1. File1
2. File2
3. File3
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''{ FileList file='/*.*' dwnl=true }''' {BR}{BR}
'''Result:'''{BR}

* [File1|File Link1]
* [File2|File Link2]
* [File3|File Link3]
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''{ FileList file='/*.*' sort='date,desc' dwnl=true }''' {BR}{BR}
'''Result:'''{BR}

* [File1|File Link1]
* [File2|File Link2]
* [File3|File Link3]
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''{ FileListfile='/*.*' sort='date,desc' dwnl=true details='date,size,downloads' }''' {BR}{BR}
'''Result:'''{BR}

* [File1|File Link1] (1-1-2010, 10 KB, 1 downloads)
* [File2|File Link2] (1-2-2010, 20 KB, 2 downloads)
* [File3|File Link3] (1-3-2010, 30 KB, 3 downloads)
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''{ FileListfile='/*.*' sort='date,asc' dwnl=true details='date,size,downloads' }''' {BR}{BR}
'''Result:'''{BR}

* [File3|File Link3] (1-3-2010, 30 KB, 3 downloads)
* [File2|File Link2] (1-2-2010, 20 KB, 2 downloads)
* [File1|File Link1] (1-1-2010, 10 KB, 1 downloads)
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''{ FileListfile='/*.*' sort='downloads,asc' dwnl=true details='date,size,downloads' }''' {BR}{BR}
'''Result:'''{BR}

* [File3|File Link3] (1-1-2010, 10 KB, 3 downloads)
* [File2|File Link2] (1-2-2010, 20 KB, 2 downloads)
* [File1|File Link1] (1-1-2010, 10 KB, 1 downloads)
)))
{BR}

==== Tables and Styling ====
To use [Keeper-Garrett-Table-Styles|tables look here]. 
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
{FileList file='/*.*'}
{BR}

===Specific Provider===
{BR}
{FileList file='/*.*' prov='SQL Server Files Storage Provider'}
{BR}

===With Links===
{BR}
{FileList file='/*.*' dwnl=true}
{BR}

===Primitive Unnumbered No Details===
{BR}
{FileList file='/*.*' dwnl=true}
{BR}

===Primitive Unnumbered All Details===
{BR}
{FileList file='/*.*' dwnl=true details='date,size,downloads'}
{BR}

===Primitive Numbered No Details===
{BR}
{FileList file='/*.*' type=# dwnl=true}
{BR}

===Primitive Numbered All Details===
{BR}
{FileList file='/*.*' type=# dwnl=true details='date,size,downloads'}
{BR}

===Primitive Sort Method 0===
{BR}
{FileList file='/*.*' sort='name,asc' dwnl=true details='date,size,downloads'}
{BR}

===Primitive Sort Method 1===
{BR}
{FileList file='/*.*' sort='name,desc' dwnl=true details='date,size,downloads'}
{BR}

===Primitive Sort Method 2===
{BR}
{FileList file='/*.*' sort='downloads,asc' dwnl=true details='date,size,downloads'}
{BR}

===Primitive Sort Method 3===
{BR}
{FileList file='/*.*' sort='downloads,desc' dwnl=true details='date,size,downloads'}
{BR}

===Primitive Sort Method 4===
{BR}
{FileList file='/*.*' sort='size,asc' dwnl=true details='date,size,downloads'}
{BR}

===Primitive Sort Method 5===
{BR}
{FileList file='/*.*' sort='size,desc' dwnl=true details='date,size,downloads'}
{BR}

===Primitive Sort Method 6===
{BR}
{FileList file='/*.*' sort='date,asc' dwnl=true details='date,size,downloads'}
{BR}

===Primitive Sort Method 7===
{BR}
{FileList file='/*.*' sort='date,desc' dwnl=true details='date,size,downloads'}
{BR}

===Primitive No Files===
{BR}
{FileList {FileList file='/*XX.YY*' sort='name,asc' dwnl=true details='date,size,downloads'}
{BR}
",
            Description = "FileListFormatter Test",
            Keywords = new string[] { "FileListFormatter", "Test" }
        },

        XHtmlTableGenerator.HelpPage

        };
    }
}
