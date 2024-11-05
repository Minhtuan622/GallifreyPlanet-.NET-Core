namespace GallifreyPlanet.Services
{
    public class FileService(IWebHostEnvironment webHostEnvironment)
    {
        public async Task<string> UploadFileAsync(IFormFile file, string? folder, string? currentFilePath = null)
        {
            if (currentFilePath is not null)
            {
                string basePath = webHostEnvironment.WebRootPath + currentFilePath;
                if (File.Exists(basePath))
                {
                    File.Delete(basePath);
                }
            }

            string fileDir = "uploads" + folder;
            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, fileDir);
            string fileName = Guid.NewGuid() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/" + fileDir + "/" + fileName;
        }
    }
}
