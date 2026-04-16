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
