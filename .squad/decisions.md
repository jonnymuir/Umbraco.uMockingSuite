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

### 2026-04-17: README moved to repo root
**By:** John (DevRel & Release Engineer)  
**Date:** 2026-04-17  
**Status:** Implemented

## Context

The README was located at `uMockingSuite/README.md` (inside the package project folder). GitHub surfaces the README from the repo root by default, so it was not visible on the repository's front page.

## Decision

- Move `README.md` to the repo root.
- Update `uMockingSuite/uMockingSuite.csproj` `<None Include>` path from `README.md` to `..\README.md` so NuGet pack still bundles it correctly.
- Add a "Built with Squad" section to the README documenting the AI team and the UK Prime Ministers casting.

## Rationale

- **GitHub discoverability:** README at root is rendered automatically on the repo homepage.
- **NuGet compatibility:** The `<None Include="..\README.md" PackagePath="\">` pattern is standard for referencing files outside the project directory.
- **Transparency:** Documenting that this was built with an AI Squad team adds interesting context for the community and celebrates the hackathon/Squad experiment.

## Impact

- `README.md` is now at repo root and visible on GitHub.
- `.csproj` updated — no pack regression.
- Committed and pushed to `origin/main` (commit `c2159ca`).

---

### 2026-04-17: GitHub Actions CI Workflow
**By:** Tony (Backend Dev)  
**Date:** 2026-04-17  
**Status:** Implemented

## Context

No CI pipeline existed for the uMockingSuite project. Without automated checks, pushes and PRs could break the build or tests without immediate visibility.

## Decision

Created `.github/workflows/ci.yml` to run on every push to `main` and every pull request targeting `main`.

**Workflow steps:**
1. Checkout code
2. Setup .NET 10.0.x (matches project target framework)
3. `dotnet restore Umbraco.AI.Demo.slnx` — explicit solution reference to avoid ambiguity
4. `dotnet build --no-restore --configuration Release`
5. `dotnet test --no-build --configuration Release --logger "trx;LogFileName=test-results.trx"`
6. Upload `*.trx` test results as artifact (always runs, even on failure)

## Rationale

- Explicit `Umbraco.AI.Demo.slnx` reference on all dotnet commands — solution file is not the default `.sln` name, so explicit reference prevents any ambiguity on the CI runner
- Release configuration used throughout for consistent, optimised output
- xUnit test runner (2.9.3 + xunit.runner.visualstudio 3.1.1) works natively with `dotnet test`
- TRX artifact upload with `if: always()` ensures test results are preserved even when tests fail — useful for diagnosis
- .NET 10.0.x version pin matches `net10.0` TargetFramework in both `uMockingSuite.csproj` and `uMockingSuite.Tests.csproj`

## Files Created

- `.github/workflows/ci.yml`

---

### 2026-04-17: NuGet Publishing & Umbraco Marketplace Setup
**By:** John (DevRel & Release Engineer)  
**Date:** 2026-04-17  
**Status:** Implemented

## Context

The uMockingSuite package was ready to publish but lacked NuGet publishing infrastructure:
- `.csproj` had `YOUR-ORG` placeholder URLs
- No GitHub Actions workflow for automated publishing
- No Umbraco Marketplace metadata

## Decisions

### 1. Repository URLs

Fixed both `RepositoryUrl` and `PackageProjectUrl` to `https://github.com/jonnymuir/Umbraco.uMockingSuite`. Added `RepositoryType = git`.

### 2. NuGet Tags for Marketplace Discoverability

The Umbraco Marketplace discovers packages by the `umbraco-package` tag on NuGet.org. Tag list updated to:

```
umbraco-package;umbraco;umbraco17;umbraco-backoffice;ai;notifications;mocking;umbraco-ai
```

`umbraco-package` is placed first to ensure it is not truncated. `backoffice` replaced with `umbraco-backoffice` (more specific). `hackathon` removed (not a discovery term). `notifications` added (describes functionality).

### 3. umbraco-marketplace.json

Created at repo root. The Umbraco Marketplace reads this file for richer listing info (compatibility versions, bug tracker URL, source URL, etc.) beyond what NuGet metadata provides.

### 4. GitHub Actions Workflow

