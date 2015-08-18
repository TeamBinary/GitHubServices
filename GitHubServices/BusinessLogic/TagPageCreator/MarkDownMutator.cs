using System;
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
        readonly ContentGenerator contentGenerator ;
        readonly DocumentParser documentParser ;

        static readonly RegexOptions Options = RegexOptions.Compiled | RegexOptions.Singleline;

        static readonly Regex SocialButtonShareEx = new Regex(
            @"<SocialShareButtons>[^<]+</SocialShareButtons>",
            Options);

        static readonly Regex CommentTextEx = new Regex(
            @"<CommentText>[^<]+</CommentText>",
            Options);


        public MarkDownMutator(IFilesystemRepository filesystemRepository, ContentGenerator contentGenerator, DocumentParser documentParser)
        {
            this.filesystemRepository = filesystemRepository;
            this.contentGenerator = contentGenerator;
            this.documentParser = documentParser;
        }

        public void Mutate(string rootFilePath)
        {
            var di = new DirectoryInfo(rootFilePath);
            foreach (var path in di.EnumerateFiles("*.md", SearchOption.AllDirectories))
            {
                var fileContent = filesystemRepository.ReadFile(path.FullName);

                if (fileContent.StartsWith("draft"))
                    continue;

                var relativePath = path.FullName.Substring(rootFilePath.Length).Replace('\\', '/');

                fileContent = MutateSocialLinks(fileContent, relativePath);
                fileContent = MutateCommentText(fileContent);
                fileContent = MutateCategoryTags(fileContent);
                fileContent = MutateFileFooter(fileContent, relativePath);

                filesystemRepository.WriteFile(path.FullName, fileContent);
            }
        }

        string MutateFileFooter(string fileContent, string relativePath)
        {
            string path = relativePath.Replace(" ", "_").Replace("/", "_");

            string footer = string.Format(@"

Read the [Introduction](https://github.com/kbilsted/CodeQualityAndReadability/blob/master/README.md) or browse the rest [of the site](https://github.com/kbilsted/CodeQualityAndReadability/blob/master/AllArticles.md)
<br>
[![Analytics](https://ga-beacon.appspot.com/UA-65034248-2/QualityAndReadability/{0})](https://github.com/igrigorik/ga-beacon)
", path);
            if (!fileContent.Contains(footer))
                fileContent += footer;

            return fileContent;
        }

        string MutateCommentText(string fileContent)
        {

            var content = CommentTextEx.Replace(
                fileContent,
                x =>
                string.Format(@"<{0}>
**Comments, corrections and other editorial changes are very welcome. Just log onto Github, press the edit button and fire away. Have I left out important information about your favourite language, press the edit button. Are there wordings that definitely are not English, press the edit button. Do you have something to elaborate.. press the edit button!! :-)**

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

                        return string.Format(@"<{0} Tags=""{1}"">
{2}</{0}>",
                            "Categories",
                            x.Groups["tags"].Value, sb.ToString());
                    });

            return content;
        }

        string MutateSocialLinks(string fileContent, string relativePath)
        {
            var url = "https://github.com/kbilsted/CodeQualityAndReadability/blob/master/" + relativePath;

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
}