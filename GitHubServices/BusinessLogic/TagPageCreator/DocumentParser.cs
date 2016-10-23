using System;
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

                var tagsForPage = ParsePage(fileContent, relativePath);
                tags.Add(tagsForPage);
            }

            return tags;
        }


        static readonly RegexOptions Options = RegexOptions.Compiled | RegexOptions.Singleline;

        static readonly Regex headerEx = new Regex("^# (?<title>.*)$", RegexOptions.Compiled | RegexOptions.Multiline);

        public static readonly Regex CategoryEx = new Regex(@"<Categories Tags=""(?<tags>[^""]*)"">[^<]+</Categories>", Options);

        public string ParsePageTitle(string pageContent)
        {
            Match headerMatch = headerEx.Match(pageContent);
            var title = headerMatch.Groups["title"].Value.Trim();

            return title;
        }

        TagCollection ParsePage(string pageContent, string fullName)
        {
            var title = ParsePageTitle(pageContent);

            TagCollection tags = new TagCollection();
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
}
