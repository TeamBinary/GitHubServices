using System;
using System.IO;
using System.Text;

namespace GitHubServices.BusinessLogic.TagPageCreator
{
    public class FilesystemRepository : IFilesystemRepository
    {
        public void EnsureEmptyTagDirectory(string tagDir)
        {
            if (Directory.Exists(tagDir))
            {
                var directory = new DirectoryInfo(tagDir);
                foreach (FileInfo file in directory.GetFiles())
                    file.Delete();
            }

            if (!Directory.Exists(tagDir))
                Directory.CreateDirectory(tagDir);
        }

        public void WriteFile(string filepath, string content, string pageTitle="")
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

        public void Copy(string sourcepath, string destinationPath)
        {
            File.Copy(sourcepath, destinationPath);
        }
    }
}