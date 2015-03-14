using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace GitHubServices.Models
{

    class TocEntry
    {
        public string Title, Level;

    }
    public class TocParser
    {
        static Regex tocRex= new Regex("(?<level>#+)( |\t)+(?<title>.*)", RegexOptions.Multiline | RegexOptions.Compiled);

        public string MakeToc(string content)
        {
            var lines = content.Split(new[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            var tocLines = lines.Where(x => x.StartsWith("#")).ToList();
            var parsedToc = tocLines
                .Select(x => tocRex.Match(x))
                .Select(x => new TocEntry() { Title = x.Groups["title"].Value.Trim(), Level = x.Groups["level"].Value });
            
            var tocString = "# Table of Content";
            if (parsedToc.Any())
            {
                var markdownedTocLines = parsedToc.Select(x => Markdowned(x));
                tocString += Environment.NewLine 
                   
                    + String.Join(Environment.NewLine, markdownedTocLines);
            }

            return tocString;
        }

        private string Markdowned(TocEntry entry)
        {
            var space = entry.Level.Replace("#", "  ").Substring(1);
            var link = "#" + entry.Title
                .Replace(" ", "-")
                .Replace(".", "")
                .Replace("(","")
                .Replace(")","")
                .ToLowerInvariant();

            return string.Format("{0}* [{1}]({2})", space, entry.Title, link);
        }
    }

}