using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Keeper.Garrett.ScrewTurn.Utility
{
    public class XHtmlTableGenerator
    {
        public static string GenerateTable(Dictionary<int, List<string>> _result, string _tblHeading, string _tblFooter, List<int> _columnsToShow, List<string> _customHeaders, List<string> _actualHeaders, string _style)
        {
            string retval = null;

          /*  if (_columnsToShow.Count <= 0 && _actualHeaders.Count <= 0)
            {
                _actualHeaders.Add("Result");
            }*/

            if (_result.Count > 0) //Only good data
            {
                //Update columns to show, add all if no override specified
              /*  if (_columnsToShow.Count <= 0)
                {
                    for (int i = 0; i < _actualHeaders.Count; i++)
                    {
                        _columnsToShow.Add(i);
                    }
                }*/

                //Update headers (with regards to the columns to show)
                string tableHeader = GenerateTableHeaders(_columnsToShow, _customHeaders, _actualHeaders, _tblHeading, _tblFooter);

                //Start building table
                retval = string.Format("<link type=\"text/css\" rel=\"stylesheet\" href=\"/public/Plugins/Keeper.Garrett.ScrewTurn.Formatters/TableStyle.css\"></link>");
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
