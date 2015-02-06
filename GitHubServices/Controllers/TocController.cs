﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using GitHubServices.Models;

namespace GitHubServices.Controllers
{
    public class TocController : ApiController
    {
        public UrlReader reader = null;

        public HttpResponseMessage CreateToc(string content)
        {
            Console.WriteLine("Content_Console: {0}", content);
            Debug.WriteLine("Content_Debug: {0}", content);

            var tocString = Logic(content);

            return Request.CreateResponse(new Toc { ToCValueForPasting = tocString });
        }

        string Logic(string url)
        {
            if (!url.ToLower().EndsWith(".md")) 
                return "";

            reader = reader ?? new UrlReader();
            var page = reader.ReadUrl(new Uri(url));

            var parser = new TocParser();
            var tocString = parser.MakeToc(page);
            return tocString;
        }
    }
}
