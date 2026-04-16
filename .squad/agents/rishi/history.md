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
