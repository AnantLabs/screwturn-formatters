using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.Core;

namespace Keeper.Garrett.ScrewTurn.QueryTableFormatter
{
    public class Help
    {
        public static readonly Page[] HelpPages = new Page[]
        { 
            new Page() {
            Fullname = "QueryTableFormatterHelp",
            Title = "Query Table Help",
            Content = @"=='''Query Table's'''==
Autogenerates table's based on SQL queries executed against a Database.{BR}
{TOC}
{BR}
=== Administrators ===
This section is targeted at wiki administrators only.{BR}
You can verify that the formatter is working by [QueryTableFormatterTest|opening this test page (be patient, it may be slow)].{BR}{BR}
==== System Requirements ====
'''Database support''':{BR}
One or more of the following drivers MUST be installed in the GAC of the server running [http://www.screwturn.eu/|ScrewTurn]:
* '''Oracle'''  ([http://www.oracle.com/technology/software/tech/windows/odpnet/index.html|Compiled against 2.102.2.20 drivers])
* '''MsSql'''   ([http://www.microsoft.com/downloads/details.aspx?FamilyID=0856EACB-4362-4B0D-8EDD-AAB15C5E04F5&displaylang=en|Compiled against .Net 2.0 drivers])
* '''MySql'''   ([http://www.mysql.com/downloads/connector/net/|Compiled against 6.2.2.0 drivers])
* '''SqLite'''  ([http://sqlite.phxsoftware.com/|Compiled against 1.0.65.0 drivers])
{BR}

==== Warning ====
(((
The use of the SQL Query Table plugin is a potential security risks. It is the administrators task to ensure that Database links set up, are secure with regards to the following:
* The db user/scheme/catalog used to access the db, '''should ONLY have READ access to tables/views'''.
* The db user/scheme/catalog used to access the db, '''should ONLY have access to exactly the tables/views which are to be queried'''.
* The db user/scheme/catalog, '''should preferbly ONLY contain views'''.
)))
{BR}

==== Guidelines ====
* Always create views, insted of writting the '''where''' clauses in the query.
* Use only '''select * from xx.MyTableView''' if possible. More advanced clauses are messy to read in the wiki markup and are '''NOT''' gaurateed to work when starting to use special chars. Use '''Views''' instead.
{BR}

==== Security ====
The plugin has builtin simple security check's for potential dangerous sql keywords.{BR}
Currently the plugin contains 125 ''forbidden'' keywords.{BR}
[QueryTableFormatterSecurityHelp| Go here to see the forbidden keywords.]{BR}
{BR}

==== Setup ====
In the formatters section of the administation view, click the select button. {BR}
Follow the guidelines in the help section or read on below.{BR}
(((
To create a DB Link use the following format:{BR}
'''{LinkKey=Type,ConnectionString}'''{BR}
Where: {BR}
* '''LinkKey''' - Your chose name, ex test
* '''Type''' - One of the supported DB's (Oracle,MsSql,MySql,SqLite)
* '''ConnectionString''' - Standard connection string
)))
{BR}

'''Examples:''' {BR}
(((
* {MyLink1=Oracle,User Id=MyUser;Password=MyPass;Data Source=TnsName;} 
* {MyLink2=MsSql,Data Source=MyServer\SQLEXPRESS;Initial Catalog=MyCatalog;User ID=MyUser;Password=MyPass;} 
* {MyLink3=MySql,Database=MyDB;Data Source=MyServer;User Id=MyUser;Password=MyPass;} 
* {MyLink4=SqLite,Data Source=MyDB.sqlite;Version=3;} 
)))
{BR}

=== Markup Usage ===
'''What can you do?'''{BR}
* Query the database
* Add a heading to your table
* Show only specified columns
* Change column order
* Override column headers and make them more ''user friendly'' 
* Change table style
* Change columns header style
* Change row style
* Use one of 3 predefined styles '''bw,bg,gb'''
* Combine several of the above options
{BR}

(((
'''Usage:'''{BR}{BR}
'''{QTable(Link,Query,TblHeading,Columns,Headers,TblStyle,HeadStyle,RowStyle)}'''{BR}{BR}
'''Where:''' {BR}
* ''Required:''
** '''Link''' - Must match a link created by the admin, '''ex. MyLink1''', ask your admin for further information.
** '''Query''' - Sql query, '''must be encapsulated in ' ' ex. 'select * from myView' '''.
* ''Optional:''
** '''TblHeading''' - Heading of the table, must be encapsulated in ' ' '''ex. 'My Heading' '''
** '''Columns''' - Columns and column order starting at 0, must be encapsulated in ' ' '''ex. 1,2,3 or 1,0,3'''. 
** '''Headers''' - Columnheaders must match column order, must be encapsulated in ' ' '''ex. Head1,Head,Head3 or Head1,Head0,Head3'''.
** '''TblStyle''' - Style format, must be encapsulated in ' ' '''ex. 'align=""center"" style=""color: #000000;""' '''.
** '''HeadFStyle''' - Style format, must be encapsulated in ' ' '''ex. 'align=""center"" style=""color: #000000;""' '''.
** '''RowStyle''' - Style format, must be encapsulated in ' ' '''ex. 'align=""center"" style=""color: #000000;""' '''.
{BR}
* All ""''','''"" '''must''' always be included in the tag.
)))
{BR}

==== Minimum ====
This will create a table with the default style of your chosen wiki theme. {BR}
(((
'''Markup:'''{BR}{BR}
'''{QTable(Link,Query,,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|  
|+  
! ColName1 !! ColName2  
|-  
| DataCell1 || DataCell2
|}
)))
{BR}

==== Styling ====
'''Default style'''{BR}
Depends on your chosen theme. {BR}
(((
'''Markup:'''{BR}{BR}
'''{QTable(Link,Query,,,,,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|  
|+  
! ColName1 !! ColName2  
|-  
| DataCell1 || DataCell2
|}
)))
{BR}

'''Predefined style: ''Black and White'' '''{BR}
(((
'''Markup:'''{BR}{BR}
'''{QTable(Link,Query,,,,'bw','bw','bw') }''' {BR}{BR}
'''Result:'''{BR}{BR}
{| border=""0"" cellpadding=""2"" cellspacing=""0"" align=""center"" style=""background-color: #EEEEEE;""
|- align=""center"" style=""background-color: #000000; color: #FFFFFF; font-weight: bold;""  
| ColName1 || ColName2  
|- align=""center"" style=""color: #000000;""
| DataCell1 || DataCell2
|}
)))
{BR}

'''Predefined style: ''Black and Grey'' '''{BR}
(((
'''Markup:'''{BR}{BR}
'''{QTable(Link,Query,,,,'bg','bg','bg') }''' {BR}{BR}
'''Result:'''{BR}{BR}
{| border=""0"" cellpadding=""2"" cellspacing=""0"" align=""center"" style=""background-color: #EEEEEE;""
|- align=""center"" style=""background-color: #000000; color: #CCCCCC; font-weight: bold;""  
| ColName1 || ColName2  
|- align=""center;""
| DataCell1 || DataCell2
|}
)))
{BR}

'''Predefined style: ''Green and Black''' ''{BR}
(((
'''Markup:'''{BR}{BR}
'''{QTable(Link,Query,,,,'gb','gb','gb') }''' {BR}{BR}
'''Result:'''{BR}{BR}
{| border=""0"" cellpadding=""2"" cellspacing=""0"" align=""center"" style=""background-color: #EEEEEE;""
|- align=""center"" style=""background-color: #88CC33; color: #000000; font-weight: bold;""  
| ColName1 || ColName2  
|- align=""center"" style=""color: #000000;""
| DataCell1 || DataCell2
|}
)))
{BR}

'''Custom style:'''{BR}
(((
'''Markup:'''{BR}{BR}
'''{QTable(Link,Query,,,,'cellspacing=""10"" style=""background-color: #88CC33; color: #000000;""','style=""color: #00AAAA;""','style=""color: #BBBB00;""') }''' {BR}{BR}
'''Result:'''{BR}{BR}
{| cellspacing=""10"" style=""background-color: #88CC33; color: #000000;""
|- style=""color: #00AAAA;""   
| ColName1 || ColName2  
|- style=""color: #BBBB00;""
| DataCell1 || DataCell2
|}
)))
{BR}

'''Custom heading + headers:'''{BR}
(((
'''Markup:'''{BR}{BR}
'''{QTable(Link,Query,'My heading','1,2','Head1,Head2',,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|  
|+ My heading  
! Head1 !! Head2
|-  
| DataCell1 || DataCell2
|}
)))
{BR}

'''Custom heading + headers + order:'''{BR}
(((
'''Markup:'''{BR}{BR}
'''{QTable(Link,Query,'My heading','2,1','Head2,Head1',,,) }''' {BR}{BR}
'''Result:'''{BR}{BR}
{|  
|+ My heading  
! Head2 !! Head1
|-  
| DataCell2 || DataCell1
|}
)))
{BR}
",
            Description = "QueryTableFormatter Help",
            Keywords = new string[] { "QueryTabelFormatter", "Help", "Formatting", "Markup", "Table", "Sql", "Query", "Oracle", "MsSql", "MySql", "SqLite" }
        },

        new Page()
        {
            Fullname = "QueryTableFormatterSecurityHelp",
            Title = "Query Table Security Help",
            Content = @"The following keywords have been forbidden in the queries for the QueryTableFormatter.{BR}
{TOC}
All words WILL be scanned for ALL types of DB's.{BR}{BR}
== Oracle keywords ==
* ADD
* ALTER
* AUDIT
* CALL
* COMMENT
* COMPRESS
* CREATE
* DELETE
* DROP
* GRANT
* INCREMENT
* INDEX
* INSERT
* INTO
* LOCK
* MODIFY
* PRIVILEGES
* RENAME
* REVOKE
* SET
* START
* TRIGGER
* TRUNCATE
* UPDATE

{BR}
== MsSql keywords ==
* ADD
* ALTER
* CASCADE
* COMMIT
* COMPUTE
* CREATE
* DELETE
* DENY
* DROP
* DUMP
* EXEC
* EXECUTE
* GRANT
* HOLDLOCK
* IDENTITY_INSERT
* INSERT
* INTO
* KILL
* PROC
* PROCEDURE
* RECONFIGURE
* REFERENCES
* REPLICATION
* RESTORE
* RESTRICT
* REVOKE
* ROLLBACK
* RULE
* SAVE
* SET
* SETUSER
* TRANSACTION
* TRIGGER
* TRUNCATE
* USE

{BR}
== MySql keywords ==
* ADD
* ALTER
* CALL
* CASCADE
* CHANGE
* CREATE
* DEC
* DECLARE
* DECLARE
* DELETE
* DROP
* EXIT
* FETCH
* FORCE
* GRANT
* INSERT
* INTO
* KILL
* LOCK
* LOOP
* PROCEDURE
* PURGE
* REFERENCES
* RELEASE
* RENAME
* REPLACE
* REQUIRE
* RESTRICT
* REVOKE
* SET
* TRIGGER
* UNLOCK
* UPDATE
* UPGRADE
* WRITE

{BR}
== SqLite keywords ==
* ABORT
* ACTION
* ADD
* ALTER
* AUTOINCREMENT
* CASCADE
* COMMIT
* CREATE
* DELETE
* DROP
* END
* INSERT
* INSTEAD
* INTO
* REFERENCES
* RELEASE
* RENAME
* REPLACE
* RESTRICT
* ROLLBACK
* SAVEPOINT
* SET
* TRANSACTION
* TRIGGER
* UPDATE
* VACUUM
",
            Description = "QueryTableFormatter forbidden Sql keywords",
            Keywords = new string[] { "QueryTabelFormatter", "Help", "Formatting", "Markup", "Table", "Sql", "Query", "Security", "Forbidden", "Oracle", "MsSql", "MySql", "SqLite" }
        },

        new Page() {
            Fullname = "QueryTableFormatterTest",
            Title = "Query Table Test",
            Content = @"== Page for testing and verifying the QueryTableFormatter ==
Page for verifying the QueryTableFormatter. You may wish to match links and queries to your setup.{BR}
{TOC}
{BR}
== Query ==
===Default===
{BR}
{QTable(test,'select * from schedule',,,,,,)}
{BR}
===Empty Table===
{BR}
{QTable(test,'select * from emptytable',,,,,,)}
{BR}
===Illegal word===
{BR}
{QTable(test,'delete * from emptytable',,,,,,)}
{BR}
===Illegal word in table name(ignore,table missing error)===
{BR}
{QTable(test,'select * from emptydeletetable',,,,,,)}
{BR}
{BR}
== Styling ==
===Default style==={BR}
{BR}
{QTable(test,'select * from schedule',,,,,,)}
{BR}
===Predefined style: ''Black and White'' ===
{BR}
{QTable(test,'select * from schedule',,,,'bw','bw','bw')}
{BR}
===Predefined style: ''Black and Grey'' ===
{BR}
{QTable(test,'select * from schedule',,,,'bg','bg','bg')}
{BR}
===Predefined style: ''Green and Black'' ===
{BR}
{QTable(test,'select * from schedule',,,,'gb','gb','gb')}
{BR}
===Custom style==={BR}
{BR}
{QTable(test,'select * from schedule',,,,'cellspacing=""10"" style=""background-color: #88CC33; color: #000000;""','style=""color: #00AAAA;""','style=""color: #BBBB00;""')}
{BR}
===Custom heading + headers==={BR}
{BR}
{QTable(test,'select * from schedule','My Heading',,'H1,H2,H3,H4',,,)}
{BR}
===Custom heading + headers + order==={BR}
{BR}
{QTable(test,'select * from schedule','My Heading','0,3,2,1','H0,H3,H2,H1',,,)}
{BR}",
            Description = "QueryTableFormatter Test",
            Keywords = new string[] { "QueryTableFormatter", "Test" }
        }
        };
    }
}
