# Squad Decisions

## Active Decisions

### 2026-04-16: uMockingSuite Package Structure
**By:** Boris (Lead & Architect)  
**Status:** Implemented

uMockingSuite targets .NET 10.0 and Umbraco.Cms 17.3.4 (matching host site). Package manages its own NuGet versions explicitly (no dependency on host CPM). Solution structure uses 3 projects: host site, package class library, test project. Package manifest at `uMockingSuite/Client/umbraco-package.json`. Host site references package via ProjectReference for local development.

**Key Files:**
- `uMockingSuite/uMockingSuite.csproj`
- `uMockingSuite/Client/umbraco-package.json`
- `uMockingSuite.Tests/uMockingSuite.Tests.csproj`
- `Umbraco.AI.Demo.slnx` (updated)

---

### 2026-04-16: uMockingSuite Notification Pipeline Design
**By:** Rishi (Umbraco v17 Specialist)  
**Status:** Implemented

Notification pipeline uses `ContentSavingNotification` (pre-save) to intercept content saves. Services registered as Scoped via Umbraco's `AddNotificationHandler` extension. `IMockingService` abstracts message logic (5 snarky messages, hash-based deterministic selection by content ID). `ContentSavingNotificationHandler` orchestrates notifications and logging. Supports batch saves via SavedEntities iteration.

**Key Components:**
- `IMockingService.cs` / `MockingService.cs`
- `ContentSavingNotificationHandler.cs`
- `uMockingSuiteComposer.cs` (DI registration)

---

### 2026-04-16: Test Approach - xUnit + Moq + FluentAssertions
**By:** Gordon (Tester/QA)  
**Status:** Implemented

Test stack selected for clarity, expressiveness, and Umbraco integration. xUnit chosen for industry standard .NET Core support and extensibility. Moq for lightweight mocking of Umbraco interfaces (IContent, ISimpleContentType). FluentAssertions for natural language assertions and superior failure messages.

**Test Coverage:** 19 tests passing (10 MockingServiceTests + 9 ContentSavingNotificationHandlerTests). All framework versions aligned to .NET 10.0 ecosystem.

**Dependencies:**
- xUnit 2.9.3, Moq 4.20.72, FluentAssertions 8.3.0
- Umbraco.Cms.Core 17.0.0

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
# Decision: uMockingSuite Package Installation and Asset Delivery

**By:** Rishi (Umbraco v17 Specialist)  
**Date:** 2026-04-16  
**Status:** Implemented

## Context

