using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.Core;

namespace Keeper.Garrett.ScrewTurn.FileContentFormatter
{
    public class Help
    {
        public static readonly Page[] HelpPages = new Page[]
        { 
            new Page() {
            Fullname = "FileContentFormatterHelp",
            Title = "File Content Help",
            Content = @"Displays the contents of a txt based file, for example html,css,txt, directly on a wiki page.{BR}
You can verify that the formatter is working by [FileContentFormatterTest|opening this test page].{BR}
{TOC}
{BR}
=== Administrators ===
No special actions required.{BR}{BR}
=== Markup Usage ===
'''What can you do?'''{BR}
* Display the content of a file(s) in a wiki page
** Currently only text based files are supported, if other file formats such as images or binaries are used, the output will be the raw data
* Embed the display file in a box with specified size (height/width)
* Specify which provider to retrieve the file from
{BR}

(((
'''Usage:'''{BR}{BR}
'''{FileCont file=XXX prov=YYY height=ZZZ width=XYZ}'''{BR}{BR}
'''Where:''' {BR}
* '''file''' - A valid path and file wildcard, must be encapsulated in ' '
* '''prov''' - Fullname as seen in the wiki File management, ex. 'Local Files Provider' and 'SQL Server Files Storage Provider'
* '''height''' - Height of the box to display the file inside
* '''width''' - Width of the box to display the file inside
{BR}
''All args which have a value that contains whitespaces, must be encapsulated in ' ', ex. '/Folder name with spaces/File.txt'. ''
{BR}
)))
{BR}

==== Display a file inside a box ====
{BR}
(((
'''Markup:'''{BR}{BR}
'''{ FileCont file='/Keeper.Garrett.Formatters/Tables/*.html' width=500 height=200}''' {BR}{BR}
'''Result:'''{BR}{BR}
{FileCont file='/Keeper.Garrett.Formatters/Tables/*.html' width=500 height=200} {BR}{BR}
)))
{BR}

==== Display a file inside a box using a different provider  ====
{BR}
(((
'''Markup:'''{BR}{BR}
'''{ FileCont file='/Keeper.Garrett.Formatters/Tables/*.html' prov='Local Files Provider' width=500 height=200 }''' {BR}{BR}
'''Result:'''{BR}{BR}
{FileCont file='/Keeper.Garrett.Formatters/Tables/*.html' width=500 height=200} {BR}{BR}
)))
{BR}

==== Use tags to avoid wiki interpretation  ====
{BR}
(((
'''Markup:'''{BR}{BR}
'''@@{ FileCont file='/Keeper.Garrett.Formatters/Tables/*.html' prov='Local Files Provider' width=200 height=200 }@@''' {BR}{BR}
'''Result:'''{BR}{BR}
@@
{FileCont file='/Keeper.Garrett.Formatters/Tables/*.html'}
@@ {BR}{BR}
)))
{BR}

==== Display a file  ====
{BR}
(((
'''Markup:'''{BR}{BR}
'''{ FileCont file='/Keeper.Garrett.Formatters/Tables/*.html' }''' {BR}{BR}
'''Result:'''{BR}{BR}
{FileCont file='/Keeper.Garrett.Formatters/Tables/*.html'} {BR}{BR}
)))
{BR}
",
            Description = "FileContentFormatter Help",
            Keywords = new string[] { "FileContentFormatter", "Help", "Formatting", "Markup", "File", "Content" }
        },

        new Page()
        {
            Fullname = "FileContentFormatterTest",
            Title = "File Content Test",
            Content = @"== Page for testing and verifying the FileContentFormatter ==
You may need to adjust the path/pattern and provider for each test, to match existing files in your wiki.{BR}
{TOC}
{BR}
===Display Content of table-examples.html Embedded===
{FileCont file='/Keeper.Garrett.Formatters/Tables/*.html' width=540 height=400}

===Display Content of TableStyles.css Embedded===
{FileCont file='/Keeper.Garrett.Formatters/Tables/*.css' width=540 height=400}

===Display Content of TableStyles.css===
{FileCont file='/Keeper.Garrett.Formatters/Tables/*.css'}

===Display Content of TableStyles.css using tags to avoid formatting===
@@
{FileCont file='/Keeper.Garrett.Formatters/Tables/*.css'}
@@

===Display Content of all files in /Tables/===
{FileCont file='/Keeper.Garrett.Formatters/Tables/*.*' width=540 height=400}

===Display Content of all files in /Tables/ No Embedding ===
{FileCont file='/Keeper.Garrett.Formatters/Tables/*.*'}
",
            Description = "FileContentFormatter Test",
            Keywords = new string[] { "FileContentFormatter", "Test" }
        }
        };
    }
}
