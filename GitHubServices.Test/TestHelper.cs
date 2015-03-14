using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using StatePrinter;
using StatePrinter.Configurations;

namespace GitHubServices.Test
{
    static class Create
    {
        public static Stateprinter Printer()
        {
            var cfg =
                ConfigurationHelper
                  .GetStandardConfiguration()
                  .SetAreEqualsMethod(Assert.AreEqual)
                  .SetCulture(CultureInfo.CreateSpecificCulture("da-DK"));
            var printer = new Stateprinter(cfg);

            return printer;
        }
    }
}
