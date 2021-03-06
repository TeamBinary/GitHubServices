﻿using System;

namespace GitHubServices.BusinessLogic.TagPageCreator.Domain
{
    public class Page : IEquatable<Page>
    {
        public readonly string Title;
        public readonly string FilePath;
        public readonly string FilePathWithHtmlExtension;
        public readonly string Path;

        public Page(string title, string filePath)
        {
            if (title.EndsWith("#"))
                title = title.Substring(0, title.Length - 1);
            Title = title.Trim();
            FilePath = filePath.Replace("\\", "/");
            FilePathWithHtmlExtension = FilePath.Substring(0, FilePath.Length - 3) + ".html";
            Path = System.IO.Path.GetDirectoryName(filePath).Replace("\\", "/");
        }

        public override int GetHashCode()
        {
            return Title.GetHashCode() ^ FilePath.GetHashCode();
        }

        public bool Equals(Page other)
        {
            if (other == null) return false;
            return Title == other.Title && FilePath == other.FilePath;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Page);
        }
    }
}