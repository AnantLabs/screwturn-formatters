using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using ScrewTurn.Wiki.PluginFramework;
using Keeper.Garrett.ScrewTurn;
using Keeper.Garrett.ScrewTurn.Utility;
using System.Reflection;

namespace Utility.Tests
{
    [TestFixture]
    [Category("Utility")]
    public class XHtmlTableGenerator_Test
    {

        [Test]
        public void Store_Files_All()
        {
            //Arrange
            var host = MockRepository.GenerateMock<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            provider.Expect(x => x.Information).Return(new ComponentInformation("SomeProvider", "", "", "", ""));
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).IgnoreArguments().Return(provider.GetType().FullName);
            host.Expect(x => x.GetFilesStorageProviders(true)).IgnoreArguments().Return(new IFilesStorageProviderV30[] { provider });

            //Dirs
            provider.Expect(x => x.ListDirectories("/")).Return(new string[] { "NoPresent1" });
            provider.Expect(x => x.ListDirectories("/Keeper.Garrett.Formatters/")).Return(new string[] { "NoPresent2" });
            provider.Expect(x => x.ListDirectories("/Keeper.Garrett.Formatters/Tables")).Return(new string[] { "NoPresent3" });

            //Files
            provider.Expect(x => x.ListFiles("/Keeper.Garrett.Formatters/Tables")).Return(new string[] { "NoFilesAtAll" });
            provider.Expect(x => x.ListFiles("/Keeper.Garrett.Formatters/Tables/Images")).Return(new string[] { "NoFilesAtAll" });


            //Act
            XHtmlTableGenerator.StoreFiles(host, "SomeFormatterName");

            //Assert
            provider.AssertWasCalled(x => x.CreateDirectory("/", "Keeper.Garrett.Formatters"));
            provider.AssertWasCalled(x => x.CreateDirectory("/Keeper.Garrett.Formatters/", "Tables"));
            provider.AssertWasCalled(x => x.CreateDirectory("/Keeper.Garrett.Formatters/Tables", "Images"));
        }

        [Test]
        public void Store_Files_None()
        {
            //Arrange
            var host = MockRepository.GenerateMock<IHostV30>();
            var provider = MockRepository.GenerateStub<IFilesStorageProviderV30>();

            provider.Expect(x => x.Information).Return(new ComponentInformation("SomeProvider", "", "", "", ""));
            host.Expect(x => x.GetSettingValue(SettingName.DefaultFilesStorageProvider)).IgnoreArguments().Return(provider.GetType().FullName);
            host.Expect(x => x.GetFilesStorageProviders(true)).IgnoreArguments().Return(new IFilesStorageProviderV30[] { provider });

            //Dirs
            provider.Expect(x => x.ListDirectories("/")).Return(new string[] { "Keeper.Garrett.Formatters" });
            provider.Expect(x => x.ListDirectories("/Keeper.Garrett.Formatters/")).Return(new string[] { "Tables" });
            provider.Expect(x => x.ListDirectories("/Keeper.Garrett.Formatters/Tables")).Return(new string[] { "Images" });

            //Files
            provider.Expect(x => x.ListFiles("/Keeper.Garrett.Formatters/Tables")).Return(new string[] { "/Keeper.Garrett.Formatters/Tables/TableStyle.css" });
            provider.Expect(x => x.ListFiles("/Keeper.Garrett.Formatters/Tables/Images")).Return(new string[] { "/Keeper.Garrett.Formatters/Tables/Images/back.png" });

            //Act
            XHtmlTableGenerator.StoreFiles(host, "SomeFormatterName");

            //Assert
            provider.AssertWasNotCalled(x => x.CreateDirectory("/", "Keeper.Garrett.Formatters"));
            provider.AssertWasNotCalled(x => x.CreateDirectory("/Keeper.Garrett.Formatters/", "Tables"));
            provider.AssertWasNotCalled(x => x.CreateDirectory("/Keeper.Garrett.Formatters/Tables", "Images"));
        }

