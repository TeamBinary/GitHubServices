using System.Collections.Generic;
using System.Linq;
using System.Text;

using GitHubServices.BusinessLogic.TagPageCreator.Domain;

namespace GitHubServices.BusinessLogic.TagPageCreator
{
    public class ContentGenerator
    {
        public string Newsletter => @"<form style=""border:1px solid #ccc;padding:3px;text-align:center;"" action=""https://tinyletter.com/QualityAndReadability"" method=""post"" target=""popupwindow"" onsubmit=""window.open('https://tinyletter.com/QualityAndReadability', 'popupwindow', 'scrollbars=yes,width=800,height=600');return true""><p><label for=""tlemail""><font color=""red"">Subscribe now to the <i>free newsletter service</i></font>.<br>Low frequency mailing list. Get notified when new articles arrive!</label></p><p><input type=""text"" onClick=""this.select();"" style=""width:140px"" name=""email"" id=""tlemail"" value=""Email address""></p><input type=""hidden"" value=""1"" name=""embed""/><input type=""submit"" value=""Subscribe"" /></form>
         ";

        public string GenerateAllArticlesPage(List<Page> pages, string baseUrl)
        {
            var groups = pages.Distinct()
                .OrderBy(x => x.Path)
                .ThenBy(x => x.Title)
                .GroupBy(x=> x.Path);

            var sb = new StringBuilder();
            AddHeader(sb, baseUrl);

            sb.Append($"## All {pages.Distinct().Count()} articles on the site");
            sb.AppendLine();
            sb.AppendLine();
            foreach (var group in groups)
            {
                sb.AppendLine("**" + group.Key + "**");
                sb.AppendLine("");
                foreach (var page in group)
                {
                    sb.AppendFormat("* [{0}]({1})", page.Title, page.FilePathWithHtmlExtension);
                    sb.AppendLine();
                }
                sb.AppendLine();
                sb.AppendLine();
            }

            AddFooter(sb);
            return sb.ToString();
        }

        public string GenerateAllTagsPage(List<Tag> tags, string baseUrl)
        {
            var sb = new StringBuilder();
            AddHeader(sb, baseUrl);

            sb.AppendFormat("## All categories on the site");
            sb.AppendLine();

            var grouping =
                tags.OrderBy(x => x.Value)
                    .Select(x => new { Firstchar = FirstChar(x), Tag = x })
                    .GroupBy(x => x.Firstchar);
            
            foreach (var group in grouping)
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("" + group.Key);
                sb.AppendLine();
                foreach (var tag in group)
                {
                    sb.AppendFormat("* {0}", CreateCategoryLink(tag.Tag, baseUrl));
                    sb.AppendLine();
                }
            }
            AddFooter(sb);
            
            return sb.ToString();
        }

        public string CreateCategoryLinkWithShields(Tag tag, string baseUrl)
        {
            return
                string.Format(
                    "[![Tag](https://img.shields.io/badge/-{0}-{1}.svg)]({2}CodeQualityAndReadability/Tags/{0}.html)",
                    tag.Value,
                    tag.HexCodeForValue, 
                    baseUrl);

        }

        public string CreateCategoryLink(Tag tag, string baseUrl)
        {
            string style = $@"color: #ffffff; font-size: 12px; margin: 1px 1px 1px 1px; padding: 2px 8px; border-radius: 4px; background-color: #{tag.HexCodeForValue};    display: inline-block;";
            string url = $"{baseUrl}Tags/{tag.Value}.html";

            return $"<a href=\"{url}\" style=\"{style}\">{tag.DisplayText}</a>";
        } 

        char FirstChar(Tag s)
        {
            return s.Value.Substring(0, 1).ToUpper()[0];
        }

        public string GenerateTagPage(Tag tag, List<Page> links, string baseUrl)
        {
            var sb = new StringBuilder();
            AddHeader(sb, baseUrl);

            sb.AppendFormat("## Pages tagged with **{0}**", tag.Value.Replace("_"," "));
            sb.AppendLine();
            sb.AppendLine();

            foreach (var link in links)
            {
                sb.AppendFormat("* [{0}](../{1})", link.Title, link.FilePathWithHtmlExtension);
                sb.AppendLine();
            }

            AddFooter(sb);
            return sb.ToString();
        }


        void AddHeader(StringBuilder sb, string baseUrl)
        {
            sb.AppendLine("# Code Quality & Readability");
            sb.AppendLine("*A site (mostly) by Kasper B. Graversen*");
            sb.AppendFormat("<br>[[Introduction]]({0}) [[All categories]]({0}AllTags.html) [[All articles]]({0}AllArticles.html)", baseUrl);
            sb.AppendLine();
            sb.AppendLine();
        }

        void AddFooter(StringBuilder sb)
        {
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
        }

    }
}