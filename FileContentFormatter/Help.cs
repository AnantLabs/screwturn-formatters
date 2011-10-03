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
** Text based files can be displayed
** PDF Files can be displayed
** Video can be displayed (Embedded in WMP only)
*** The following files will be '''attempted''' to be played by WMP:
**** .aif, .aifc, .aiff, .asf , .asx, .au, .avi, .cda, .dvr-ms, .ivf, .m1v, .m3u, .mid, .midi, .mov,
**** .mp2, .mp3, .mpa, .mpe, .mpeg, .mpg, .mpv2, .qt, .rmi, .snd, .vob, .wav, .wax, .wm, .wma, .wms,
**** .wmv, .wmx, .wmz, .wpl, .wvx
** The raw content of a file can be displayed
* Embed the display file in a box with specified size (height/width)
* Specify which provider to retrieve the file from
{BR}

(((
'''Usage:'''{BR}{BR}
'''{ FileCont file=XXX prov=YYY height=ZYX width=XYZ raw=ZZZ rows=XXY cols=YYX }'''{BR}{BR}
'''Where:''' {BR}
* '''file''' - A valid path and file wildcard, must be encapsulated in ' '
* '''prov''' - Fullname as seen in the wiki File management, ex. 'Local Files Provider' and 'SQL Server Files Storage Provider'
* '''height''' - Height of the box to display the file inside
** If used with raw=true remember to add unit, ex. px or %
* '''width''' - Width of the box to display the file inside
** If used with raw=true remember to add unit, ex. px or %
* '''raw''' - True or false, will display the raw content of a file.
** '''RAW SHOULD ALWAYS BE USED WITH CAUTION, USE ONLY ON SMALL FILES'''
** '''rows''' - Applies only when using raw=true, can be used instead of height to set the number of rows to display
** '''cols''' - Applies only when using raw=true, can be used instead of width to set the number of cols to display

{BR}
''All args which have a value that contains whitespaces, must be encapsulated in ' ', ex. '/Folder name with spaces/File.txt'. ''{BR}
'''There's no guarantee that your file can be displayed, it depends on the browser used, plugins installed, server etc.. However basic Text files and ''standard wide spread'' video/pdf file types should work in most browsers.'''{BR}{BR}
It may require a couple of attempts find the right way of displaying your file, remember that there are '''< nowiki >''' and '''< nobr >''' tags which might help you get the right result.
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

==== Display the code of a html page in a box ====
{BR}
(((
'''Markup:'''{BR}{BR}
'''{ FileCont file='/Keeper.Garrett.Formatters/Tables/*.html' prov='Local Files Provider' raw=true rows=12 cols=80 }''' {BR}{BR}
'''Result:'''{BR}{BR}
{FileCont file='/Keeper.Garrett.Formatters/Tables/*.html' raw=true rows=12 cols=80 }
{BR}{BR}
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


==== Using tags to display the entire code of a html page  ====
{BR}
(((
'''Markup:'''{BR}{BR}
'''@ @ { FileCont file='/Keeper.Garrett.Formatters/Tables/*.html' prov='Local Files Provider' raw=true } @ @''' {BR}{BR}
'''Result:'''{BR}{BR}
@@
{FileCont file='/Keeper.Garrett.Formatters/Tables/*.html' raw=true }
@@
{BR}{BR}
)))
{BR}


==== Without tags, displaying an entire html page  ====
{BR}
(((
'''Markup:'''{BR}{BR}
'''{ FileCont file='/Keeper.Garrett.Formatters/Tables/*.html' prov='Local Files Provider' raw=true }''' {BR}{BR}
'''Result:'''{BR}{BR}
{FileCont file='/Keeper.Garrett.Formatters/Tables/*.html' raw=true }
{BR}{BR}
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
{FileCont file='/Keeper.Garrett.Formatters/Tables/*.css' width=540 height=400 raw=true rows=10 cols=50}


===Display Content of TableStyles.css===
{FileCont file='/Keeper.Garrett.Formatters/Tables/*.css' raw=true}


===Display Content of TableStyles.css using tags to avoid formatting===
@@
{FileCont file='/Keeper.Garrett.Formatters/Tables/*.css' raw=true}
@@

===Display Content of all files in /Tables/===
{FileCont file='/Keeper.Garrett.Formatters/Tables/*.*' width=540 height=400 raw=true rows=5 cols =50}


===Display Content of all files in /Tables/ No Embedding ===
{FileCont file='/Keeper.Garrett.Formatters/Tables/*.*' raw=true}
",
            Description = "FileContentFormatter Test",
            Keywords = new string[] { "FileContentFormatter", "Test" }
        }
        };
    }
}
