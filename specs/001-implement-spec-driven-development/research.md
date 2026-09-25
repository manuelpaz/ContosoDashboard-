# Research: Document Upload and Management

## Decision: Local, secure file storage with a storage abstraction

The project will store uploaded document files in a dedicated local directory outside `wwwroot`, using a GUID-based naming pattern and an `IFileStorageService` abstraction.

**Rationale**:
- It matches the product requirement for offline local-only training use.
- It prevents user-controlled file names from creating path traversal risks.
- It supports future migration to Azure storage without changing the document service or page logic.
- It preserves the current architecture and avoids introducing cloud dependencies into the training application.

**Alternatives considered**:
- Saving files directly under `wwwroot`: rejected because it exposes uploaded content to direct web access and violates the security requirement.
- Storing raw file content only in the database: rejected because the project requirement explicitly calls for local file storage and separation of metadata from blob content.
- Using direct `Azure.Storage.Blobs` code in the training build: rejected because the feature must remain offline and self-contained.

## Decision: Project-scoped sharing model for v1

Document sharing will be restricted to project members or other users explicitly authorized within the project membership model.

**Rationale**:
- It aligns with the repository’s role and project-user authorization model.
- It reduces the risk of cross-project leaks, which is important in a training environment.
- It keeps implementation smaller and easier to validate before release.

## Decision: Integer document keys and string categories

The data model will use integer `DocumentId` values, and the document category will be stored as text values rather than an enum or foreign key enumeration.

**Rationale**:
- This matches the project’s existing pattern of integer keys for user and project entities.
- It keeps the model easy to explain and implement for the training scenario.
- It avoids unnecessary schema complexity in a local workshop application.

## Decision: Service and page integration pattern

The new functionality will be implemented through the current layered design: `Models` and `Data` for metadata, `Services` for validation and storage workflows, and the existing Razor/Blazor pages for UI access.

**Rationale**:
- It follows the repository’s established architecture and minimizes disruption.
- It supports clean authorization checks in the service layer before data access is performed.
- It keeps business logic, UI, and persistence responsibilities separated.

## Decision: Async malware scanning with Azure Functions and Queue Storage

The upload workflow will enqueue a document-scan job after a file is accepted and stored, and an Azure Function triggered by Queue Storage will perform the malware scan asynchronously.

**Rationale**:
- It prevents the user upload workflow from being blocked by a slow or unavailable virus-scanning service.
- It aligns with Azure-native patterns for decoupled background processing.
- It keeps the main application responsive while still preserving security checks before a document is fully released to other users.

**Implementation pattern**:
- The main app writes a message containing the document id, storage path, and correlation metadata to a queue.
- A queue-triggered Azure Function picks up the message, retrieves the file from storage, performs the scan, and records the final status in the document metadata or audit log.
- A failed or quarantined scan denies access until the file is reviewed or replaced.
- In the local offline training environment, the queue message can be stubbed or mocked to keep the app runnable without Azure resources.

**Alternatives considered**:
- Synchronous scan during upload: rejected because it creates a poor user experience and makes file uploads vulnerable to latency and failure.
- Polling a background service inside the same app: rejected because it is less scalable and does not match the Azure migration path.
- Skipping scan enforcement in the training environment: rejected because the feature requirement specifically requires malware validation before storage.

## Open follow-up requirements

No unresolved clarifications remain in the feature spec. The remaining work is implementation detail and validation during the build phase.
