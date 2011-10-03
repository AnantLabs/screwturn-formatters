using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using ScrewTurn.Wiki.PluginFramework;
using Keeper.Garrett.ScrewTurn;
using Keeper.Garrett.ScrewTurn.Utility;

namespace Utility.Tests
{
    [TestFixture]
    [Category("Utility")]
    public class Avatar_Test
    {
        [Test]
        public void No_Email()
        {
            //Arrange

            //Act
            var result = Avatar.GenerateAvatarLink(null);

            //Assert
            Assert.AreEqual("<img src=\"http://www.gravatar.com/avatar/dd4236bdb7608cac87ba85716056bafb?d=identicon\" alt=\"Gravatar\" height=\"80\" width=\"80\" />", result);
        }

        [Test]
        public void Standard_Email()
        {
            //Arrange

            //Act
            var result = Avatar.GenerateAvatarLink("Christian.H.Mikkkelsen@gmail.com");

            //Assert
            Assert.AreEqual("<img src=\"http://www.gravatar.com/avatar/df65bc2a681586b5db9ebe6960a027a7?d=identicon\" alt=\"Gravatar\" height=\"80\" width=\"80\" />", result);
        }
    }
}