Trigger: `push` of `v*.*.*` tags. Steps: restore → build → pack → NuGet push (using `NUGET_API_KEY` secret) → GitHub Release. `--skip-duplicate` flag makes the workflow idempotent.

### 5. PackageIcon Placeholder

Added `<PackageIcon>icon.png</PackageIcon>` to the csproj without creating the file. This primes the property for when an icon is designed, without breaking the build.

### 6. Documentation

Created `.github/NUGET_SETUP.md` with instructions for Jonny to:
1. Generate and store the NuGet API key as a GitHub Actions secret
2. Trigger a release via `git tag v0.x.x && git push origin v0.x.x`

---

### 2026-04-17: Pin System.Security.Cryptography.Xml to Patched Version
**By:** Maggie (Security Expert)  
**Date:** 2026-04-17  
**Status:** Implemented

## Context

`Umbraco.Cms 17.3.4` transitively pulls `System.Security.Cryptography.Xml` 8.0.0, which carries two high-severity advisories:

| Advisory | Description | CVSS |
|---|---|---|
| GHSA-37gx-xxp4-5rgx | DoS via infinite loop in `EncryptedXml` | 7.5 High |
| GHSA-w3x6-4m5h-cxqf | High severity vulnerability | High |

Both were reported as `NU1903` build warnings. Since the Umbraco.Cms version is fixed at 17.3.4 and cannot be changed without a larger upgrade, the fix must be a transitive override in the package project itself.

## Decision

Add an explicit `<PackageReference>` for `System.Security.Cryptography.Xml` at version `10.0.6` in `uMockingSuite/uMockingSuite.csproj`. This forces NuGet to resolve the dependency floor upward to the patched release.

**Package pinned:**
- `System.Security.Cryptography.Xml` → `10.0.6` (patched version for .NET 10)

**No `<PrivateAssets>all</PrivateAssets>`** — this is a security floor pin and must flow to downstream consumers of the NuGet package so they also receive the safe version.

## Rationale

1. Cannot bump `Umbraco.Cms` to resolve the transitive dep — version is pinned at 17.3.4 per project requirements.
2. Explicit version pin is the standard .NET supply chain security pattern for overriding bad transitive deps.
3. 10.0.6 is the correct patched version for the net10.0 target framework — advisory confirmed `>=10.0.0, <=10.0.5` is affected.
4. Latest stable release (10.0.6) chosen rather than minimum patched (8.0.3) to remain on the net10.0 release train and benefit from all subsequent fixes.

## Verification

- `dotnet list package --vulnerable --include-transitive` → "no vulnerable packages"
- `dotnet build uMockingSuite/uMockingSuite.csproj -c Release` → 0 errors
- Committed `b733f60`, pushed to main

## Affected Files

- `uMockingSuite/uMockingSuite.csproj` — added explicit `PackageReference` for `System.Security.Cryptography.Xml` 10.0.6

---

### 2026-04-17: Documentation and NuGet Packaging Strategy
**By:** John (DevRel & Release Engineer)  
**Date:** 2026-04-17  
**Status:** Implemented

## Context

uMockingSuite needed professional documentation, GitHub repository hygiene, and proper NuGet packaging before it could be released to the community. The package was built at the Manchester Umbraco AI Hackathon as a demonstration/study vehicle, and that origin story needed to be front-and-center in all public-facing materials.

## Decision

Implemented a complete DevRel and packaging stack:

### Documentation
- **README.md** — Comprehensive package documentation with:
  - Hackathon origin story and "study vehicle" positioning
  - Clear installation and configuration steps
  - Technical "How It Works" section explaining extension points
  - Development/contributing guide
  - "disengage" future vision teaser
- **LICENSE** — MIT license (standard for open-source Umbraco packages)
- **CHANGELOG.md** — Keep a Changelog format, v0.1.0 initial release documented
- **CONTRIBUTING.md** — Welcoming contribution guide with ground rules and local setup

### GitHub Repository Hygiene
- Issue templates for bug reports and feature requests
- Structured to encourage community contributions
- Acknowledges hackathon/learning context in templates

### NuGet Package Metadata
Enhanced `uMockingSuite.csproj` with:
- Detailed description mentioning hackathon origin and Umbraco AI integration
- Comprehensive tags: `umbraco;umbraco-package;umbraco17;ai;backoffice;mocking;hackathon;umbraco-ai`
- `PackageReadmeFile`, `PackageLicenseExpression`, `Copyright`, `RepositoryUrl`, `PackageProjectUrl`
- README.md included in NuGet package root

