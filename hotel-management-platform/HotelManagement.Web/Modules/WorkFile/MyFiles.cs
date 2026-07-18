
namespace Management_Hotel_2025.Modules.WorkFile
{
    public class MyFiles : IMyFiles
    {
        public string CreateFolder(string folderName)
        {

            if (string.IsNullOrWhiteSpace(folderName))
                throw new ArgumentException("Tên thư mục không hợp lệ", nameof(folderName));

            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName);

            DirectoryInfo result = Directory.CreateDirectory(folderPath);

            Console.WriteLine($"Thư mục đã được tạo hoặc tồn tại vclllll: {result.FullName}");
            return result.FullName;
        }

        public void DeleteFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Đường dẫn file không hợp lệ", nameof(filePath));
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Console.WriteLine($"Đã xóa file: {filePath}");
            }
            else
            {
                Console.WriteLine($"File không tồn tại: {filePath}");
            }
        }


        public void DeleteFolder(string folderPath)
        {
            bool recursive = true;
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                throw new ArgumentException("Đường dẫn không hợp lệ", nameof(folderPath));
            }

            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, recursive);
                Console.WriteLine($"Đã xóa thư mục: {folderPath}");
            }
            else
            {
                Console.WriteLine($"Thư mục không tồn tại: {folderPath}");
            }
        }

        public async Task<string> SaveFiles(IFormFile file, string pathfolder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Không có file nào để lưu.", nameof(file));

            if (string.IsNullOrWhiteSpace(pathfolder))
                throw new ArgumentException("Đường dẫn thư mục không hợp lệ", nameof(pathfolder));

            if (!Directory.Exists(pathfolder))
            {
                Directory.CreateDirectory(pathfolder);
            }

            string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";

            string filePath = Path.Combine(pathfolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }



        public bool IsFileExist(string filePath, IFormFile file)
        {
            if (file == null || string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Đường dẫn hoặc file không hợp lệ");
            string fullPath = Path.Combine(filePath, file.FileName);
            return File.Exists(fullPath);
        }

        public string ChangeFileName(string FileName)
        {
            string extension = Path.GetExtension(FileName);
            string newFileName = $"{Guid.NewGuid()}{extension}";
            return newFileName;
        }


        public string GetFilePath(string folderPath, string fileName)
        {
            if (string.IsNullOrWhiteSpace(folderPath) || string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Đường dẫn hoặc tên file không hợp lệ");
            return Path.Combine(folderPath, fileName);
        }




    }
}

