namespace GitHubServices.BusinessLogic.TagPageCreator
{
    public interface IFilesystemRepository
    {
        void EmptyTagDirectory(string tagDir);

        void WriteFile(string filepath, string content, string pageTitle);

        string ReadFile(string filepath);

        void Copy(string sourcepath, string destinationPath);
    }
}