The uMockingSuite package needed to be properly installed and active when the Umbraco.AI.Demo site runs. The package consists of two parts:
1. **Server-side components**: Composer, notification handlers, services (C#)
2. **Client-side components**: Backoffice package manifest and future UI extensions

## Decision

Implemented the Umbraco 17 standard package asset delivery pattern:

### Server-Side (Already Working)
- Project reference from `Umbraco.AI.Demo.csproj` to `uMockingSuite.csproj` enables automatic composer discovery
- `uMockingSuiteComposer : IComposer` is auto-discovered and registered by Umbraco at startup
- No additional configuration needed

### Client-Side (Implemented)
- **Package manifest location**: `uMockingSuite/App_Plugins/uMockingSuite/umbraco-package.json`
- **Build configuration**: Added `<Content Include="App_Plugins\**" CopyToOutputDirectory="Always" />` to `uMockingSuite.csproj`
- **Deployment**: MSBuild copies App_Plugins folder to both package bin and host site bin outputs

## Rationale

This follows Umbraco 17's expected package structure:
1. **App_Plugins convention**: Backoffice packages must place their `umbraco-package.json` under `App_Plugins/{PackageName}/`
2. **Build-time copy**: Using `CopyToOutputDirectory="Always"` ensures assets flow through the ProjectReference chain
3. **No runtime complexity**: No need for embedded resources, custom middleware, or manual file copying
4. **Development-friendly**: Same structure works for both local development (ProjectReference) and NuGet package distribution

## Implementation Changes

### Files Modified
- `uMockingSuite/uMockingSuite.csproj` — added `<ItemGroup>` with `<Content>` directive

### Files Created
- `uMockingSuite/App_Plugins/uMockingSuite/umbraco-package.json` — copied from `Client/umbraco-package.json`

### Verified Outputs
- ✅ Build succeeds with no errors
- ✅ `App_Plugins/uMockingSuite/umbraco-package.json` present in package bin output
- ✅ `App_Plugins/uMockingSuite/umbraco-package.json` present in host site bin output
- ✅ Composer auto-discovery confirmed (via `IComposer` interface and ProjectReference)

## Future Considerations

When Theresa adds TypeScript/Lit UI extensions:
- Place compiled JS bundle at `App_Plugins/uMockingSuite/dist/bundle.js`
- Update `umbraco-package.json` to reference the bundle in the `extensions` array
- The existing `<Content Include="App_Plugins\**">` directive will automatically copy all new client assets

---

### 2026-04-16: Umbraco 17 Toast Notification via workspaceContext
**By:** Theresa (UI/Frontend Expert)  
**Status:** Implemented

Implemented using Umbraco 17's `workspaceContext` extension pattern with `UmbControllerBase` (not LitElement). Observes `UMB_DOCUMENT_WORKSPACE_CONTEXT.isSubmitting` state transitions (`true` → `false`) to detect save completion. Fetch calls Management API with bearer token from `UMB_AUTH_CONTEXT`. Uses `UMB_NOTIFICATION_CONTEXT.peek()` with warning type for display. Manifest field is `api` (not `element`) for workspaceContext extensions. Silent error handling prevents disrupting save workflow.

**Key Files:**
- `uMockingSuite/App_Plugins/uMockingSuite/umockingsuite-workspace-context.js`
- `uMockingSuite/App_Plugins/uMockingSuite/umbraco-package.json` (extended)

---

### 2026-04-16: uMockingSuite Management API Endpoint
**By:** Rishi (Umbraco v17 Specialist)  
**Status:** Implemented (pending Tony's AI service completion)

Implemented Umbraco 17 Management API controller at `/umbraco/management/api/v1/umockingsuite/mocking-message`. Extends `ManagementApiControllerBase`, versioned with `[ApiVersion("1.0")]`, protected with `[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]`. Accepts `contentName` and `contentTypeAlias` query parameters. Delegates to `IMockingService.GetMockingMessageAsync()` for message generation. Returns `{ "message": "..." }` JSON. Switched notification handler from pre-save to post-save (`ContentSavedNotification`) since JS can only call API after save completes.

**Key Files:**
- `uMockingSuite/Controllers/MockingController.cs`
- `uMockingSuite/Notifications/ContentSavedNotificationHandler.cs` (renamed from ContentSavingNotificationHandler)
- `uMockingSuite/Services/IMockingService.cs` (added async method)

---

### 2026-04-16: Claude AI Integration in MockingService
**By:** Tony (Backend Dev)  
**Status:** Implemented

Integrated Claude via `Umbraco.AI.Core.Chat.IAIChatService` inline builder API. Added `Umbraco.AI.Core` 1.9.0 to `uMockingSuite.csproj` with `<PrivateAssets>all</PrivateAssets>` to prevent DLL copy (host provides at runtime). Uses `builder.WithAlias("chat")` to leverage default chat profile. Fallback to deterministic hash-based selection if AI service unavailable or throws exception. System prompt instructs Claude to give short, witty, snarky 1-2 sentence critiques without emoji/hashtags (plain text only).

**Key Files:**
- `uMockingSuite/Services/MockingService.cs` (AI implementation with fallback)
- `uMockingSuite/uMockingSuite.csproj` (added dependency)

---

### 2026-04-17: Authenticated Fetch in uMockingSuite Workspace Context
**By:** Theresa (UI/Frontend Expert)  
**Status:** Implemented

Fixed silent 401 failures in fetch request. Root cause: `MockingController` endpoint requires `[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]` but fetch had no Authorization header. Solution: Import `UMB_AUTH_CONTEXT` from `@umbraco-cms/backoffice/auth`, consume in constructor, call `await this.#authContext?.getLatestToken()` before fetch, and pass `Authorization: Bearer <token>` in headers. Added defensive fallbacks: `getName()` can return null on first observation (fallback to `'this content'`); `getContentType()` may return object or string (handle both); removed hard guards; added `console.debug` logging for DevTools verification.

**Key Files:**
- `uMockingSuite/App_Plugins/uMockingSuite/umockingsuite-workspace-context.js`

---

### 2026-04-17: Save Detection Approach for uMockingSuite
**By:** Theresa (UI/Frontend Expert)  
**Status:** Implemented

In Umbraco 17.3.4, the `isSubmitting` observable on `UmbDocumentWorkspaceContext` emits `undefined` once on load and then never again. The save button does not update this observable. Solution: Use two complementary strategies:

**PRIMARY:** Wrap `requestSubmit` on the context instance
```javascript
const originalRequestSubmit = context.requestSubmit.bind(context);
context.requestSubmit = async (...args) => {
    await originalRequestSubmit(...args);
    this.#handleSaveCompleted();
};
```
The save button calls `requestSubmit()` on the document workspace context. Monkey-patching the instance method allows reliable interception without private API access.

**SECONDARY:** Fixed `isSubmitting` observer (fallback)
- Skip `undefined` emissions (the initial load emission)
- Only fire on a strict `true → false` transition
- Initialize `#wasSubmitting = undefined` (not `false`) so the first real emission doesn't false-trigger

**Additional Details:**
- `#isFetchingMockMessage` guard prevents double-firing if both strategies trigger simultaneously
- `finally` block resets guard so subsequent saves always work
- Prototype chain property enumeration logged on context acquisition for ongoing diagnostics

**Key Files:**
- `uMockingSuite/App_Plugins/uMockingSuite/umockingsuite-workspace-context.js`

---

### 2026-04-17: Settings Dashboard UI Implementation
**By:** Theresa (UI/Frontend Expert)  
**Status:** Implemented

Created a settings dashboard in the Umbraco 17 backoffice where users select which AI profile to use for generating mocking comments.

**Component Architecture:**
- **Element Type:** `LitElement` with `UmbElementMixin` (dashboard elements are UI components)
- **File:** `umockingsuite-settings.js` — vanilla JS module, no build step
- **Custom Element:** `umockingsuite-settings`

**UI Components:**
- Layout: `<uui-box>` for card container
- Form Control: `<uui-select>` with `<uui-select-option>` for profile dropdown
- Actions: `<uui-button>` with `look="primary"` and `color="positive"`
- Feedback: Inline status messages with conditional CSS classes (success/error/loading)

**Data Flow:**
1. **On Load:** Parallel fetch of profiles and current settings using `Promise.all`
2. **Auth Pattern:** Consume `UMB_AUTH_CONTEXT`, call `getLatestToken()`, pass as `Authorization: Bearer` header
3. **On Save:** PUT request to settings endpoint with inline status and notification toast
4. **Error Handling:** HTTP status checks with user-friendly messages

**Manifest Configuration:**
```json
{
  "type": "dashboard",
  "alias": "uMockingSuite.Dashboard.Settings",
  "element": "/App_Plugins/uMockingSuite/umockingsuite-settings.js",
  "elementName": "umockingsuite-settings",
  "meta": {
    "label": "uMockingSuite",
    "pathname": "umockingsuite"
  },
  "conditions": [{
    "alias": "Umb.Condition.SectionAlias",
    "match": "Umb.Section.Settings"
  }]
}
```

**Styling:** Uses Umbraco design tokens (`--uui-*` CSS custom properties), responsive layout (max-width: 480px), semantic color tokens for status messages.

**API Contract:**
- `GET /umbraco/management/api/v1/umockingsuite/profiles` → `[{alias, name, ...}]` (array)
- `GET /umbraco/management/api/v1/umockingsuite/settings` → `{profileAlias: string}`
- `PUT /umbraco/management/api/v1/umockingsuite/settings` → `{profileAlias: string}` (body & response)

**Rationale:**
1. No build step ensures immediate deployment
2. Parallel loading reduces page load time
3. Dual feedback (inline + toast) provides clear UX
4. Design system compliance ensures native Umbraco appearance
5. Settings section placement matches configuration purpose

**Files Modified:**
- Created: `uMockingSuite/App_Plugins/uMockingSuite/umockingsuite-settings.js`
- Modified: `uMockingSuite/App_Plugins/uMockingSuite/umbraco-package.json` (dashboard extension)
- Copied to: `Umbraco.AI.Demo/App_Plugins/uMockingSuite/`

---

### 2026-04-17: Settings Backend - Profile Selection API
**By:** Tony (Backend Dev)  
**Status:** Implemented

The `MockingService` was hardcoded to use `.WithAlias("chat")` but no profile with that alias exists—only "default-chat" is seeded. Users need the ability to choose which AI profile to use from their configured profiles.

**Three-Layer Solution:**

**1. Settings Service Layer**
- **Interface:** `IUMockingSuiteSettingsService`
  - `Task<string> GetProfileAliasAsync()` — Returns profile alias, defaults to "default-chat"
  - `Task SetProfileAliasAsync(string alias)` — Saves profile alias
- **Implementation:** `UMockingSuiteSettingsService`
  - Uses Umbraco's `IKeyValueService` for persistent storage (storage key: `"uMockingSuite.ProfileAlias"`)
  - `IKeyValueService` is synchronous; wrapped with `Task.FromResult()` and `Task.CompletedTask` for async API
  - Default fallback: "default-chat" (matches seed data, not "chat")

**2. Management API Layer**
- **Controller:** `SettingsController` at `/umbraco/management/api/v1/umockingsuite`

Three endpoints:

**GET `/settings`**
- Returns: `{profileAlias: "default-chat"}`
- Used by UI to load current configuration

**PUT `/settings`**
- Body: `{profileAlias: "new-alias"}`
- Returns: `{profileAlias: "new-alias"}`
- Validates that alias is not empty

**GET `/profiles`**
- Returns: `[{id: guid, alias: "default-chat", name: "Default Chat", capability: "Chat"}, ...]`
- Returns array directly (not wrapped in object)
- Filters to `AICapability.Chat` profiles only
- Uses `IAIProfileService.GetProfilesAsync(AICapability.Chat, ct)` from Umbraco.AI.Core

**3. Service Integration**
- Injected `IUMockingSuiteSettingsService` into `MockingService`
- In `GetMockingMessageAsync()`, calls `await _settingsService.GetProfileAliasAsync()`
- Passes result to `builder.WithAlias(profileAlias)` instead of hardcoded "chat"
- Registered as Scoped in `uMockingSuiteComposer`

**Key Discovery: IAIProfileService**
- `GetProfilesAsync(AICapability capability, CancellationToken ct)` — All profiles for capability
- `GetProfileByAliasAsync(string alias, CancellationToken ct)` — Specific profile
- Avoided: Direct SQLite queries, HTTP calls to Umbraco AI API, static/hardcoded lists

**API Response Patterns:**
- **List endpoints** (`/profiles`) → return arrays directly `[{...}, {...}]`
- **Detail/settings endpoints** (`/settings`) → return objects `{profileAlias: "..."}`

**Files Created:**
- `uMockingSuite/Services/IUMockingSuiteSettingsService.cs`
- `uMockingSuite/Services/UMockingSuiteSettingsService.cs`
- `uMockingSuite/Controllers/SettingsController.cs`

**Files Modified:**
- `uMockingSuite/Services/MockingService.cs` — Settings service injection and profile alias lookup
- `uMockingSuite/Composers/uMockingSuiteComposer.cs` — Settings service registration

**Testing:** Build succeeded, App_Plugins copied correctly, profile alias default matches seed data.
