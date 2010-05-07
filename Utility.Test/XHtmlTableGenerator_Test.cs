using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using ScrewTurn.Wiki.PluginFramework;
using Keeper.Garrett.ScrewTurn;
using Keeper.Garrett.ScrewTurn.Utility;

namespace UnitTest
{
    [TestFixture]
    [Category("Utility")]
    public class XHtmlTableGenerator_Test
    {
        [Test]
        public void Null_Arguments()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>(), new List<string>(), new List<string>() { "Head1","Head2","Head3" },null);

            //Assert
            Assert.AreEqual("", result);
        }


        [Test]
        public void NoOfColoumns_1()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>(), new List<string>(), new List<string>() { "Head1" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup>\n\t\t<col class=\"col-odd\" />\n\t</colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead>\n\t\t<tr>\n\t\t\t<th scope=\"col\" class=\"standard-head\">Head1</th>\n\t\t</tr>\n\t</thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot>\n\t\t<tr>\n\t\t\t<td class=\"standard-foot\"></td>\n\t\t</tr>\n\t</tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t</tr>\n\t\t<tr class=\"row-even\">\n\t\t\t<td>Col1</td>\n\t\t</tr>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t</tr>\n\t</tbody>"));
        }

        [Test]
        public void NoOfColoumns_2()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>(), new List<string>(), new List<string>() { "Head1","Head2" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup>\n\t\t<col class=\"col-odd\" />\n\t\t<col class=\"col-even\" />\n\t</colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead>\n\t\t<tr>\n\t\t\t<th scope=\"col\" class=\"first-head\">Head1</th>\n\t\t\t<th scope=\"col\" class=\"last-head\">Head2</th>\n\t\t</tr>\n\t</thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot>\n\t\t<tr>\n\t\t\t<td colspan=\"1\" class=\"first-foot\"></td>\n\t\t\t<td class=\"last-foot\"/>\n\t\t</tr>\n\t</tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t</tr>\n\t\t<tr class=\"row-even\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t</tr>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t</tr>\n\t</tbody>"));
        }

        [Test]
        public void NoOfColoumns_3()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>(), new List<string>(), new List<string>() { "Head1", "Head2", "Head3" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup>\n\t\t<col class=\"col-odd\" />\n\t\t<col class=\"col-even\" />\n\t\t<col class=\"col-odd\" />\n\t</colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead>\n\t\t<tr>\n\t\t\t<th scope=\"col\" class=\"first-head\">Head1</th>\n\t\t\t<th scope=\"col\" class=\"standard-head\">Head2</th>\n\t\t\t<th scope=\"col\" class=\"last-head\">Head3</th>\n\t\t</tr>\n\t</thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot>\n\t\t<tr>\n\t\t\t<td colspan=\"2\" class=\"first-foot\"></td>\n\t\t\t<td class=\"last-foot\"/>\n\t\t</tr>\n\t</tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t\t<tr class=\"row-even\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t</tbody>"));
        }

        [Test]
        public void NoOfColoumns_4()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>(), new List<string>(), new List<string>() { "Head1", "Head2", "Head3", "Head4" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup>\n\t\t<col class=\"col-odd\" />\n\t\t<col class=\"col-even\" />\n\t\t<col class=\"col-odd\" />\n\t\t<col class=\"col-even\" />\n\t</colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead>\n\t\t<tr>\n\t\t\t<th scope=\"col\" class=\"first-head\">Head1</th>\n\t\t\t<th scope=\"col\" class=\"standard-head\">Head2</th>\n\t\t\t<th scope=\"col\" class=\"standard-head\">Head3</th>\n\t\t\t<th scope=\"col\" class=\"last-head\">Head4</th>\n\t\t</tr>\n\t</thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot>\n\t\t<tr>\n\t\t\t<td colspan=\"3\" class=\"first-foot\"></td>\n\t\t\t<td class=\"last-foot\"/>\n\t\t</tr>\n\t</tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t\t<tr class=\"row-even\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t</tbody>"));
        }

        [Test]
        public void NoOfColoumns_1_Heading()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, "My Heading", null, new List<int>(), new List<string>(), new List<string>() { "Head1" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup>\n\t\t<col class=\"col-odd\" />\n\t</colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead>\n\t\t<tr>\n\t\t\t<td colspan=\"1\" class=\"heading\">My Heading</td>\n\t\t</tr>\n\t\t<tr>\n\t\t\t<th scope=\"col\" class=\"standard-head\">Head1</th>\n\t\t</tr>\n\t</thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot>\n\t\t<tr>\n\t\t\t<td class=\"standard-foot\"></td>\n\t\t</tr>\n\t</tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t</tr>\n\t\t<tr class=\"row-even\">\n\t\t\t<td>Col1</td>\n\t\t</tr>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t</tr>\n\t</tbody>"));
        }

        [Test]
        public void NoOfColoumns_4_Heading()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, "My Heading", null, new List<int>(), new List<string>(), new List<string>() { "Head1", "Head2", "Head3", "Head4" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup>\n\t\t<col class=\"col-odd\" />\n\t\t<col class=\"col-even\" />\n\t\t<col class=\"col-odd\" />\n\t\t<col class=\"col-even\" />\n\t</colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead>\n\t\t<tr>\n\t\t\t<td colspan=\"4\" class=\"heading\">My Heading</td>\n\t\t</tr>\n\t\t<tr>\n\t\t\t<th scope=\"col\" class=\"first-head\">Head1</th>\n\t\t\t<th scope=\"col\" class=\"standard-head\">Head2</th>\n\t\t\t<th scope=\"col\" class=\"standard-head\">Head3</th>\n\t\t\t<th scope=\"col\" class=\"last-head\">Head4</th>\n\t\t</tr>\n\t</thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot>\n\t\t<tr>\n\t\t\t<td colspan=\"3\" class=\"first-foot\"></td>\n\t\t\t<td class=\"last-foot\"/>\n\t\t</tr>\n\t</tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t\t<tr class=\"row-even\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t</tbody>"));
        }

        [Test]
        public void NoOfColoumns_1_Footer()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, null, "My Footer", new List<int>(), new List<string>(), new List<string>() { "Head1" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup>\n\t\t<col class=\"col-odd\" />\n\t</colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead>\n\t\t<tr>\n\t\t\t<th scope=\"col\" class=\"standard-head\">Head1</th>\n\t\t</tr>\n\t</thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot>\n\t\t<tr>\n\t\t\t<td class=\"standard-foot\">My Footer</td>\n\t\t</tr>\n\t</tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t</tr>\n\t\t<tr class=\"row-even\">\n\t\t\t<td>Col1</td>\n\t\t</tr>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t</tr>\n\t</tbody>"));
        }

        [Test]
        public void NoOfColoumns_4_Footer()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, null, "My Footer", new List<int>(), new List<string>(), new List<string>() { "Head1", "Head2", "Head3", "Head4" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup>\n\t\t<col class=\"col-odd\" />\n\t\t<col class=\"col-even\" />\n\t\t<col class=\"col-odd\" />\n\t\t<col class=\"col-even\" />\n\t</colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead>\n\t\t<tr>\n\t\t\t<th scope=\"col\" class=\"first-head\">Head1</th>\n\t\t\t<th scope=\"col\" class=\"standard-head\">Head2</th>\n\t\t\t<th scope=\"col\" class=\"standard-head\">Head3</th>\n\t\t\t<th scope=\"col\" class=\"last-head\">Head4</th>\n\t\t</tr>\n\t</thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot>\n\t\t<tr>\n\t\t\t<td colspan=\"3\" class=\"first-foot\">My Footer</td>\n\t\t\t<td class=\"last-foot\"/>\n\t\t</tr>\n\t</tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t\t<tr class=\"row-even\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t</tbody>"));
        }

        [Test]
        public void NoOfColoumns_1_HeadingAndFooter()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, "My Heading", "My Footer", new List<int>(), new List<string>(), new List<string>() { "Head1" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup>\n\t\t<col class=\"col-odd\" />\n\t</colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead>\n\t\t<tr>\n\t\t\t<td colspan=\"1\" class=\"heading\">My Heading</td>\n\t\t</tr>\n\t\t<tr>\n\t\t\t<th scope=\"col\" class=\"standard-head\">Head1</th>\n\t\t</tr>\n\t</thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot>\n\t\t<tr>\n\t\t\t<td class=\"standard-foot\">My Footer</td>\n\t\t</tr>\n\t</tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t</tr>\n\t\t<tr class=\"row-even\">\n\t\t\t<td>Col1</td>\n\t\t</tr>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t</tr>\n\t</tbody>"));
        }

        [Test]
        public void NoOfColoumns_4_HeadingAndFooter()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, "My Heading", "My Footer", new List<int>(), new List<string>(), new List<string>() { "Head1", "Head2", "Head3", "Head4" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup>\n\t\t<col class=\"col-odd\" />\n\t\t<col class=\"col-even\" />\n\t\t<col class=\"col-odd\" />\n\t\t<col class=\"col-even\" />\n\t</colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead>\n\t\t<tr>\n\t\t\t<td colspan=\"4\" class=\"heading\">My Heading</td>\n\t\t</tr>\n\t\t<tr>\n\t\t\t<th scope=\"col\" class=\"first-head\">Head1</th>\n\t\t\t<th scope=\"col\" class=\"standard-head\">Head2</th>\n\t\t\t<th scope=\"col\" class=\"standard-head\">Head3</th>\n\t\t\t<th scope=\"col\" class=\"last-head\">Head4</th>\n\t\t</tr>\n\t</thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot>\n\t\t<tr>\n\t\t\t<td colspan=\"3\" class=\"first-foot\">My Footer</td>\n\t\t\t<td class=\"last-foot\"/>\n\t\t</tr>\n\t</tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t\t<tr class=\"row-even\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t</tbody>"));
        }

        [Test]
        public void Default_Style()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>(), new List<string>(), new List<string>() { "Head1" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<table id=\"default\">"));
        }

        [Test]
        public void Custom_Style()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>(), new List<string>(), new List<string>() { "Head1" }, "MyStyle");

            //Assert
            Assert.AreEqual(true, result.Contains("<table id=\"MyStyle\">"));
        }

        #region Column Order
        [Test]
        public void NoOfColoumns_3_Display_0_2()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>() { 0, 2}, new List<string>(), new List<string>() { "Head1", "Head2", "Head3" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup>\n\t\t<col class=\"col-odd\" />\n\t\t<col class=\"col-even\" />\n\t</colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead>\n\t\t<tr>\n\t\t\t<th scope=\"col\" class=\"first-head\">Head1</th>\n\t\t\t<th scope=\"col\" class=\"last-head\">Head3</th>\n\t\t</tr>\n\t</thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot>\n\t\t<tr>\n\t\t\t<td colspan=\"1\" class=\"first-foot\"></td>\n\t\t\t<td class=\"last-foot\"/>\n\t\t</tr>\n\t</tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t\t<tr class=\"row-even\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t</tbody>"));
        }

        [Test]
        public void NoOfColoumns_3_Display_1()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>() { 1 }, new List<string>(), new List<string>() { "Head1", "Head2", "Head3" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup>\n\t\t<col class=\"col-odd\" />\n\t</colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead>\n\t\t<tr>\n\t\t\t<th scope=\"col\" class=\"standard-head\">Head2</th>\n\t\t</tr>\n\t</thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot>\n\t\t<tr>\n\t\t\t<td class=\"standard-foot\"></td>\n\t\t</tr>\n\t</tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col2</td>\n\t\t</tr>\n\t\t<tr class=\"row-even\">\n\t\t\t<td>Col2</td>\n\t\t</tr>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col2</td>\n\t\t</tr>\n\t</tbody>"));
        }

        [Test]
        public void NoOfColoumns_3_Display_0_1_2_3()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>() { 0,1,2,3 }, new List<string>(), new List<string>() { "Head1", "Head2", "Head3" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup>\n\t\t<col class=\"col-odd\" />\n\t\t<col class=\"col-even\" />\n\t\t<col class=\"col-odd\" />\n\t\t<col class=\"col-even\" />\n\t</colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead>\n\t\t<tr>\n\t\t\t<th scope=\"col\" class=\"first-head\">Head1</th>\n\t\t\t<th scope=\"col\" class=\"standard-head\">Head2</th>\n\t\t\t<th scope=\"col\" class=\"standard-head\">Head3</th>\n\t\t\t<th scope=\"col\" class=\"last-head\">?Missing Header?</th>\n\t\t</tr>\n\t</thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot>\n\t\t<tr>\n\t\t\t<td colspan=\"3\" class=\"first-foot\"></td>\n\t\t\t<td class=\"last-foot\"/>\n\t\t</tr>\n\t</tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t\t<tr class=\"row-even\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t</tbody>"));
        }

        [Test]
        public void NoOfColoumns_3_Display_3_2_1_0()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>() { 3, 2, 1, 0 }, new List<string>(), new List<string>() { "Head1", "Head2", "Head3" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup>\n\t\t<col class=\"col-odd\" />\n\t\t<col class=\"col-even\" />\n\t\t<col class=\"col-odd\" />\n\t\t<col class=\"col-even\" />\n\t</colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead>\n\t\t<tr>\n\t\t\t<th scope=\"col\" class=\"first-head\">?Missing Header?</th>\n\t\t\t<th scope=\"col\" class=\"standard-head\">Head3</th>\n\t\t\t<th scope=\"col\" class=\"standard-head\">Head2</th>\n\t\t\t<th scope=\"col\" class=\"last-head\">Head1</th>\n\t\t</tr>\n\t</thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot>\n\t\t<tr>\n\t\t\t<td colspan=\"3\" class=\"first-foot\"></td>\n\t\t\t<td class=\"last-foot\"/>\n\t\t</tr>\n\t</tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col3</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col1</td>\n\t\t</tr>\n\t\t<tr class=\"row-even\">\n\t\t\t<td>Col3</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col1</td>\n\t\t</tr>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col3</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col1</td>\n\t\t</tr>\n\t</tbody>"));
        }
        #endregion

        #region Custom Headers
        [Test]
        public void NoOfColoumns_3_CustomHeaders_3()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>(), new List<string>() { "H1","H2","H3" }, new List<string>() { "Head1", "Head2", "Head3" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup>\n\t\t<col class=\"col-odd\" />\n\t\t<col class=\"col-even\" />\n\t\t<col class=\"col-odd\" />\n\t</colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead>\n\t\t<tr>\n\t\t\t<th scope=\"col\" class=\"first-head\">H1</th>\n\t\t\t<th scope=\"col\" class=\"standard-head\">H2</th>\n\t\t\t<th scope=\"col\" class=\"last-head\">H3</th>\n\t\t</tr>\n\t</thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot>\n\t\t<tr>\n\t\t\t<td colspan=\"2\" class=\"first-foot\"></td>\n\t\t\t<td class=\"last-foot\"/>\n\t\t</tr>\n\t</tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t\t<tr class=\"row-even\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t</tbody>"));
        }

        [Test]
        public void NoOfColoumns_3_CustomHeaders_2()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>(), new List<string>() { "H1", "H2" }, new List<string>() { "Head1", "Head2", "Head3" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup>\n\t\t<col class=\"col-odd\" />\n\t\t<col class=\"col-even\" />\n\t\t<col class=\"col-odd\" />\n\t</colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead>\n\t\t<tr>\n\t\t\t<th scope=\"col\" class=\"first-head\">H1</th>\n\t\t\t<th scope=\"col\" class=\"standard-head\">H2</th>\n\t\t\t<th scope=\"col\" class=\"last-head\">Head3</th>\n\t\t</tr>\n\t</thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot>\n\t\t<tr>\n\t\t\t<td colspan=\"2\" class=\"first-foot\"></td>\n\t\t\t<td class=\"last-foot\"/>\n\t\t</tr>\n\t</tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t\t<tr class=\"row-even\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col2</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t</tbody>"));
        }

        [Test]
        public void NoOfColoumns_3_CustomHeaders_3_Display_1()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>() { 1 }, new List<string>() { "H1", "H2", "H3" }, new List<string>() { "Head1", "Head2", "Head3" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup>\n\t\t<col class=\"col-odd\" />\n\t</colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead>\n\t\t<tr>\n\t\t\t<th scope=\"col\" class=\"standard-head\">H2</th>\n\t\t</tr>\n\t</thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot>\n\t\t<tr>\n\t\t\t<td class=\"standard-foot\"></td>\n\t\t</tr>\n\t</tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col2</td>\n\t\t</tr>\n\t\t<tr class=\"row-even\">\n\t\t\t<td>Col2</td>\n\t\t</tr>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col2</td>\n\t\t</tr>\n\t</tbody>"));
        }

        [Test]
        public void NoOfColoumns_3_CustomHeaders_3_Display_0_2()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>() { 0,2 }, new List<string>() { "H1", "H2", "H3" }, new List<string>() { "Head1", "Head2", "Head3" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup>\n\t\t<col class=\"col-odd\" />\n\t\t<col class=\"col-even\" />\n\t</colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead>\n\t\t<tr>\n\t\t\t<th scope=\"col\" class=\"first-head\">H1</th>\n\t\t\t<th scope=\"col\" class=\"last-head\">H3</th>\n\t\t</tr>\n\t</thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot>\n\t\t<tr>\n\t\t\t<td colspan=\"1\" class=\"first-foot\"></td>\n\t\t\t<td class=\"last-foot\"/>\n\t\t</tr>\n\t</tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t\t<tr class=\"row-even\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col1</td>\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t</tbody>"));
        }


        [Test]
        public void NoOfColoumns_3_CustomHeaders_2_Display_2()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>() { 2 }, new List<string>() { "H1", "H2" }, new List<string>() { "Head1", "Head2", "Head3" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup>\n\t\t<col class=\"col-odd\" />\n\t</colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead>\n\t\t<tr>\n\t\t\t<th scope=\"col\" class=\"standard-head\">Head3</th>\n\t\t</tr>\n\t</thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot>\n\t\t<tr>\n\t\t\t<td class=\"standard-foot\"></td>\n\t\t</tr>\n\t</tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t\t<tr class=\"row-even\">\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t\t<tr class=\"row-odd\">\n\t\t\t<td>Col3</td>\n\t\t</tr>\n\t</tbody>"));
        }

        [Test]
        public void NoOfColoumns_2_CustomHeaders_2_Display_2()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2" } },
                {1, new List<string>() { "Col1","Col2" } },
                {2, new List<string>() { "Col1","Col2" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>() { 2 }, new List<string>() { "H1", "H2" }, new List<string>() { "Head1", "Head2" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup>\n\t\t<col class=\"col-odd\" />\n\t</colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead>\n\t\t<tr>\n\t\t\t<th scope=\"col\" class=\"standard-head\">?Missing Header</th>\n\t\t</tr>\n\t</thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot>\n\t\t<tr>\n\t\t\t<td class=\"standard-foot\"></td>\n\t\t</tr>\n\t</tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody>\n\t\t<tr class=\"row-odd\">\n\t\t</tr>\n\t\t<tr class=\"row-even\">\n\t\t</tr>\n\t\t<tr class=\"row-odd\">\n\t\t</tr>\n\t</tbody>"));
        }
        #endregion
    }
}
