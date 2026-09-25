using ContosoDashboard.Services;

namespace ContosoDashboard.Tests;

public class DocumentServiceTests
{
    [Fact]
    public void ValidateUpload_WhenTitleIsMissing_ReturnsValidationError()
    {
        var errors = DocumentService.ValidateUpload(string.Empty, "Project Documents", "report.pdf", 1048576L);

        Assert.Contains(errors, e => e.Contains("title", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidateUpload_WhenFileExceedsLimit_ReturnsValidationError()
    {
        var errors = DocumentService.ValidateUpload("Quarterly Report", "Reports", "report.pdf", 26L * 1024 * 1024);

        Assert.Contains(errors, e => e.Contains("25 MB", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidateUpload_WhenFileTypeIsSupported_ReturnsNoErrors()
    {
        var errors = DocumentService.ValidateUpload("Quarterly Report", "Reports", "report.pdf", 1024L);

        Assert.Empty(errors);
    }
}
