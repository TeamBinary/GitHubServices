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

        public void GenerateSite(string rootPath)
        {
            var tags = parser.GetTags(rootPath);

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
}