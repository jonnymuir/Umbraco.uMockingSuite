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
