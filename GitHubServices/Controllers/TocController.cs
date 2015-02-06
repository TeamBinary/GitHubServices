using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using GitHubServices.Models;

namespace GitHubServices.Controllers
{
    public class TocController : ApiController
    {
        public HttpResponseMessage CreateToc(string content)
        {
            Console.WriteLine("Content_Console: {0}", content);
            Debug.WriteLine("Content_Debug: {0}", content);

            var urlreader = new UrlReader();
            var page = urlreader.ReadUrl(new Uri(content));

            TocParser parser = new TocParser();
            var tocString = parser.MakeToc(page);

            return Request.CreateResponse(new Toc { ToCValueForPasting = tocString });
        }
    }
}
