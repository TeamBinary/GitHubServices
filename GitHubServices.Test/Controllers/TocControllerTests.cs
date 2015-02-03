using System;
using System.Net.Http;
using System.Web.Http;
using GitHubServices.Controllers;
using GitHubServices.Models;
using NUnit.Framework;
using PowerAssert;
using StatePrinter;
using StatePrinter.Configurations;

namespace GitHubServices.Test.Controllers
{
  [TestFixture]
  [Category("UnitTest")]
  public class TocControllerTests
  {
    private readonly TocController controller = new TocController
                                                {
                                                  Request = new HttpRequestMessage(),
                                                  Configuration = new HttpConfiguration()
                                                };

    [Test]
    public void ContentDoesNotContainAnyHeaders()
    {
      // Arrange
      var expectedToc = new Toc { ToCValueForPasting = "# Table of Content" };
      const string content = "There is something here, but no headers";

      // Act
      var response = controller.CreateToc(content);

      // Assert
      var printer = GetTestPrinter();
      Toc actualToc = null;
      PAssert.IsTrue(() => response.TryGetContentValue(out actualToc));
      Assert.AreEqual(printer.PrintObject(expectedToc), printer.PrintObject(actualToc));
    }

    [Test]
    public void ContentContainsSingleHeader()
    {
      // Arrange
      var expectedToc = new Toc
      {
        ToCValueForPasting = "# Table of Content" + Environment.NewLine + "* Right Here"
      };
      var content = "There is a single header" + Environment.NewLine + "# Right Here" + Environment.NewLine + "But nothing more";

      // Act
      var response = controller.CreateToc(content);

      // Assert
      var printer = GetTestPrinter();
      Toc actualToc = null;
      PAssert.IsTrue(() => response.TryGetContentValue(out actualToc));
      Assert.AreEqual(printer.PrintObject(expectedToc), printer.PrintObject(actualToc));
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

      // Act
      var response = controller.CreateToc(content);

      // Assert
      var printer = GetTestPrinter();
      Toc actualToc = null;
      PAssert.IsTrue(() => response.TryGetContentValue(out actualToc));
      Assert.AreEqual(printer.PrintObject(expectedToc), printer.PrintObject(actualToc));
    }

    private static Stateprinter GetTestPrinter()
    {
      var cfg = ConfigurationHelper.GetStandardConfiguration();
      var printer = new Stateprinter(cfg);

      return printer;
    }
  }
}
