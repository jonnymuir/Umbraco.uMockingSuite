# Project Context

- **Owner:** Jonny Muir
- **Project:** uMockingSuite — A Umbraco 17 backoffice package that gives snarky, mocking "advice" when content editors create or update content.
- **Stack:** C# / .NET 10, Umbraco 17.3.4, TypeScript, Lit (Umbraco backoffice UI), Umbraco Package Manifests
- **Created:** 2026-04-16

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

📌 Team update (2026-04-16T19-21-57Z): Scaffold complete — package structure (.NET 10.0, Umbraco 17.3.4), notification pipeline (ContentSavingNotification, Scoped DI, IMockingService), test suite (19/19 passing). umbraco-package.json at uMockingSuite/Client/ ready for backoffice UI components. — Boris, Rishi, Gordon

📌 **Claude AI Integration via Umbraco.AI (2026-04-16)**: Integrated `Umbraco.AI.Core` 1.9.0 into `MockingService` to generate AI-powered mocking messages. Key points:
- Added `Umbraco.AI.Core` as package reference with `<PrivateAssets>all</PrivateAssets>` (host provides at runtime)
- Used `IAIChatService.GetChatResponseAsync` with inline builder API (`builder.WithAlias("chat")`)
- `IAIChatService` wraps `Microsoft.Extensions.AI` with Umbraco-specific profiles/middleware
- `ChatResponse.Text` property contains the AI-generated text (not `ChatResponse.Message.Text`)
- Constructor injection with nullable `IAIChatService?` allows graceful fallback if AI not configured
- System prompt instructs Claude to be snarky but not cruel, plain text only (no emoji/hashtags)
- Full exception handling with logging and deterministic fallback to hardcoded messages
- Both sync (`GetMockingMessage(IContent)`) and async (`GetMockingMessageAsync(string, string)`) methods maintained for backwards compatibility

📌 **Settings Service & Profile Selection (2026-04-17)**: Implemented configurable AI profile selection system with backend API and service layer:
- **IUMockingSuiteSettingsService** — Async interface for getting/setting profile alias
- **UMockingSuiteSettingsService** — Implementation using Umbraco's `IKeyValueService` (synchronous, wrapped in Task for async API)
- `IKeyValueService` stores settings in DB with key `"uMockingSuite.ProfileAlias"`, defaults to `"default-chat"` (not `"chat"`)
- **SettingsController** (Management API) — Three endpoints:
  - `GET /umbraco/management/api/v1/umockingsuite/settings` → returns `{profileAlias: string}`
  - `PUT /umbraco/management/api/v1/umockingsuite/settings` → accepts `{profileAlias: string}` body
  - `GET /umbraco/management/api/v1/umockingsuite/profiles` → returns array of profile objects `[{id, alias, name, capability}]` directly (not wrapped)
- **IAIProfileService** from `Umbraco.AI.Core.Profiles` — Use `GetProfilesAsync(AICapability.Chat, ct)` to list chat profiles programmatically
- Updated `MockingService.GetMockingMessageAsync()` to call `_settingsService.GetProfileAliasAsync()` and pass to `builder.WithAlias(profileAlias)`
- All services registered as Scoped in `uMockingSuiteComposer`
- **API Response Patterns**: List endpoints return arrays directly (not `{profiles: [...]}`), detail/settings endpoints return objects
- Theresa's settings UI (`umockingsuite-settings.js`) consumes these endpoints with authenticated fetch using `UMB_AUTH_CONTEXT`

📌 Team update (2026-04-17T00:33:18Z): Settings feature complete — Backend API (profiles, settings endpoints), Frontend UI (dashboard dropdown), Save detection enhancements (requestSubmit wrapping). MockingService now uses user-selected profile instead of hardcoded "chat". Decisions merged, inbox cleared. — Theresa, Tony