### App_Plugins Packaging Path (Critical Fix)
Changed from:
```xml
<PackagePath>content\%(RecursiveDir)%(Filename)%(Extension)</PackagePath>
```
To:
```xml
<PackagePath>content\App_Plugins\%(RecursiveDir)%(Filename)%(Extension)</PackagePath>
```

**Why:** Umbraco packages require backoffice assets at `content/App_Plugins/{PackageName}/` in the NuGet package so they deploy correctly to the host site's `wwwroot/App_Plugins/` on install. The `%(RecursiveDir)` MSBuild property only gives us `uMockingSuite/`, not `App_Plugins/uMockingSuite/`. Explicitly prepending `App_Plugins\` in the PackagePath ensures correct structure.

## Rationale

### Hackathon Story Front-and-Center
This package's value is as a learning resource and demonstration, not as a production tool. Every piece of documentation emphasizes this to:
1. Set appropriate expectations (no SLA, welcome to contribute)
2. Encourage forking and experimentation
3. Celebrate the Manchester Umbraco AI Hackathon as community-building

### README Tone
Balanced technical accuracy with personality. The package is called uMockingSuite and generates snarky comments—the documentation can have some warmth and humor while remaining professional.

### "disengage" Teaser
Including the future vision shows this is a prototype with a roadmap, not abandonware. Invites community to be part of what comes next.

### NuGet Packaging Path
This was **critical**. Incorrect packaging would result in backoffice assets not deploying on `dotnet add package`, breaking the package. Validated with `unzip -l *.nupkg` to confirm `content/App_Plugins/uMockingSuite/` structure.

## Validation

Verified via:
1. `dotnet build` (Debug and Release) — succeeded
2. `dotnet pack` — succeeded, package size ~20KB
3. `unzip -l uMockingSuite.0.1.0.nupkg` — confirmed structure:
   - `lib/net10.0/uMockingSuite.dll`
   - `content/App_Plugins/uMockingSuite/umbraco-package.json`
   - `content/App_Plugins/uMockingSuite/umockingsuite-settings.js`
   - `content/App_Plugins/uMockingSuite/umockingsuite-workspace-context.js`
   - `README.md` at root

## What's Ready

- ✅ Package ready for NuGet publishing (pending final repo URL)
- ✅ GitHub repo ready for community contributions
- ✅ Documentation explains what it is, how it works, and how to extend it
- ✅ CHANGELOG ready for versioning workflow
- ✅ Issue templates guide users to provide useful bug reports and feature requests

## Next Steps

1. Finalize GitHub repository URL (currently placeholder `YOUR-ORG`)
2. Update `RepositoryUrl` and `PackageProjectUrl` in `.csproj`
3. Publish v0.1.0 to NuGet
4. Announce on Umbraco community channels with hackathon story

## Files Created/Modified

**Created:**
- `uMockingSuite/README.md` (replaced existing stub)
- `uMockingSuite/LICENSE`
- `CHANGELOG.md`
- `CONTRIBUTING.md`
- `.github/ISSUE_TEMPLATE/bug_report.md`
- `.github/ISSUE_TEMPLATE/feature_request.md`

**Modified:**
- `uMockingSuite/uMockingSuite.csproj` — Enhanced NuGet metadata, fixed App_Plugins packaging path, added README to pack

---

### 2026-04-17: GitHub Repository Publication
**By:** John (DevRel & Release Engineer)  
**Date:** 2026-04-17  
**Status:** Implemented

## Context

uMockingSuite project was ready for public release with documentation, NuGet metadata, GitHub issue templates, and CONTRIBUTING guide. The project needed to be published to a GitHub repository to enable community collaboration and NuGet package distribution.

## Decision

Published the uMockingSuite project to a new public GitHub repository under the project owner's account.

### Repository Details
- **URL:** https://github.com/jonnymuir/Umbraco.uMockingSuite
- **Visibility:** Public
- **Branch:** main
- **Description:** "A Umbraco 17 backoffice package that dishes out mocking, snarky advice when content is created"

## Implementation

