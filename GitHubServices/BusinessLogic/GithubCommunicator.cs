using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

using GitHubServices.BusinessLogic.Json;

using Newtonsoft.Json;

namespace GitHubServices.BusinessLogic
{
    public class GithubCommunicator
    {
        GitHhubBase64 gitHhubBase64;

        ExceptionHelper exceptionHelper;

        public GithubCommunicator(GitHhubBase64 gitHhubBase64, ExceptionHelper exceptionHelper)
        {
            this.gitHhubBase64 = gitHhubBase64;
            this.exceptionHelper = exceptionHelper;
        }

        public void Go()
        {
           try
           {
               var client = CreateHttpClient();

               Console.WriteLine(client.GetAsync("/user").Result);
           }
           catch (AggregateException e)
            {
               exceptionHelper.Print(e);
            }
        }


        public void Update(string path, string newContent, string sha, string commitMessage)
        {
            try
            {
                using (var client = CreateHttpClient())
                {
                    var put = new PutContent()
                                  {
                                      content = gitHhubBase64.Base64GithubEncode(newContent),
                                      message = commitMessage,
                                      sha = sha
                                  };
                    using (HttpResponseMessage response = client.PutAsync(path, new StringContent(JsonConvert.SerializeObject(put))).Result)
                    {
                        Console.WriteLine(response);
                    }
                }
            }
            catch (AggregateException e)
            {
                Console.WriteLine("Errr..");
                exceptionHelper.Print(e);
            }
        }

        public string GetContent(string path)
        {
            try
            {
                using (var client = CreateHttpClient())
                using (HttpResponseMessage response = client.GetAsync(path).Result)
                using (HttpContent httpContent = response.Content)
                {
                    string rawJson = httpContent.ReadAsStringAsync().Result;
                    var conte = JsonConvert.DeserializeObject<ContentObject>(rawJson);

                    var pageContent = gitHhubBase64.GetStringFromBase64(conte);
                    return pageContent;
                }
            }
            catch (AggregateException e)
            {
                Console.WriteLine("Errr..");
                exceptionHelper.Print(e);
                throw e;
            }
        }


        static HttpClient CreateHttpClient()
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri("https://api.github.com");
            client.DefaultRequestHeaders.Add(
                "Authorization",
                "token c7eddc30a907d8c1d138ddb0848ede028ed30567");
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add(
                "User-Agent",
                "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");

            return client;
        }

      
    }
}