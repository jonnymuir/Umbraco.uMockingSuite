# Changelog

All notable changes to uMockingSuite will be documented here.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.0] — 2026-04-17

### Added
- **Initial release** — built at the Manchester Umbraco AI Hackathon
- AI-powered mocking toast notifications on content save in Umbraco 17 backoffice
- Settings dashboard in the Umbraco backoffice Settings section to choose AI profile
- Management API endpoints:
  - `GET /umbraco/management/api/v1/umockingsuite/mocking-message` — Generate mocking comment
  - `GET /umbraco/management/api/v1/umockingsuite/profiles` — List available AI profiles
  - `GET /umbraco/management/api/v1/umockingsuite/settings` — Get current settings
  - `PUT /umbraco/management/api/v1/umockingsuite/settings` — Save profile selection
- Support for all Umbraco AI providers (Claude, OpenAI, Google, etc.)
- Graceful fallback to deterministic snarky messages when AI service unavailable
- workspaceContext extension that intercepts document save events
- Dashboard UI using Lit and Umbraco UI Library components
- Comprehensive test suite (19 passing tests with xUnit, Moq, FluentAssertions)

### Technical Details
- Targets .NET 10.0 and Umbraco 17.3.4
- Depends on Umbraco.AI.Core 1.9.0
- Package manifest at `App_Plugins/uMockingSuite/umbraco-package.json`
- Settings persistence via `IKeyValueService`
- Authenticated Management API access with bearer token from `UMB_AUTH_CONTEXT`

[0.1.0]: https://github.com/YOUR-ORG/uMockingSuite/releases/tag/v0.1.0
