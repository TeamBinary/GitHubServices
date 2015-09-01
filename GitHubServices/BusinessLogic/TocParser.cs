using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GitHubServices.Models
{
    public class TocEntry
    {
        public string Title, Level;
    }

    public class ContentReplacer
    {
        static string title = @"(T|t)able (O|o)f (C|c)ontent\s*(\r?\n)+";
        static string tocline = @"(\s*\*[^\n]*\n)*";

        static readonly Regex TocRex = new Regex(title + tocline, RegexOptions.Singleline | RegexOptions.Compiled);

        public string TryReplaceToc(string content, string newToc)
        {
            var match = TocRex.Match(content);
            if (!match.Success)
                return null;

            return content.Substring(0, match.Index)
                + newToc
                + content.Substring(match.Index + match.Length);
        }
    }

    public class TocParser
    {
        static readonly Regex TocRex = new Regex("^(?<level>#+)( |\t)+(?<title>.*)", RegexOptions.Multiline | RegexOptions.Compiled);

        public string MakeToc(string content)
        {
            var parsedToc = Parse(content);

            var tocString = MarkDown(parsedToc);
            return tocString;
        }

        string MarkDown(List<TocEntry> parsedToc)
        {
            var tocString = "Table of Content";
            if (!parsedToc.Any())
                return tocString;

            var markdownedTocLines = parsedToc.Select(x => Markdowned(x));
            tocString += Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine, markdownedTocLines)
                         + Environment.NewLine;

            return tocString;
        }


        List<TocEntry> Parse(string content)
        {
            var lines = content.Split(new[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

            var tocLines = 
                lines.SkipWhile(x => !x.ToLower().Contains("table of content"))
                .Skip(1)
                .Select(x => x.Trim())
                .Where(x => x.StartsWith("#"))
                .ToArray();

            var parsedToc =
                tocLines.Select(x => TocRex.Match(x))
                    .Select(x => new TocEntry()
                    {
                        Title = x.Groups["title"].Value,
                        Level = x.Groups["level"].Value
                    });
            return parsedToc.ToList();
        }

        string MarkdownedGithub(TocEntry entry)
        {
            string space = entry.Level == "" ? "" : entry.Level.Replace("#", "  ").Substring(1);
            var link = "#" + entry.Title
                .Trim()
                .Replace(".", "")
                .Replace(",", "")
                .Replace(":", "")
                .Replace("!", "")
                .Replace("/", "")
                .Replace("\"", "")
                .Replace("`", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace(" ", "-")
                .ToLowerInvariant();

            return string.Format("{0}* [{1}]({2})", space, entry.Title, link);
        }

        // for markdowndeep
        string Markdowned(TocEntry entry)
        {
            Regex stuffRemover = new Regex("(`[^`]*`)|(\\d+)");
            string space = entry.Level == "" ? "" : entry.Level.Replace("#", "  ").Substring(1);
            var link = "#" + stuffRemover.Replace(entry.Title, x=>"")
                .Trim()
                .Replace(".", "")
                .Replace(",", "")
                .Replace(":", "")
                .Replace("!", "")
                .Replace("/", "")
                .Replace("\"", "")
                .Replace("`", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace(" ", "-")
                .ToLowerInvariant();

            return string.Format("{0}* [{1}]({2})", space, entry.Title, link);
        }
    }

}