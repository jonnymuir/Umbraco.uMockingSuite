# Project Context

- **Owner:** Jonny Muir
- **Project:** uMockingSuite — A Umbraco 17 backoffice package that gives snarky, mocking "advice" when content editors create or update content.
- **Stack:** C# / .NET 10, Umbraco 17.3.4, TypeScript, Lit (Umbraco backoffice UI), Umbraco Package Manifests
- **Created:** 2026-04-16

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-04-16: Umbraco 17.3.4 Notification Pipeline Setup
- **Umbraco Version:** 17.3.4 (.NET 10.0 runtime) — confirmed by Boris during package structure phase
- **Notification API:** `ContentSavingNotification` from `Umbraco.Cms.Core.Notifications` - fires BEFORE content save, allowing pre-save logic
- **Composer Pattern:** `IComposer` interface with `Compose(IUmbracoBuilder builder)` method - no special ordering attributes needed for basic registration
- **DI Registration:** 
  - Services: `builder.Services.AddScoped<TInterface, TImplementation>()`
  - Notification handlers: `builder.AddNotificationHandler<TNotification, THandler>()`
- **Notification Handler:** Implement `INotificationHandler<TNotification>` from `Umbraco.Cms.Core.Events`
- **Key Namespaces:**
  - `Umbraco.Cms.Core.Composing` - Composer interfaces
  - `Umbraco.Cms.Core.DependencyInjection` - IUmbracoBuilder extension methods
  - `Umbraco.Cms.Core.Notifications` - Notification types
  - `Umbraco.Cms.Core.Events` - INotificationHandler interface
  - `Umbraco.Cms.Core.Models` - IContent and other content models

📌 Team update (2026-04-16T19-21-57Z): Package structure ready — .NET 10.0, Umbraco 17.3.4, local version management. Notification pipeline scaffolded. 19 tests passing. — Boris

### 2026-04-16: Umbraco 17 Management API Controller Pattern
- **Base Class:** `ManagementApiControllerBase` from `Umbraco.Cms.Api.Management.Controllers` namespace
- **Route Pattern:** Base sets `/umbraco/management/api/v{version:apiVersion}/`, class-level `[Route]` adds suffix
- **Required Attributes:**
  - `[ApiVersion("1.0")]` from `Asp.Versioning` namespace (transitively available from Umbraco.Cms)
  - `[ApiExplorerSettings(GroupName = "PackageName")]` for Swagger grouping
  - `[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]` from `Umbraco.Cms.Web.Common.Authorization` for backoffice auth
- **Controller Discovery:** Controllers in package assemblies are auto-discovered when host references the package - no manual registration needed
- **Full Route Example:** `[Route("umockingsuite/mocking-message")]` → `/umbraco/management/api/v1/umockingsuite/mocking-message`
- **Async Pattern:** Controllers can inject services and call `async Task<IActionResult>` methods, returning `Ok(new { property = value })`
- **Package Structure Change:** Switched from `ContentSavingNotification` (pre-save) to `ContentSavedNotification` (post-save) to align with new architecture where JS calls API after save completes

### 2026-04-16: Umbraco 17 Package Asset Delivery Pattern
- **App_Plugins Standard:** In Umbraco 17, backoffice packages must deliver `umbraco-package.json` via `App_Plugins/{PackageName}/` directory
- **Project Structure:** 
  - Created `uMockingSuite/App_Plugins/uMockingSuite/umbraco-package.json` (copied from Client folder)
  - Added `<Content Include="App_Plugins\**" CopyToOutputDirectory="Always" />` to `.csproj`
- **Deployment Flow:** Build process copies App_Plugins to both package bin output AND host site bin output via ProjectReference
- **Composer Discovery:** `IComposer` implementations are auto-discovered from referenced assemblies - no additional configuration needed
- **Verification:** Build successful, App_Plugins folder confirmed in both `/uMockingSuite/bin/Debug/net10.0/App_Plugins/uMockingSuite/` and `/Umbraco.AI.Demo/bin/Debug/net10.0/App_Plugins/uMockingSuite/`
- **Notes:** The `Client/umbraco-package.json` remains as source, but runtime discovery happens from `App_Plugins/` location
