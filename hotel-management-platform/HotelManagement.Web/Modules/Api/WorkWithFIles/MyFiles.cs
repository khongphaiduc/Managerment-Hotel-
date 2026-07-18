
namespace API_BookingHotel.Modules.WorkWithFIles
{
    public class MyFiles : IMyFiles
    {
        public string CreateFolder(string folderName)
        {
            throw new NotImplementedException();
        }

        public void DeleteFile(string filePath)
        {
            throw new NotImplementedException();
        }

        public void DeleteFolder(string folderName)
        {
            throw new NotImplementedException();
        }

        public string GetFilePath(string folderPath, string fileName)
        {
            throw new NotImplementedException();
        }

        public bool IsFileExist(string filePath, IFormFile file)
        {
            throw new NotImplementedException();
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



    }
}
