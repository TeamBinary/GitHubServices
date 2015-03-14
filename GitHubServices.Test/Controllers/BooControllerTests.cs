using System;
using System.Globalization;
using System.Net.Http;
using System.Web.Http;
using GitHubServices.Controllers;
using GitHubServices.Models;
using Moq;
using NUnit.Framework;
using PowerAssert;
using StatePrinter;
using StatePrinter.Configurations;

namespace GitHubServices.Test.Controllers
{
  [TestFixture]
  [Category("UnitTest")]
  public class BooControllerTests
  {
    const string anyUrl = "http://a.com/file.md";
    readonly BooController controller = new BooController
                                                {
                                                  Request = new HttpRequestMessage(),
                                                  Configuration = new HttpConfiguration(),
                                                };


    // [Test]
    public void ContentContainsSingleHeader()
    {
      // Arrange
      var expectedToc = new Toc
      {
        ToCValueForPasting = "# Table of Content" + Environment.NewLine + "* Right Here"
      };
      var content = "There is a single header" + Environment.NewLine + "# Right Here" + Environment.NewLine + "But nothing more";
      var reader = new Mock<UrlReader>(MockBehavior.Strict);
      reader.Setup(x => x.ReadUrl(It.IsAny<Uri>())).Returns(content);

      // Act
      var response = controller.Get(anyUrl);

      // Assert
      var printer = Create.Printer();
      Toc actualToc = null;
      PAssert.IsTrue(() => response.TryGetContentValue(out actualToc));
      var expected = @"new Toc()
{
    ToCValueForPasting = ""Support custom ordering of collections::Open, projection should enable adding computed field::Open, integration with coverty::Open, Add tabular as an output form::Open, Stateprinter.printobject should not use optional paramters::Open""
}";



      printer.Assert.PrintIsSame(expected, actualToc);
    }

  

    
  }
}
