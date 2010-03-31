using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.Core;

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
* Format your table
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
'''{EventLog(Machine,Log,Filter,Results,Heading,Columns,Headers,TblStyle,HeadStyle,RowStyle)}'''{BR}{BR}
'''Where:''' {BR}
* ''Data:''
** '''Machine''' - Name/IP of a the machine to fetch the eventlog from. Leave blank for localhost.
** '''Log''' - Name of the log, '''must be encapsulated in ' ' (ex. 'Application')'''.
** '''Filter''' - Filter to limit the search results, '''must be encapsulated in ' ' (ex. 'type=error,id=1')'''. Use the following filters, seperate by "","".
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
* ''Formatting:''
** '''TblHeading''' - Heading of the table, must be encapsulated in ' ' '''ex. 'My Heading' '''
** '''Columns''' - Columns and column order starting at 0, must be encapsulated in ' ' '''ex. 1,2,3 or 1,0,3'''. See the next 2 lines for special formatting of columns.
*** If left blank, the 4 columns (Type,Date,Source,Description) will be shown.
*** If valued 'all', all possible columns will be shown (10 in all)
** '''Headers''' - Columnheaders will override default naming, '''must be encapsulated in ' ' ex. 'Head1,Head2' '''
** '''TblStyle''' - Style format, '''must be encapsulated in ' ' ex. 'align=""center"" style=""color: #000000;""' '''
** '''HeadFStyle''' - Style format, '''must be encapsulated in ' ' ex. 'align=""center"" style=""color: #000000;""' '''
** '''RowStyle''' - Style format,'''must be encapsulated in ' ' ex. 'align=""center"" style=""color: #000000;""' '''
{BR}
* All ""''','''"" '''must''' always be included in the tag, at all times, even if the content is blank.
)))
{BR}

