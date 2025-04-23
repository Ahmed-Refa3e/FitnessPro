using Core.DTOs.GeneralDTO;
using Microsoft.AspNetCore.Http;

namespace Core.Utilities
{
    public static class AddImageHelper
    {
        public static StringResult chickImagePath(IFormFile file, string storagePath)
        {
            if (file is null)
            {
                return new StringResult();
            }
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            var contentType = file.ContentType.ToLower();
            if (!allowedExtensions.Contains(fileExtension) || !contentType.StartsWith("image/"))
            {
                return new StringResult { Massage = "Invalid file type, Only images are allowed" };
            }
            var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(storagePath, fileName);
            return new StringResult { Id = filePath };
        }
        /*
private async Task<List<string>> BackupFiles(List<string> paths, string backupDir)
{
    var backupFiles = new List<string>();
    foreach (var path in paths)
    {
        if (File.Exists(path))
        {
            var backupPath = Path.Combine(backupDir, Path.GetFileName(path));
            File.Move(path, backupPath);
            backupFiles.Add(backupPath);
        }
    }
    return backupFiles;
}
private async Task<void> DeleteFiles(List<string> paths)
{
    foreach (var path in paths)
    {
        if (File.Exists(path))
            File.Delete(path);
    }
}
private async Task<void> RestoreBackupFiles(string backupDir)
{
    foreach (var backupPath in Directory.GetFiles(backupDir))
    {
        var originalPath = Path.Combine(_storagePath, Path.GetFileName(backupPath));
        File.Move(backupPath, originalPath);
    }
}*/
    }
}
