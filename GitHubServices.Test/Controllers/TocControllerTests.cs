﻿using System;
using System.Net.Http;
using System.Web.Http;
using GitHubServices.Controllers;
using GitHubServices.Models;
using Moq;
using NUnit.Framework;
using PowerAssert;

namespace GitHubServices.Test.Controllers
{
  [TestFixture]
  [Category("UnitTest")]
  public class TocControllerTests
  {
    const string anyUrl = "http://a.com/file.md";
    readonly TocController controller = new TocController
                                                {
                                                  Request = new HttpRequestMessage(),
                                                  Configuration = new HttpConfiguration(),
                                                };

    [Test]
    public void ContentDoesNotContainAnyHeaders()
    {
      // Arrange
      var expectedToc = new Toc { ToCValueForPasting = "# Table of Content" };

      const string content = "There is something here, but no headers";
      var reader = new Mock<UrlReader>(MockBehavior.Strict);
      reader.Setup(x => x.ReadUrl(It.IsAny<Uri>())).Returns(content);
      controller.reader = reader.Object;

      // Act
      var response = controller.Get(anyUrl);

      // Assert
      var printer = Create.Printer();
      Toc actualToc = null;
      PAssert.IsTrue(() => response.TryGetContentValue(out actualToc));
     // Assert.AreEqual(printer.PrintObject(expectedToc), printer.PrintObject(actualToc));
    }

    [Test]
    public void ContentContainsSingleHeader_no_toc_marker()
    {
      // Arrange
      var content = "There is a single header" + Environment.NewLine 
          + "# Right Here" + Environment.NewLine 
          + "But nothing more";
      var reader = new Mock<UrlReader>(MockBehavior.Strict);
      reader.Setup(x => x.ReadUrl(It.IsAny<Uri>())).Returns(content);
      controller.reader = reader.Object;

      // Act
      var response = controller.Get(anyUrl);

      // Assert
      var printer = Create.Printer();
      Toc actualToc = null;
      PAssert.IsTrue(() => response.TryGetContentValue(out actualToc));
      var expected = @"new Toc()
{
    ToCValueForPasting = ""Table of Content""
}";
      printer.Assert.PrintIsSame(expected, actualToc);
    }


    [Test]
    public void ContentContainsSingleHeader()
    {
        // Arrange
        var content = "There is a single header" + Environment.NewLine 
            + "Table of content" + Environment.NewLine 
            + "# Right Here" + Environment.NewLine 
            + "But nothing more";
        var reader = new Mock<UrlReader>(MockBehavior.Strict);
        reader.Setup(x => x.ReadUrl(It.IsAny<Uri>())).Returns(content);
        controller.reader = reader.Object;

        // Act
        var response = controller.Get(anyUrl);

        // Assert
        var printer = Create.Printer();
        Toc actualToc = null;
        PAssert.IsTrue(() => response.TryGetContentValue(out actualToc));
        var expected = @"new Toc()
{
    ToCValueForPasting = ""Table of Content
 * [Right Here](#right-here)
""
}";
        printer.Assert.PrintIsSame(expected, actualToc);
    }

    [Test]
    public void ContentContainsMultipleHeaders()
    {
      // Arrange
      var expectedToc = new Toc
      {
        ToCValueForPasting = "# Table of Content" + Environment.NewLine + "* One" + Environment.NewLine + "* Two" + Environment.NewLine + "* Three"
      };
      var content = "There contains multiple headers" + Environment.NewLine + "# One" + Environment.NewLine + "But nothing more" + Environment.NewLine + "# Two" + Environment.NewLine + "# Three";
      var reader = new Mock<UrlReader>(MockBehavior.Strict);
      reader.Setup(x => x.ReadUrl(It.IsAny<Uri>())).Returns(content);
      controller.reader = reader.Object;

      // Act
      var response = controller.Get(anyUrl);

      // Assert
      var printer = Create.Printer();
      Toc actualToc = null;
      PAssert.IsTrue(() => response.TryGetContentValue(out actualToc));
      //Assert.AreEqual(printer.PrintObject(expectedToc), printer.PrintObject(actualToc));
    }

  }
}
