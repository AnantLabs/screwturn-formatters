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
    public class ReverseDataComparer_Test
    {
        [Test]
        public void Standard_Sort()
        {
            //Arrange
            var dict = new SortedList<DateTime, string>();
            dict.Add(new DateTime(2010, 1, 1), "1");
            dict.Add(new DateTime(2010, 2, 1), "2");
            dict.Add(new DateTime(2010, 3, 1), "3");
            dict.Add(new DateTime(2010, 4, 1), "4");

            //Act

            //Assert
            Assert.AreEqual(3, dict.IndexOfKey(new DateTime(2010, 4, 1)));
            Assert.AreEqual(2, dict.IndexOfKey(new DateTime(2010, 3, 1)));
            Assert.AreEqual(1, dict.IndexOfKey(new DateTime(2010, 2, 1)));
            Assert.AreEqual(0, dict.IndexOfKey(new DateTime(2010, 1, 1)));
        }

        [Test]
        public void Custom_Sort()
        {
            //Arrange
            var dict = new SortedList<DateTime, string>(new ReverseDateComparer());
            dict.Add(new DateTime(2010, 1, 1), "1");
            dict.Add(new DateTime(2010, 2, 1), "2");
            dict.Add(new DateTime(2010, 3, 1), "3");
            dict.Add(new DateTime(2010, 4, 1), "4");

            //Act

            //Assert
            Assert.AreEqual(0, dict.IndexOfKey(new DateTime(2010, 4, 1)));
            Assert.AreEqual(1, dict.IndexOfKey(new DateTime(2010, 3, 1)));
            Assert.AreEqual(2, dict.IndexOfKey(new DateTime(2010, 2, 1)));
            Assert.AreEqual(3, dict.IndexOfKey(new DateTime(2010, 1, 1)));
        }
    }
}
