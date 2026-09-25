# Document Management Service Contract

## Purpose

The document management service provides a secure workflow for uploading, validating, searching, downloading, sharing, and deleting project and personal documents while enforcing authorization rules.

## Service boundary

The contract is intentionally expressed at the service level because this repository uses a Blazor Server application rather than a separate API host.

## Core operations

### UploadDocumentAsync

**Request**
- `DocumentUploadRequest`
  - `Title: string`
  - `Description: string?`
  - `Category: string`
  - `ProjectId: int?`
  - `TaskId: int?`
  - `Tags: string[]`
  - `FileStream: Stream`
  - `FileName: string`
  - `ContentType: string`

**Response**
- `Result<DocumentRecord>`
  - success when validation passes and file is stored
  - error when unsupported extension, oversize, or unauthorized project access is detected

**Rules**
- Only a valid authenticated user may upload.
- `Title` is required.
- `Category` must be one of the allowed values.
- `ProjectId` is optional for personal documents but must be authorized when provided.
- `StoredFileName` must be generated server-side and never based on user input.

### GetAccessibleDocumentsAsync

**Request**
- `DocumentQuery` with optional `Category`, `ProjectId`, `DateRange`, and `SearchTerm`

**Response**
- `IReadOnlyList<DocumentSummary>`

**Rules**
- Only documents the current user is authorized to access are returned.
- Project or personal ownership must be respected.
- Search results are filtered before response generation.

### DownloadDocumentAsync

**Request**
- `DocumentId: int`

**Response**
- `FileDownloadResult` or an authorization error

**Rules**
- Downloads are allowed only to authorized project members or document owners.
- The file is served through an authorized route rather than direct web server exposure.

### ShareDocumentAsync

**Request**
- `DocumentId: int`
- `RecipientUserIds: int[]`
- `SharedByUserId: int`

**Response**
- `Result` indicating success or access rejection

**Rules**
- Recipients must already be project members or otherwise valid project-authorized users.
- Share events must create notification records and remain auditable.

### DeleteDocumentAsync

**Request**
- `DocumentId: int`
- `RequestedByUserId: int`

**Response**
- `Result`

**Rules**
- Only the document owner or an authorized manager may delete the document.
- Deleted documents must be removed from storage and marked deleted before final removal.

## Security expectations

- Authorization checks happen in the service layer before any data or storage operation.
- User input must never be used directly in file paths or naming.
- All access operations must be auditable via document activity records.
