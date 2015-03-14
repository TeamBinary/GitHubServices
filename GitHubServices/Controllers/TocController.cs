using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using GitHubServices.Models;

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

    public class TocController : ApiController
    {
        public UrlReader reader = null;

        // http://localhost:1547/api/toc?url=https://raw.githubusercontent.com/kbilsted/StatePrinter/master/README.md
        public HttpResponseMessage Get(string url)
        {
            Console.WriteLine(string.Format("Content_Console: {0}", url));
            Debug.WriteLine(string.Format("Content_Debug: {0}", url));

            //var tocString = Logic(url);
         //   return Request.CreateResponse(new Toc { ToCValueForPasting = ""+ ticString});
           
            var envvar = ConfigurationManager.AppSettings["githubservices_token"];
            if(envvar != null)
            {
                var fives = envvar.Count(x => x == '5');
                return Request.CreateResponse(new Toc { ToCValueForPasting = "" + fives });
            }
            return Request.CreateResponse(new Toc { ToCValueForPasting = "no value found" });
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
