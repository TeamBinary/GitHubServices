using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Ajax.Utilities;

namespace GitHubServices.Test.BusinessLogic.TagPageCreator
{
    public abstract class StringDomainObject
    {
        protected readonly string value;

        protected StringDomainObject(string value)
        {
            this.value = value;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return string.Equals(value, (obj as string));
        }

        public override string ToString()
        {
            return value;
        }
    }

    public class TagPageGenerator
    {
        public string GenerateAllTagsPage(Dictionary<string, string> tagToUrl)
        {
            
        }

        public string GenerateTagPage(string tag, List<Page> links)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("# Pages tagged with **{0}**", tag);
            sb.AppendLine();
            sb.AppendLine();

            foreach (var link in links)
            {
                sb.AppendFormat("* [{0}]({1})", link.Name, link.FilePath);
                sb.AppendLine();
            }
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("This file is auto generated - do not edit..");

            return sb.ToString();
        }
    }

    public class Tag : StringDomainObject
    {
        public Tag(string value)
            : base(value)
        {
        }

        public string Value
        {
            get
            {
                return value;
            }
        }
    }

    public class TagCollector
    {
        public TagCollection GetTags(string rootFilePath)
        {
            var tagsCollection = new TagCollection();
            var di = new DirectoryInfo(rootFilePath);
            foreach (var path in di.EnumerateFiles("*.md", SearchOption.AllDirectories))
            {
                var relativePath = path.FullName.Substring(rootFilePath.Length);
                var fileContent = File.ReadAllText(path.FullName);
                var tagsForPage = parsePage(fileContent, relativePath);
                tagsCollection.Add(tagsForPage);
            }

            return tagsCollection;
        }


        readonly Regex headerEx = new Regex("^# (?<title>.*)$", RegexOptions.Compiled | RegexOptions.Multiline);

        readonly Regex tagEx = new Regex(@"\[!\[Stats\]\(https://img.shields.io/badge/Tag-(?<tagname>.*)-([0-9a-fA-F]){6}\.svg\)\]", RegexOptions.Compiled);

        TagCollection parsePage(string pageContent, string fullName)
        {
            TagCollection tags = new TagCollection();
            Match headerMatch = headerEx.Match(pageContent);
            var title = headerMatch.Groups["title"].Value.Trim();

            foreach (Match match in tagEx.Matches(pageContent))
            {
                var tag = match.Groups["tagname"].Value;
                tags.Add(tag, new Page(title, fullName));
            }

            return tags;
        }
    }


    public class TagCollection
    {
        public Dictionary<string, List<Page>> TagToPages = new Dictionary<string, List<Page>>();

        public void Add(string tag, params Page[] url)
        {
            List<Page> urls;
            if (!TagToPages.TryGetValue(tag, out urls))
                TagToPages.Add(tag, urls = new List<Page>());
            urls.AddRange(url);
        }

        public void Add(TagCollection tagsForPage)
        {
            foreach (var kv in tagsForPage.TagToPages)
                Add(kv.Key, kv.Value.ToArray());
        }
    }

    public class Page : IEquatable<Page>
    {
        public readonly string Name;
        public readonly string FilePath;

        public Page(string name, string filePath)
        {
            Name = name;
            FilePath = filePath;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ FilePath.GetHashCode();
        }

        public bool Equals(Page other)
        {
            if (other == null) return false;
            return Name == other.Name && FilePath == other.FilePath;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Page);
        }
    }
}
