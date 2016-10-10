using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GitHubServices.Models;

using NUnit.Framework;

namespace GitHubServices.Test.BusinessLogic
{
    /// <summary>
    /// run these to update the TOC's of the stateprinter project
    /// </summary>
    //[Explicit]
    [TestFixture]
    public class StatePrinterProjectHelper
    {
        [Test]
        public void DeleteThisWhenTheServiceIsRunning()
        {
            DirectoryInfo di = new DirectoryInfo(@"C:\Users\kbg\Documents\GitHub\StatePrinter\doc\");
            foreach (var path in di.EnumerateFiles("*.md"))
            {
                Console.Write(path.FullName);
                Handle(path.FullName);
            }
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
