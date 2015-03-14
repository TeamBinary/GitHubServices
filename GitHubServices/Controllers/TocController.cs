using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using GitHubServices.Models;
using Octokit;

namespace GitHubServices.Controllers
{

    public class TestController : ApiController
    {
        // http://localhost:1547/api/test/ss/sd
        public string Get(string p1, string p2)
        {
            return p1 + " " + p2;
        }
    }

    static class Configuration22
    {
        static Lazy<string> val = new Lazy<string>(() =>
            {
                var path = @"c:\temp\githubservice_token.txt";
                if (File.Exists(path))
                    return File.ReadAllText(path);

                return ConfigurationManager.AppSettings["githubservices_token"];
            });

        public static string GitHubAccessToken
        {
            get { return val.Value; }

        }
    }


    public class BooController : ApiController
    {
        public HttpResponseMessage Get(string url)
        {
            return Request.CreateResponse(new Toc { ToCValueForPasting = makeTestingStringForTempWork() });
        }

        string makeTestingStringForTempWork()
        {
            var client = new GitHubClient(new ProductHeaderValue("my-cool-app-name"));

            var accessToken = Configuration22.GitHubAccessToken;

            var tokenAuth = new Credentials(accessToken);
            client.Credentials = tokenAuth;

            Console.WriteLine("***Issues***");
            var issues = client.Issue.GetForRepository("kbilsted", "stateprinter").Result;
            var res = string.Join(", ", issues.Select(x => x.Title + "::" + x.State));
            Console.WriteLine(res);
            return res;
        }

    }

    public class TocController : ApiController
    {
        public UrlReader reader = null;

        // http://localhost:1547/api/toc?url=https://raw.githubusercontent.com/kbilsted/StatePrinter/master/README.md
        public HttpResponseMessage Get(string url)
        {
            Console.WriteLine(string.Format("Content_Console: {0}", url));
            Debug.WriteLine(string.Format("Content_Debug: {0}", url));

            var tocString = Logic(url);
            return Request.CreateResponse(new Toc { ToCValueForPasting = "" + tocString });
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
