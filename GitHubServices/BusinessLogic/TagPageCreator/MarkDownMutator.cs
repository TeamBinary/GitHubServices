using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using GitHubServices.BusinessLogic.TagPageCreator.Domain;

namespace GitHubServices.BusinessLogic.TagPageCreator
{
    public class MarkDownMutator
    {
        readonly IFilesystemRepository filesystemRepository;
        readonly ContentGenerator contentGenerator;
        readonly DocumentParser documentParser;

        static readonly RegexOptions Options = RegexOptions.Compiled | RegexOptions.Singleline;

        static readonly Regex SocialButtonShareEx = new Regex(
            @"<SocialShareButtons>[^<]+</SocialShareButtons>",
            Options);

        static readonly Regex CommentTextEx = new Regex(
            @"<CommentText>[^<]+</CommentText>",
            Options);

        static readonly Regex AllTagsEx = new Regex(@"<AllTags\s* />", Options);


        public MarkDownMutator(IFilesystemRepository filesystemRepository, ContentGenerator contentGenerator, DocumentParser documentParser)
        {
            this.filesystemRepository = filesystemRepository;
            this.contentGenerator = contentGenerator;
            this.documentParser = documentParser;
        }

        public void Mutate(ReadWritePaths rootFilePath, TagCollection tags)
        {
            var di = new DirectoryInfo(rootFilePath.ReadPath);
            foreach (var path in di.EnumerateFiles("*.md", SearchOption.AllDirectories))
            {
                var fileContent = filesystemRepository.ReadFile(path.FullName);

                if (fileContent.StartsWith("draft"))
                    continue;

                var relativePath = path.FullName.Substring(rootFilePath.ReadPath.Length).Replace('\\', '/');

                fileContent = MutateSocialLinks(fileContent, relativePath);
                fileContent = MutateCommentText(fileContent);
                fileContent = MutateCategoryTags(fileContent);
                fileContent = MutateAllTagsLine(fileContent, tags);
                //fileContent = MutateFileFooter(fileContent, relativePath);

                filesystemRepository.WriteFile(Path.Combine(rootFilePath.WritePath, relativePath), fileContent);
            }
        }

        string MutateAllTagsLine(string fileContent, TagCollection tags)
        {
            var content = AllTagsEx.Replace(
                fileContent,
                z => string.Join(" ", tags
                    .Select(x => x.Key)
                    .OrderBy(x=>x.Value)
                    .Select(x => contentGenerator.CreateCategoryLink(x))));
            return content;
        }

        string MutateCommentText(string fileContent)
        {

            var content = CommentTextEx.Replace(
                fileContent,
                x =>
                @"**Corrections and other editorial changes are very welcome. Just log onto Github, press the edit button and fire away. Have I left out important information about your favourite language, press the edit button. Are there wordings that definitely are not English, press the edit button. Do you have something to elaborate.. press the edit button!! :-)**

");

            return content;
        }





        string MutateCategoryTags(string fileContent)
        {

            var content = DocumentParser.CategoryEx.Replace(
                fileContent,
                x =>
                    {
                        Tag[] parsedTags = documentParser.GetTheTags(x.Groups["tags"].Value);
                        var sb = new StringBuilder();
                        foreach (var tag in parsedTags)
                        {
                            sb.Append(contentGenerator.CreateCategoryLink(tag));
                            sb.Append(Environment.NewLine);
                        }

                        return string.Format(@"{0}",sb.ToString());
                    });

            return content;
        }

        string MutateSocialLinks(string fileContent, string relativePath)
        {
            var url = string.Format("http://kbilsted.github.io/CodeQualityAndReadability/{0}.html", relativePath.Substring(0, relativePath.Length-3));

            string title = new string(fileContent.TakeWhile(x => x != '\n').ToArray()).Substring(1).Trim();
            title = title.Replace(" ", "%20");

            var content = SocialButtonShareEx.Replace(
                fileContent,
                x =>
                string.Format(@"
[![Reddit this]({0}reddit.png)](https://www.reddit.com/submit?url={1}&title={2})
[![Tweet this]({0}twitter.png)](https://twitter.com/intent/tweet?url={1}&text={2}&via=kbilsted)
[![Googleplus this]({0}gplus.png)](https://plus.google.com/share?url={1})
[![Facebook this]({0}facebook.png)](https://facebook.com/sharer.php?u={1}&t={2})
[![LinkedIn this]({0}linkedin.png)](http://www.linkedin.com/shareArticle?mini=true&url={1})
[![Feedly this]({0}feedly.png)](http://cloud.feedly.com/#subscription%2Ffeed%2F{1})
[![Ycombinator this]({0}ycombinator.png)](http://news.ycombinator.com/submitlink?u={1}&t={2})
",
                    "http://kbilsted.github.io/CodeQualityAndReadability/img/", url, title));


            return content;
        }
    }
}