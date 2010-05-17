using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.Core;
using Keeper.Garrett.ScrewTurn.Utility;

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
* [Keeper-Garrett-Table-Styles|Make all sorts of visual changes to the table generated from the query]{BR}

(((
'''Usage:'''{BR}{BR}
'''{ QTable conn= query= }'''{BR}{BR}
'''Where:''' {BR}
* '''conn''' - Must match a link created by the admin, '''ex. MyLink1''', ask your admin for further information.
* '''query''' - Sql query, '''must be encapsulated in ' ' ex. 'select * from myView' '''.
* To control and modify the display of columns use the [Keeper-Garrett-Table-Styles|table tags found here].

{BR}
'' All args which have a value that contains whitespaces, must be encapsulated in ' ', ex. 'select * from myView'.  ''
)))
{BR}

==== Minimum ====
This will create a table with the default style of your chosen wiki theme. {BR}
(((
'''Markup:'''{BR}{BR}
'''{ QTable conn=MyLink query='select * from myView'}''' {BR}{BR}
'''Result:'''{BR}{BR}
{|  
|+  
! ColName1 !! ColName2  
|-  
| DataCell1 || DataCell2
|}
)))
{BR}

==== Tables and Styling ====
To use [Keeper-Garrett-Table-Styles|tables look here]. 
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
==Default==
{BR}
{QTable conn=test query='select * from schedule'}
{BR}
==Empty Table==
{BR}
{QTable conn=test query='select * from emptytable'}
{BR}
==Illegal word==
{BR}
{QTable conn=test query='delete * from emptytable'}
{BR}
==Illegal word in table name(ignore,table missing error)==
{BR}
{QTable conn=test query='select * from emptydeletetable'}
{BR}",
            Description = "QueryTableFormatter Test",
            Keywords = new string[] { "QueryTableFormatter", "Test" }
        },

        XHtmlTableGenerator.HelpPage
        };
    }
}
