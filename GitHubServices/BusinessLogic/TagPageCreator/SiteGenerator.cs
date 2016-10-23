using System.Collections.Generic;
using System.IO;
using System.Linq;

using GitHubServices.BusinessLogic.TagPageCreator.Domain;

namespace GitHubServices.BusinessLogic.TagPageCreator
{
    public class SiteGenerator
    {
        readonly ContentGenerator generator;
        readonly IFilesystemRepository filesystemRepository;
        readonly DocumentParser parser;
        readonly MarkDownMutator mutator;

        public SiteGenerator(ContentGenerator generator, IFilesystemRepository filesystemRepository, DocumentParser parser, MarkDownMutator mutator)
        {
            this.generator = generator;
            this.filesystemRepository = filesystemRepository;
            this.parser = parser;
            this.mutator = mutator;
        }

        public void GenerateSite(ReadWritePaths rootPath, string baseUrl, string editBaseUrl)
        {
            var tags = parser.GetTags(rootPath.ReadPath);

            CopyImmutableFiles(rootPath);
            mutator.Mutate(rootPath, tags, baseUrl, editBaseUrl);
            AllArticlesPage(rootPath.WritePath, tags, baseUrl);
            TagPages(rootPath.WritePath, tags, baseUrl);
            AllTagsPage(rootPath.WritePath, tags, baseUrl);
        }

        void CopyImmutableFiles(ReadWritePaths rootPath)
        {
            var di = new DirectoryInfo(rootPath.ReadPath);
            foreach (var path in di.EnumerateFiles("*.*", SearchOption.AllDirectories))
            {
                if (IsToCopy(path) && !path.FullName.StartsWith(rootPath.WritePath))
                {
                    var relativePath = path.FullName.Substring(rootPath.ReadPath.Length);
                    filesystemRepository.Copy(path.FullName, Path.Combine(rootPath.WritePath, relativePath));
                }
            }

        }

        bool IsToCopy(FileInfo path)
        {
            string ext = path.Extension.ToLower();
            var isImage = ext == ".png" || ext == ".jpeg" || ext == ".gif" || ext == ".jpg" || ext == ".ico";
            var isVideo = ext == ".mp4";
            var html = ext == ".css" || ext == ".js";
            var isRelevant = html || path.FullName.EndsWith("CNAME");

            return isImage || isVideo|| isRelevant;
        }


        void AllTagsPage(string writePath, TagCollection tags, string baseUrl)
        {
            var allTags = generator.GenerateAllTagsPage(tags.Select(x => x.Key).ToList(), baseUrl);
            filesystemRepository.WriteFile(Path.Combine(writePath, "AllTags.md"), allTags, "All categories on Quality and Readability");
        }

        void TagPages(string writePath, TagCollection tags, string baseUrl)
        {
            var tagDir = Path.Combine(writePath, "Tags");
            filesystemRepository.EmptyTagDirectory(tagDir);
            
            foreach (KeyValuePair<Tag, List<Page>> tag in tags)
            {
                var tagPage = generator.GenerateTagPage(tag.Key, tag.Value, baseUrl);
                filesystemRepository.WriteFile(Path.Combine(tagDir, tag.Key + ".html"), tagPage, "Pages related to "+ tag.Key.Value);
            }
        }

        void AllArticlesPage(string writePath, TagCollection tags, string baseUrl)
        {
            var allArticles = generator.GenerateAllArticlesPage(tags.SelectMany(x => x.Value).ToList(), baseUrl);
            filesystemRepository.WriteFile(Path.Combine(writePath, "AllArticles.md"), allArticles, "All articles on Quality and Readability");
        }
    }


    public class ReadWritePaths
    {
        public readonly string ReadPath, WritePath;

        public ReadWritePaths(string readPath, string writePath)
        {
            ReadPath = readPath;
            WritePath = writePath;
        }
    }

}