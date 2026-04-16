# Project Context

- **Owner:** Jonny Muir
- **Project:** uMockingSuite — A Umbraco 17 backoffice package that gives snarky, mocking "advice" when content editors create or update content.
- **Stack:** C# / .NET 10, Umbraco 17.3.4, TypeScript, Lit (Umbraco backoffice UI), Umbraco Package Manifests
- **Created:** 2026-04-16

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### Package structure setup (2026-04-16)
- Created `uMockingSuite/` class library project targeting `net10.0` with Umbraco.Cms 17.3.4 (PrivateAssets=all)
- Created `uMockingSuite.Tests/` xUnit test project targeting `net10.0`, referencing main package project
- Both projects added to `Umbraco.AI.Demo.slnx`
- Demo host site (`Umbraco.AI.Demo.csproj`) references uMockingSuite via ProjectReference
- Host site uses `net10.0` (not net9.0 as initially assumed) — Umbraco 17.3.4 requires .NET 10
- Host site uses Central Package Management (`Directory.Packages.props`) at project level, NOT repo root. uMockingSuite specifies versions explicitly.
- Package manifest at `uMockingSuite/Client/umbraco-package.json` — Umbraco 17 format
- Folder structure: Composers/, Notifications/, Services/, Client/src/, wwwroot/
- Pre-existing test files existed in uMockingSuite.Tests/ — fixed missing `using Xunit;` and `using Umbraco.Cms.Core.Events;` to get build green
- Updated test project packages to latest compatible versions (xunit 2.9.3, xunit.runner.visualstudio 3.1.1, FluentAssertions 8.3.0, Microsoft.NET.Test.Sdk 17.14.1)

📌 Team update (2026-04-16T19-21-57Z): Test infrastructure complete — 19 tests passing, xUnit + Moq + FluentAssertions. Package ready for integration with backoffice UI. — Gordon
