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
            VerifyCSS(_source);
            VerifyTable(_source, _style);
            VerifyColGroup(_source, _headers.Count);
            VerifyHeading(_source, _head, _headers.Count);
            VerifyHeaders(_source, _headers);
            VerifyFooter(_source, _foot, _headers.Count);
            VerifyRows(_source, _rows);
        }

        private static void VerifyCSS(string _source)
        {
            Assert.AreEqual(true, _source.Contains("<link type=\"text/css\" rel=\"stylesheet\" href=\"/public/Plugins/Keeper.Garrett.ScrewTurn.Formatters/TableStyle.css\"></link>"), "Style error");
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
                    expected = string.Format("{0}\n\t\t<col class=\"col-odd\" />", expected);
                }
                else
                {
                    expected = string.Format("{0}\n\t\t<col class=\"col-even\" />", expected);
                }
            }
            expected = string.Format("{0}\n\t</colgroup>", expected, "Table ColGroup, error");

            Assert.AreEqual(true, _source.Contains(expected));
        }

        private static void VerifyHeading(string _source, string _head, int _headerCount)
        {
            if (string.IsNullOrEmpty(_head) == false)
            {
                Assert.AreEqual(true, _source.Contains(string.Format("<thead>\n\t\t<tr>\n\t\t\t<td colspan=\"{0}\" class=\"heading\">{1}</td>\n\t\t</tr>", _headerCount, _head)), "Table Haading, error");
            }
        }

        private static void VerifyHeaders(string _source, List<string> _headers)
        {
            if (_headers.Count == 1)
            {
                Assert.AreEqual(true, _source.Contains(string.Format("<tr>\n\t\t\t<th scope=\"col\" class=\"standard-head\">{0}</th>\n\t\t</tr>\n\t</thead>", _headers[0])), "Table 1 Header, error");
            }
            else if (_headers.Count == 2)
            {
                Assert.AreEqual(true, _source.Contains(string.Format("<tr>\n\t\t\t<th scope=\"col\" class=\"first-head\">{0}</th>\n\t\t\t<th scope=\"col\" class=\"last-head\">{1}</th>\n\t\t</tr>\n\t</thead>"
                    , _headers[0]
                    , _headers[1])), "Table 2 Headers, error");
            }
            else if (_headers.Count == 3)
            {
                Assert.AreEqual(true, _source.Contains(string.Format("<tr>\n\t\t\t<th scope=\"col\" class=\"first-head\">{0}</th>\n\t\t\t<th scope=\"col\" class=\"standard-head\">{1}</th>\n\t\t\t<th scope=\"col\" class=\"last-head\">{2}</th>\n\t\t</tr>\n\t</thead>"
                    , _headers[0]
                    , _headers[1]
                    , _headers[2])), "Table 3 Headers, error");
            }
            else
            {

                Assert.AreEqual(true, _source.Contains(string.Format("<tr>\n\t\t\t<th scope=\"col\" class=\"first-head\">{0}</th>", _headers[0])), "Table First Head, error");

                for (int i = 1; i < _headers.Count - 1; i++)
                {
                    Assert.AreEqual(true, _source.Contains(string.Format("\n\t\t\t<th scope=\"col\" class=\"standard-head\">{0}</th>\n\t\t", _headers[i])), "Table Standard Head, error");
                }

                Assert.AreEqual(true, _source.Contains(string.Format("<th scope=\"col\" class=\"last-head\">{0}</th>\n\t\t</tr>\n\t</thead>", _headers[_headers.Count - 1])), "Table Last Head, error");
            }
        }

        private static void VerifyFooter(string _source, string _foot, int _headerCount)
        {
            if (_headerCount == 1)
            {
                Assert.AreEqual(true, _source.Contains(string.Format("<tfoot>\n\t\t<tr>\n\t\t\t<td class=\"standard-foot\">{0}</td>\n\t\t</tr>\n\t</tfoot>", _foot)), "Table Footer expected, error");
            }
            else
            {
                Assert.AreEqual(true, _source.Contains(string.Format("<tfoot>\n\t\t<tr>\n\t\t\t<td colspan=\"{0}\" class=\"first-foot\">{1}</td>\n\t\t\t<td class=\"last-foot\"/>\n\t\t</tr>\n\t</tfoot>", _headerCount - 1, _foot)), "Table Footer, error");
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
                    row = "\n\t\t<tr class=\"row-odd\">";
                }
                else
                {
                    row = "\n\t\t<tr class=\"row-even\">";
                }

                for(int j = 0; j < _rows[i].Count; j++)
                {
                    row = string.Format("{0}\n\t\t\t<td>{1}</td>",row,_rows[i][j]);
                }

                row = string.Format("{0}\n\t\t</tr>",row);

                Assert.AreEqual(true,_source.Contains(row),string.Format("Table Row {0}, error",i));
            }

            Assert.AreEqual(true, _source.Contains("</tbody>"),"Table Row stop, error");
        }
    }
}
