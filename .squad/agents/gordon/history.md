# Project Context

- **Owner:** Jonny Muir
- **Project:** uMockingSuite — A Umbraco 17 backoffice package that gives snarky, mocking "advice" when content editors create or update content.
- **Stack:** C# / .NET 10, Umbraco 17.3.4, TypeScript, Lit (Umbraco backoffice UI), Umbraco Package Manifests
- **Created:** 2026-04-16

## Learnings

### Test Infrastructure (2026-04-16)
- Created `uMockingSuite.Tests` project targeting .NET 10.0 (current SDK available) — confirmed by Boris during package structure phase
- Test stack: xUnit 2.9.3, Moq 4.20.72, FluentAssertions 8.3.0
- Required dependencies: `Umbraco.Cms.Core` 17.0.0, `Microsoft.Extensions.Logging.Abstractions` 10.0.0 (matching Umbraco's version requirement)
- Tests successfully built and all 19 tests passing

### Mocking Umbraco Interfaces
- Used Moq to create test doubles for Umbraco interfaces (`IContent`, `ISimpleContentType`)
- `IContent.ContentType` returns `ISimpleContentType` in Umbraco 17
- `ContentSavingNotification` constructor requires `List<IContent>` and `Umbraco.Cms.Core.Events.EventMessages`
- Umbraco notifications are testable with proper mocking - no need for integration tests at this stage

### Test Coverage Implemented
- **MockingService**: 10 tests covering message generation, null handling, deterministic behavior, various content types, edge cases
- **ContentSavingNotificationHandler**: 9 tests covering service integration, logging, multiple content items, empty notifications
- All tests use Arrange-Act-Assert pattern with clear comments
- Mock verification ensures proper service calls and logging behavior

📌 Team update (2026-04-16T19-21-57Z): Umbraco 17.3.4 target confirmed — .NET 10.0 framework. umbraco-package.json manifest at uMockingSuite/Client/. Ready for backoffice UI components (Theresa) and notification delivery mechanism (Tony). — Rishi
