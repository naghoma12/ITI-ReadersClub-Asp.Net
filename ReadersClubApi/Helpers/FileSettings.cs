using Microsoft.AspNetCore.Http;

namespace ReadersClubApi.Helpers
{
    public static class FileSettings
    {
        public static string UploadFile(IFormFile file, string folderName, string webRootPath)
        {
            string folderPath = Path.Combine(webRootPath, "Uploads", folderName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return fileName;
        }
    }
}