        [Test]
        public void NewColNames()
        {
            //Arrange
            var data = new Dictionary<string,int>()
            {
                {"col1",0},
                {"col2",1},
                {"col3",2}
            };

            var newCols = new List<int>();
            var newColNames = new List<string>();
            //Act
            XHtmlTableGenerator.GenerateColumnsAndColumnNames(data, new List<string>() { "Col1", "Col2", "Col3" }, "Col2", "Col1,Col2", "mycol1,mycol2", out newCols, out newColNames);

            //Assert
            Assert.AreEqual(2, newCols.Count);
            Assert.AreEqual(3, newColNames.Count);
            Assert.AreEqual(0, newCols[0]);
            Assert.AreEqual(1, newCols[1]);
            Assert.AreEqual("mycol1", newColNames[0]);
            Assert.AreEqual("mycol2", newColNames[1]);
            Assert.AreEqual("Col3", newColNames[2]);
        }

        [Test]
        public void NewColNames_AllCols()
        {
            //Arrange
            var data = new Dictionary<string, int>()
            {
                {"col1",0},
                {"col2",1},
                {"col3",2}
            };

            var newCols = new List<int>();
            var newColNames = new List<string>();
            //Act
            XHtmlTableGenerator.GenerateColumnsAndColumnNames(data, new List<string>() { "Col1", "Col2", "Col3" }, "Col2", "all", "mycol1,mycol2", out newCols, out newColNames);

            //Assert
            Assert.AreEqual(3, newCols.Count);
            Assert.AreEqual(3, newColNames.Count);
            Assert.AreEqual(0, newCols[0]);
            Assert.AreEqual(1, newCols[1]);
            Assert.AreEqual(2, newCols[2]);
            Assert.AreEqual("mycol1", newColNames[0]);
            Assert.AreEqual("mycol2", newColNames[1]);
            Assert.AreEqual("Col3", newColNames[2]);
        }

