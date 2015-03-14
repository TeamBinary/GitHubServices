using System;

using GitHubServices.BusinessLogic;

using NUnit.Framework;

namespace GitHubServices.Test
{
    [TestFixture]
    class JsonConnectTest
    {
        [Test]
        public void getContent()
        {
            var path = "/repos/kbilsted/AutoHasher/contents/README.md";
//            var res = new GithubCommunicator(new GitHhubBase64(), new ExceptionHelper()).GetContent(path);

         //   Console.WriteLine(res);
        }

        [Test]
        public void Update()
        {
            var path = "/repos/kbilsted/AutoHasher/contents/README.md";
         // TODO   new GithubCommunicator(new GitHhubBase64(), new ExceptionHelper()).Update();
        }

    }
}
