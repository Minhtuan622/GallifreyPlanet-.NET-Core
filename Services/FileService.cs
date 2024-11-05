namespace GallifreyPlanet.Services
{
    public class FileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string? folder, string? currentFilePath = null)
        {
            if (currentFilePath is not null)
            {
                string basePath = _webHostEnvironment.WebRootPath + currentFilePath;
                if (File.Exists(basePath))
                {
                    File.Delete(basePath);
                }
            }

            string fileDir = "uploads" + folder;
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, fileDir);
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
