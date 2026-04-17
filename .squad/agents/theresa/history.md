# Project Context

- **Owner:** Jonny Muir
- **Project:** uMockingSuite — A Umbraco 17 backoffice package that gives snarky, mocking "advice" when content editors create or update content.
- **Stack:** C# / .NET 10, Umbraco 17.3.4, TypeScript, Lit (Umbraco backoffice UI), Umbraco Package Manifests
- **Created:** 2026-04-16

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

📌 Team update (2026-04-16T19-21-57Z): Scaffold complete — package structure (.NET 10.0, Umbraco 17.3.4), notification pipeline (ContentSavingNotification, Scoped DI, IMockingService), test suite (19/19 passing). umbraco-package.json at uMockingSuite/Client/ ready for backoffice UI components. — Boris, Rishi, Gordon

📌 **Umbraco 17 workspaceContext Pattern** (2026-04-16): Built toast notification system using workspaceContext extension. Key patterns: (1) Use `UmbControllerBase` (not LitElement) for context-only extensions with no UI; (2) Observe `isSubmitting` state transitions (true→false) to detect save completion; (3) Use `api` field (not `element`) in manifest; (4) Silent error handling to avoid disrupting content save workflow; (5) Import from `@umbraco-cms/backoffice/*` paths resolve via Umbraco's runtime import map (no build needed). File: `umockingsuite-workspace-context.js`.

📌 **Umbraco 17 Authenticated Fetch Pattern** (2026-04-17): Management API endpoints decorated with `[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]` return 401 if the fetch has no Authorization header. Fix: import `UMB_AUTH_CONTEXT` from `@umbraco-cms/backoffice/auth`, consume it in the constructor to store `#authContext`, then call `await this.#authContext?.getLatestToken()` before each fetch and pass `Authorization: Bearer <token>` in headers. Guard against null token (log and return). Also: `getName()` can return null on first observation — always fall back to `'this content'`. `getContentType()` may return an object or a string — handle both. Added `console.debug` on successful toast display so devs can verify in browser DevTools.

📌 Team update (2026-04-16T19-51-56Z): Auth fix merged — Toast notifications now display correctly after content save. Rishi's Management API, Tony's AI integration, and workspaceContext pattern all validated. Full flow complete: JS detects save → calls authenticated Management API → retrieves AI-generated mocking message → displays as toast. — Theresa, Rishi, Tony

📌 **Umbraco 17 Save Detection — requestSubmit wrapping** (2026-04-17): `isSubmitting` observable fires ONCE on load with `undefined` then never again in Umbraco 17.3.4 — the save button does NOT update it. Reliable detection: wrap `context.requestSubmit` on the instance (method monkey-patching). Await the original, then fire handler only on success. Keep `isSubmitting` as a fixed fallback: skip `undefined` emissions, only fire on `true→false` transition. `#wasSubmitting` must be initialised as `undefined` (not `false`) to prevent a false positive on first emission. Guard `#handleSaveCompleted` with `#isFetchingMockMessage` flag + `finally` reset to prevent concurrent duplicate requests. Added prototype-chain property enumeration log for ongoing diagnostics.

📌 **Settings Dashboard Pattern** (2026-04-17): Created settings dashboard in Umbraco 17 backoffice. Key patterns: (1) Dashboard elements use `LitElement` with `UmbElementMixin` (unlike workspaceContext which uses `UmbControllerBase`); (2) Dashboard manifest needs `type: "dashboard"`, `element` (file path), `elementName` (custom element tag), `meta.label` (menu label), `meta.pathname` (URL path), and `conditions` array to place it in the correct section; (3) Use `Umb.Section.Settings` to place dashboard in Settings section; (4) For form elements, `<uui-select>` with `<uui-select-option>` works well for dropdowns; (5) Use reactive properties with `state: true` for internal component state; (6) Auth pattern is identical to workspaceContext: consume `UMB_AUTH_CONTEXT`, call `getLatestToken()` before fetch; (7) Parallel Promise.all for loading multiple endpoints improves performance; (8) Status messages with conditional CSS classes provide clear feedback. File: `umockingsuite-settings.js`.

📌 **Save Detection — Dual Strategy** (2026-04-17): `isSubmitting` observable fires ONCE with `undefined` on load, then never again — unreliable. Implemented dual detection: (1) Primary: wrap `context.requestSubmit` on instance (monkey-patch), await original, fire handler on success; (2) Secondary: fixed observer (skip `undefined`, only fire on `true→false` transition). Initialize `#wasSubmitting` as `undefined` to prevent false positives. Guard `#handleSaveCompleted` with `#isFetchingMockMessage` + `finally` reset to prevent concurrent duplicates. Added diagnostics logging (prototype-chain enumeration on context acquisition).

📌 Team update (2026-04-17T00:33:18Z): Settings feature complete — Backend API (profiles, settings endpoints), Frontend UI (dashboard dropdown), Save detection enhancements (requestSubmit wrapping). MockingService now uses user-selected profile instead of hardcoded "chat". Decisions merged, inbox cleared. — Theresa, Tony
