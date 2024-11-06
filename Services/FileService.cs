namespace GallifreyPlanet.Services;

public class FileService(IWebHostEnvironment webHostEnvironment)
{
    public async Task<string> UploadFileAsync(IFormFile file, string? folder, string? currentFilePath = null)
    {
        if (currentFilePath is not null)
        {
            var basePath = webHostEnvironment.WebRootPath + currentFilePath;
            if (File.Exists(path: basePath))
            {
                File.Delete(path: basePath);
            }
        }

        var fileDir = "uploads" + folder;
        var uploadsFolder = Path.Combine(path1: webHostEnvironment.WebRootPath, path2: fileDir);
        var fileName = Guid.NewGuid() + "_" + file.FileName;
        var filePath = Path.Combine(path1: uploadsFolder, path2: fileName);

        using (var fileStream = new FileStream(path: filePath, mode: FileMode.Create))
        {
            await file.CopyToAsync(target: fileStream);
        }

        return "/" + fileDir + "/" + fileName;
    }
}