using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using ScrewTurn.Wiki.PluginFramework;
using Keeper.Garrett.ScrewTurn;

namespace Formatters.Tests
{
    public class AssertTable
    {

        public static void VerifyTable(string _source, string _style, string _head, string _foot, List<string> _headers, Dictionary<int, List<string>> _rows)
        {
            if (string.IsNullOrEmpty(_style) == false && _style == "generic")
            {
                VerifyCSSGeneric(_source, _style);
                VerifyTableGeneric(_source, _style);
                VerifyColGroupGeneric(_source, _headers.Count);
                VerifyHeadingGeneric(_source, _head, _headers.Count, _style);
                VerifyHeadersGeneric(_source, _headers, _style);
                VerifyFooterGeneric(_source, _foot, _headers.Count);
                VerifyRowsGeneric(_source, _rows);
            }
            else
            {
                VerifyCSS(_source, _style);
                VerifyTable(_source, _style);
                VerifyColGroup(_source, _headers.Count);
                VerifyHeading(_source, _head, _headers.Count, _style);
                VerifyHeaders(_source, _headers, _style);
                VerifyFooter(_source, _foot, _headers.Count);
                VerifyRows(_source, _rows);
            }
        }

        private static void VerifyCSS(string _source, string _style)
        {
            if (string.IsNullOrEmpty(_style) == false)
            {
                Assert.AreEqual(true, _source.Contains("<link type=\"text/css\" rel=\"stylesheet\" href=\"GetFile.aspx?File=/Keeper.Garrett.Formatters/Tables/TableStyle.css\"></link>"), "Style error");
            }
            else 
            {
                Assert.AreEqual(false, _source.Contains("<link type=\"text/css\" rel=\"stylesheet\" href=\"GetFile.aspx?File=/Keeper.Garrett.Formatters/Tables/TableStyle.css\"></link>"), "Style error");
            }
        }


        private static void VerifyTable(string _source, string _style)
        {
            if (string.IsNullOrEmpty(_style) == false)
            {
                Assert.AreEqual(true, _source.Contains(string.Format("<table id=\"{0}\">",_style)), "Table id=, error");
            }
            else
            {
                Assert.AreEqual(true, _source.Contains("<table id=\"default\">"), "Table id=, error");
            }

            Assert.AreEqual(true, _source.Contains("</table>"), "Table tag close, error");
        }

        private static void VerifyColGroup(string _source, int _headerCount)
        {
            var expected = "<colgroup>";
            for (int i = 0; i < _headerCount; i++)
            {
                if (i % 2 == 0)
                {
                    expected = string.Format("{0}<col class=\"col-odd\"/>", expected);
                }
                else
                {
                    expected = string.Format("{0}<col class=\"col-even\"/>", expected);
                }
            }
            expected = string.Format("{0}</colgroup>", expected, "Table ColGroup, error");

            Assert.AreEqual(true, _source.Contains(expected));
        }

        private static void VerifyHeading(string _source, string _head, int _headerCount, string _style)
        {
            if (string.IsNullOrEmpty(_head) == false)
            {
                Assert.AreEqual(true, _source.Contains(string.Format("<thead><tr><td colspan=\"{0}\" class=\"heading\">{1}</td></tr>", _headerCount, _head)), "Table Haading, error");
            }
        }

        private static void VerifyHeaders(string _source, List<string> _headers, string _style)
        {
            if (_headers.Count == 1)
            {
                Assert.AreEqual(true, _source.Contains(string.Format("<tr><th scope=\"col\" class=\"standard-head\">{0}</th></tr></thead>", _headers[0])), "Table 1 Header, error");
            }
            else if (_headers.Count == 2)
            {
                Assert.AreEqual(true, _source.Contains(string.Format("<tr><th scope=\"col\" class=\"first-head\">{0}</th><th scope=\"col\" class=\"last-head\">{1}</th></tr></thead>"
                    , _headers[0]
                    , _headers[1])), "Table 2 Headers, error");
            }
            else if (_headers.Count == 3)
            {
                Assert.AreEqual(true, _source.Contains(string.Format("<tr><th scope=\"col\" class=\"first-head\">{0}</th><th scope=\"col\" class=\"standard-head\">{1}</th><th scope=\"col\" class=\"last-head\">{2}</th></tr></thead>"
                    , _headers[0]
                    , _headers[1]
                    , _headers[2])), "Table 3 Headers, error");
            }
            else
            {

                Assert.AreEqual(true, _source.Contains(string.Format("<tr><th scope=\"col\" class=\"first-head\">{0}</th>", _headers[0])), "Table First Head, error");

                for (int i = 1; i < _headers.Count - 1; i++)
                {
                    Assert.AreEqual(true, _source.Contains(string.Format("<th scope=\"col\" class=\"standard-head\">{0}</th>", _headers[i])), "Table Standard Head, error");
                }

                Assert.AreEqual(true, _source.Contains(string.Format("<th scope=\"col\" class=\"last-head\">{0}</th></tr></thead>", _headers[_headers.Count - 1])), "Table Last Head, error");
            }
        }

