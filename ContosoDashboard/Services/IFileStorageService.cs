namespace ContosoDashboard.Services;

public interface IFileStorageService
{
    Task<(string StoredFileName, string StoredFilePath)> SaveAsync(Stream fileStream, string originalFileName, CancellationToken cancellationToken = default);
    Task DeleteAsync(string filePath, CancellationToken cancellationToken = default);
}