### Step 1: .gitignore Enhancement
- **Previous state:** Minimal .gitignore with Squad runtime and App_Plugins exclusions only
- **Issue:** bin/ and obj/ directories from .NET builds were being tracked, bloating the repository
- **Solution:** Added comprehensive .NET gitignore patterns:
  - Build artifacts: `bin/`, `obj/`, `*.dll`, `*.exe`, `*.pdb`
  - IDE/Editor: `.vs/`, `.vscode/`, `*.user`, `*.suo`, `.idea/`
  - NuGet: `.nuget/`, `*.nupkg`, `*.snupkg`, `packages/`
  - Coverage/Tests: `*.coverage`, `*.coveragexml`, `TestResults/`, `*.trx`
  - OS: `.DS_Store`, `Thumbs.db`

### Step 2: Repository Cleanup & Staging
- Executed `git rm -r --cached .` to clear tracking of all files
- Executed `git add .` with updated .gitignore rules to re-stage project files
- Result: 271 files changed; build artifacts properly excluded (32,265 lines deleted)
- Verified `.github/`, `CHANGELOG.md`, `CONTRIBUTING.md` staged correctly
- Removed from tracking: IDE launch configs (`.vscode/`), tracked build artifacts

### Step 3: Initial Commit
- Committed as: "chore: clean up gitignore and commit all project work"
- Included Co-authored-by trailer for Copilot
- Message documents both .gitignore scope and content included

### Step 4: GitHub Repository Creation & Push
- Used `gh repo create` CLI command with:
  - Public visibility flag (`--public`)
  - Source directory (`.`)
  - Remote name (`origin`)
  - Immediate push (`--push`)
- Branch: main
- Remote: `https://github.com/jonnymuir/Umbraco.uMockingSuite.git`

## Rationale

### .gitignore Comprehensiveness
- Prevents future developers from accidentally committing large build artifacts
- Follows industry-standard .NET exclusion patterns
- Reduces repository size and clone times
- Makes git operations responsive

### Public Repository
- Aligns with Umbraco community standards (open-source packages)
- Enables external collaboration and contributions
- Supports NuGet publishing pipeline (GitHub URLs in package metadata)
- Provides source transparency for AI/ML educational purposes (study vehicle)

### Immediate Push
- Establishes GitHub as source-of-truth immediately
- No local-only state—all future work references the public repo
- Enables team members to clone and contribute immediately

## Verification

✅ .gitignore properly updated with .NET patterns  
✅ 271 files committed (source code, docs, Squad config)  
✅ 32,265 deletions (build artifacts removed from tracking)  
✅ Repository created at GitHub  
✅ Initial commit pushed to origin/main  
✅ Remote configured: `origin https://github.com/jonnymuir/Umbraco.uMockingSuite.git`

## Files Committed (Summary)

**New/Modified Files:**
- `.gitignore` — expanded with .NET patterns
- `.github/ISSUE_TEMPLATE/bug_report.md` — new
- `.github/ISSUE_TEMPLATE/feature_request.md` — new
- `CHANGELOG.md` — new
- `CONTRIBUTING.md` — new
- `.squad/agents/john/charter.md` — new
- `.squad/agents/john/history.md` — new
- `uMockingSuite/` — all source files (C#, manifests, JS)
- `uMockingSuite.Tests/` — test suite (source only, no build artifacts)
- `Umbraco.AI.Demo/` — demo site (source only)
- `.squad/` — team config and decisions

**Deleted from Tracking:**
- All `bin/` and `obj/` directories
- `.vscode/launch.json`, `.vscode/tasks.json`
- All transitive dependency DLLs
- IDE-specific files (`*.user`, `*.suo`)

## Next Steps

1. **NuGet Publishing:** When ready, create GitHub Actions workflow to publish `dotnet pack` output to NuGet.org
2. **Repository URL Updates:** Update `uMockingSuite.csproj` placeholder URLs (currently `https://github.com/YOUR-ORG/...`) to final: `https://github.com/jonnymuir/Umbraco.uMockingSuite`
3. **Community Onboarding:** Monitor GitHub Issues and PRs; respond to community questions and contributions
4. **Release Workflow:** Use semantic versioning and CHANGELOG-driven release tags for future versions

---

**Decision Complete:** Project is now publicly available at https://github.com/jonnymuir/Umbraco.uMockingSuite with clean git history, proper .NET build artifact exclusions, and comprehensive documentation.