        private static void VerifyFooter(string _source, string _foot, int _headerCount)
        {
            if (_headerCount == 1)
            {
                if (string.IsNullOrEmpty(_foot) == false)
                {
                    Assert.AreEqual(true, _source.Contains(string.Format("<tfoot><tr><td class=\"standard-foot\">{0}</td></tr></tfoot>", _foot)), "Table Footer expected, error");
                }
                else
                {
                    Assert.AreEqual(false, _source.Contains(string.Format("<tfoot><tr><td class=\"standard-foot\">{0}</td></tr></tfoot>", _foot)), "Table Footer expected, error");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(_foot) == false)
                {
                    Assert.AreEqual(true, _source.Contains(string.Format("<tfoot><tr><td colspan=\"{0}\" class=\"first-foot\">{1}</td><td class=\"last-foot\"/></tr></tfoot>", _headerCount - 1, _foot)), "Table Footer, error");
                }
                else
                {
                    Assert.AreEqual(false, _source.Contains(string.Format("<tfoot><tr><td colspan=\"{0}\" class=\"first-foot\">{1}</td><td class=\"last-foot\"/></tr></tfoot>", _headerCount - 1, _foot)), "Table Footer, error");
                }
            }
        }

        private static void VerifyRows(string _source, Dictionary<int, List<string>> _rows)
        {
            Assert.AreEqual(true, _source.Contains("<tbody>"), "Table Row start, error");

            for(int i = 0; i < _rows.Count; i++)
            {
                string row = "";

                if(i % 2 == 0)
                {
                    row = "<tr class=\"row-odd\">";
                }
                else
                {
                    row = "<tr class=\"row-even\">";
                }

                for(int j = 0; j < _rows[i].Count; j++)
                {
                    row = string.Format("{0}<td>{1}</td>",row,_rows[i][j]);
                }

                row = string.Format("{0}</tr>",row);

                Assert.AreEqual(true,_source.Contains(row),string.Format("Table Row {0}, error",i));
            }

            Assert.AreEqual(true, _source.Contains("</tbody>"),"Table Row stop, error");
        }

        #region Generic
        private static void VerifyCSSGeneric(string _source, string _style)
        {
            Assert.AreEqual(true, _source.Contains("<link type=\"text/css\" rel=\"stylesheet\" href=\"GetFile.aspx?File=/Keeper.Garrett.Formatters/Tables/TableStyle.css\"></link>"), "Style error");
        }

        private static void VerifyTableGeneric(string _source, string _style)
        {
            if (string.IsNullOrEmpty(_style) == false)
            {
                Assert.AreEqual(true, _source.Contains(string.Format("<table id=\"{0}\">", _style)), "Table id=, error");
            }
            else
            {
                Assert.AreEqual(true, _source.Contains("<table id=\"default\">"), "Table id=, error");
            }

            Assert.AreEqual(true, _source.Contains("</table>"), "Table tag close, error");
        }

        private static void VerifyColGroupGeneric(string _source, int _headerCount)
        {
            var expected = "<colgroup>";
            for (int i = 0; i < _headerCount; i++)
            {
                if (i % 2 == 0)
                {
                    expected = string.Format("{0}<col />", expected);
                }
                else
                {
                    expected = string.Format("{0}<col />", expected);
                }
            }
            expected = string.Format("{0}</colgroup>", expected, "Table ColGroup, error");

            Assert.AreEqual(true, _source.Contains(expected));
        }

