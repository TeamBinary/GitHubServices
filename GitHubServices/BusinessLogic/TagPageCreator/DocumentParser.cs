using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using GitHubServices.BusinessLogic.TagPageCreator.Domain;

namespace GitHubServices.BusinessLogic.TagPageCreator
{


    public class DocumentParser
    {
        readonly IFilesystemRepository filesystemRepository;

        public DocumentParser(IFilesystemRepository filesystemRepository)
        {
            this.filesystemRepository = filesystemRepository;
        }
        
        public TagCollection GetTags(string rootFilePath)
        {
            var tags = new TagCollection();
            var di = new DirectoryInfo(rootFilePath);
            foreach (var path in di.EnumerateFiles("*.md", SearchOption.AllDirectories))
            {
                var relativePath = path.FullName.Substring(rootFilePath.Length);
                var fileContent = filesystemRepository.ReadFile(path.FullName);
                
                if(fileContent.StartsWith("draft"))
                    continue;

                var tagsForPage = parsePage(fileContent, relativePath);
                tags.Add(tagsForPage);
            }

            return tags;
        }


        static readonly RegexOptions Options = RegexOptions.Compiled | RegexOptions.Singleline;

        static readonly Regex headerEx = new Regex("^# (?<title>.*)$", RegexOptions.Compiled | RegexOptions.Multiline);

        public static readonly Regex CategoryEx = new Regex(
    @"<Categories Tags=""(?<tags>[^""]*)"">[^<]+</Categories>",
    Options);

        TagCollection parsePage(string pageContent, string fullName)
        {
            TagCollection tags = new TagCollection();
            Match headerMatch = headerEx.Match(pageContent);
            var title = headerMatch.Groups["title"].Value.Trim();

            foreach (Tag tag in GetTagsFromContent(pageContent))
                tags.Add(tag, new Page(title, fullName));

            return tags;
        }

        Tag[] GetTagsFromContent(string content)
        {
            return GetTheTags(CategoryEx.Match(content).Groups["tags"].Value);
        }

        public Tag[] GetTheTags(string tagsArgument)
        {
            Tag[] parsedTags = tagsArgument
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(y => new Tag(y.Trim()))
                .ToArray();
            return parsedTags;
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
