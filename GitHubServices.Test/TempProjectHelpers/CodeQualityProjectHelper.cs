using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GitHubServices.BusinessLogic.TagPageCreator;
using GitHubServices.Models;

using NUnit.Framework;

using StatePrinter.TestAssistance;

namespace GitHubServices.Test.BusinessLogic
{
    /// <summary>
    /// run these to update the TOC's of the code quality project
    /// </summary>
    [Explicit]
    [TestFixture]
    class CodeQualityProjectHelper
    {
        readonly string basepath = @"C:\Users\kbg\Documents\GitHub\CodeQualityAndReadability\";

        [Test]
        public void DeleteThisWhenTheServiceIsRunning_makeToc()
        {
            DirectoryInfo di = new DirectoryInfo(basepath);
            foreach (var path in di.EnumerateFiles("*.md", SearchOption.AllDirectories))
            {
                if(path.FullName.EndsWith("Readme.md"))
                    continue;
                Console.Write(path.FullName);
                Handle(path.FullName);
            }
        }


        [Test]
        public void DeleteThisWhenTheServiceIsRunning_generatePages()
        {
            var gen = new PageGenerator(new ContentGenerator(), new FilesystemRepository(), new TagCollector());
            gen.GeneratePages(basepath);
        }

        [Test]
        public void MakeTags()
        {
            TagCollector co = new TagCollector();
            var tags = co.GetTags(basepath);
            string exp = @"new TagsCollection()
{
    Pages[""Design""] = new List<Page>()
    Pages[0] = new Page(), ref: 0
    {
        Name = ""Malleable code through using decorators
""
        FilePath = ""Articles\Design\MalleableCodeUsingDecorators.md""
    }
    Pages[""SOLID""] = new List<Page>()
    Pages[0] =  -> 0
    Pages[1] = new Page(), ref: 1
    {
        Name = ""Book review: Adaptive Code via C#
""
        FilePath = ""BookReviews\Adaptive Code via CS.md""
    }
    Pages[""Single_Responsibility_Principle""] = new List<Page>()
    Pages[0] =  -> 0
    Pages[""Design_Pattern""] = new List<Page>()
    Pages[0] =  -> 0
    Pages[""Decorator""] = new List<Page>()
    Pages[0] =  -> 0
    Pages[""Wrapper""] = new List<Page>()
    Pages[0] =  -> 0
    Pages[""Cache""] = new List<Page>()
    Pages[0] =  -> 0
    Pages[""Book_Review""] = new List<Page>()
    Pages[0] =  -> 1
}";

            var assert = Create.Assert();
            assert.Configuration.Test.SetAutomaticTestRewrite(x => false);
            assert.PrintEquals(exp, tags);
        }

        void Handle(string path)
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
