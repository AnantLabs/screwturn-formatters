using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Keeper.Garrett.ScrewTurn.Utility
{
    public class TableGenerator
    {
        //Builtin table styles
        public static readonly string BlackWhite_Tabel_1 = "border=\"0\" cellpadding=\"2\" cellspacing=\"0\" align=\"center\" style=\"background-color: #EEEEEE;\"";
        public static readonly string BlackWhite_Head_1 = "align=\"center\" style=\"background-color: #000000; color: #FFFFFF; font-weight: bold;\"";
        public static readonly string BlackWhite_Row_1 = "align=\"center\" style=\"color: #000000;\"";

        public static readonly string BlackGrey_Tabel_2 = "border=\"0\" cellpadding=\"2\" cellspacing=\"0\" align=\"center\" style=\"background-color: #EEEEEE;\"";
        public static readonly string BlackGrey_Head_2 = "align=\"center\" style=\"background-color: #000000; color: #CCCCCC; font-weight: bold;\"";
        public static readonly string BlackGrey_Row_2 = "align=\"center\"";

        public static readonly string GreenBlack_Tabel_3 = "border=\"0\" cellpadding=\"2\" cellspacing=\"0\" align=\"center\" style=\"background-color: #EEEEEE;\"";
        public static readonly string GreenBlack_Head_3 = "align=\"center\" style=\"background-color: #88CC33; color: #000000; font-weight: bold;\"";
        public static readonly string GreenBlack_Row_3 = "align=\"center\" style=\"color: #000000;\"";

        public static string GenerateTable(Dictionary<int, List<string>> _result, string _tblHeading, List<int> _columnsToShow, List<string> _customHeaders, List<string> _actualHeaders, string _tblFormat, string _headFormat, string _rowFormat)
        {
            string retval = null;

            CheckForWarning(ref _result);

            if (_columnsToShow.Count <= 0 && _actualHeaders.Count <= 0)
            {
                _actualHeaders.Add("Result");
            }

            //Check for styling
            ApplyStyles(ref _tblFormat, ref _headFormat, ref _rowFormat);

            if (_result.Count > 0) //Only good data
            {
                //Update columns to show, add all if no override specified
                if (_columnsToShow.Count <= 0)
                {
                    for (int i = 0; i < _actualHeaders.Count; i++)
                    {
                        _columnsToShow.Add(i);
                    }
                }

                //Update headers (with regards to the columns to show)
                string headAndFormat = GenerateTableHeader(_columnsToShow, _customHeaders, _actualHeaders, _headFormat);

                //Start building table
                retval = string.Format("{{| {0} \n|+ {1} \n{2}", _tblFormat.ToString(), _tblHeading.ToString(), headAndFormat);

                //Each row
                foreach (var row in _result.Values)
                {
                    retval = string.Format("{0}|- {1} \n", retval, _rowFormat);

                    //Each Column requested (all or custom)
                    for (int i = 0; i < _columnsToShow.Count; i++)
                    {
                        var columnToUse = _columnsToShow[i];
                        //Add only value if in range of the columns to show
                        if (columnToUse < row.Count)
                        {
                            retval = string.Format("{0}| {1} |", retval, row[columnToUse]);
                        }
                    }

                    //Delete last splitter
                    retval = string.Format("{0}\n", retval.Remove(retval.Length - 1));

                }

                //Close table
                retval = string.Format("{0}|}}", retval);
            }

            return retval;
        }

        private static void CheckForWarning(ref Dictionary<int, List<string>> _result)
        {
            var listToInsert = new List<string>();

            if (_result.ContainsKey(int.MinValue) == true)
            {
                listToInsert = _result[int.MinValue];
                _result.Remove(int.MinValue);
            }

            if (_result.ContainsKey(int.MinValue + 1) == true)
            {
                listToInsert = _result[int.MinValue + 1];
                _result.Remove(int.MinValue + 1);
            }

            if (_result.ContainsKey(int.MinValue + 2) == true)
            {
                listToInsert = _result[int.MinValue + 2];
                _result.Remove(int.MinValue + 2);
            }

            if (listToInsert.Count > 0)
            {
                _result.Add(_result.Count, listToInsert);
            }
        }

        private static string GenerateTableHeader(List<int> _columnsToShow, List<string> _customHeaders, List<string> _actualHeaders, string _headFormat)
        {
            string retval = "";

            var headers = new Dictionary<int,string>();

            //Custom order and columns
            for(int i = 0; i < _columnsToShow.Count; i++)
            {
                //Custom headers (must match 1 - 1)
                if(i < _customHeaders.Count)
                {
                    headers.Add(i,_customHeaders[i].Trim());
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
            //Are there special formatting?
            if(string.IsNullOrEmpty(_headFormat) == false)
            {
                retval = string.Format("|- {0} \n",_headFormat);

                foreach(var header in headers)
                {
                    retval = string.Format("{0}| {1} |", retval, header.Value);
                }
            }
            else
            {
                foreach(var header in headers)
                {
                    retval = string.Format("{0}! {1} !", retval, header.Value);
                }
            }

            //Remove last splitter
            retval = string.Format("{0}\n", retval.Remove(retval.Length - 1));

            return retval;
        }

        private static void ApplyStyles(ref string _tblFormat, ref string _headFormat, ref string _rowFormat)
        {
            //Check if any of the builtin styles are wanted, and apply
            //Theme 1 Black/White
            _tblFormat = (_tblFormat.ToLower() == "bw" ? BlackWhite_Tabel_1 : _tblFormat);
            _headFormat = (_headFormat.ToLower() == "bw" ? BlackWhite_Head_1 : _headFormat);
            _rowFormat = (_rowFormat.ToLower() == "bw" ? BlackWhite_Row_1 : _rowFormat);
            //Theme 2 Black/Grey
            _tblFormat = (_tblFormat.ToLower() == "bg" ? BlackGrey_Tabel_2 : _tblFormat);
            _headFormat = (_headFormat.ToLower() == "bg" ? BlackGrey_Head_2 : _headFormat);
            _rowFormat = (_rowFormat.ToLower() == "bg" ? BlackGrey_Row_2 : _rowFormat);
            //Theme 3 Green/Black
            _tblFormat = (_tblFormat.ToLower() == "gb" ? GreenBlack_Tabel_3 : _tblFormat);
            _headFormat = (_headFormat.ToLower() == "gb" ? GreenBlack_Head_3 : _headFormat);
            _rowFormat = (_rowFormat.ToLower() == "gb" ? GreenBlack_Row_3 : _rowFormat);
        }
    }
}
