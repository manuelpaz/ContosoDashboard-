using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using ContosoDashboard.Data;
using ContosoDashboard.Models;

namespace ContosoDashboard.Services;

public interface IDocumentService
{
    Task<List<Document>> GetAccessibleDocumentsAsync(int currentUserId, int? projectId = null);
    Task<Document?> GetDocumentByIdAsync(int documentId, int currentUserId);
    Task<Document> UploadDocumentAsync(
        int currentUserId,
        string title,
        string category,
        string? description,
        int? projectId,
        Stream fileStream,
        long fileSizeBytes,
        string fileName,
        string contentType);
    Task<bool> DeleteDocumentAsync(int documentId, int currentUserId);
}

public class DocumentService : IDocumentService
{
    private const long MaxFileSizeBytes = 25L * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
        ".txt", ".png", ".jpg", ".jpeg", ".gif", ".bmp"
    };

    private static readonly HashSet<string> AllowedCategories = new(StringComparer.OrdinalIgnoreCase)
    {
        "Project Documents",
        "Team Resources",
        "Personal Files",
        "Reports",
        "Presentations",
        "Other"
    };

    private readonly ApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public DocumentService(ApplicationDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public static List<string> ValidateUpload(string title, string category, string fileName, long fileSizeBytes)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(title))
        {
            errors.Add("Document title is required.");
        }

        if (string.IsNullOrWhiteSpace(category) || !AllowedCategories.Contains(category.Trim()))
        {
            errors.Add("Document category must be one of the allowed values.");
        }

        if (string.IsNullOrWhiteSpace(fileName) || Path.GetExtension(fileName) is not { Length: > 0 } extension || !AllowedExtensions.Contains(extension))
        {
            errors.Add("Unsupported file type. Please upload a supported document format.");
        }

        if (fileSizeBytes <= 0)
        {
            errors.Add("Document content is required.");
        }

        if (fileSizeBytes > MaxFileSizeBytes)
        {
            errors.Add("File size must be 25 MB or less.");
        }

        return errors;
    }

    public async Task<List<Document>> GetAccessibleDocumentsAsync(int currentUserId, int? projectId = null)
    {
        var query = _context.Documents
            .Include(d => d.UploadedByUser)
            .Include(d => d.Project)
            .Include(d => d.DocumentShares)
            .Where(d => !d.IsDeleted)
            .Where(d =>
                d.UploadedByUserId == currentUserId ||
                (d.ProjectId.HasValue &&
                    (
                        d.Project != null &&
                        (d.Project.ProjectManagerId == currentUserId ||
                         d.Project.ProjectMembers.Any(pm => pm.UserId == currentUserId))
                    )) ||
                d.DocumentShares.Any(ds => ds.UserId == currentUserId && ds.IsActive));

        if (projectId.HasValue)
        {
            query = query.Where(d => d.ProjectId == projectId.Value);
        }

        return await query
            .OrderByDescending(d => d.UploadDateUtc)
            .ToListAsync();
    }

    public async Task<Document?> GetDocumentByIdAsync(int documentId, int currentUserId)
    {
        var document = await _context.Documents
            .Include(d => d.UploadedByUser)
            .Include(d => d.Project)
            .Include(d => d.DocumentShares)
            .FirstOrDefaultAsync(d => d.DocumentId == documentId && !d.IsDeleted);

        if (document == null)
        {
            return null;
        }

        var isOwner = document.UploadedByUserId == currentUserId;
        var isProjectMember = document.ProjectId.HasValue &&
            await _context.Projects
                .AnyAsync(p => p.ProjectId == document.ProjectId.Value &&
                    (p.ProjectManagerId == currentUserId || p.ProjectMembers.Any(pm => pm.UserId == currentUserId)));

        var isSharedRecipient = document.DocumentShares.Any(ds => ds.UserId == currentUserId && ds.IsActive);

        return isOwner || isProjectMember || isSharedRecipient ? document : null;
    }

    public async Task<Document> UploadDocumentAsync(
        int currentUserId,
        string title,
        string category,
        string? description,
        int? projectId,
        Stream fileStream,
        long fileSizeBytes,
        string fileName,
        string contentType)
    {
        var validationErrors = ValidateUpload(title, category, fileName, fileSizeBytes);
        if (validationErrors.Count > 0)
        {
            throw new InvalidOperationException(string.Join(" ", validationErrors));
        }

        if (projectId.HasValue)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectMembers)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId.Value);

            if (project == null)
            {
                throw new InvalidOperationException("Project was not found.");
            }

            var isAuthorized = project.ProjectManagerId == currentUserId ||
                project.ProjectMembers.Any(pm => pm.UserId == currentUserId);

            if (!isAuthorized)
            {
                throw new InvalidOperationException("You are not authorized to upload a document for that project.");
            }
        }

        var (storedFileName, storedFilePath) = await _fileStorageService.SaveAsync(fileStream, fileName);

        var document = new Document
        {
            Title = title.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            Category = category.Trim(),
            UploadedByUserId = currentUserId,
            ProjectId = projectId,
            OriginalFileName = fileName.Trim(),
            StoredFileName = storedFileName,
            StoredFilePath = storedFilePath,
            ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType,
            FileSizeBytes = fileSizeBytes,
            UploadDateUtc = DateTime.UtcNow,
            UpdatedDateUtc = DateTime.UtcNow
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        var scanJob = new DocumentScanJob
        {
            DocumentId = document.DocumentId,
            QueueMessageId = $"local-{Guid.NewGuid():N}",
            Status = "Queued",
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.DocumentScanJobs.Add(scanJob);
        await _context.SaveChangesAsync();

        return document;
    }

    public async Task<bool> DeleteDocumentAsync(int documentId, int currentUserId)
    {
        var document = await _context.Documents
            .FirstOrDefaultAsync(d => d.DocumentId == documentId);

        if (document == null || document.IsDeleted)
        {
            return false;
        }

        var isOwner = document.UploadedByUserId == currentUserId;
        var isProjectManager = document.ProjectId.HasValue &&
            await _context.Projects.AnyAsync(p => p.ProjectId == document.ProjectId.Value && p.ProjectManagerId == currentUserId);

        if (!isOwner && !isProjectManager)
        {
            return false;
        }

        document.IsDeleted = true;
        document.UpdatedDateUtc = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(document.StoredFilePath) && File.Exists(document.StoredFilePath))
        {
            File.Delete(document.StoredFilePath);
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
