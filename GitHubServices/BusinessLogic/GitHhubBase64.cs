using System;
using System.Collections.Generic;
using System.Text;

using GitHubServices.BusinessLogic.Json;

namespace GitHubServices.BusinessLogic
{
    public class GitHhubBase64
    {
        public string GetStringFromBase64(ContentObject conte)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(conte.content.Replace("\\n", "")));
        }

        public string Base64GithubEncode(string content)
        {
            var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(content));
            var newlined = string.Join(@"\n", Splitter(base64String));

            return newlined;
        }

        IEnumerable<string> Splitter(string s)
        {
            const int GitHubLineLength = 60;
            int pos = 0;
            int max = s.Length;

            while (true)
            {
                if (max - pos < GitHubLineLength)
                {
                    yield return s.Substring(pos);
                    yield break;
                }

                string sub = s.Substring(pos, GitHubLineLength);
                pos += GitHubLineLength;
                yield return sub;
            }
        }
    }
}