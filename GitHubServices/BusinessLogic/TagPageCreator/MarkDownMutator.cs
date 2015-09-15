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
            "<SocialShareButtons>[^<]+</SocialShareButtons>",
            Options);

        static readonly Regex CommentTextEx = new Regex(
            "<CommentText>[^<]+</CommentText>",
            Options);

        static readonly Regex AllTagsEx = new Regex(@"<AllTags\s* />", Options);

        static readonly Regex BaseUrlTagEx = new Regex("<BaseUrl/>", Options);


        public MarkDownMutator(IFilesystemRepository filesystemRepository, ContentGenerator contentGenerator, DocumentParser documentParser)
        {
            this.filesystemRepository = filesystemRepository;
            this.contentGenerator = contentGenerator;
            this.documentParser = documentParser;
        }

        public void Mutate(ReadWritePaths rootFilePath, TagCollection tags, string baseUrl, string editBaseUrl)
        {
            var di = new DirectoryInfo(rootFilePath.ReadPath);
            foreach (var path in di.EnumerateFiles("*.md", SearchOption.AllDirectories))
            {
                var fileContent = filesystemRepository.ReadFile(path.FullName);

                if (fileContent.StartsWith("draft"))
                    continue;

                var relativePath = path.FullName.Substring(rootFilePath.ReadPath.Length).Replace('\\', '/');

                fileContent = MutateSocialLinks(fileContent, baseUrl, relativePath);
                fileContent = MutateCommentText(fileContent, relativePath, editBaseUrl);
                fileContent = MutateCategoryTags(fileContent, baseUrl);
                fileContent = MutateAllTagsLine(fileContent, tags, baseUrl);
                fileContent = MutateBaseUrlTag(fileContent, baseUrl);
                var articleCount = tags.SelectMany(x => x.Value).Distinct().Count();
                fileContent = MutateTagArticleCount(fileContent, articleCount);
                var title = documentParser.ParsePageTitle(fileContent);
                filesystemRepository.WriteFile(Path.Combine(rootFilePath.WritePath, relativePath), fileContent, title);
            }
        }

        string MutateAllTagsLine(string fileContent, TagCollection tags, string baseUrl)
        {
            var content = AllTagsEx.Replace(
                fileContent,
                z => string.Join(" ", tags
                    .Select(x => x.Key)
                    .OrderBy(x=>x.Value)
                    .Select(x => contentGenerator.CreateCategoryLink(x, baseUrl))));
            return content;
        }

        string MutateCommentText(string fileContent, string relativeReadPath, string editBaseUrl)
        {
            string url = editBaseUrl + relativeReadPath;
            var textBody = string.Format(@"**Comments are very welcome! Corrections and other editorial changes are very welcome. <a href=""{0}"">Just go to Github, press the edit button and fire away.</a> 
Have I left out important information about your favourite language, press the edit button. Are there wordings that definitely are not English, press the edit button. 
Do you have something to elaborate.. press the edit button!! :-)**

<br>", url);

            var disqusStuff = @"<div id=""disqus_thread""></div>
<script type=""text/javascript"">
    /* * * CONFIGURATION VARIABLES * * */
    var disqus_shortname = 'qualityandreadability';
    
    /* * * DON'T EDIT BELOW THIS LINE * * */
    (function() {
        var dsq = document.createElement('script'); dsq.type = 'text/javascript'; dsq.async = true;
        dsq.src = '//' + disqus_shortname + '.disqus.com/embed.js';
        (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);
    })();
</script>
<noscript>Please enable JavaScript to view the <a href=""https://disqus.com/?ref_noscript"" rel=""nofollow"">comments powered by Disqus.</a></noscript>";

            var content = CommentTextEx.Replace(
                fileContent,
                x =>
                textBody + disqusStuff
                );

            return content;
        }


        string MutateBaseUrlTag(string fileContent, string baseUrl)
        {
            var content = BaseUrlTagEx.Replace(fileContent, x => baseUrl);
            return content;
        }



        string MutateCategoryTags(string fileContent, string baseUrl)
        {

            var content = DocumentParser.CategoryEx.Replace(
                fileContent,
                x =>
                    {
                        Tag[] parsedTags = documentParser.GetTheTags(x.Groups["tags"].Value);
                        var sb = new StringBuilder();
                        foreach (var tag in parsedTags)
                        {
                            sb.Append(contentGenerator.CreateCategoryLink(tag, baseUrl));
                            sb.Append(Environment.NewLine);
                        }

                        return sb.ToString();
                    });

            return content;
        }


        string MutateSocialLinks(string fileContent, string baseUrl, string relativePath)
        {
            var url = string.Format("{0}{1}.html", baseUrl, relativePath.Substring(0, relativePath.Length-3));

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
", baseUrl+"img/", url, title));


            return content;
        }

        public static readonly Regex ArticleCountEx = new Regex(@"<ArticleCount/>", Options);

        string MutateTagArticleCount(string fileContent, int numberOfPages)
        {
            return ArticleCountEx.Replace(fileContent, numberOfPages.ToString());
        }
    }
}