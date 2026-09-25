namespace ContosoDashboard.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _storageDirectory;

    public LocalFileStorageService(IWebHostEnvironment environment)
    {
        _storageDirectory = Path.Combine(environment.ContentRootPath, "App_Data", "uploads");
        Directory.CreateDirectory(_storageDirectory);
    }

    public async Task<(string StoredFileName, string StoredFilePath)> SaveAsync(Stream fileStream, string originalFileName, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(originalFileName);
        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var storedFilePath = Path.Combine(_storageDirectory, storedFileName);

        await using var targetStream = File.Create(storedFilePath);
        await fileStream.CopyToAsync(targetStream, cancellationToken);

        return (storedFileName, storedFilePath);
    }

    public Task DeleteAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}
