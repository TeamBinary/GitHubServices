using System.Globalization;
using StatePrinting;
using StatePrinting.Configurations;
using StatePrinting.TestAssistance;
using StatePrinting.ValueConverters;

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
                  cfg.LegacyBehaviour.TrimTrailingNewlines = true;
            var printer = new Stateprinter(cfg);

            return printer;
        }

        public static Asserter Assert()
        {
            return Printer().Assert;
        }

        public static Asserter Assert2()
        {
            var printer = Printer();
            printer.Configuration.Add(new StringConverter(""));
            return printer.Assert;
        }
    }
}
