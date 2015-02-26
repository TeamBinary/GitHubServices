using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Octokit;

namespace GitHubServices.Test.GithubCalling
{
    [TestFixture]
    class CallGithubby
    {
        [Test]
        public async void TestName()
        {

            var accessToken = "7d414c5210ba577872e1e785f2b5c7d4b13622cb";
            var client = new GitHubClient(new ProductHeaderValue("my-cool-app-name"));

            var tokenAuth = new Credentials(accessToken);
            client.Credentials = tokenAuth;

            Console.WriteLine("***Issues***");
            var issues = await client.Issue.GetForRepository("kbilsted", "stateprinter");
            Console.WriteLine(issues.ToList().Count);

            Console.WriteLine("**user*");
            var user = await client.User.Current();
            Console.WriteLine(user.Name);


            Console.WriteLine("**clone**");
            var repo = await client.Repository.Get("octokit", "octokit.net");
            client.Connection.Post<>()
            client.Repository.

            Console.WriteLine("** REadme**");
            var readme = await client.Repository.Content.GetReadme("kbilsted", "stateprinter");
            Console.WriteLine(readme.Url);
            Console.WriteLine(readme.Name);
            Console.WriteLine(readme.Content.Substring(0,40));        
        }
    }
}
