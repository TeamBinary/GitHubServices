using System.IO;
using System.Text;

using MarkdownDeep;

namespace GitHubServices.BusinessLogic.TagPageCreator {
    public class HtmlWriter : IFilesystemRepository
    {
        readonly HtmlTransformer transformer;

        readonly string baseUrl;

		public HtmlWriter(string baseUrl, ContentGenerator generator) {
            this.baseUrl = baseUrl;
            transformer = new HtmlTransformer(generator);
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
            readonly ContentGenerator generator;

            public HtmlTransformer(ContentGenerator generator)
            {
                this.generator = generator;
            }

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

      

            public string MarkdownToHtml(string markdownContent, string pageTitle, string baseUrl) {
                string footer = $@"<br>
<br>
Read the [Introduction]({baseUrl}) or browse the rest [of the site]({baseUrl}AllArticles.html)
<br>
<br>
";
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
                var htmlWithCss = $@"<html>
<head>
<title>{pageTitle}</title>

<script src=""https://cdn.rawgit.com/google/code-prettify/master/loader/run_prettify.js""></script>
<link href=""http://firstclassthoughts.co.uk/atelier-forest-light.css"" type =""text/css"" rel=""stylesheet"" />

<link href=""{baseUrl}github-markdown.css"" type =""text/css"" rel=""stylesheet"">
<link rel='shortcut icon' type='image/x-icon' href='{baseUrl}favicon.ico'/>
<style>
      .markdown-body {{
                min-width: 200px;
                max-width: 790px;
                margin: 0 auto;
                padding: 30px;
            }}
</style>

{googleAnalytics}

</head>
<body onload=""prettyPrint()"">
<article class=""markdown-body"">

{html}


</article>
</body>
</html>";

                return htmlWithCss;
            }
        }
    }
}