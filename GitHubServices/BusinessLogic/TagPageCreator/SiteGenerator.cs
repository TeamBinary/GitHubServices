using System.IO;
using System.Linq;

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

        public void GenerateSite(ReadWritePaths rootPath)
        {
            var tags = parser.GetTags(rootPath.ReadPath);

            CopyImmutableFiles(rootPath);
            mutator.Mutate(rootPath, tags);
            AllArticlesPage(rootPath.WritePath, tags);
            TagPages(rootPath.WritePath, tags);
            AllTagsPage(rootPath.WritePath, tags);
        }

        void CopyImmutableFiles(ReadWritePaths rootPath)
        {
            var di = new DirectoryInfo(rootPath.ReadPath);
            foreach (var path in di.EnumerateFiles("*.*", SearchOption.AllDirectories))
            {
                if (IsToCopy(path))
                {
                    var relativePath = path.FullName.Substring(rootPath.ReadPath.Length);
                    filesystemRepository.Copy(path.FullName, Path.Combine(rootPath.WritePath, relativePath));
                }
            }

        }

        bool IsToCopy(FileInfo path)
        {
            string ext = path.Extension.ToLower();
            var isImage = ext == ".png" || ext == ".jpeg" || ext == ".gif" || ext == ".jpg";
            var isRelevant= ext == ".css" ;
            return isImage || isRelevant;
        }

        void AllTagsPage(string writePath, TagCollection tags)
        {
            var allTags = generator.GenerateAllTagsPage(tags.Select(x => x.Key).ToList());
            filesystemRepository.WriteFile(Path.Combine(writePath, "AllTags.md"), allTags);
        }

        void TagPages(string writePath, TagCollection tags)
        {
            var tagDir = Path.Combine(writePath, "Tags");
            filesystemRepository.EmptyTagDirectory(tagDir);
            
            foreach (var tag in tags)
            {
                var tagPage = generator.GenerateTagPage(tag.Key, tag.Value);
                filesystemRepository.WriteFile(Path.Combine(tagDir, tag.Key + ".html"), tagPage);
            }
        }

        void AllArticlesPage(string writePath, TagCollection tags)
        {
            var allArticles = generator.GenerateAllArticlesPage(tags.SelectMany(x => x.Value).ToList());
            filesystemRepository.WriteFile(Path.Combine(writePath, "AllArticles.md"), allArticles);
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