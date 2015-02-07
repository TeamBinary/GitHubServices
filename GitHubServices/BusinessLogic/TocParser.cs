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
            var lines = content.Split(new[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            var tocLines = lines.Where(x => x.StartsWith("#")).ToList();
            var fakeTocLines =
                tocLines
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