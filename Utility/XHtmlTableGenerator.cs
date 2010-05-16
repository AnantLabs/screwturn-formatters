using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using ScrewTurn.Wiki.PluginFramework;
using Keeper.Garrett.ScrewTurn.Core;

namespace Keeper.Garrett.ScrewTurn.Utility
{
    public class XHtmlTableGenerator
    {
        #region Help Page
        public static readonly Page HelpPage = new Page()
        {
            Description = "",
            Fullname = "",
            Keywords = new string[] { ""},
            Title = "",
            Content = @"=='''Table Styles'''==
This page describes the usage of tables and their syntax when working with the [http://keeper.endoftheinternet.org|Keeper Garrett Formatters].{BR}
{TOC}
{BR}
=== Support By ===
Currently 4 of the formatters uses the table styles, they are:{BR}
* [CategoryListFormatterHelp|CategoryListFormatter]
* [EventLogFormatterHelp|EventLogFormatter]
* [FileListFormatterHelp|FileListFormatter]
* [QueryTableFormatterHelp|QueryTableFormatter]
{BR}

=== Markup Usage ===
'''What can you do?'''{BR}
* Add a heading to your table
* Add a footer to your table
* Show only specified columns
* Change column order
* Override column headers 
* Use one of the 11 predefined styles 
* Define your own style
{BR}

(((
'''Usage:'''{BR}{BR}
'''{SomeFormatter( ...Formatter_Specific_Args=XX... head=XXX foot=YYY cols=ZZZ colnames=XYZ style=ZXY}'''{BR}{BR}
'''Where:''' {BR}
* '''head''' - Heading of the table
* '''foot''' - Footer of the table
* '''cols''' - Name and order of the columns to show, must be seperated by ,. Ex. 'col1,col2,col3'
* '''colnames''' - Custom column names, must be seperated by ,. Ex. 'col1,col2,col3'
* '''style''' - Name of one of the predefined styles, if not specified the default wiki style is used.

''All args which have a value that contains whitespaces, must be encapsulated in ' ', ex. 'My Heading'.''
{BR}
)))
{BR}

==== Add A Heading ====
(((
'''Markup:'''{BR}{BR}
'''{SomeFormatter( ... head='My Heading'}''' {BR}{BR}
'''Result:'''{BR}{BR}
{|  
|+ My Heading 
! Col1 !! Col2  
|-  
| DataCell1 || DataCell2
|}
)))
{BR}

==== Add A Footer ====
(((
'''Markup:'''{BR}{BR}
'''{SomeFormatter( ... foot='My Footer'}''' {BR}{BR}
'''Result:'''{BR}{BR}
{|  
! Col1 !! Col2  
|-  
| DataCell1 || DataCell2
|}
)))
{BR}

==== Change Column Order and Display ====
(((
'''Markup:'''{BR}{BR}
'''{SomeFormatter( ... cols='Col1,Col3,Col2'}''' {BR}{BR}
'''Result:'''{BR}{BR}
{|  
|+ 
! Col1 !! Col3 !! Col2
|-  
| DataCell1 || DataCell3 || DataCell2
|}
{BR}
''Here a 4 column called Col4 is omitted since it is not mentioned.''
)))
{BR}

==== Change Column Names ====
(((
'''Markup:'''{BR}{BR}
'''{SomeFormatter( ... colnames='X,Y,Z'}''' {BR}{BR}
'''Result:'''{BR}{BR}
{|  
|+ 
! X !! Y !! Z
|-  
| DataCell1 || DataCell2 || DataCell3
|}
)))

'''OR'''

(((
'''Markup:'''{BR}{BR}
'''{SomeFormatter( ... cols='Col1,Col3,Col2' colnames='X,Y,Z'}''' {BR}{BR}
'''Result:'''{BR}{BR}
{|  
|+ 
! X !! Y !! Z
|-  
| DataCell1 || DataCell3 || DataCell2
|}
)))
{BR}

==== Table Styling ====
(((
'''Markup:'''{BR}{BR}
'''{SomeFormatter( ... style='hor-minimalist-a'}''' {BR}{BR}
'''Result:'''
<link type=""text/css"" rel=""stylesheet"" href=""GetFile.aspx?File=/Keeper.Garrett.Formatters/Tables/TableStyle.css""></link>
<table id=""hor-minimalist-a"" summary=""Summary""> 
	<colgroup> 
		<col class=""col-odd"" /> 
		<col class=""col-even"" /> 
		<col class=""col-odd"" /> 
	</colgroup> 
 
	<thead> 
		<tr> 
			<td colspan=""3"" class=""heading"">hor-minimalist-a</td> 
		</tr> 
		<tr> 
			<th scope=""col"" class=""first-head"">Head1</th> 
			<th scope=""col"" class=""standard-head"">Head2</th> 
			<th scope=""col"" class=""last-head"">Head3</th> 
		</tr> 
	</thead> 
 
	<tfoot> 
		<tr> 
			<td colspan=""2"" class=""first-foot"">My Footer</td> 
			<td class=""last-foot""/> 
		</tr> 
	</tfoot> 
 
	<tbody> 
		<tr class=""row-odd""> 
			<td>Col1</td> 
			<td>Col2</td> 
			<td>Col3</td> 
		</tr> 
		<tr class=""row-even""> 
			<td>Col1</td> 
			<td>Col2</td> 
			<td>Col3</td> 
		</tr> 
		<tr class=""row-odd""> 
			<td>Col1</td> 
			<td>Col2</td> 
			<td>Col3</td> 
		</tr> 
	</tbody> 
</table> 
)))
{BR}
If the style argument is not supplied the default wiki table style is used.{BR}
There are 11 predefined styles which are bundled with all the formatters (which uses tables).{BR}{BR}
'''Predefined table styles:'''
# hor-minimalist-a
# hor-minimalist-b
# ver-minimalist
# box-table-a
# box-table-b
# ver-zebra
# hor-zebra-a
# hor-zebra-b
# rounded-corner
# background-image
# gradient-style

'''All these styles are defined in the TableStyle.css file located in default file storage provider on the following path:'''{BR}
[{UP}/Keeper.Garrett.Formatters/Tables/TableStyle.css|/Keeper.Garrett.Formatters/Tables/TableStyle.css] 
{BR}
'''So to create your own custom style simply add your CSS to this file.'''
{BR}

'''For a visual preview go here, or look further below (requires the FileContentFormatter):'''{BR}
[{UP}/Keeper.Garrett.Formatters/Tables/table-examples.html|/Keeper.Garrett.Formatters/Tables/table-examples.html]
{BR}
'''Examples:'''{BR}
{FileCont file='/Keeper.Garrett.Formatters/Tables/table-examples.html'}"
        };
        #endregion

        public static void StoreFiles(IHostV30 _host, string _formatter)
        {
            var resourcePath = string.Format("Keeper.Garrett.ScrewTurn.{0}.Resources.Tables",_formatter);

            //Setup store files
            var persister = new FilePersister("Tables");
            persister.AddDir("Images");
            persister.AddFile("/", "TableStyle.css",        string.Format("{0}.TableStyle.css",resourcePath));
            persister.AddFile("/", "table-examples.html",   string.Format("{0}.table-examples.html", resourcePath));
            persister.AddFile("Images", "back.png",         string.Format("{0}.Images.back.png", resourcePath));
            persister.AddFile("Images", "left.png",         string.Format("{0}.Images.left.png", resourcePath));
            persister.AddFile("Images", "blurry.jpg",       string.Format("{0}.Images.blurry.jpg", resourcePath));
            persister.AddFile("Images", "botleft.png",      string.Format("{0}.Images.botleft.png",resourcePath));
            persister.AddFile("Images", "botright.png",     string.Format("{0}.Images.botright.png",resourcePath));
            persister.AddFile("Images", "gradback.png",     string.Format("{0}.Images.gradback.png",resourcePath));
            persister.AddFile("Images", "gradhead.png",     string.Format("{0}.Images.gradhead.png",resourcePath));
            persister.AddFile("Images", "gradhover.png",    string.Format("{0}.Images.gradhover.png",resourcePath));
            persister.AddFile("Images", "left.png",         string.Format("{0}.Images.left.png",resourcePath));
            persister.AddFile("Images", "right.png",        string.Format("{0}.Images.right.png", resourcePath));
            persister.StoreFiles(_host);
        }

        public static string GenerateTable(Dictionary<int, List<string>> _result, string _tblHeading, string _tblFooter, List<int> _columnsToShow, List<string> _customHeaders, List<string> _actualHeaders, string _style)
        {
            string retval = null;

            if (_result.Count > 0) //Only good data
            {
                //Update headers (with regards to the columns to show)
                string tableHeader = GenerateTableHeaders(_columnsToShow, _customHeaders, _actualHeaders, _tblHeading, _tblFooter);

                //Start building table                                                   
                retval = string.Format("<link type=\"text/css\" rel=\"stylesheet\" href=\"GetFile.aspx?File=/Keeper.Garrett.Formatters/Tables/TableStyle.css\"></link>");
                retval = string.Format("{0}\n<table id=\"{1}\">{2}\n\n\t<tbody>", retval, (string.IsNullOrEmpty(_style) == true ? "default" : _style), tableHeader);

                //Each row
                for (int i = 0; i < _result.Values.Count; i++)
                {
                    var row = _result[i];

                    if (i % 2 == 0)
                    {
                        retval = string.Format("{0}\n\t\t<tr class=\"row-odd\">", retval);
                    }
                    else
                    {
                        retval = string.Format("{0}\n\t\t<tr class=\"row-even\">", retval);
                    }

                    //Each Column requested (all or custom)
                    for (int j = 0; j < _columnsToShow.Count; j++)
                    {
                        var columnToUse = _columnsToShow[j];
                        //Add only value if in range of the columns to show
                        if (columnToUse < row.Count)
                        {
                            retval = string.Format("{0}\n\t\t\t<td>{1}</td>", retval, row[columnToUse]);
                        }
                    }

                    //Close tr
                    retval = string.Format("{0}\n\t\t</tr>", retval);
                }

                //Close table
                retval = string.Format("{0}\n\t</tbody>\n</table>", retval);
            }

            return retval;
        }

        private static string GenerateTableHeaders(List<int> _columnsToShow, List<string> _customHeaders, List<string> _actualHeaders, string _heading, string _footer)
        {
            var headers = new Dictionary<int,string>();

            //Custom order and columns
            for(int i = 0; i < _columnsToShow.Count; i++)
            {
                //Custom headers (must match 1 - 1)
                if (_columnsToShow[i] < _customHeaders.Count)
                {
                    headers.Add(i, _customHeaders[_columnsToShow[i]].Trim());
                 //Fill possible ends with actual header IF avaliable, does not match 1 - 1
                }//When no custom headers or order apply the columnsToShow wil be linear 1,2,3,4,5, based on actual header count
                else if(_columnsToShow[i] < _actualHeaders.Count)
                {
                    headers.Add(i, _actualHeaders[_columnsToShow[i]].Trim());
                }
                else
                {
                    headers.Add(i,"?Missing Header?");
                }
            }

            //Header formatting
            string heading = "";
            string colGroup = "";
            string headerGroup = "";
            string footerGroup = "";

            //heading
            if (string.IsNullOrEmpty(_heading) == false)
            {
                heading = string.Format("\n\t\t<tr>\n\t\t\t<td colspan=\"{0}\" class=\"heading\">{1}</td>\n\t\t</tr>", headers.Count, _heading);
            }

            //colgroup
            colGroup = "\n\t<colgroup>";
            //thead
            headerGroup = "\n\t<thead>";
            headerGroup = string.Format("{0}{1}\n\t\t<tr>", headerGroup, (string.IsNullOrEmpty(heading) == false ? heading : ""));
            //tfoot
            footerGroup = "\n\t<tfoot>";
            footerGroup = string.Format("{0}\n\t\t<tr>", footerGroup);

            for(int i = 0; i < headers.Count; i++)
            {
                //First header AND we have at least 2
                if (i == 0 && headers.Count >= 2)
                {
                    headerGroup = string.Format("{0}\n\t\t\t<th scope=\"col\" class=\"first-head\">{1}</th>", headerGroup, headers[i]);

                    footerGroup = string.Format("{0}\n\t\t\t<td colspan=\"{1}\" class=\"first-foot\">{2}</td>", footerGroup, headers.Count - 1, _footer);
                }//Last header
                else if (i == (headers.Count - 1) && headers.Count >= 2)
                {
                    headerGroup = string.Format("{0}\n\t\t\t<th scope=\"col\" class=\"last-head\">{1}</th>", headerGroup, headers[i]);

                    footerGroup = string.Format("{0}\n\t\t\t<td class=\"last-foot\"/>", footerGroup);
                }//All else AND if only 1
                else
                {
                    headerGroup = string.Format("{0}\n\t\t\t<th scope=\"col\" class=\"standard-head\">{1}</th>", headerGroup, headers[i]);

                    if (i == 0 && headers.Count == 1)
                    {
                        footerGroup = string.Format("{0}\n\t\t\t<td class=\"standard-foot\">{1}</td>", footerGroup, _footer);
                    }
                }

                if (i % 2 == 0)
                {
                    colGroup = string.Format("{0}\n\t\t<col class=\"col-odd\" />", colGroup);
                }
                else
                {
                    colGroup = string.Format("{0}\n\t\t<col class=\"col-even\" />", colGroup);
                }
            }
            //colgroup
            colGroup = string.Format("{0}\n\t</colgroup>", colGroup);
            //thead
            headerGroup = string.Format("{0}\n\t\t</tr>", headerGroup);
            headerGroup = string.Format("{0}\n\t</thead>", headerGroup);
            //tfoot
            footerGroup = string.Format("{0}\n\t\t</tr>", footerGroup);
            footerGroup = string.Format("{0}\n\t</tfoot>", footerGroup);

            //Combine
            return string.Format("{0}\n{1}\n{2}", colGroup, headerGroup, footerGroup);
        }

        public static void GenerateColumnsAndColumnNames(Dictionary<string, int> _colsKeyNamesDict, List<string> _allColNames, string _defaultColNames, string _cols, string _colNames, out List<int> _newCols, out List<string> _newColNames)
        {
            _newCols = new List<int>();
            _newColNames = new List<string>();

            //Setup the colnames
            foreach (var colName in _allColNames)
            {
                _newColNames.Add(string.Format("{0}{1}", colName.Substring(0,1).ToUpper(), colName.Substring(1)));
            }

            //Setup column order id's
            if (_cols.ToLower() == "all")
            {
                foreach (var key in _colsKeyNamesDict.Values)
                {
                    _newCols.Add(key);
                }
            }
            else
            {
                var tmpColumnsIds = _cols.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var str in tmpColumnsIds)
                {
                    var key = str.ToLower();
                    if (_colsKeyNamesDict.ContainsKey(key) == true)
                    {
                        _newCols.Add(_colsKeyNamesDict[key]);
                    }
                }

                //Failsafe if there are no cols found -> Use defaults
                if (_newCols.Count <= 0)
                {
                    var tmpColIds = _defaultColNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var str in tmpColIds)
                    {
                        var key = str.ToLower();
                        if (_colsKeyNamesDict.ContainsKey(key) == true)
                        {
                            _newCols.Add(_colsKeyNamesDict[key]);
                        }
                    }
                }
            }

            var tmpColumnNames = _colNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tmpColumnNames.Length && i < _newCols.Count; i++)
            {
                _newColNames[_newCols[i]] = tmpColumnNames[i];
            }
        }
    }
}
