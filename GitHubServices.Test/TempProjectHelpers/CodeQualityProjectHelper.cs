using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using GitHubServices.BusinessLogic.TagPageCreator;
using GitHubServices.BusinessLogic.TagPageCreator.Domain;
using GitHubServices.Models;

using NUnit.Framework;

namespace GitHubServices.Test.BusinessLogic
{
    /// <summary>
    /// run these to update the TOC's of the code quality project
    /// </summary>
    [TestFixture]
    public class CodeQualityProjectHelper
    {
        const string readPath = @"C:\src\CodeQualityAndReadability\";
        const string writePath = readPath+"docs";

        readonly string baseUrl = @"http://firstclassthoughts.co.uk/";

        readonly string editBaseUrl = "https://github.com/kbilsted/CodeQualityAndReadability/blob/master/";

        [Test]
        public void DeleteThisWhenTheServiceIsRunning_makeToc()
        {
            DirectoryInfo di = new DirectoryInfo(readPath);
            foreach (var path in di.EnumerateFiles("*.md", SearchOption.AllDirectories))
            {
                if (path.FullName.Contains(Path.DirectorySeparatorChar + "Tags" + Path.DirectorySeparatorChar))
                    continue;
                if(path.FullName.EndsWith("Readme.md"))
                    continue;
                Console.Write(path.FullName);
                MutateTocSection(path.FullName);
            }
        }


        [Test]
        public void DeleteThisWhenTheServiceIsRunning_generatePages()
        {
            var contentGenerator = new ContentGenerator();

            var filesystemRepository = new HtmlWriter(baseUrl, contentGenerator);

            var documentParser = new DocumentParser();

            var siteGenerator = new SiteGenerator(
                contentGenerator,
                filesystemRepository,
                documentParser,
                new MarkDownMutator(filesystemRepository, contentGenerator));
            
            siteGenerator.GenerateSite(new ReadWritePaths(readPath, writePath), baseUrl, editBaseUrl);
        }

//        [Explicit]
//        [Test]
//        public void TagCollector_GetTags()
//        {
//            DocumentParser co = new DocumentParser(new FilesystemRepository());
//            var tags = co.GetTags(readPath);
//            string exp = @"new TagsCollection()
//{
//    Pages[""Design""] = new List<Page>()
//    Pages[0] = new Page(), ref: 0
//    {
//        Name = ""Malleable code through using decorators
//""
//        FilePath = ""Articles\Design\MalleableCodeUsingDecorators.md""
//    }
//    Pages[""SOLID""] = new List<Page>()
//    Pages[0] =  -> 0
//    Pages[1] = new Page(), ref: 1
//    {
//        Name = ""Book review: Adaptive Code via C#
//""
//        FilePath = ""BookReviews\Adaptive Code via CS.md""
//    }
//    Pages[""Single_Responsibility_Principle""] = new List<Page>()
//    Pages[0] =  -> 0
//    Pages[""Design_Pattern""] = new List<Page>()
//    Pages[0] =  -> 0
//    Pages[""Decorator""] = new List<Page>()
//    Pages[0] =  -> 0
//    Pages[""Wrapper""] = new List<Page>()
//    Pages[0] =  -> 0
//    Pages[""Cache""] = new List<Page>()
//    Pages[0] =  -> 0
//    Pages[""Book_Review""] = new List<Page>()
//    Pages[0] =  -> 1
//}";

//            var assert = Create.Assert();
//            assert.Configuration.Test.SetAutomaticTestRewrite(x => false);
//            assert.PrintEquals(exp, tags);
//        }

        void MutateTocSection(string path)
        {
            var content = File.ReadAllText(path);

            var toc = new TocParser().MakeToc(content);
            var newContent = new ContentReplacer().TryReplaceToc(content, toc);
            if (newContent != null && content != newContent)
            {
                Console.Write("    ... Updating");
                File.WriteAllText(path, newContent, Encoding.UTF8);
            }

            Console.WriteLine("");
        }
    }
}
