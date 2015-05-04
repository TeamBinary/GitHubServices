using System.Globalization;

using StatePrinter;
using StatePrinter.Configurations;
using StatePrinter.TestAssistance;

namespace GitHubServices.Test
{
    static class Create
    {
        public static Stateprinter Printer()
        {
            var cfg =
                ConfigurationHelper
                  .GetStandardConfiguration()
                  .SetAreEqualsMethod(NUnit.Framework.Assert.AreEqual)
                  .SetCulture(CultureInfo.CreateSpecificCulture("da-DK"));
            var printer = new Stateprinter(cfg);

            return printer;
        }

        public static Asserter Assert()
        {
            return Printer().Assert;
        }
    }
}