        [Test]
        public void NewColNames_UseDefaultCols()
        {
            //Arrange
            var data = new Dictionary<string, int>()
            {
                {"col1",0},
                {"col2",1},
                {"col3",2}
            };

            var newCols = new List<int>();
            var newColNames = new List<string>();
            //Act
            XHtmlTableGenerator.GenerateColumnsAndColumnNames(data, new List<string>() { "Col1", "Col2", "Col3" }, "Col2", "", "mycol1,mycol2", out newCols, out newColNames);

            //Assert
            Assert.AreEqual(1, newCols.Count);
            Assert.AreEqual(3, newColNames.Count);
            Assert.AreEqual(1, newCols[0]);
            Assert.AreEqual("Col1", newColNames[0]);
            Assert.AreEqual("mycol1", newColNames[1]); //Should potentially be mycol2...
            Assert.AreEqual("Col3", newColNames[2]);
        }

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
            Assert.AreEqual("<table id=\"default\"><colgroup></colgroup><thead><tr></tr></thead><tbody><tr class=\"row-odd\"></tr><tr class=\"row-even\"></tr><tr class=\"row-odd\"></tr></tbody></table>", result);
        }

        [Test]
        public void GenericStyle()
        {
            //Arrange
            var data = new Dictionary<int, List<string>>()
            {
                {0, new List<string>() { "Col1","Col2","Col3" } },
                {1, new List<string>() { "Col1","Col2","Col3" } },
                {2, new List<string>() { "Col1","Col2","Col3" } },
            };

            //Act
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>(), new List<string>(), new List<string>() { "Head1", "Head2", "Head3" }, "generic");

            //Assert
            Assert.AreEqual("<link type=\"text/css\" rel=\"stylesheet\" href=\"GetFile.aspx?File=/Keeper.Garrett.Formatters/Tables/TableStyle.css\"></link><table id=\"generic\"><colgroup></colgroup><thead><tr></tr></thead><tbody><tr class=\"tablerow\"></tr><tr class=\"tablerowalternate\"></tr><tr class=\"tablerow\"></tr></tbody></table>", result);
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
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>() { 0 }, new List<string>(), new List<string>() { "Head1" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup><col class=\"col-odd\"/></colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead><tr><th scope=\"col\" class=\"standard-head\">Head1</th></tr></thead>"));
            Assert.AreEqual(false, result.Contains("<tfoot><tr><td class=\"standard-foot\"></td></tr></tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody><tr class=\"row-odd\"><td>Col1</td></tr><tr class=\"row-even\"><td>Col1</td></tr><tr class=\"row-odd\"><td>Col1</td></tr></tbody>"));
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
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>() { 0,1 }, new List<string>(), new List<string>() { "Head1", "Head2" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup><col class=\"col-odd\"/><col class=\"col-even\"/></colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead><tr><th scope=\"col\" class=\"first-head\">Head1</th><th scope=\"col\" class=\"last-head\">Head2</th></tr></thead>"));
            Assert.AreEqual(false, result.Contains("<tfoot><tr><td colspan=\"1\" class=\"first-foot\"></td><td class=\"last-foot\"/></tr></tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody><tr class=\"row-odd\"><td>Col1</td><td>Col2</td></tr><tr class=\"row-even\"><td>Col1</td><td>Col2</td></tr><tr class=\"row-odd\"><td>Col1</td><td>Col2</td></tr></tbody>"));
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
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>() { 0,1,2 }, new List<string>(), new List<string>() { "Head1", "Head2", "Head3" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup><col class=\"col-odd\"/><col class=\"col-even\"/><col class=\"col-odd\"/></colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead><tr><th scope=\"col\" class=\"first-head\">Head1</th><th scope=\"col\" class=\"standard-head\">Head2</th><th scope=\"col\" class=\"last-head\">Head3</th></tr></thead>"));
            Assert.AreEqual(false, result.Contains("<tfoot><tr><td colspan=\"2\" class=\"first-foot\"></td><td class=\"last-foot\"/></tr></tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody><tr class=\"row-odd\"><td>Col1</td><td>Col2</td><td>Col3</td></tr><tr class=\"row-even\"><td>Col1</td><td>Col2</td><td>Col3</td></tr><tr class=\"row-odd\"><td>Col1</td><td>Col2</td><td>Col3</td></tr></tbody>"));
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
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>() { 0,1,2,3 }, new List<string>(), new List<string>() { "Head1", "Head2", "Head3", "Head4" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup><col class=\"col-odd\"/><col class=\"col-even\"/><col class=\"col-odd\"/><col class=\"col-even\"/></colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead><tr><th scope=\"col\" class=\"first-head\">Head1</th><th scope=\"col\" class=\"standard-head\">Head2</th><th scope=\"col\" class=\"standard-head\">Head3</th><th scope=\"col\" class=\"last-head\">Head4</th></tr></thead>"));
            Assert.AreEqual(false, result.Contains("<tfoot><tr><td colspan=\"3\" class=\"first-foot\"></td><td class=\"last-foot\"/></tr></tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody><tr class=\"row-odd\"><td>Col1</td><td>Col2</td><td>Col3</td></tr><tr class=\"row-even\"><td>Col1</td><td>Col2</td><td>Col3</td></tr><tr class=\"row-odd\"><td>Col1</td><td>Col2</td><td>Col3</td></tr></tbody>"));
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
            var result = XHtmlTableGenerator.GenerateTable(data, "My Heading", null, new List<int>() { 0 }, new List<string>(), new List<string>() { "Head1" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup><col class=\"col-odd\"/></colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead><tr><td colspan=\"1\" class=\"heading\">My Heading</td></tr><tr><th scope=\"col\" class=\"standard-head\">Head1</th></tr></thead>"));
            Assert.AreEqual(false, result.Contains("<tfoot><tr><td class=\"standard-foot\"></td></tr></tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody><tr class=\"row-odd\"><td>Col1</td></tr><tr class=\"row-even\"><td>Col1</td></tr><tr class=\"row-odd\"><td>Col1</td></tr></tbody>"));
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
            var result = XHtmlTableGenerator.GenerateTable(data, "My Heading", null, new List<int>() { 0, 1, 2, 3 }, new List<string>(), new List<string>() { "Head1", "Head2", "Head3", "Head4" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup><col class=\"col-odd\"/><col class=\"col-even\"/><col class=\"col-odd\"/><col class=\"col-even\"/></colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead><tr><td colspan=\"4\" class=\"heading\">My Heading</td></tr><tr><th scope=\"col\" class=\"first-head\">Head1</th><th scope=\"col\" class=\"standard-head\">Head2</th><th scope=\"col\" class=\"standard-head\">Head3</th><th scope=\"col\" class=\"last-head\">Head4</th></tr></thead>"));
            Assert.AreEqual(false, result.Contains("<tfoot><tr><td colspan=\"3\" class=\"first-foot\"></td><td class=\"last-foot\"/></tr></tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody><tr class=\"row-odd\"><td>Col1</td><td>Col2</td><td>Col3</td></tr><tr class=\"row-even\"><td>Col1</td><td>Col2</td><td>Col3</td></tr><tr class=\"row-odd\"><td>Col1</td><td>Col2</td><td>Col3</td></tr></tbody>"));
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
            var result = XHtmlTableGenerator.GenerateTable(data, null, "My Footer", new List<int>() { 0 }, new List<string>(), new List<string>() { "Head1" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup><col class=\"col-odd\"/></colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead><tr><th scope=\"col\" class=\"standard-head\">Head1</th></tr></thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot><tr><td class=\"standard-foot\">My Footer</td></tr></tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody><tr class=\"row-odd\"><td>Col1</td></tr><tr class=\"row-even\"><td>Col1</td></tr><tr class=\"row-odd\"><td>Col1</td></tr></tbody>"));
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
            var result = XHtmlTableGenerator.GenerateTable(data, null, "My Footer", new List<int>() { 0,1,2,3 }, new List<string>(), new List<string>() { "Head1", "Head2", "Head3", "Head4" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup><col class=\"col-odd\"/><col class=\"col-even\"/><col class=\"col-odd\"/><col class=\"col-even\"/></colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead><tr><th scope=\"col\" class=\"first-head\">Head1</th><th scope=\"col\" class=\"standard-head\">Head2</th><th scope=\"col\" class=\"standard-head\">Head3</th><th scope=\"col\" class=\"last-head\">Head4</th></tr></thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot><tr><td colspan=\"3\" class=\"first-foot\">My Footer</td><td class=\"last-foot\"/></tr></tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody><tr class=\"row-odd\"><td>Col1</td><td>Col2</td><td>Col3</td></tr><tr class=\"row-even\"><td>Col1</td><td>Col2</td><td>Col3</td></tr><tr class=\"row-odd\"><td>Col1</td><td>Col2</td><td>Col3</td></tr></tbody>"));
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
            var result = XHtmlTableGenerator.GenerateTable(data, "My Heading", "My Footer", new List<int>() { 0 }, new List<string>(), new List<string>() { "Head1" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup><col class=\"col-odd\"/></colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead><tr><td colspan=\"1\" class=\"heading\">My Heading</td></tr><tr><th scope=\"col\" class=\"standard-head\">Head1</th></tr></thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot><tr><td class=\"standard-foot\">My Footer</td></tr></tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody><tr class=\"row-odd\"><td>Col1</td></tr><tr class=\"row-even\"><td>Col1</td></tr><tr class=\"row-odd\"><td>Col1</td></tr></tbody>"));
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
            var result = XHtmlTableGenerator.GenerateTable(data, "My Heading", "My Footer", new List<int>() { 0, 1, 2, 3 }, new List<string>(), new List<string>() { "Head1", "Head2", "Head3", "Head4" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup><col class=\"col-odd\"/><col class=\"col-even\"/><col class=\"col-odd\"/><col class=\"col-even\"/></colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead><tr><td colspan=\"4\" class=\"heading\">My Heading</td></tr><tr><th scope=\"col\" class=\"first-head\">Head1</th><th scope=\"col\" class=\"standard-head\">Head2</th><th scope=\"col\" class=\"standard-head\">Head3</th><th scope=\"col\" class=\"last-head\">Head4</th></tr></thead>"));
            Assert.AreEqual(true, result.Contains("<tfoot><tr><td colspan=\"3\" class=\"first-foot\">My Footer</td><td class=\"last-foot\"/></tr></tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody><tr class=\"row-odd\"><td>Col1</td><td>Col2</td><td>Col3</td></tr><tr class=\"row-even\"><td>Col1</td><td>Col2</td><td>Col3</td></tr><tr class=\"row-odd\"><td>Col1</td><td>Col2</td><td>Col3</td></tr></tbody>"));
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
            Assert.AreEqual(true, result.Contains("<colgroup><col class=\"col-odd\"/><col class=\"col-even\"/></colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead><tr><th scope=\"col\" class=\"first-head\">Head1</th><th scope=\"col\" class=\"last-head\">Head3</th></tr></thead>"));
            Assert.AreEqual(false, result.Contains("<tfoot><tr><td colspan=\"1\" class=\"first-foot\"></td><td class=\"last-foot\"/></tr></tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody><tr class=\"row-odd\"><td>Col1</td><td>Col3</td></tr><tr class=\"row-even\"><td>Col1</td><td>Col3</td></tr><tr class=\"row-odd\"><td>Col1</td><td>Col3</td></tr></tbody>"));
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
            Assert.AreEqual(true, result.Contains("<colgroup><col class=\"col-odd\"/></colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead><tr><th scope=\"col\" class=\"standard-head\">Head2</th></tr></thead>"));
            Assert.AreEqual(false, result.Contains("<tfoot><tr><td class=\"standard-foot\"></td></tr></tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody><tr class=\"row-odd\"><td>Col2</td></tr><tr class=\"row-even\"><td>Col2</td></tr><tr class=\"row-odd\"><td>Col2</td></tr></tbody>"));
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
            Assert.AreEqual(true, result.Contains("<colgroup><col class=\"col-odd\"/><col class=\"col-even\"/><col class=\"col-odd\"/><col class=\"col-even\"/></colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead><tr><th scope=\"col\" class=\"first-head\">Head1</th><th scope=\"col\" class=\"standard-head\">Head2</th><th scope=\"col\" class=\"standard-head\">Head3</th><th scope=\"col\" class=\"last-head\">?Missing Header?</th></tr></thead>"));
            Assert.AreEqual(false, result.Contains("<tfoot><tr><td colspan=\"3\" class=\"first-foot\"></td><td class=\"last-foot\"/></tr></tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody><tr class=\"row-odd\"><td>Col1</td><td>Col2</td><td>Col3</td></tr><tr class=\"row-even\"><td>Col1</td><td>Col2</td><td>Col3</td></tr><tr class=\"row-odd\"><td>Col1</td><td>Col2</td><td>Col3</td></tr></tbody>"));
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
            Assert.AreEqual(true, result.Contains("<colgroup><col class=\"col-odd\"/><col class=\"col-even\"/><col class=\"col-odd\"/><col class=\"col-even\"/></colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead><tr><th scope=\"col\" class=\"first-head\">?Missing Header?</th><th scope=\"col\" class=\"standard-head\">Head3</th><th scope=\"col\" class=\"standard-head\">Head2</th><th scope=\"col\" class=\"last-head\">Head1</th></tr></thead>"));
            Assert.AreEqual(false, result.Contains("<tfoot><tr><td colspan=\"3\" class=\"first-foot\"></td><td class=\"last-foot\"/></tr></tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody><tr class=\"row-odd\"><td>Col3</td><td>Col2</td><td>Col1</td></tr><tr class=\"row-even\"><td>Col3</td><td>Col2</td><td>Col1</td></tr><tr class=\"row-odd\"><td>Col3</td><td>Col2</td><td>Col1</td></tr></tbody>"));
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
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>() { 0, 1, 2 }, new List<string>() { "H1", "H2", "H3" }, new List<string>() { "Head1", "Head2", "Head3" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup><col class=\"col-odd\"/><col class=\"col-even\"/><col class=\"col-odd\"/></colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead><tr><th scope=\"col\" class=\"first-head\">H1</th><th scope=\"col\" class=\"standard-head\">H2</th><th scope=\"col\" class=\"last-head\">H3</th></tr></thead>"));
            Assert.AreEqual(false, result.Contains("<tfoot><tr><td colspan=\"2\" class=\"first-foot\"></td><td class=\"last-foot\"/></tr></tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody><tr class=\"row-odd\"><td>Col1</td><td>Col2</td><td>Col3</td></tr><tr class=\"row-even\"><td>Col1</td><td>Col2</td><td>Col3</td></tr><tr class=\"row-odd\"><td>Col1</td><td>Col2</td><td>Col3</td></tr></tbody>"));
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
            var result = XHtmlTableGenerator.GenerateTable(data, null, null, new List<int>() { 0, 1, 2 }, new List<string>() { "H1", "H2" }, new List<string>() { "Head1", "Head2", "Head3" }, null);

            //Assert
            Assert.AreEqual(true, result.Contains("<colgroup><col class=\"col-odd\"/><col class=\"col-even\"/><col class=\"col-odd\"/></colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead><tr><th scope=\"col\" class=\"first-head\">H1</th><th scope=\"col\" class=\"standard-head\">H2</th><th scope=\"col\" class=\"last-head\">Head3</th></tr></thead>"));
            Assert.AreEqual(false, result.Contains("<tfoot><tr><td colspan=\"2\" class=\"first-foot\"></td><td class=\"last-foot\"/></tr></tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody><tr class=\"row-odd\"><td>Col1</td><td>Col2</td><td>Col3</td></tr><tr class=\"row-even\"><td>Col1</td><td>Col2</td><td>Col3</td></tr><tr class=\"row-odd\"><td>Col1</td><td>Col2</td><td>Col3</td></tr></tbody>"));
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
            Assert.AreEqual(true, result.Contains("<colgroup><col class=\"col-odd\"/></colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead><tr><th scope=\"col\" class=\"standard-head\">H2</th></tr></thead>"));
            Assert.AreEqual(false, result.Contains("<tfoot><tr><td class=\"standard-foot\"></td></tr></tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody><tr class=\"row-odd\"><td>Col2</td></tr><tr class=\"row-even\"><td>Col2</td></tr><tr class=\"row-odd\"><td>Col2</td></tr></tbody>"));
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
            Assert.AreEqual(true, result.Contains("<colgroup><col class=\"col-odd\"/><col class=\"col-even\"/></colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead><tr><th scope=\"col\" class=\"first-head\">H1</th><th scope=\"col\" class=\"last-head\">H3</th></tr></thead>"));
            Assert.AreEqual(false, result.Contains("<tfoot><tr><td colspan=\"1\" class=\"first-foot\"></td><td class=\"last-foot\"/></tr></tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody><tr class=\"row-odd\"><td>Col1</td><td>Col3</td></tr><tr class=\"row-even\"><td>Col1</td><td>Col3</td></tr><tr class=\"row-odd\"><td>Col1</td><td>Col3</td></tr></tbody>"));
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
            Assert.AreEqual(true, result.Contains("<colgroup><col class=\"col-odd\"/></colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead><tr><th scope=\"col\" class=\"standard-head\">Head3</th></tr></thead>"));
            Assert.AreEqual(false, result.Contains("<tfoot><tr><td class=\"standard-foot\"></td></tr></tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody><tr class=\"row-odd\"><td>Col3</td></tr><tr class=\"row-even\"><td>Col3</td></tr><tr class=\"row-odd\"><td>Col3</td></tr></tbody>"));
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
            Assert.AreEqual(true, result.Contains("<colgroup><col class=\"col-odd\"/></colgroup>"));
            Assert.AreEqual(true, result.Contains("<thead><tr><th scope=\"col\" class=\"standard-head\">?Missing Header?</th></tr></thead>"));
            Assert.AreEqual(false, result.Contains("<tfoot><tr><td class=\"standard-foot\"></td></tr></tfoot>"));
            Assert.AreEqual(true, result.Contains("<tbody><tr class=\"row-odd\"></tr><tr class=\"row-even\"></tr><tr class=\"row-odd\"></tr></tbody>"));
        }
        #endregion
    }
}
