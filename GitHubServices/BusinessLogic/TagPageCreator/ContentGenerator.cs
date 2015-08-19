using System.Collections.Generic;
using System.Linq;
using System.Text;

using GitHubServices.BusinessLogic.TagPageCreator.Domain;

namespace GitHubServices.BusinessLogic.TagPageCreator
{
    public class ContentGenerator
    {
        public string GenerateAllArticlesPage(List<Page> pages)
        {
            var groups = pages.Distinct()
                .OrderBy(x => x.Path)
                .ThenBy(x => x.Title)
                .GroupBy(x=> x.Path);

            var sb = new StringBuilder();
            AddHeader(sb);

            sb.AppendFormat("## All articles on the site");
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

        public string GenerateAllTagsPage(List<Tag> tags)
        {
            var sb = new StringBuilder();
            AddHeader(sb);

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
                    sb.AppendFormat("* {0}", CreateCategoryLink(tag.Tag));
                    sb.AppendLine();
                }
            }
            AddFooter(sb);
            
            return sb.ToString();
        }

        public string CreateCategoryLink(Tag tag)
        {
            return
                string.Format(
                    "[![Tag](https://img.shields.io/badge/-{0}-{1}.svg)](http://kbilsted.github.io/CodeQualityAndReadability/Tags/{0}.html)",
                    tag.Value,
                    tag.HexCodeForValue);

        }
        static char FirstChar(Tag s)
        {
            return s.Value.Substring(0, 1).ToUpper()[0];
        }

        public string GenerateTagPage(Tag tag, List<Page> links)
        {
            var sb = new StringBuilder();
            AddHeader(sb);

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


        void AddHeader(StringBuilder sb)
        {
            sb.AppendLine("# Code Quality & Readability");
            sb.AppendLine("*A site (mostly) by Kasper B. Graversen*");
            sb.AppendLine("<br>[[Introduction]](http://kbilsted.github.io/CodeQualityAndReadability/) [[All categories]](http://kbilsted.github.io/CodeQualityAndReadability/AllTags.html) [[All articles]](http://kbilsted.github.io/CodeQualityAndReadability/AllArticles.html)");

            sb.AppendLine();
        }

        void AddFooter(StringBuilder sb)
        {
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            //sb.AppendLine("*This file is auto generated - do not edit..*");
        }
    }
}