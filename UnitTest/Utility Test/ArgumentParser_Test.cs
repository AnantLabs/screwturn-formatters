using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using ScrewTurn.Wiki.PluginFramework;
using System.Web;
using Keeper.Garrett.ScrewTurn;
using Keeper.Garrett.ScrewTurn.FileListFormatter;
using Keeper.Garrett.ScrewTurn.FileContentFormatter;
using Keeper.Garrett.ScrewTurn.Utility;

namespace UnitTest
{
    [TestFixture]
    [Category("Utility")]
    public class ArgumentParser_Test
    {
        [Test]
        public void No_Arguments()
        {
            //Arrange
            var parser = new ArgumentParser();

            //Act
            var result = parser.Parse("!\"#¤%&/()?QAZXSWEDCVFRTGBNHYUJM;KIOL:_ÆPØ*^`?>§½@£$€{[]}|~1234567890+´`qwertyuiopå¨asdfghjklæø'zxcvbnm,.-<");

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void StartWhitespace_3_Enclosed_Arguments()
        {
            //Arrange
            var parser = new ArgumentParser();

            //Act
            var result = parser.Parse("   a='aaa' b='bbb' c='ccc'");

            //Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("aaa", result["a"]);
            Assert.AreEqual("bbb", result["b"]);
            Assert.AreEqual("ccc", result["c"]);
        }

        [Test]
        public void StartWhitespace_3_NotEnclosed_Arguments()
        {
            //Arrange
            var parser = new ArgumentParser();

            //Act
            var result = parser.Parse("   a=aaa b=bbb c=ccc");

            //Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("aaa", result["a"]);
            Assert.AreEqual("bbb", result["b"]);
            Assert.AreEqual("ccc", result["c"]);
        }

        [Test]
        public void EndWhitespace_3_Enclosed_Arguments()
        {
            //Arrange
            var parser = new ArgumentParser();

            //Act
            var result = parser.Parse("a='aaa' b='bbb' c='ccc'  ");

            //Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("aaa", result["a"]);
            Assert.AreEqual("bbb", result["b"]);
            Assert.AreEqual("ccc", result["c"]);
        }

        [Test]
        public void EndWhitespace_3_NotEnclosed_Arguments()
        {
            //Arrange
            var parser = new ArgumentParser();

            //Act
            var result = parser.Parse("a=aaa b=bbb c=ccc  ");

            //Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("aaa", result["a"]);
            Assert.AreEqual("bbb", result["b"]);
            Assert.AreEqual("ccc", result["c"]);
        }

        [Test]
        public void Whitespaces_3_Enclosed_Arguments()
        {
            //Arrange
            var parser = new ArgumentParser();

            //Act
            var result = parser.Parse("   a='aaa' b='bbb' c='ccc'  ");

            //Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("aaa", result["a"]);
            Assert.AreEqual("bbb", result["b"]);
            Assert.AreEqual("ccc", result["c"]);
        }

        [Test]
        public void StuffedEnds_3_NotEnclosed_Arguments()
        {
            //Arrange
            var parser = new ArgumentParser();

            //Act
            var result = parser.Parse("=asd,!\"#¤¤%¤&/() a=aaa b=bbb c=ccc =FDPOEMGPGDFH;*_;");

            //Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("aaa", result["a"]);
            Assert.AreEqual("bbb", result["b"]);
            Assert.AreEqual("ccc", result["c"]);
        }

        [Test]
        public void Combined_Enclosements_3_Arguments()
        {
            //Arrange
            var parser = new ArgumentParser();

            //Act
            var result = parser.Parse("a='aaa' b=bbb c='ccc'");

            //Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("aaa", result["a"]);
            Assert.AreEqual("bbb", result["b"]);
            Assert.AreEqual("ccc", result["c"]);
        }

        [Test]
        public void UsingSlash_3_Enclosed_Arguments()
        {
            //Arrange
            var parser = new ArgumentParser();

            //Act
            var result = parser.Parse("   //a='aaa' /b='bbb' /c='ccc'");

            //Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("aaa", result["a"]);
            Assert.AreEqual("bbb", result["b"]);
            Assert.AreEqual("ccc", result["c"]);
        }
    }
}
