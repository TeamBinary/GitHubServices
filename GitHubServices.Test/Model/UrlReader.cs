using System;

using GitHubServices.Models;

using NUnit.Framework;

namespace GitHubServices.Test.Model
{
    [TestFixture]
    [Category("UnitTest")]
    class UrlReaderTest
    {
        [Test]
        public void TestGetContent()
        {
            var sut = new UrlReader();
            var content = sut.ReadUrl(new Uri("https://www.google.dk/"));
            Assert.IsTrue(content.Contains("<title>Google</title>"));
        }
    }

}