==== Examples ====
All examples here contacts the local eventlog only for better readbility.
{BR}
'''Minimum'''
(((
'''Markup:'''{BR}{BR}
'''{EventLog(,'Application',,3,,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|
|+
! Type !! Date !! Source !! Description   
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Information.png|Information] Information || 01-01-2010 01:00:00 || Winlogon || Description
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Warning.png|Warning] Warning || 01-01-2010 01:00:00 || Winlogon || Description
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Error.png|Error] Error || 01-01-2010 01:00:00 || Winlogon || Description
|}
)))
{BR}
'''All columns'''
(((
'''Markup:'''{BR}{BR}
'''{EventLog(,'Application',,3,'all',,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|
|+
! Id !! Type !! Date !! Time !! Source !! Category !! Event !! User !! Computer !! Description   
|-
| 0 || [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Information.png|Information] Information || 01-01-2010 01:00:00 || 01-01-2010 01:00:00 || Winlogon || 0 || X || User || MyMachine || Description
|-
| 1 || [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Warning.png|Warning] Warning || 01-01-2010 01:00:00 || 01-01-2010 01:00:00 || Winlogon || 0 || Y || User || MyMachine || Description
|-
| 2 || [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Error.png|Error] Error || 01-01-2010 01:00:00 || 01-01-2010 01:00:00 || Winlogon || 0 || Z || User || MyMachine || Description
|}
)))
{BR}
'''Filter - Type 1'''
(((
'''Markup:'''{BR}{BR}
'''{EventLog(,'Application','type=error',3,,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|
|+
! Type !! Date !! Source !! Description   
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Error.png|Error] Error || 01-01-2010 01:00:01 || Winlogon || Description
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Error.png|Error] Error || 01-01-2010 01:00:02 || Winlogon || Description
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Error.png|Error] Error || 01-01-2010 01:00:03 || Winlogon || Description
|}
)))
{BR}
'''Filter - Type 2'''
(((
'''Markup:'''{BR}{BR}
'''{EventLog(,'Application','type=warnanderror',3,,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|
|+
! Type !! Date !! Source !! Description   
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Error.png|Error] Error || 01-01-2010 01:00:01 || Winlogon || Description
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Warning.png|Warning] Warning || 01-01-2010 01:00:02 || Winlogon || Description
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Error.png|Error] Error || 01-01-2010 01:00:03 || Winlogon || Description
|}
)))
{BR}
'''Filter - Date 1'''
(((
'''Markup:'''{BR}{BR}
'''{EventLog(,'Application','date=01-01-2010',3,,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|
|+
! Type !! Date !! Source !! Description   
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Information.png|Information] Information || 01-01-2010 00:00:00 || Winlogon || Description
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Warning.png|Warning] Warning || 01-01-2010 01:00:00 || Winlogon || Description
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Error.png|Error] Error || 01-02-2010 02:00:00 || Winlogon || Description
|}
)))
{BR}
'''Filter - Date 2'''
(((
'''Markup:'''{BR}{BR}
'''{EventLog(,'Application','date=01-01-2010 01:00:00',3,,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|
|+
! Type !! Date !! Source !! Description   
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Information.png|Information] Information || 01-01-2010 01:00:00 || Winlogon || Description
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Warning.png|Warning] Warning || 01-01-2010 02:00:00 || Winlogon || Description
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Error.png|Error] Error || 01-02-2010 03:00:00 || Winlogon || Description
|}
)))
{BR}
'''Filter - Description'''
(((
'''Markup:'''{BR}{BR}
'''{EventLog(,'Application','description=database',3,,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|
|+
! Type !! Date !! Source !! Description   
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Information.png|Information] Information || 01-01-2010 01:00:00 || Winlogon || ....dataBase....
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Warning.png|Warning] Warning || 01-01-2010 02:00:00 || Winlogon || ....DATABase....
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Error.png|Error] Error || 01-02-2010 03:00:00 || Winlogon || ....DataBase....
|}
)))
{BR}
'''Filter - Multiple filters'''
(((
'''Markup:'''{BR}{BR}
'''{EventLog(,'Application','type=information,source=winlogon,description=license',3,,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|
|+
! Type !! Date !! Source !! Description   
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Information.png|Information] Information || 01-01-2010 01:00:00 || Winlogon || ....license....
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Information.png|Information] Information || 01-01-2010 01:00:00 || Winlogon || ....license....
|-
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Information.png|Information] Information || 01-01-2010 01:00:00 || Winlogon || ....license....
|}
)))
{BR}
==== Styling ====
'''Default style'''{BR}
Depends on your chosen theme. {BR}
(((
'''Markup:'''{BR}{BR}
'''{EventLog(,'Application',,,,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|  
|+  
! Type !! Date !! Source !! Description   
|-  
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Information.png|Information] Information || 01-01-2010 01:00:00 || Winlogon || Description
|-  
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Warning.png|Warning] Warning || 01-01-2010 01:00:00 || Winlogon || Description
|-  
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Error.png|Error] Error || 01-01-2010 01:00:00 || Winlogon || Description
|}
)))
{BR}

'''Predefined style: ''Black and White'' '''{BR}
(((
'''Markup:'''{BR}{BR}
'''{EventLog(,'Application',,,,,,'bw','bw','bw') }''' {BR}{BR}
'''Result:'''{BR}{BR}
{| border=""0"" cellpadding=""2"" cellspacing=""0"" align=""center"" style=""background-color: #EEEEEE;"" 
|+
|- align=""center"" style=""background-color: #000000; color: #FFFFFF; font-weight: bold;""    
| Type || Date || Source || Description   
|- align=""center"" style=""color: #000000;"" 
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Information.png|Information] Information || 01-01-2010 01:00:00 || Winlogon || Description
|- align=""center"" style=""color: #000000;"" 
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Warning.png|Warning] Warning || 01-01-2010 01:00:00 || Winlogon || Description
|- align=""center"" style=""color: #000000;"" 
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Error.png|Error] Error || 01-01-2010 01:00:00 || Winlogon || Description
|}
)))
{BR}



'''Predefined style: ''Black and Grey'' '''{BR}
(((
'''Markup:'''{BR}{BR}
'''{EventLog(,'Application',,,,,,'bg','bg','bg') }''' {BR}{BR}
'''Result:'''{BR}{BR}
{| border=""0"" cellpadding=""2"" cellspacing=""0"" align=""center"" style=""background-color: #EEEEEE;"" 
|+
|- align=""center"" style=""background-color: #000000; color: #CCCCCC; font-weight: bold;""  
| Type || Date || Source || Description      
|- align=""center""
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Information.png|Information] Information || 01-01-2010 01:00:00 || Winlogon || Description
|- align=""center""
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Warning.png|Warning] Warning || 01-01-2010 01:00:00 || Winlogon || Description
|- align=""center"" 
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Error.png|Error] Error || 01-01-2010 01:00:00 || Winlogon || Description
|}
)))
{BR}

'''Predefined style: ''Green and Black''' ''{BR}
(((
'''Markup:'''{BR}{BR}
'''{EventLog(,'Application',,,,,,'gb','gb','gb') }''' {BR}{BR}
'''Result:'''{BR}{BR}
{| border=""0"" cellpadding=""2"" cellspacing=""0"" align=""center"" style=""background-color: #EEEEEE;"" 
|+
|- align=""center"" style=""background-color: #88CC33; color: #000000; font-weight: bold;""  
| Type || Date || Source || Description      
|- align=""center"" style=""color: #000000;""
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Information.png|Information] Information || 01-01-2010 01:00:00 || Winlogon || Description
|- align=""center"" style=""color: #000000;""
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Warning.png|Warning] Warning || 01-01-2010 01:00:00 || Winlogon || Description
|- align=""center"" style=""color: #000000;"" 
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Error.png|Error] Error || 01-01-2010 01:00:00 || Winlogon || Description
|}
)))
{BR}

'''Custom style:'''{BR}
(((
'''Markup:'''{BR}{BR}
'''{EventLog(,'Application',,,,,,'cellspacing=""10"" style=""background-color: #88CC33; color: #000000;""','style=""color: #00AAAA;""','style=""color: #BBBB00;""') }''' {BR}{BR}
'''Result:'''{BR}{BR}
{| cellspacing=""10"" style=""background-color: #88CC33; color: #000000;""
|+
|- style=""color: #00AAAA;""
| Type || Date || Source || Description      
|- style=""color: #BBBB00;""
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Information.png|Information] Information || 01-01-2010 01:00:00 || Winlogon || Description
|- style=""color: #BBBB00;""
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Warning.png|Warning] Warning || 01-01-2010 01:00:00 || Winlogon || Description
|- style=""color: #BBBB00;""
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Error.png|Error] Error || 01-01-2010 01:00:00 || Winlogon || Description
|}
)))
{BR}

'''Custom heading + headers:'''{BR}
(((
'''Markup:'''{BR}{BR}
'''{EventLog(,'Application',,,'My Heading',,'H1,H2,H3,H4',,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
{| 
|+ My Heading
! H1 !! H2 !! H3 !! H4 
|- 
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Information.png|Information] Information || 01-01-2010 01:00:00 || Winlogon || Description
|- 
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Warning.png|Warning] Warning || 01-01-2010 01:00:00 || Winlogon || Description
|- 
| [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Error.png|Error] Error || 01-01-2010 01:00:00 || Winlogon || Description
|}
)))
{BR}


