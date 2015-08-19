using System;
using System.IO;
using System.Text;

using MarkdownDeep;

namespace GitHubServices.BusinessLogic.TagPageCreator
{

    public interface IFilesystemRepository
    {
        void EmptyTagDirectory(string tagDir);

        void WriteFile(string filepath, string content);

        string ReadFile(string filepath);

        void Copy(string sourcepath, string destinationPath);
    }

    public class FilesystemRepository : IFilesystemRepository
    {
        public void EmptyTagDirectory(string tagDir)
        {
            if (Directory.Exists(tagDir))
            {
                var directory = new DirectoryInfo(tagDir);
                foreach (FileInfo file in directory.GetFiles())
                    file.Delete();
            }

            if (!Directory.Exists(tagDir))
                Directory.CreateDirectory(tagDir);
        }

        public void WriteFile(string filepath, string content)
        {
            bool write = false;
            if (!File.Exists(filepath))
                write = true;
            else
            {
                if (File.ReadAllText(filepath) != content)
                    write = true;
            }

            if (write)
            {
                Console.WriteLine("Writing " + filepath);
                File.WriteAllText(filepath, content, new UTF8Encoding(true));
            }
        }

        public string ReadFile(string filepath)
        {
            return File.ReadAllText(filepath, new UTF8Encoding());
        }

        public void Copy(string sourcepath, string destinationPath)
        {
            File.Copy(sourcepath, destinationPath);
        }
    }


    public class HtmlWriter : IFilesystemRepository
    {
        readonly HtmlTransformer transformer = new HtmlTransformer();

        public void EmptyTagDirectory(string tagDir)
        {
            if (Directory.Exists(tagDir))
            {
                var directory = new DirectoryInfo(tagDir);
                foreach (FileInfo file in directory.GetFiles())
                    file.Delete();
            }

            if (!Directory.Exists(tagDir))
                Directory.CreateDirectory(tagDir);
        }

        public void WriteFile(string filepath, string content)
        {
            var path = Path.GetDirectoryName(filepath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var destination = Path.Combine(path , Path.GetFileNameWithoutExtension(filepath) )+ ".html";
            var html = transformer.MarkdownToHtml(content);
            File.WriteAllText(destination, html, new UTF8Encoding(true));
        }

        public string ReadFile(string filepath)
        {
            return File.ReadAllText(filepath, new UTF8Encoding());
        }

        public void Copy(string sourcepath, string destinationPath)
        {
            var path = Path.GetDirectoryName(destinationPath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            File.Copy(sourcepath, destinationPath, true);
        }


        class HtmlTransformer
        {
            Markdown md = new MarkdownDeep.Markdown()
                              {
                                  ExtraMode = true,
                                  SafeMode = false,
                                  AutoHeadingIDs = true,
                                  FormatCodeBlock =
                                      (m, code) => string.Format(
                                          "<pre class=\"prettyprint\"><code>{0}</code></pre>\n",
                                          code),
                              };

            public string MarkdownToHtml(string markdownContent)
            {
                string footer = @"<br>
<br>
Read the [Introduction](http://kbilsted.github.io/CodeQualityAndReadability/) or browse the rest [of the site](http://kbilsted.github.io/CodeQualityAndReadability/AllArticles.html)
<br>
";
                string html = md.Transform(markdownContent + footer);

                string htmlWithCss = string.Format(@"
<html>
<script src=""https://cdn.rawgit.com/google/code-prettify/master/loader/run_prettify.js""></script>
<link rel=""stylesheet"" href=""http://kbilsted.github.io/CodeQualityAndReadability/github-markdown.css"">
<style>
      .markdown-body {{
                min-width: 200px;
                max-width: 790px;
                margin: 0 auto;
                padding: 30px;
            }}
</style>
<article class=""markdown-body"">

{0}


</article>
</html>", html);

                return htmlWithCss;
            }
        }
    }
}