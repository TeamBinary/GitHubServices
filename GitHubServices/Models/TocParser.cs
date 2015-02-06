using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitHubServices.Models
{
    public class TocParser
    {
        public string MakeToc(string content)
        {
            var fakeTocLines =
                content.Split(
                    new[] { Environment.NewLine },
                    StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => x.StartsWith("#"))
                    .Select(x => x.Replace("#", ""))
                    .ToList();

            var tocString = "# Table of Content";
            if (fakeTocLines.Any())
            {
                tocString += Environment.NewLine + "*"
                            + String.Join(Environment.NewLine + "*", fakeTocLines);
            }

            return tocString;
        }
    }

}