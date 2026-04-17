# Contributing to uMockingSuite

👋 Welcome! Thanks for considering contributing to uMockingSuite.

This project was born at the **Manchester Umbraco AI Hackathon** and is intended to be a friendly, welcoming place for developers to learn, experiment, and build together. Whether you're fixing a typo or adding a major feature, your contribution is valued.

## Ground Rules

1. **Be kind.** This is a learning project and a study vehicle for the Umbraco community. Treat others with respect.
2. **Keep it simple.** uMockingSuite intentionally avoids over-engineering. Prefer clarity over cleverness.
3. **Write tests.** If you're adding or changing behavior, update the test suite. We use xUnit, Moq, and FluentAssertions.
4. **Update documentation.** If you change how something works, update the README or inline comments.

## How to Contribute

### Reporting Bugs

Found a bug? Open an issue! Include:

- **What you expected to happen**
- **What actually happened**
- **Steps to reproduce**
- **Umbraco version, .NET version, and AI provider** (Claude, OpenAI, etc.)

### Suggesting Features

Have an idea? Open an issue! Tell us:

- **What problem it solves**
- **How you envision it working**
- **Whether you'd be willing to implement it yourself**

### Submitting Code

1. **Fork the repo** and create a branch for your work
2. **Run the demo site locally** to test your changes:
   ```bash
   cd Umbraco.AI.Demo
   dotnet run
   ```
3. **Write or update tests** as needed:
   ```bash
   cd uMockingSuite.Tests
   dotnet test
   ```
4. **Build the package** to ensure everything compiles:
   ```bash
   cd uMockingSuite
   dotnet build
   ```
5. **Submit a pull request** with a clear description of what you changed and why

## Development Setup

### Prerequisites

- .NET 10 SDK
- A working Umbraco 17 installation (or use the included demo site)
- Umbraco AI package installed and configured

### Running the Demo Site

```bash
cd Umbraco.AI.Demo
dotnet run
```

The demo site has a `ProjectReference` to uMockingSuite, so changes to the package are reflected immediately. An MSBuild target automatically copies `App_Plugins` assets on build.

### Project Structure

- **`uMockingSuite/`** — The package itself (class library)
  - `Composers/` — DI registration
  - `Controllers/` — Management API endpoints
  - `Notifications/` — Content save handlers
  - `Services/` — Mocking service with AI integration
  - `App_Plugins/uMockingSuite/` — Backoffice package manifest and UI
- **`uMockingSuite.Tests/`** — Test suite (xUnit)
- **`Umbraco.AI.Demo/`** — Demo site for local development

### Testing

```bash
cd uMockingSuite.Tests
dotnet test
```

We aim for high test coverage on the C# side. Frontend tests are welcome but not required (yet).

## Code Style

- **C#:** Follow standard .NET conventions. Use `dotnet format` if you have it.
- **JavaScript:** Vanilla ES modules, no build step. Keep it simple.
- **Comments:** Explain *why*, not *what*. The code should be self-documenting where possible.

## Pull Request Process

1. Ensure all tests pass
2. Update the README if you've changed functionality
3. Add a bullet point to `CHANGELOG.md` under `[Unreleased]` describing your change
4. Submit your PR with a clear description
5. Be patient! This is a community project and reviews may take a few days

## Community

This is a hackathon project that celebrates learning and experimentation. If you're stuck, open an issue and ask for help. If you see someone else stuck, offer a hand. Let's build something fun together.

## License

By contributing, you agree that your contributions will be licensed under the MIT License.
