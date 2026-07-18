namespace Management_Hotel_2025.Modules.WorkFile
{
    public interface IMyFiles
    {

        public Task<string> SaveFiles(IFormFile files, string folderName);


        public void DeleteFile(string filePath);

        public string CreateFolder(string folderName);


        public void DeleteFolder(string folderName);

        bool IsFileExist(string filePath, IFormFile file);
        string GetFilePath(string folderPath, string fileName);
    }
}
