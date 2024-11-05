namespace GallifreyPlanet.Services
{
    public class FileService(IWebHostEnvironment webHostEnvironment)
    {
        public async Task<string> UploadFileAsync(IFormFile file, string? folder, string? currentFilePath = null)
        {
            if (currentFilePath is not null)
            {
                string basePath = webHostEnvironment.WebRootPath + currentFilePath;
                if (File.Exists(path: basePath))
                {
                    File.Delete(path: basePath);
                }
            }

            string fileDir = "uploads" + folder;
            string uploadsFolder = Path.Combine(path1: webHostEnvironment.WebRootPath, path2: fileDir);
            string fileName = Guid.NewGuid() + "_" + file.FileName;
            string filePath = Path.Combine(path1: uploadsFolder, path2: fileName);

            using (FileStream fileStream = new FileStream(path: filePath, mode: FileMode.Create))
            {
                await file.CopyToAsync(target: fileStream);
            }

            return "/" + fileDir + "/" + fileName;
        }
    }
}
