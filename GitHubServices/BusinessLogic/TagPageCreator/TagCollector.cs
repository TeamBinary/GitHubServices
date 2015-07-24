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

        readonly MarkDownMutator mutator;

        public SiteGenerator(ContentGenerator generator, IFilesystemRepository filesystemRepository, TagCollector collector, MarkDownMutator mutator)
        {
            this.generator = generator;
            this.filesystemRepository = filesystemRepository;
            this.collector = collector;
            this.mutator = mutator;
        }

        public void GenerateSite(string rootPath)
        {
            var tags = collector.GetTags(rootPath);

            mutator.Mutate(rootPath);
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
                .GroupBy(x => x.Path);

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

            sb.AppendFormat("## All categories on the site");
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
                    sb.AppendFormat("* [{0}](Tags/{1}.md)", tag.Value.Replace("_", " "), tag.Value);
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
            sb.AppendLine("<br>[[Introduction]](https://github.com/kbilsted/CodeQualityAndReadability) [[All categories]](https://github.com/kbilsted/CodeQualityAndReadability/blob/master/AllTags.md) [[All articles]](https://github.com/kbilsted/CodeQualityAndReadability/blob/master/AllArticles.md)");

            sb.AppendLine();
        }

        void AddFooter(StringBuilder sb)
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


        static readonly Regex headerEx = new Regex("^# (?<title>.*)$", RegexOptions.Compiled | RegexOptions.Multiline);

        static readonly Regex tagEx = new Regex(@"\[!\[Stats\]\(https://img.shields.io/badge/Tag-(?<tagname>.*)-([0-9a-fA-F]){6}\.svg\)\]", RegexOptions.Compiled);


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

    public class MarkDownMutator
    {
        readonly IFilesystemRepository filesystemRepository;

        static readonly Regex SocialButtonShareEx =
      new Regex(@"<SocialShareButtons>[^<]+</SocialShareButtons>",
          RegexOptions.Compiled | RegexOptions.Singleline);

        static readonly Regex CategoryButtonsEx =
      new Regex(@"<Categories Tags=""(?<tags>[^""]*)"">[^<]+</Categories>",
          RegexOptions.Compiled | RegexOptions.Singleline);

        static readonly Regex CommentTextEx =
new Regex(@"<CommentText>[^<]+</CommentText>",
    RegexOptions.Compiled | RegexOptions.Singleline);


        public MarkDownMutator(IFilesystemRepository filesystemRepository)
        {
            this.filesystemRepository = filesystemRepository;
        }

        public void Mutate(string rootFilePath)
        {
            var di = new DirectoryInfo(rootFilePath);
            foreach (var path in di.EnumerateFiles("*.md", SearchOption.AllDirectories))
            {
                var fileContent = filesystemRepository.ReadFile(path.FullName);

                fileContent = MutateSocialLinks(rootFilePath, fileContent, path);
                fileContent = MutateCommentText(fileContent);
                fileContent = MutateCategoryTags(fileContent);
                filesystemRepository.WriteFile(path.FullName, fileContent);

            }
        }

        string MutateCommentText(string fileContent)
        {
            var content = CommentTextEx.Replace(
            fileContent,
            x =>
            string.Format(@"<{0}>
**Comments, corrections and other editorial changes are very welcome. Just log onto Github, press the edit button and fire away. Have I left out important information about your favorite langue, press the edit button. Are there wordings that definitely are not english, press the edit button. Do you have something to elaborate.. press the edit button!! :-)**

*Comments should go below this line (and use the following template).*

Name: Bubba Jones
> text..  
> text..  

</{0}>",
"CommentText"));


            return content;
        }


        string MutateCategoryTags(string fileContent)
        {

            var content = CategoryButtonsEx.Replace(
               fileContent,
               x =>
               {
                   var tagsArgument = x.Groups["tags"].Value;
                   Tag[] parsedTags = tagsArgument
                       .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                       .Select(y => new Tag(y.Trim()))
                       .ToArray();
                   var sb = new StringBuilder();
                   foreach (var tag in parsedTags)
                       sb.Append(MakeATag(tag));

                   return string.Format(@"<{0} Tags=""{1}"">
{2}</{0}>",
    "Categories",
    tagsArgument, sb.ToString());
               });

            return content;
        }

        string MakeATag(Tag tag)
        {
            return string.Format("[![Stats](https://img.shields.io/badge/Tag-{0}-99CC00.svg)](https://github.com/kbilsted/CodeQualityAndReadability/blob/master/Tags/{0}.md)\r\n", tag.Value);
        }

        string MutateSocialLinks(string rootFilePath, string fileContent, FileInfo path)
        {
            var url = "https://github.com/kbilsted/CodeQualityAndReadability/blob/master/" + path.FullName.Substring(rootFilePath.Length).Replace('\\', '/');

            string title = new string(fileContent.TakeWhile(x => x != '\n').ToArray()).Substring(1).Trim();
            title = title.Replace(" ", "%20");

            var content = SocialButtonShareEx.Replace(
                fileContent,
                x =>
                string.Format(@"<{0}>
[![Reddit this]({1}reddit.png)](https://www.reddit.com/submit?url={2}&title={3})
[![Tweet this]({1}twitter.png)](https://twitter.com/intent/tweet?url={2}&text={3}&via=kbilsted)
[![Googleplus this]({1}gplus.png)](https://plus.google.com/share?url={2})
[![Facebook this]({1}facebook.png)](https://facebook.com/sharer.php?u={2}&t={3})
[![LinkedIn this]({1}linkedin.png)](http://www.linkedin.com/shareArticle?mini=true&url={2})
[![Feedly this]({1}feedly.png)](http://cloud.feedly.com/#subscription%2Ffeed%2F{2})
[![Ycombinator this]({1}ycombinator.png)](http://news.ycombinator.com/submitlink?u={2}&t={3})
</{0}>",
  "SocialShareButtons",
  "https://github.com/kbilsted/CodeQualityAndReadability/blob/master/img/", url, title));


            return content;
        }
    }

    public class TagCollection : IEnumerable<KeyValuePair<Tag, List<Page>>>
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
