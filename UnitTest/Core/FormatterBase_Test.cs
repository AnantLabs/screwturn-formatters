using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using ScrewTurn.Wiki.PluginFramework;
using System.Web;
using Keeper.Garrett.ScrewTurn;
using ScrewTurn.Wiki;
using Keeper.Garrett.ScrewTurn.CategoryListFormatter;
using Keeper.Garrett.ScrewTurn.Core;

namespace UnitTest
{
    /// <summary>
    /// Category,noPosts,useLastMod,showCloud,showArchive,about,bottom,style  
    /// </summary>
    [TestFixture]
    [Category("Formatter")]
    public class FormatterBase_Test
    {
        [Test]
        public void Shutdown()
        {
            //Arrange
            var formatter = new CategoryListFormatter();

            //Act
            formatter.Shutdown();

            //Assert
            Assert.AreEqual(true,true);
        }

        [Test]
        public void PrepareTitle()
        {
            //Arrange
            var formatter = new CategoryListFormatter();

            //Act
            var result = formatter.PrepareTitle("MyTitle",null);

            //Assert
            Assert.AreEqual("MyTitle", result);
        }

        [Test]
        public void PerformPhase1()
        {
            //Arrange
            var formatter = new CategoryListFormatter();

            //Act
            var result = formatter.PerformPhase1;

            //Assert
            Assert.AreEqual(true, result);
        }

        [Test]
        public void PerformPhase2()
        {
            //Arrange
            var formatter = new CategoryListFormatter();

            //Act
            var result = formatter.PerformPhase2;

            //Assert
            Assert.AreEqual(false, result);
        }

        [Test]
        public void PerformPhase3()
        {
            //Arrange
            var formatter = new CategoryListFormatter();

            //Act
            var result = formatter.PerformPhase3;

            //Assert
            Assert.AreEqual(false, result);
        }

        [Test]
        public void Init_Setup()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();

            //Act
            formatter.Init(host, "MyConf");

            //Assert
            Assert.AreEqual("MyConf", formatter.Configuration);
        }

        [Test]
        public void Init_Priority()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();

            //Act
            formatter.Init(host, "MyConf", 99);

            //Assert
            Assert.AreEqual("MyConf", formatter.Configuration);
            Assert.AreEqual(99, formatter.ExecutionPriority);
        }

        [Test]
        public void Init_LowPriority()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();

            //Act
            formatter.Init(host, "MyConf", -100);

            //Assert
            Assert.AreEqual("MyConf", formatter.Configuration);
            Assert.AreEqual(50, formatter.ExecutionPriority);
        }

        [Test]
        public void Init_HighPriority()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();

            //Act
            formatter.Init(host, "MyConf", 101);

            //Assert
            Assert.AreEqual("MyConf", formatter.Configuration);
            Assert.AreEqual(50, formatter.ExecutionPriority);
        }

        [Test]
        public void Init_NullPages()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();

            //Act
            formatter.Init(host, "MyConf", null);

            //Assert
            Assert.AreEqual("MyConf", formatter.Configuration);
            Assert.AreEqual(50, formatter.ExecutionPriority);
        }

        [Test]
        public void Init_NullPages_Priority()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();

            //Act
            formatter.Init(host, "MyConf", 49, null);

            //Assert
            Assert.AreEqual("MyConf", formatter.Configuration);
            Assert.AreEqual(49, formatter.ExecutionPriority);
        }

        [Test]
        public void Init_NullPages_LowPriority()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();

            //Act
            formatter.Init(host, "MyConf", -100, null);

            //Assert
            Assert.AreEqual("MyConf", formatter.Configuration);
            Assert.AreEqual(50, formatter.ExecutionPriority);
        }

        [Test]
        public void Init_NullPages_HighPriority()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var host = MockRepository.GenerateStub<IHostV30>();

            //Act
            formatter.Init(host, "MyConf", 101, null);

            //Assert
            Assert.AreEqual("MyConf", formatter.Configuration);
            Assert.AreEqual(50, formatter.ExecutionPriority);
        }

        [Test]
        public void Information()
        {
            //Arrange
            var formatter = new CategoryListFormatter();

            //Act
            var result = formatter.Information;

            //Assert
            Assert.AreEqual("Christian Hollerup Mikkelsen", result.Author);
            Assert.AreEqual("CategoryListFormatter", result.Name);
            Assert.AreEqual(null, result.UpdateUrl);
            Assert.AreEqual("http://keeper.endoftheinternet.org/", result.Url);
            Assert.AreEqual(true, result.Version.Contains("3.0"));
        }

        [Test]
        public void ExecutionPriority()
        {
            //Arrange
            var formatter = new CategoryListFormatter();

            //Act
            var result = formatter.ExecutionPriority;

            //Assert
            Assert.AreEqual(50, result);
        }

        [Test]
        public void Format()
        {
            //Arrange
            var formatter = new CategoryListFormatter();
            var context = new ContextInformation(false, false, FormattingContext.Header, null, "", HttpContext.Current, "", new string[] { "" });

            //Act
            var result = formatter.Format("",context,FormattingPhase.Phase3);

            //Assert
            Assert.AreEqual("", result);
        }

        [Test]
        public void Configuration()
        {
            //Arrange
            var formatter = new CategoryListFormatter();

            //Act
            var result = formatter.Configuration;

            //Assert
            Assert.AreEqual(null, result);
        }

        [Test]
        public void ConfigHelpHtml()
        {
            //Arrange
            var formatter = new CategoryListFormatter();

            //Act
            var result = formatter.ConfigHelpHtml;

            //Assert
            Assert.AreEqual("No configuration needed for this formatter.", result);
        }
    }
}
