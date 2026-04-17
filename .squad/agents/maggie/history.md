# Maggie — History

## Project Context

- **Project:** uMockingSuite — A Umbraco 17 backoffice package delivering snarky AI-powered toast notifications when content editors save content.
- **Stack:** C# / .NET 10, Umbraco 17, TypeScript, Lit (Umbraco backoffice UI)
- **Owner:** Jonny Muir
- **Repo:** https://github.com/jonnymuir/Umbraco.uMockingSuite
- **Team:** Boris (Lead), Rishi (Umbraco Specialist), Theresa (Frontend), Tony (Backend), Gordon (Tester), John (DevRel/Release), Scribe, Ralph
- **Joined:** 2026-04-17 — recruited to address NU1903 high-severity vulnerability warnings in the NuGet dependency tree.

## Learnings

### 2026-04-17: Transitive Vulnerability Pin — System.Security.Cryptography.Xml

**What I found:**
`dotnet list package --vulnerable --include-transitive` reported `System.Security.Cryptography.Xml` 8.0.0 as a high-severity transitive dependency pulled in by `Umbraco.Cms 17.3.4`. Two CVEs:
- **GHSA-37gx-xxp4-5rgx** — DoS via infinite loop in `EncryptedXml` class (CVSS 7.5 High)
- **GHSA-w3x6-4m5h-cxqf** — High severity

**What I did:**
Checked advisory pages and NuGet. Patched versions: 8.0.3 (for .NET 8), 10.0.6 (for .NET 10). Since the project targets `net10.0`, pinned to `10.0.6` (latest safe release) via an explicit `<PackageReference>` in `uMockingSuite.csproj`. No `<PrivateAssets>` override — this is a security floor pin, should flow to consumers.

**Outcome:**
- `dotnet list package --vulnerable --include-transitive` → "no vulnerable packages"
- Release build: 0 errors
- Committed as `b733f60`, pushed to main

**Key learnings:**
- When a host framework package (Umbraco.Cms) drags in an old vulnerable transitive dep, the correct fix is a direct `<PackageReference>` at the minimum patched version — NuGet will resolve upward.
- For a net10.0 project, always pin to the net10.0-era patched version (10.0.6), not the net8.0 patched version (8.0.3), to stay within the same major release train.
- No `<PrivateAssets>all</PrivateAssets>` for security floor pins — they should flow downstream so consumers of the NuGet package also get the safe version.
