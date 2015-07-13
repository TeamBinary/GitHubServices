using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using GitHubServices.BusinessLogic.TagPageCreator.Domain;

namespace GitHubServices.BusinessLogic.TagPageCreator
{
    public class SiteGenerator
    {
        readonly ContentGenerator generator;
        readonly IFilesystemRepository filesystemRepository;
        readonly TagCollector collector;

        public SiteGenerator(ContentGenerator generator, IFilesystemRepository filesystemRepository, TagCollector collector)
        {
            this.generator = generator;
            this.filesystemRepository = filesystemRepository;
            this.collector = collector;
        }

        public void GenerateSite(string rootPath)
        {
            var tags = collector.GetTags(rootPath);

            AllArticlesPage(rootPath, tags);
            TagPages(rootPath, tags);
            AllTagsPage(rootPath, tags);
        }

        void AllTagsPage(string rootPath, TagCollection tags)
        {
            var allTags = generator.GenerateAllTagsPage(tags.Select(x => x.Key).ToList());
            filesystemRepository.WriteFile(Path.Combine(rootPath, "AllTags.md"), allTags);
        }

        void TagPages(string rootPath, TagCollection tags)
        {
            var tagDir = Path.Combine(rootPath, "Tags");
            filesystemRepository.EmptyTagDirectory(tagDir);
            
            foreach (var tag in tags)
            {
                var tagPage = generator.GenerateTagPage(tag.Key, tag.Value);
                filesystemRepository.WriteFile(Path.Combine(tagDir, tag.Key + ".md"), tagPage);
            }
        }

        void AllArticlesPage(string rootPath, TagCollection tags)
        {
            var allArticles = generator.GenerateAllArticlesPage(tags.SelectMany(x => x.Value).ToList());
            filesystemRepository.WriteFile(Path.Combine(rootPath, "AllArticles.md"), allArticles);
        }
    }

    public interface IFilesystemRepository
    {
        void EmptyTagDirectory(string rootFolder);

        void WriteFile(string filepath, string content);

        string ReadFile(string filepath);
    }

    public class FilesystemRepository : IFilesystemRepository
    {
        public void EmptyTagDirectory(string tagDir)
        {
            if (Directory.Exists(tagDir))
            {
                var directory = new DirectoryInfo(tagDir);
                foreach (FileInfo file in directory.GetFiles())
                    file.Delete();
                Directory.Delete(tagDir);
            }
            Directory.CreateDirectory(tagDir);
        }

        public void WriteFile(string filepath, string content)
        {
            bool write = false;
            if (!File.Exists(filepath))
                write = true;
            else
            {
                if (File.ReadAllText(filepath) != content)
                    write = true;
            }

            if (write)
            {
                Console.WriteLine("Writing " + filepath);
                File.WriteAllText(filepath, content, new UTF8Encoding(true));
            }
        }

        public string ReadFile(string filepath)
        {
            return File.ReadAllText(filepath, new UTF8Encoding());
        }
    }


    public class ContentGenerator
    {
        public string GenerateAllArticlesPage(List<Page> pages)
        {
            var groups = pages.Distinct()
                .OrderBy(x => x.Path)
                .ThenBy(x => x.Title)
                .GroupBy(x=> x.Path);

            var sb = new StringBuilder();
            addHeader(sb);

            sb.AppendFormat("## All articles on the site");
            sb.AppendLine();
            sb.AppendLine();
            foreach (var group in groups)
            {
                sb.AppendLine("**" + group.Key + "**");
                foreach (var page in group)
                {
                    sb.AppendFormat("* [{0}]({1})", page.Title, page.FilePath);
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
            addHeader(sb);

            sb.AppendFormat("## All tags on the site");
            sb.AppendLine();

            var grouping =
                tags.OrderBy(x => x.Value)
                .Select(x => new { Firstchar = FirstChar(x), x.Value })
                .GroupBy(x => x.Firstchar);
            
            foreach (var group in grouping)
            {
                sb.AppendLine();
                sb.AppendLine("" + group.Key);
                foreach (var tag in group)
                {
                    sb.AppendFormat("* [{0}](Tags/{0}.md)", tag.Value);
                    sb.AppendLine();
                }
            }
            AddFooter(sb);
            
            return sb.ToString();
        }

        static char FirstChar(Tag s)
        {
            return s.Value.Substring(0, 1).ToUpper()[0];
        }

        public string GenerateTagPage(Tag tag, List<Page> links)
        {
            var sb = new StringBuilder();
            addHeader(sb);

            sb.AppendFormat("## Pages tagged with **{0}**", tag);
            sb.AppendLine();
            sb.AppendLine();

            foreach (var link in links)
            {
                sb.AppendFormat("* [{0}](../{1})", link.Title, link.FilePath);
                sb.AppendLine();
            }

            AddFooter(sb);
            return sb.ToString();
        }


        void addHeader(StringBuilder sb)
        {
            sb.AppendLine("# Code Quality & Readability");
            sb.AppendLine("*A site by Kasper B. Graversen*");
            sb.AppendLine("<br>[[All categories]](https://github.com/kbilsted/CodeQualityAndReadability/blob/master/AllTags.md) [[All articles]](https://github.com/kbilsted/CodeQualityAndReadability/blob/master/AllArticles.md)");

            sb.AppendLine();
        }

        static void AddFooter(StringBuilder sb)
        {
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("*This file is auto generated - do not edit..*");
        }
    }

    public class TagCollector
    {
        readonly IFilesystemRepository filesystemRepository;

        public TagCollector(IFilesystemRepository filesystemRepository)
        {
            this.filesystemRepository = filesystemRepository;
        }
        
        public TagCollection GetTags(string rootFilePath)
        {
            var tagsCollection = new TagCollection();
            var di = new DirectoryInfo(rootFilePath);
            foreach (var path in di.EnumerateFiles("*.md", SearchOption.AllDirectories))
            {
                var relativePath = path.FullName.Substring(rootFilePath.Length);
                var fileContent = filesystemRepository.ReadFile(path.FullName);
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
                var tag = new Tag(match.Groups["tagname"].Value);
                tags.Add(tag, new Page(title, fullName));
            }

            return tags;
        }
    }


    public class TagCollection : IEnumerable<KeyValuePair<Tag,List<Page>>>
    {
        readonly Dictionary<Tag, List<Page>> Tags = new Dictionary<Tag, List<Page>>();
        readonly Dictionary<string, Tag> lowerCaseDistinct = new Dictionary<string, Tag>(); 

        public void Add(Tag tag, params Page[] url)
        {
            Tag distinctTag;
            if (!lowerCaseDistinct.TryGetValue(tag.Value.ToLower(), out distinctTag))
            {
                distinctTag = tag;
                lowerCaseDistinct.Add(tag.Value.ToLower(), tag);
            }

            List<Page> urls;
            if (!Tags.TryGetValue(distinctTag, out urls))
                Tags.Add(tag, urls = new List<Page>());
            urls.AddRange(url);
        }

        public void Add(TagCollection tagsForPage)
        {
            foreach (var kv in tagsForPage.Tags)
                Add(kv.Key, kv.Value.ToArray());
        }

        public IEnumerator<KeyValuePair<Tag, List<Page>>> GetEnumerator()
        {
            return Tags.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
