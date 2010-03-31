using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using ScrewTurn.Wiki.PluginFramework;
using System.Web;
using Keeper.Garrett.ScrewTurn;
using Keeper.Garrett.ScrewTurn.CategoryListFormatter;

namespace UnitTest
{
    [TestFixture]
    [Category("Formatter")]
    public class CategoryListFormatter_Test
    {
 
        [TestFixtureSetUp]
        public void Setup()
        {
        }

        [Test]
        public void DBLinkEntries()
        {
            //Arrange
            var host = MockRepository.GenerateStub<IHostV30>();
            var formatter = new CategoryListFormatter();

            //Host
            host.Expect(x => x.GetCurrentUser()).Repeat.Any().Return(new UserInfo("Garrett", "Garrett", "", true, DateTime.Now, null));

            string input = "bla bla bla {CategoryList(Product,false,false)} bla bla bla";

            //Dict page
            var context = new ContextInformation(false, false, FormattingContext.PageContent, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            formatter.Init(host, "");
            var retval = formatter.Format(input, context, FormattingPhase.Phase1);

            //Assert
            Assert.AreEqual(retval, input);
        }

    }
}
