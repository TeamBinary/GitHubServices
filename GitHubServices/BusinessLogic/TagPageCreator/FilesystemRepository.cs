using System;
using System.IO;
using System.Text;

using MarkdownDeep;

namespace GitHubServices.BusinessLogic.TagPageCreator
{

    public interface IFilesystemRepository
    {
        void EmptyTagDirectory(string tagDir);

        void WriteFile(string filepath, string content, string pageTitle);

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

        public void WriteFile(string filepath, string content, string pageTitle="")
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

        readonly string baseUrl;

        public HtmlWriter(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

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

        public void WriteFile(string filepath, string content, string pageTitle)
        {
            var path = Path.GetDirectoryName(filepath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var destination = Path.Combine(path , Path.GetFileNameWithoutExtension(filepath) )+ ".html";
            var html = transformer.MarkdownToHtml(content, pageTitle, baseUrl);
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
            readonly Markdown md = new Markdown()
                              {
                                  ExtraMode = true,
                                  SafeMode = false,
                                  AutoHeadingIDs = true,
                                  FormatCodeBlock =
                                      (m, code) => string.Format(
                                          "<pre class=\"prettyprint\"><code>{0}</code></pre>\n",
                                          code),

                              };

            public string MarkdownToHtml(string markdownContent, string pageTitle, string baseUrl)
            {
                string footer = String.Format(@"<br>
<br>
Read the [Introduction]({0}) or browse the rest [of the site]({0}AllArticles.html)
<br>
", baseUrl);
                string html = md.Transform(markdownContent + footer);
                string googleAnalytics = @"
<script>
  (function(i,s,o,g,r,a,m){{i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){{
  (i[r].q=i[r].q||[]).push(arguments)}},i[r].l=1*new Date();a=s.createElement(o),
  m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
  }})(window,document,'script','//www.google-analytics.com/analytics.js','ga');

  ga('create', 'UA-66546851-1', 'auto');
  ga('send', 'pageview');
</script>
";
                string htmlWithCss = string.Format(@"<html>
<head>
<title>{0}</title>
<script src=""https://cdn.rawgit.com/google/code-prettify/master/loader/run_prettify.js""></script>
<link rel=""stylesheet"" href=""{3}github-markdown.css"">
<link rel='shortcut icon' type='image/x-icon' href='{3}favicon.ico' />
<style>
      .markdown-body {{
                min-width: 200px;
                max-width: 790px;
                margin: 0 auto;
                padding: 30px;
            }}
</style>

{1}

</head>
<body>
<article class=""markdown-body"">

{2}


</article>
</body>
</html>", pageTitle, googleAnalytics, html, baseUrl);

                return htmlWithCss;
            }
        }
    }
}