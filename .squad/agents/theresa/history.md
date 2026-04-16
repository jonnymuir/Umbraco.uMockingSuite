# Project Context

- **Owner:** Jonny Muir
- **Project:** uMockingSuite — A Umbraco 17 backoffice package that gives snarky, mocking "advice" when content editors create or update content.
- **Stack:** C# / .NET 10, Umbraco 17.3.4, TypeScript, Lit (Umbraco backoffice UI), Umbraco Package Manifests
- **Created:** 2026-04-16

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

📌 Team update (2026-04-16T19-21-57Z): Scaffold complete — package structure (.NET 10.0, Umbraco 17.3.4), notification pipeline (ContentSavingNotification, Scoped DI, IMockingService), test suite (19/19 passing). umbraco-package.json at uMockingSuite/Client/ ready for backoffice UI components. — Boris, Rishi, Gordon

📌 **Umbraco 17 workspaceContext Pattern** (2026-04-16): Built toast notification system using workspaceContext extension. Key patterns: (1) Use `UmbControllerBase` (not LitElement) for context-only extensions with no UI; (2) Observe `isSubmitting` state transitions (true→false) to detect save completion; (3) Use `api` field (not `element`) in manifest; (4) Silent error handling to avoid disrupting content save workflow; (5) Import from `@umbraco-cms/backoffice/*` paths resolve via Umbraco's runtime import map (no build needed). File: `umockingsuite-workspace-context.js`.
