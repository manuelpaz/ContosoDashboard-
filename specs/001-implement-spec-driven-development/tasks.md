# Tasks: Document Upload and Management

**Input**: Design documents from `/specs/001-implement-spec-driven-development/`
**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `quickstart.md`

## Format: `[ID] [P?] [Story] Description`

- **[P]**: independent work that can run in parallel
- **[Story]**: `US1`, `US2`, `US3`, or `US4`
- Include exact file paths in each task description

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare the document feature structure and configuration before implementing the file workflow.

- [X] T001 Create secure document feature folders and storage conventions under `ContosoDashboard/Models/`, `ContosoDashboard/Services/`, and `ContosoDashboard/Pages/`
- [X] T002 Add the initial document-related DbContext configuration and seed hooks in `ContosoDashboard/Data/ApplicationDbContext.cs`
- [X] T003 [P] Add upload/storage settings and Azure queue placeholders in `ContosoDashboard/appsettings.json` and `ContosoDashboard/Program.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Build the storage, validation, and access foundation before any document story can be delivered.

- [X] T004 Implement `ContosoDashboard/Models/Document.cs`, `ContosoDashboard/Models/DocumentShare.cs`, and `ContosoDashboard/Models/DocumentScanJob.cs` with the validation constraints from `data-model.md`
- [X] T005 [P] Implement `ContosoDashboard/Services/IFileStorageService.cs` and `ContosoDashboard/Services/LocalFileStorageService.cs` for secure local file persistence outside `wwwroot`
- [X] T006 Implement `ContosoDashboard/Services/DocumentService.cs` with upload validation, metadata persistence, versioning, and access filtering logic
- [X] T007 [P] Add queue-message and scan-status workflow hooks in `ContosoDashboard/Services/DocumentService.cs` and `ContosoDashboard/Data/ApplicationDbContext.cs` for `Queued`, `Scanning`, `Clean`, `Infected`, and `Failed` states

**Checkpoint**: The foundation is ready for story implementation.

---

## Phase 3: User Story 1 - Upload and organize work documents (Priority: P1) 🎯 MVP

**Goal**: Users can upload valid documents, keep them organized by project or personal context, and see them in the correct list.

**Independent Test**: A signed-in user can upload a valid PDF or Office file with a title and category, and the file appears in the correct list without bypassing validation.

### Implementation for User Story 1

- [X] T008 [US1] Add upload validation for title, category, supported MIME types, and 25 MB size limit in `ContosoDashboard/Services/DocumentService.cs`
- [X] T009 [P] [US1] Implement secure file persistence and unique server-side naming in `ContosoDashboard/Services/LocalFileStorageService.cs`
- [X] T010 [US1] Persist uploaded document metadata, project association, and uploader details in `ContosoDashboard/Data/ApplicationDbContext.cs`
- [X] T011 [P] [US1] Create the upload form and document page in `ContosoDashboard/Pages/Documents.razor` and link it from `ContosoDashboard/Shared/NavMenu.razor`

**Checkpoint**: User Story 1 should be independently functional and testable.

---

## Phase 4: User Story 2 - Find and review documents when needed (Priority: P1)

**Goal**: Users can locate only the documents they are authorized to view and filter by project, category, date, and file size.

**Independent Test**: A user can search and filter the document list and only sees relevant, authorized records.

### Implementation for User Story 2

- [ ] T012 [P] [US2] Add search, filter, and sorting query logic in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T013 [US2] Implement access-aware document listing and result rendering in `ContosoDashboard/Pages/Documents.razor`
- [ ] T014 [P] [US2] Surface document results in project and personal views in `ContosoDashboard/Pages/Projects.razor` and `ContosoDashboard/Pages/ProjectDetails.razor`

**Checkpoint**: User Stories 1 and 2 work independently for supported project and personal scenarios.

---

## Phase 5: User Story 3 - Share and manage access to documents (Priority: P2)

**Goal**: Owners and project managers can share documents with authorized users while denying access outside the allowed project scope.

**Independent Test**: A project member can access a shared document and a user outside the project cannot access it.

### Implementation for User Story 3

- [ ] T015 [P] [US3] Add `DocumentShare` entitlement and same-project access checks in `ContosoDashboard/Models/DocumentShare.cs` and `ContosoDashboard/Services/DocumentService.cs`
- [ ] T016 [US3] Implement share, revoke, and notification triggers in `ContosoDashboard/Services/DocumentService.cs` and `ContosoDashboard/Services/NotificationService.cs`
- [ ] T017 [P] [US3] Add unauthorized access denial and audit record writes in `ContosoDashboard/Services/DocumentService.cs` and `ContosoDashboard/Data/ApplicationDbContext.cs`
- [ ] T018 [US3] Build the share UI, recipient selection, and access summary in `ContosoDashboard/Pages/Documents.razor`

**Checkpoint**: Sharing and access enforcement work independently with the project membership model.

---

## Phase 6: User Story 4 - Stay informed through dashboard and task integration (Priority: P3)

**Goal**: Recent document activity and related attachments appear in the dashboard and within the task/project context.

**Independent Test**: Project members can see new documents in dashboard and task-related views without exposing unrelated records.

### Implementation for User Story 4

- [ ] T019 [P] [US4] Add recent-document queries and dashboard retrieval logic in `ContosoDashboard/Services/DashboardService.cs`
- [ ] T020 [US4] Render recent document activity in `ContosoDashboard/Pages/Index.razor` and `ContosoDashboard/Pages/ProjectDetails.razor`
- [ ] T021 [P] [US4] Wire task attachments and document references in `ContosoDashboard/Services/TaskService.cs` and `ContosoDashboard/Pages/Tasks.razor`

**Checkpoint**: The document feature integrates with dashboard and task views without breaking core upload behavior.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Final security review, cleanup, and validation pass across all stories.

- [ ] T022 [P] Review and harden antivirus queue gating and scan-status enforcement in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T023 [P] Add delete confirmation, orphan cleanup, and audit logging in `ContosoDashboard/Services/DocumentService.cs` and `ContosoDashboard/Pages/Documents.razor`
- [ ] T024 Run the validation pass from `specs/001-implement-spec-driven-development/quickstart.md` and fix regressions before merge

---

## Dependencies & Execution Order

### Phase dependencies

- **Setup**: No dependencies; starts first.
- **Foundational**: Depends on Setup completion and blocks all user story work.
- **User Story phases**: Each story depends on the Foundational phase and can be implemented independently once the base service is ready.
- **Polish**: Depends on all desired stories being completed and validated.

### User story dependency order

1. **User Story 1 (P1)**: Upload and organize documents
2. **User Story 2 (P1)**: Search, filter, and review documents
3. **User Story 3 (P2)**: Share and manage access
4. **User Story 4 (P3)**: Dashboard and task integration

### Parallel opportunities

- `T003`, `T005`, `T007`, `T009`, `T011`, `T012`, `T014`, `T015`, `T017`, `T019`, `T021`, `T022`, and `T023` are marked `[P]` and can be worked in parallel once dependent prerequisites exist.
- The four story phases can also be staffed in parallel by separate team members after the foundation is complete.

---

## Parallel Example: Story Delivery

```text
Developer A: T008-T011 (US1)
Developer B: T012-T014 (US2)
Developer C: T015-T018 (US3)
Developer D: T019-T021 (US4)
```

---

## Implementation Strategy

### MVP First

1. Complete Phase 1 and Phase 2.
2. Complete User Story 1.
3. Validate upload and project/persona list behavior with the quickstart steps.
4. Extend to User Story 2 and then the sharing and dashboard tasks.

### Incremental delivery

1. Deliver secure upload and metadata storage.
2. Add search and classification visibility.
3. Add same-project share enforcement and notifications.
4. Finish dashboard and task integration.
5. Run the final polish and security review.

---

## Notes

- All tasks include exact file paths to the current repository structure.
- The document feature remains aligned with the offline-first training app model while using an Azure-ready queue-scanning design for future virus scanning.
- Validation should happen with real user flows from `quickstart.md` rather than mock-only assertions.
