using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.Core;
using Keeper.Garrett.ScrewTurn.Utility;

namespace Keeper.Garrett.ScrewTurn.EventLogFormatter
{
    public class Help
    {
        public static readonly Page[] HelpPages = new Page[]
        { 
            new Page() {
            Fullname = "EventLogFormatterHelp",
            Title = "EventLog Table Help",
            Content = @"=='''EventLog Tables'''==
Autogenerates a table based on an eventlog and show the associated eventlog entries, all based on user input.{BR}
{TOC}
{BR}
=== Administrators ===
Administrator might need to be involved depending on the security settings of the machine which is hosting the wiki and the remote machines with eventlogs which might be contacted.{BR}
You can verify that the formatter is working by [EventLogFormatterTest|opening this test page (be patient, it may be slow)].{BR}{BR}
=== Markup Usage ===
'''What can you do?'''{BR}
* Generate a table of eventlog entries.
** Filter eventlog by 1 or more of the following: Id, Type, Date, Time, Source, Category, Event, User, Computer and Description
** Contact remote machine's event logs (permission setup might be required)
** Limit your search result by setting an upper limit
** Display one or more of the following eventlog properties:
*** type
*** date
*** time
*** source
*** category
*** event
*** user
*** computer
*** description
* [Keeper-Garrett-Table-Styles|Use tables instead of lists]
{BR}

(((
'''Usage:'''{BR}{BR}
'''{ EventLog machine= log= filter= results= }'''{BR}{BR}
'''Where:''' {BR}
* ''Data:''
** '''machine''' - Name/IP of a the machine to fetch the eventlog from. Ommit for localhost.
** '''log''' - Name of the log
** '''filter''' - Filter to limit the search results. Use the following filters, seperate by "","".
*** '''""Id""''' - Index id of the entry
*** '''""Type""''' - Must be one of:
**** Information
**** Warning
**** Error
**** InfoAndWarn
**** InforAndError
**** WarnAndError
*** '''""Date""''' - Date and time the eventlog entry was created, may depend on machine setup (ex. 01-01-2010 or 01-01-2010 01:30:00 )
**** This is a eventlogdate >= myDate test.
**** Alternatively use: -1 or -x for days to look back, ex ,date=-1,
*** '''""Time""''' - Date and time the eventlog was written, may depend on machine setup (ex. 01-01-2010 or 01-01-2010 01:30:00 )
**** Alternatively use: -1 or -x for days to look back, ex ,date=-1,
**** This is always a eventlogtime >= myTime test.
*** '''""Source""''' - Source of the eventlog entry 
*** '''""Category""''' - Category of the eventlog entry
*** '''""Event""''' - EventId/InstanceId of the eventlog entry
*** '''""User""''' - User name of the eventlog entry
*** '''""Computer""''' - Computer name of the eventlog entry
*** '''""Description""''' - Actual eventlog entry description. When using this field a string search will be performed, so use keywords such as: MyApp, database or other.
**** ""' '"" - does not apply to this field, search must match entry string to reveal results.
** '''Results''' - Number which limits the amount of results returned, use this with care since it may be '''VERY''' cpu intensive to query the eventlog and scan for matches. Default is 15 results.
* To display additional columns use the [Keeper-Garrett-Table-Styles|table tags found here]. Simply use the values described above in the 'cols' argument.
{BR}
''All args which have a value that contains whitespaces, must be encapsulated in ' ', ex. 'My Category'.  ''
)))
{BR}

==== Examples ====
All examples here contacts the local eventlog only for better readbility.
{BR}
'''Minimum'''
(((
'''Markup:'''{BR}{BR}
'''{ EventLog log='Application' results=3 }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|
|+
! Type !! Date !! Source !! Description   
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Information.png|Information] Information || 01-01-2010 01:00:00 || Winlogon || Description
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Warning.png|Warning] Warning || 01-01-2010 01:00:00 || Winlogon || Description
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Error.png|Error] Error || 01-01-2010 01:00:00 || Winlogon || Description
|}
)))
{BR}
'''All columns'''
(((
'''Markup:'''{BR}{BR}
'''{ EventLog log='Application' results=3 cols=all}''' {BR}{BR}
'''Result:'''{BR}{BR}
{|
|+
! Id !! Type !! Date !! Time !! Source !! Category !! Event !! User !! Computer !! Description   
|-
| 0 || [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Information.png|Information] Information || 01-01-2010 01:00:00 || 01-01-2010 01:00:00 || Winlogon || 0 || X || User || MyMachine || Description
|-
| 1 || [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Warning.png|Warning] Warning || 01-01-2010 01:00:00 || 01-01-2010 01:00:00 || Winlogon || 0 || Y || User || MyMachine || Description
|-
| 2 || [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Error.png|Error] Error || 01-01-2010 01:00:00 || 01-01-2010 01:00:00 || Winlogon || 0 || Z || User || MyMachine || Description
|}
)))
{BR}
'''Filter - Type 1'''
(((
'''Markup:'''{BR}{BR}
'''{ EventLog log='Application' filter='type=error' results=3 }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|
|+
! Type !! Date !! Source !! Description   
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Error.png|Error] Error || 01-01-2010 01:00:01 || Winlogon || Description
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Error.png|Error] Error || 01-01-2010 01:00:02 || Winlogon || Description
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Error.png|Error] Error || 01-01-2010 01:00:03 || Winlogon || Description
|}
)))
{BR}
'''Filter - Type 2'''
(((
'''Markup:'''{BR}{BR}
'''{ EventLog log='Application' filter='type=warnanderror' results=3 }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|
|+
! Type !! Date !! Source !! Description   
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Error.png|Error] Error || 01-01-2010 01:00:01 || Winlogon || Description
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Warning.png|Warning] Warning || 01-01-2010 01:00:02 || Winlogon || Description
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Error.png|Error] Error || 01-01-2010 01:00:03 || Winlogon || Description
|}
)))
{BR}
'''Filter - Date 1'''
(((
'''Markup:'''{BR}{BR}
'''{ EventLog log='Application' filter='date=01-01-2010' results=3 }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|
|+
! Type !! Date !! Source !! Description   
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Information.png|Information] Information || 01-01-2010 00:00:00 || Winlogon || Description
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Warning.png|Warning] Warning || 01-01-2010 01:00:00 || Winlogon || Description
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Error.png|Error] Error || 01-02-2010 02:00:00 || Winlogon || Description
|}
)))
{BR}
'''Filter - Date 2'''
(((
'''Markup:'''{BR}{BR}
'''{ EventLog log='Application' filter='date=01-01-2010 01:00:00' results=3 }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|
|+
! Type !! Date !! Source !! Description   
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Information.png|Information] Information || 01-01-2010 01:00:00 || Winlogon || Description
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Warning.png|Warning] Warning || 01-01-2010 02:00:00 || Winlogon || Description
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Error.png|Error] Error || 01-02-2010 03:00:00 || Winlogon || Description
|}
)))
{BR}
'''Filter - Description'''
(((
'''Markup:'''{BR}{BR}
'''{ EventLog log='Application' filter='description=database' results=3 }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|
|+
! Type !! Date !! Source !! Description   
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Information.png|Information] Information || 01-01-2010 01:00:00 || Winlogon || ....dataBase....
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Warning.png|Warning] Warning || 01-01-2010 02:00:00 || Winlogon || ....DATABase....
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Error.png|Error] Error || 01-02-2010 03:00:00 || Winlogon || ....DataBase....
|}
)))
{BR}
'''Filter - Multiple filters'''
(((
'''Markup:'''{BR}{BR}
'''{ EventLog log='Application' filter='type=information,source=winlogon,description=license' results=3 }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|
|+
! Type !! Date !! Source !! Description   
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Information.png|Information] Information || 01-01-2010 01:00:00 || Winlogon || ....license....
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Information.png|Information] Information || 01-01-2010 01:00:00 || Winlogon || ....license....
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Images/Information.png|Information] Information || 01-01-2010 01:00:00 || Winlogon || ....license....
|}
)))
{BR}
==== Tables and Styling ====
To use [Keeper-Garrett-Table-Styles|tables look here]. 
{BR}
",
            Description = "EventLogFormatter Help",
            Keywords = new string[] { "EventLogFormatter", "Help", "Formatting", "Markup", "EventLog", "Table" }
        },

        new Page() {
            Fullname = "EventLogFormatterTest",
            Title = "EventLog Table Test",
            Content = @"== Page for testing and verifying the EventLogFormatter ==
Page for verifying the EventLogFormatter.{BR}
{TOC}
{BR}
===Default===
{EventLog log='Application' results=3}
{BR}
===All columns===
{EventLog log='Application' results=3 cols=all}
{BR}
===Filter - Type 1===
{EventLog log='Application' filter='type=error' results=3}
{BR}
",
            Description = "EventLogFormatter Test",
            Keywords = new string[] { "EventLogFormatter", "Test" }
        },

        XHtmlTableGenerator.HelpPage
        };
    }
}
