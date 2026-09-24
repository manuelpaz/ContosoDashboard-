<!--
Sync Impact Report
- Version change: template scaffold -> 1.0.0
- Modified principles:
  - [PRINCIPLE_1_NAME] -> I. Training-First Security and Responsibility
  - [PRINCIPLE_2_NAME] -> II. User-Centered Access and Role Integrity
  - [PRINCIPLE_3_NAME] -> III. Verification Before Merge
  - [PRINCIPLE_4_NAME] -> IV. Architectural Clarity and Isolation
  - [PRINCIPLE_5_NAME] -> V. Offline-First Change Safety
- Added sections: Core Principles, Additional Constraints, Development Workflow, Governance
- Removed sections: None; the previous file was a template with placeholder values
- Follow-up TODOs: none
-->

# ContosoDashboard Constitution

## Core Principles

### I. Training-First Security and Responsibility
This project exists for educational use and must remain explicit about its training-only scope. All features, sample data, and security examples MUST be understandable as instructional patterns rather than production-grade controls. The repository MUST not imply that mock authentication, local-only storage, or demo data are ready for production deployment without a clear migration plan and separate review.

### II. User-Centered Access and Role Integrity
Access decisions MUST be driven by authenticated identity, role, and ownership checks. Every page and service that exposes project, task, or user data MUST enforce authorization before returning or mutating data. Cross-user access, insecure direct object references, and role bypasses are prohibited, even in example workflows, because they undermine the training objective and the application’s security model.

### III. Verification Before Merge
Every behavior change MUST be validated with a concrete check before completion: automated tests where feasible, targeted manual validation for UI or auth paths, and explicit evidence that the changed behavior matches the requirement. No feature or bug fix may be merged with unverified assumptions, and the final implementation must be traceable to a tested scenario.

### IV. Architectural Clarity and Isolation
The codebase MUST maintain clear boundaries between data access, domain models, business services, and user-facing pages. Infrastructure dependencies such as authentication and storage MUST be abstracted so local training behavior and future cloud migration can be swapped without rewriting core business logic. Simpler, well-scoped abstractions are preferred over hidden coupling.

### V. Offline-First Change Safety
This application MUST remain runnable in a local, offline training environment without external cloud dependencies. Changes must preserve that operating model, document any required setup, and avoid accidental coupling to non-local services. Breaking changes require a migration path, clear documentation, and review of the impact on the project’s learning goals and usability.

## Additional Constraints

The following constraints are mandatory for all work in this repository:

- The project is intended for training and demonstration only; any production-ready assumptions must be explicitly documented and separated from the default local implementation.
- Mock authentication and authorization are permitted only for the educational scenario defined in the project documentation; they MUST not be treated as a production identity provider.
- Sensitive or user-scoped data MUST be isolated by identity, role, and project membership checks before access is granted.
- Local-first configuration, sample data, and file handling MUST remain compatible with offline and workshop-based execution.
- New features MUST not add hidden dependencies that prevent the app from running in the intended training environment.
- Security, privacy, and access-control changes MUST be reviewed for both correctness and clarity so the learning experience remains understandable to students.

## Development Workflow

All work on ContosoDashboard MUST follow this process:

- Start from a clear requirement or issue description and identify affected pages, services, and data flows before implementation.
- Prefer small, reviewable changes that preserve the existing architecture and keep the application easy to explain in a training setting.
- Validate the changed behavior with the smallest relevant test or manual scenario before completion.
- Document user-visible behavior changes, migration notes, and security implications when they affect the training experience.
- Review for compliance with the constitution before merging: correctness, security, architecture, and clarity of intent must all be evaluated.

## Governance

This constitution supersedes informal project practices and acts as the baseline for technical decisions in the repository. Any change in scope, architecture, security model, or workflow MUST be evaluated against these principles before implementation is approved.

Amendments require a documented rationale, a version bump, and a review that confirms the change preserves the project’s training intent and operating constraints. Compliance with the constitution is expected in all planning, implementation, and review activities.

**Version**: 1.0.0 | **Ratified**: 2026-09-24 | **Last Amended**: 2026-09-24
