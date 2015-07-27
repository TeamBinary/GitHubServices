using System;
using System.IO;
using System.Text;

namespace GitHubServices.BusinessLogic.TagPageCreator
{

    public interface IFilesystemRepository
    {
        void EmptyTagDirectory(string rootFolder);

        void WriteFile(string filepath, string content);

        string ReadFile(string filepath);
    }

    public class FilesystemRepository : IFilesystemRepository
    {
        public void EmptyTagDirectory(string tagDir)
        {
            if (Directory.Exists(tagDir))
            {
                var directory = new DirectoryInfo(tagDir);
                foreach (FileInfo file in directory.GetFiles())
                    file.Delete();
                Directory.Delete(tagDir);
            }
            Directory.CreateDirectory(tagDir);
        }

        public void WriteFile(string filepath, string content)
        {
            bool write = false;
            if (!File.Exists(filepath))
                write = true;
            else
            {
                if (File.ReadAllText(filepath) != content)
                    write = true;
            }

            if (write)
            {
                Console.WriteLine("Writing " + filepath);
                File.WriteAllText(filepath, content, new UTF8Encoding(true));
            }
        }

        public string ReadFile(string filepath)
        {
            return File.ReadAllText(filepath, new UTF8Encoding());
        }
    }
}