'''Custom heading + headers + order:'''{BR}
(((
'''Markup:'''{BR}{BR}
'''{EventLog(,'Application',,,'My Heading','9,2,4,1','Dscr,When,From,Cat',,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
{| 
|+ My Heading
! Dscr !! When !! From !! Cat
|- 
| Description || 01-01-2010 01:00:00 || Winlogon || [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Information.png|Information] Information 
|- 
| Description || 01-01-2010 01:00:00 || Winlogon || [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Warning.png|Warning] Warning 
|- 
| Description || 01-01-2010 01:00:00 || Winlogon || [image||{UP}/Keeper.Garrett.Formatters/EventLogFormatter/Error.png|Error] Error 
|}
)))
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
== Data ==
===Default===
{BR}
{EventLog(,'Application',,3,,,,,,)}
{BR}
===All columns===
{BR}
{EventLog(,'Application',,3,,'all',,,,)}
{BR}
===Filter - Type 1===
{BR}
{EventLog(,'Application','type=error',3,,,,,,)}
{BR}
===Filter - Type 2===
{BR}
{EventLog(,'Application','type=warnanderror',3,,,,,,)}
{BR}
===Filter - Date 1===
{BR}
{EventLog(,'Application','date=01-01-2010',3,,,,,,)}
{BR}
===Filter - Date 2===
{BR}
{EventLog(,'Application','date=01-01-2010 01:00:00',3,,,,,,)}
{BR}
===Filter - Date 3===
{BR}
{EventLog(,'Application','date=-1',3,,,,,,)}
{BR}
===Filter - Description===
{BR}
{EventLog(,'Application','description=service',3,,,,,,)}
{BR}
===Filter - Multiple filters===
{BR}
{EventLog(,'System','type=information,source=Service Control Manager,description=service',3,,,,,,)}
{BR}
{BR}
== Styling ==
===Default style==={BR}
{BR}
{EventLog(,'Application',,3,,,,,,)}
{BR}
===Predefined style: ''Black and White'' ===
{BR}
{EventLog(,'Application',,3,,,,'bw','bw','bw')}
{BR}
===Predefined style: ''Black and Grey'' ===
{BR}
{EventLog(,'Application',,3,,,,'bg','bg','bg')}
{BR}
===Predefined style: ''Green and Black'' ===
{BR}
{EventLog(,'Application',,3,,,,'gb','gb','gb')}
{BR}
===Custom style:==={BR}
{BR}
{EventLog(,'Application',,3,,,,'cellspacing=""10"" style=""background-color: #88CC33; color: #000000;""','style=""color: #00AAAA;""','style=""color: #BBBB00;""')}
{BR}
===Custom heading + headers:==={BR}
{BR}
{EventLog(,'Application',,3,'My Heading',,'H1,H2,H3,H4',,,)}
{BR}
===Custom heading + headers + order:==={BR}
{BR}
{EventLog(,'Application',,3,'My Heading','2,4,1,9','When,From,Cat,Dscr',,,)}
{BR}",
            Description = "EventLogFormatter Test",
            Keywords = new string[] { "EventLogFormatter", "Test" }
        }
        };
    }
}
