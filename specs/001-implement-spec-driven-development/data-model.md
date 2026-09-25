# Data Model: Document Upload and Management

## Core entities

### Document

Represents a stored work document and its metadata.

| Field | Type | Constraints | Notes |
|---|---|---|---|
| `DocumentId` | int | PK, required | Integer key consistent with user/project ids |
| `Title` | string | required, max 255 | User-facing name |
| `Description` | string? | max 2000 | Optional details |
| `Category` | string | required, max 100 | Text category such as `Project Documents` |
| `UploadedByUserId` | int | required | Owner/reference to `User` |
| `ProjectId` | int? | optional | Project association |
| `TaskId` | int? | optional | Task-specific attachment |
| `OriginalFileName` | string | required, max 255 | Original user-supplied file name for UI only |
| `StoredFileName` | string | required, max 255 | GUID-based filename used on disk |
| `StoredFilePath` | string | required, max 500 | Relative or server-local storage path |
| `ContentType` | string | required, max 255 | MIME type |
| `FileSizeBytes` | long | required | Size in bytes |
| `UploadDateUtc` | DateTime | required | Timestamp |
| `UpdatedDateUtc` | DateTime | required | Modified version timestamp |
| `IsDeleted` | bool | default false | Soft-delete tracking |
| `IsArchived` | bool | default false | Optional archival state |

Relationships:
- Many `Document` rows relate to one `User` uploader.
- Many `Document` rows relate to zero or one `Project`.
- Many `Document` rows relate to zero or one `Task`.
- A `Document` can have multiple `DocumentShare` rows.

### DocumentShare

Tracks the explicit access granted to recipients.

| Field | Type | Constraints | Notes |
|---|---|---|---|
| `DocumentShareId` | int | PK, required | |
| `DocumentId` | int | required | Parent document |
| `UserId` | int | required | Recipient user |
| `SharedByUserId` | int | required | Owner or manager who granted access |
| `SharedAtUtc` | DateTime | required | Share timestamp |
| `IsActive` | bool | default true | Allow future revoke flows |

Relationships:
- `DocumentShare` belongs to a single `Document`.
- `DocumentShare` belongs to a single recipient user and one sharing user.

### Notification

Existing entity already supports user-facing alerts. The document feature will create or reuse notifications when a share event occurs or a project document is added.

### DocumentScanJob

Represents the asynchronous malware-scanning state for a document after upload.

| Field | Type | Constraints | Notes |
|---|---|---|---|
| `DocumentScanJobId` | int | PK, required | |
| `DocumentId` | int | required | Parent document |
| `QueueMessageId` | string | required, max 255 | Azure queue correlation id |
| `Status` | string | required, max 50 | `Queued`, `Scanning`, `Clean`, `Infected`, `Failed` |
| `StartedAtUtc` | DateTime? | optional | |
| `CompletedAtUtc` | DateTime? | optional | |
| `ScanResultDetails` | string? | optional, max 2000 | Scanner summary or error details |
| `CreatedAtUtc` | DateTime | required | |

Relationships:
- Each document may have zero or one active `DocumentScanJob` record.
- A run is created immediately after upload and updated by the queue-triggered scan function.

### ProjectMember

Existing entity already enforces that project members are authorized participants. This will be the primary validation path for same-project sharing rules.

## Validation rules

- `Title` is required.
- `Category` must be one of the approved text values: `Project Documents`, `Team Resources`, `Personal Files`, `Reports`, `Presentations`, `Other`.
- `ContentType` must be within the approved allowlist for document upload.
- `FileSizeBytes` must be <= 25 MB.
- `StoredFileName` must be unique and generated server-side.
- `StoredFilePath` must not depend on user input.
- `ProjectId` may be null only for personal document entries.
- A recipient of a share must be authorized by project membership or explicit access rules.
- A document cannot be treated as fully active until the associated scan job is `Clean` or an approved override status is recorded.

## State transitions

### Document lifecycle
- Draft/Uploaded: file accepted, metadata stored, file persisted to local storage.
- Active: visible in project and personal lists.
- Updated: metadata changed or file replaced.
- Shared: one or more `DocumentShare` records created, notifications sent.
- Deleted: document marked deleted and storage removed, with confirmation workflow.

### Share lifecycle
- Created when user grants access to a recipient.
- Active while `IsActive = true`.
- Revoked when access is removed or document is deleted.

### Scan lifecycle
- `Queued`: document accepted and message sent to the scan queue.
- `Scanning`: queue-triggered Azure Function is processing the file.
- `Clean`: file passes the scan and can be considered active for view/download.
- `Infected` or `Failed`: file is quarantined, access restricted, and flagged for review.
