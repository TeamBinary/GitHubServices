using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace GitHubServices.Models
{
    public class UrlReader
    {
        public virtual string ReadUrl(Uri url)
        {
            var webRequest = WebRequest.Create(url);

            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                string result = reader.ReadToEnd();
                
                return result;
            }
        }
    }
}