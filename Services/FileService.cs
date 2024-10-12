﻿namespace GallifreyPlanet.Services
{
    public class FileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<string> UploadFileAsync(IFormFile avatarFile, string? uploadFolder = null)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + avatarFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (FileStream? fileStream = new FileStream(filePath, FileMode.Create))
            {
                await avatarFile.CopyToAsync(fileStream);
            }

            return "/uploads" + uploadFolder + "/" + uniqueFileName;
        }
    }
}
