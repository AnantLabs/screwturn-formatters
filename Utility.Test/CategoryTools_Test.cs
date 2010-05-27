using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Keeper.Garrett.ScrewTurn.Utility;
using Rhino.Mocks;
using ScrewTurn.Wiki.PluginFramework;

namespace Utility.Tests
{
    [TestFixture]
    [Category("Utility")]
    public class CategoryTools_Test
    {
        [Test]
        public void GetCategory_RootNamespace()
        {
            //Arrange
            var host = MockRepository.GenerateMock<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();

            var catInfo1 = MockRepository.GenerateStub<CategoryInfo>("Test", provider);
            var catInfo2 = MockRepository.GenerateStub<CategoryInfo>("Test", provider);

            NamespaceInfo rootNs = null;
            NamespaceInfo testNs = new NamespaceInfo("TestNamespace", provider, null);

            //Expect
            provider.Expect(x => x.GetCategory("Test")).Return(catInfo1);
            provider.Expect(x => x.GetCategory("Test")).Return(catInfo2);
            provider.Expect(x => x.GetCategories(rootNs)).Return(new CategoryInfo[2] { catInfo1, catInfo2 });
            provider.Expect(x => x.GetCategories(testNs)).Return(new CategoryInfo[2] { catInfo1, catInfo2 });

            host.Expect(x => x.FindNamespace(null)).Return(rootNs);
            host.Expect(x => x.FindNamespace("TestNamespace")).Return(testNs);

            //Act
            var cats = CategoryTools.GetCategoryInformation(host, provider, "Test", null, "");

            //Assert
            Assert.AreEqual(1, cats.Count);
            Assert.AreEqual("Test", cats[0].FullName);
        }

        [Test]
        public void GetCategory_TestNamespace()
        {
            //Arrange
            var host = MockRepository.GenerateMock<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();

            var catInfo1 = MockRepository.GenerateStub<CategoryInfo>("Test1", provider);
            var catInfo2 = MockRepository.GenerateStub<CategoryInfo>("Test2", provider);

            NamespaceInfo rootNs = null;
            NamespaceInfo testNs = new NamespaceInfo("TestNamespace", provider, null);

            //Expect
            provider.Expect(x => x.GetCategories(testNs)).Return(new CategoryInfo[2] { catInfo1, catInfo2 });

            host.Expect(x => x.FindNamespace(null)).Return(rootNs);
            host.Expect(x => x.FindNamespace("TestNamespace")).Return(testNs);

            //Act
            var cats = CategoryTools.GetCategoryInformation(host, provider, "Test2", "TestNamespace", "");

            //Assert
            Assert.AreEqual(1, cats.Count);
            Assert.AreEqual("Test2", cats[0].FullName);
        }

        [Test]
        public void GetCategory_TestNamespace_GetRoot()
        {
            //Arrange
            var host = MockRepository.GenerateMock<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();

            var catInfo1 = MockRepository.GenerateStub<CategoryInfo>("Test2", provider);

            NamespaceInfo rootNs = null;
            NamespaceInfo testNs = new NamespaceInfo("TestNamespace", provider, null);

            //Expect
            provider.Expect(x => x.GetCategory("Test2")).Return(catInfo1);

            host.Expect(x => x.FindNamespace(null)).Return(rootNs);
            host.Expect(x => x.FindNamespace("TestNamespace")).Return(testNs);

            //Act
            var cats = CategoryTools.GetCategoryInformation(host, provider, "Test2", "TestNamespace", "root");

            //Assert
            Assert.AreEqual(1, cats.Count);
            Assert.AreEqual("Test2", cats[0].FullName);
        }

        [Test]
        public void GetCategory_RootNamespace_GetTest()
        {
            //Arrange
            var host = MockRepository.GenerateMock<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();

            var catInfo1 = MockRepository.GenerateStub<CategoryInfo>("Test2", provider);
            var catInfo2 = MockRepository.GenerateStub<CategoryInfo>("Test2", provider);

            NamespaceInfo rootNs = null;
            NamespaceInfo testNs = new NamespaceInfo("TestNamespace", provider, null);

            //Expect
            provider.Expect(x => x.GetCategories(testNs)).Return(new CategoryInfo[2] { catInfo1, catInfo2 });

            host.Expect(x => x.FindNamespace(null)).Return(rootNs);
            host.Expect(x => x.FindNamespace("TestNamespace")).Return(testNs);

            //Act
            var cats = CategoryTools.GetCategoryInformation(host, provider, "Test2", "", "TestNamespace");

            //Assert
            Assert.AreEqual(1, cats.Count);
            Assert.AreEqual("Test2", cats[0].FullName);
        }

        [Test]
        public void GetCategory_RootNamespace_GetTest_GetRoot()
        {
            //Arrange
            var host = MockRepository.GenerateMock<IHostV30>();
            var provider = MockRepository.GenerateStub<IPagesStorageProviderV30>();

            var catInfo1 = MockRepository.GenerateStub<CategoryInfo>("Test2", provider);
            var catInfo2 = MockRepository.GenerateStub<CategoryInfo>("Test2", provider);

            NamespaceInfo rootNs = null;
            NamespaceInfo testNs = new NamespaceInfo("TestNamespace", provider, null);

            //Expect
            provider.Expect(x => x.GetCategory("Test2")).Return(catInfo1);
            provider.Expect(x => x.GetCategories(testNs)).Return(new CategoryInfo[2] { catInfo1, catInfo2 });

            host.Expect(x => x.FindNamespace(null)).Return(rootNs);
            host.Expect(x => x.FindNamespace("TestNamespace")).Return(testNs);

            //Act
            var cats = CategoryTools.GetCategoryInformation(host, provider, "Test2", "", "TestNamespace,RooT");

            //Assert
            Assert.AreEqual(2, cats.Count);
            Assert.AreEqual("Test2", cats[0].FullName);
            Assert.AreEqual("Test2", cats[1].FullName);
        }
    }
}
