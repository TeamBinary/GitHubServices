using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using GitHubServices.BusinessLogic.TagPageCreator.Domain;

namespace GitHubServices.BusinessLogic.TagPageCreator
{
	public class DocumentParser
	{
		public TagCollection GetTags(string rootFilePath)
		{
			var tags = new TagCollection();
			var di = new DirectoryInfo(rootFilePath);
			foreach (var path in di.EnumerateFiles("*.md", SearchOption.AllDirectories))
			{
				var relativePath = path.FullName.Substring(rootFilePath.Length);
				var fileContent = File.ReadAllText(path.FullName, new UTF8Encoding(true));

				if (DocumentParserCore.IsDraftFile(fileContent))
					continue;

				var tagsForPage = DocumentParserCore.ParsePage(fileContent, relativePath);
				tags.Add(tagsForPage);
			}

			return tags;
		}
	}

	static class DocumentParserCore
	{
		static readonly RegexOptions Options = RegexOptions.Compiled | RegexOptions.Singleline;

		public static readonly Regex CategoryEx = new Regex(@"<Categories Tags=""(?<tags>[^""]*)"">[^<]+</Categories>", Options);
		static readonly Regex headerEx = new Regex("^# (?<title>.*)$", RegexOptions.Compiled | RegexOptions.Multiline);

		public static string ParsePageTitle(string pageContent)
		{
			Match headerMatch = headerEx.Match(pageContent);
			var title = headerMatch.Groups["title"].Value.Trim();

			return title;
		}

		static Tag[] GetTagsFromContent(string content)
		{
			return GetTheTags(CategoryEx.Match(content).Groups["tags"].Value);
		}

		public static Tag[] GetTheTags(string tagsArgument)
		{
			Tag[] parsedTags = tagsArgument
				.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
				.Select(y => new Tag(y.Trim()))
				.ToArray();
			return parsedTags;
		}

		internal static TagCollection ParsePage(string pageContent, string fullName)
		{
			var title = ParsePageTitle(pageContent);

			var tags = GetTagsFromContent(pageContent)
				.Select(x => Tuple.Create(x, new[] {new Page(title, fullName)}));

			return new TagCollection(tags);
		}

		public static bool IsDraftFile(string fileContent)
		{
			return fileContent.StartsWith("draft");
		}
	}
}
