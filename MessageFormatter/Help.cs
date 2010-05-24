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
            Fullname = "MessageFormatterHelp",
            Title = "Message Formatter Help",
            Content = @"Displays a message as highlighted item, usefull for displaying warnings,information, notes etc.{BR}
You can verify that the formatter is working by [MessageFormatterTest|opening this test page].{BR}
{TOC}
{BR}
=== Administrators ===
No special actions required.{BR}{BR}
=== Markup Usage ===
'''What can you do?'''{BR}
* Display a highlighted message in a wiki page
* Customize the displayed message in a css file.
{BR}

(((
'''Usage:'''{BR}{BR}
'''< msg type=XXX head=YYY >ZZZ< /msg >'''{BR}{BR}
'''Where:''' {BR}
* '''type''' - A type defined in the css file. Default the following are supplied with the formatter:
** tip
** information
** note
** important
** caution
** warning
** system
** error
** question
* '''head''' - An alternate heading, default the type name is used as heading
* '''ZZZ''' - Represents the message to display, can be wiki formatted.

{BR}
''All args which have a value that contains whitespaces, must be encapsulated in ' ', ex. 'My Heading'. ''
{BR}
)))
{BR}

==== List of default message displays ====
{BR}
(((
'''Markup:'''{BR}{BR}
'''< msg type=tip>Test message< /msg>''' {BR}{BR}
'''Result:'''{BR}{BR}
<msg type=tip>Test message</msg>{BR}{BR}
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''< msg type=information>Test message< /msg>''' {BR}{BR}
'''Result:'''{BR}{BR}
<msg type=information>Test message</msg>{BR}{BR}
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''< msg type=note>Test message< /msg>''' {BR}{BR}
'''Result:'''{BR}{BR}
<msg type=note>Test message</msg>{BR}{BR}
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''< msg type=important>Test message< /msg>''' {BR}{BR}
'''Result:'''{BR}{BR}
<msg type=important>Test message</msg>{BR}{BR}
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''< msg type=caution>Test message< /msg>''' {BR}{BR}
'''Result:'''{BR}{BR}
<msg type=caution>Test message</msg>{BR}{BR}
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''< msg type=warning>Test message< /msg>''' {BR}{BR}
'''Result:'''{BR}{BR}
<msg type=warning>Test message</msg>{BR}{BR}
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''< msg type=system>Test message< /msg>''' {BR}{BR}
'''Result:'''{BR}{BR}
<msg type=system>Test message</msg>{BR}{BR}
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''< msg type=error>Test message< /msg>''' {BR}{BR}
'''Result:'''{BR}{BR}
<msg type=error>Test message</msg>{BR}{BR}
)))
{BR}
(((
'''Markup:'''{BR}{BR}
'''< msg type=question>Test message< /msg>''' {BR}{BR}
'''Result:'''{BR}{BR}
<msg type=question>Test message</msg>{BR}{BR}
)))
{BR}
==== Customization  ====
To customize the existing defaults or create your own styles look in the following path of your default file storage provider:
* '''/Keeper.Garrett.Formatters/MessageFormatter/MessageStyle.css''' - All styling is done here
* '''/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/*.png''' - 24x24 pixel images
* '''/Keeper.Garrett.Formatters/MessageFormatter/Images-32x32/*.png''' - 32x32 pixel images

The following images are provided with the formatter for easier customization. {BR}
<table style=""border: 1px solid black;"">
<tr><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/add_blue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/add_green.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/add_grey.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/add_lightblue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/add_red.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/add_yellow.png]</td></tr>
<tr><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/delete_blue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/delete_green.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/delete_grey.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/delete_lightblue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/delete_red.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/delete_yellow.png]</td></tr>
<tr><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/empty_blue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/empty_green.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/empty_grey.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/empty_lightblue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/empty_red.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/empty_yellow.png]</td></tr>
<tr><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/error_blue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/error_green.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/error_grey.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/error_lightblue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/error_red.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/error_yellow.png]</td></tr>
<tr><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/exclamation_blue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/exclamation_green.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/exclamation_grey.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/exclamation_lightblue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/exclamation_red.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/exclamation_yellow.png]</td></tr>
<tr><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/information_blue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/information_green.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/information_grey.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/information_lightblue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/information_red.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/information_yellow.png]</td></tr>
<tr><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/question_blue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/question_green.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/question_grey.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/question_lightblue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/question_red.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/question_yellow.png]</td></tr>
<tr><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/left_blue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/left_green.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/left_grey.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/left_lightblue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/left_red.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/left_yellow.png]</td></tr>
<tr><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/right_blue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/right_green.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/right_grey.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/right_lightblue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/right_red.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/right_yellow.png]</td></tr>
<tr><td/><td/><td/><td/><td/><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/warning.png]</td></tr>
</table>

Courtesy of [http://www.famfamfam.com|fam fam fam] and [http://www.fatcow.com/free-icon| fat cow] with minor adjustments.
",
            Description = "MessageFormatter Help",
            Keywords = new string[] { "MessageFormatter", "Help", "Formatting", "Markup", "Message", }
        },

        new Page()
        {
            Fullname = "MessageFormatterTest",
            Title = "Message Formatter Test",
            Content = @"== Page for testing and verifying the MessageFormatter ==
{TOC}
{BR}
===Display Different Message Types===

<msg type=tip>Test message</msg>

<msg type=information>Test message</msg>

<msg type=note>Test message</msg>

<msg type=important>Test message</msg>

<msg type=caution>Test message</msg>

<msg type=warning>Test message</msg>

<msg type=system>Test message</msg>

<msg type=error>Test message</msg>

<msg type=question>Test message</msg>

===Display Different Message Types With Heading override===

<msg type=tip head='My Head'>Test message</msg>

<msg type=information head='My Head'>Test message</msg>

<msg type=note head='My Head'>Test message</msg>

<msg type=important head='My Head'>Test message</msg>

<msg type=caution head='My Head'>Test message</msg>

<msg type=warning head='My Head'>Test message</msg>

<msg type=system head='My Head'>Test message</msg>

<msg type=error head='My Head'>Test message</msg>

<msg type=question head='My Head'>Test message</msg>

===Display Avaliable Images===
<table style=""border: 1px dashed black;\"">
<tr><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/add_blue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/add_green.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/add_grey.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/add_lightblue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/add_red.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/add_yellow.png]</td></tr>
<tr><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/delete_blue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/delete_green.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/delete_grey.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/delete_lightblue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/delete_red.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/delete_yellow.png]</td></tr>
<tr><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/empty_blue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/empty_green.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/empty_grey.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/empty_lightblue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/empty_red.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/empty_yellow.png]</td></tr>
<tr><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/error_blue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/error_green.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/error_grey.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/error_lightblue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/error_red.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/error_yellow.png]</td></tr>
<tr><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/exclamation_blue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/exclamation_green.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/exclamation_grey.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/exclamation_lightblue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/exclamation_red.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/exclamation_yellow.png]</td></tr>
<tr><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/information_blue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/information_green.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/information_grey.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/information_lightblue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/information_red.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/information_yellow.png]</td></tr>
<tr><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/question_blue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/question_green.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/question_grey.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/question_lightblue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/question_red.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/question_yellow.png]</td></tr>
<tr><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/left_blue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/left_green.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/left_grey.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/left_lightblue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/left_red.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/left_yellow.png]</td></tr>
<tr><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/right_blue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/right_green.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/right_grey.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/right_lightblue.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/right_red.png]</td><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/right_yellow.png]</td></tr>
<tr><td/><td/><td/><td/><td/><td>[image||{UP}/Keeper.Garrett.Formatters/MessageFormatter/Images-24x24/warning.png]</td></tr>
</table>
",
            Description = "MessageFormatter Test",
            Keywords = new string[] { "MessageFormatter", "Test" }
        }
        };
    }
}
