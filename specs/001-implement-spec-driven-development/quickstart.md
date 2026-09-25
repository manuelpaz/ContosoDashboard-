# Quickstart: Document Upload and Management Validation

## Prerequisites

- .NET SDK for the current project version is installed.
- SQL Server LocalDB is available for the training app.
- The application can run in the repository root using the existing Blazor Server startup flow.

## Validation steps

1. Start the application:
   ```powershell
   cd ContosoDashboard
   dotnet run
   ```
2. Log in using one of the seeded users, such as the project manager or employee account.
3. Navigate to the project or personal document area.
4. Upload a valid PDF or Office document with a title and category.
5. Confirm the upload succeeds and the document appears in the appropriate document list.
6. Search for the document by title, category, or uploader name and verify it appears only for authorized users.
7. Share the document with an authorized project member and confirm an in-app notification is rendered.
8. Attempt to access the same document from a user outside the project and verify access is denied.
9. Delete the uploaded document after confirmation and verify the record is removed from the UI and underlying storage.

## Expected results

- Upload validation prevents unsupported extensions and oversize files.
- Documents appear in the correct personal or project list.
- Search results are restricted to accessible records.
- Same-project recipients can view shared documents.
- Unauthorized users cannot access protected documents.
- The document activity remains visible in the dashboard and project context.

## Further verification

- Use manual checks for role-based access and UI flows.
- Add automated tests later for upload validation, access control, and share notification behavior.