        private static void VerifyHeadingGeneric(string _source, string _head, int _headerCount, string _style)
        {
            if (string.IsNullOrEmpty(_head) == false)
            {
                Assert.AreEqual(true, _source.Contains(string.Format("<thead><tr><td colspan=\"{0}\" class=\"tableheader\">{1}</td></tr>", _headerCount, _head)), "Table Haading, error");
            }
        }

        private static void VerifyHeadersGeneric(string _source, List<string> _headers, string _style)
        {
            if (_headers.Count == 1)
            {
                Assert.AreEqual(true, _source.Contains(string.Format("<tr><th scope=\"col\" class=\"tableheader\">{0}</th></tr></thead>", _headers[0])), "Table 1 Header, error");
            }
            else if (_headers.Count == 2)
            {
                Assert.AreEqual(true, _source.Contains(string.Format("<tr><th scope=\"col\" class=\"tableheader\">{0}</th><th scope=\"col\" class=\"tableheader\">{1}</th></tr></thead>"
                    , _headers[0]
                    , _headers[1])), "Table 2 Headers, error");
            }
            else if (_headers.Count == 3)
            {
                Assert.AreEqual(true, _source.Contains(string.Format("<tr><th scope=\"col\" class=\"tableheader\">{0}</th><th scope=\"col\" class=\"tableheader\">{1}</th><th scope=\"col\" class=\"tableheader\">{2}</th></tr></thead>"
                    , _headers[0]
                    , _headers[1]
                    , _headers[2])), "Table 3 Headers, error");
            }
            else
            {

                Assert.AreEqual(true, _source.Contains(string.Format("<tr><th class=\"tableheader\">{0}</th>", _headers[0])), "Table First Head, error");

                for (int i = 1; i < _headers.Count - 1; i++)
                {
                    Assert.AreEqual(true, _source.Contains(string.Format("<th class=\"tableheader\">{0}</th>", _headers[i])), "Table Standard Head, error");
                }

                Assert.AreEqual(true, _source.Contains(string.Format("<th class=\"tableheader\">{0}</th></tr></thead>", _headers[_headers.Count - 1])), "Table Last Head, error");
            }
        }

        private static void VerifyFooterGeneric(string _source, string _foot, int _headerCount)
        {
            if (_headerCount == 1)
            {
                if (string.IsNullOrEmpty(_foot) == false)
                {
                    Assert.AreEqual(true, _source.Contains(string.Format("<tfoot><tr><td class=\"standard-foot\">{0}</td></tr></tfoot>", _foot)), "Table Footer expected, error");
                }
                else
                {
                    Assert.AreEqual(false, _source.Contains(string.Format("<tfoot><tr><td class=\"standard-foot\">{0}</td></tr></tfoot>", _foot)), "Table Footer expected, error");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(_foot) == false)
                {
                    Assert.AreEqual(true, _source.Contains(string.Format("<tfoot><tr><td colspan=\"{0}\" class=\"first-foot\">{1}</td><td class=\"last-foot\"/></tr></tfoot>", _headerCount - 1, _foot)), "Table Footer, error");
                }
                else
                {
                    Assert.AreEqual(false, _source.Contains(string.Format("<tfoot><tr><td colspan=\"{0}\" class=\"first-foot\">{1}</td><td class=\"last-foot\"/></tr></tfoot>", _headerCount - 1, _foot)), "Table Footer, error");
                }
            }
        }

        private static void VerifyRowsGeneric(string _source, Dictionary<int, List<string>> _rows)
        {
            Assert.AreEqual(true, _source.Contains("<tbody>"), "Table Row start, error");

            for (int i = 0; i < _rows.Count; i++)
            {
                string row = "";

                if (i % 2 == 0)
                {
                    row = "<tr class=\"tablerow\">";
                }
                else
                {
                    row = "<tr class=\"tablerowalternate\">";
                }

                for (int j = 0; j < _rows[i].Count; j++)
                {
                    row = string.Format("{0}<td>{1}</td>", row, _rows[i][j]);
                }

                row = string.Format("{0}</tr>", row);

                Assert.AreEqual(true, _source.Contains(row), string.Format("Table Row {0}, error", i));
            }

            Assert.AreEqual(true, _source.Contains("</tbody>"), "Table Row stop, error");
        }
        #endregion
    }
}
