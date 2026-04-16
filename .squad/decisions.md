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
