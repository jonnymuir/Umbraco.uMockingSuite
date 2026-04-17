# John — History & Learnings

## Project Context

- **Project:** uMockingSuite — A Umbraco 17 backoffice package. When a content editor creates or updates content in the Umbraco backoffice, the package intercepts the event and displays a witty, mocking notification powered by AI.
- **Stack:** C# / .NET 9, Umbraco 17, TypeScript, Lit (Umbraco backoffice UI), Umbraco Package Manifests
- **Requested by:** Jonny Muir
- **Origin:** Built at the Manchester Umbraco AI Hackathon as a demonstration of how easy it is to get going with the Umbraco AI package. Intended to be a great vehicle for studying how to add AI features to your own Umbraco work. Expected to evolve into a future package called "disengage".

## Team

- Boris — Lead & Architect
- Rishi — Umbraco v17 Specialist
- Theresa — UI/Frontend Expert
- Tony — Backend Dev
- Gordon — Tester/QA
- John — DevRel & Release Engineer (me)

## Key File Paths

- Package source: `uMockingSuite/uMockingSuite.csproj`
- App_Plugins (source of truth): `uMockingSuite/App_Plugins/uMockingSuite/`
- App_Plugins (deployed by MSBuild): `Umbraco.AI.Demo/App_Plugins/uMockingSuite/`
- Package manifest: `uMockingSuite/App_Plugins/uMockingSuite/umbraco-package.json`
- Test project: `uMockingSuite.Tests/`
- Demo site: `Umbraco.AI.Demo/`

## Learnings

### 2026-04-17: GitHub Publication & Repository Setup

**Context:** Jonny requested full project publication to a new GitHub repository with proper .gitignore configuration to prevent build artifacts from being tracked.

**What I Did:**

1. **Enhanced .gitignore** with comprehensive .NET patterns:
   - Build outputs: `bin/`, `obj/`, `*.dll`, `*.exe`, `*.pdb`
   - IDE/tooling: `.vs/`, `.vscode/`, `*.user`, `*.suo`, `.idea/`
   - NuGet artifacts: `.nuget/`, `*.nupkg`, `*.snupkg`, `packages/`
   - Coverage and test results: `*.coverage`, `TestResults/`, `*.trx`
   - OS files: `.DS_Store`, `Thumbs.db`

2. **Repository cleanup:**
   - Ran `git rm -r --cached .` to clear all tracked files
   - Re-added with `git add .` — new .gitignore rules excluded 32,265 lines of build artifacts
   - Committed 271 meaningful files (source, docs, Squad config, GitHub templates)
   - Removed IDE launch configs and build outputs from git tracking

3. **GitHub repository creation:**
   - Created public repo via `gh repo create Umbraco.uMockingSuite --public --source=. --push`
   - Final URL: https://github.com/jonnymuir/Umbraco.uMockingSuite
   - Branch: main, remote: origin
   - Description: "A Umbraco 17 backoffice package that dishes out mocking, snarky advice when content is created"

**Key Decisions:**

- **Standard .NET patterns:** Used industry-standard exclusions to prevent future git bloat from CI/CD builds
- **Immediate push:** Published to GitHub immediately (no local-only state) to establish GitHub as source-of-truth
- **Clean commit message:** Documented both .gitignore scope and content committed for future reference
- **Public visibility:** Aligns with Umbraco community standards and supports open-source NuGet distribution

**Verification Results:**
- ✅ .gitignore properly configured (32K+ lines of artifacts excluded)
- ✅ 271 files successfully committed
- ✅ Initial commit pushed to origin/main
- ✅ Remote URL verified: `https://github.com/jonnymuir/Umbraco.uMockingSuite.git`
- ✅ Repository is public and ready for community collaboration

**Impact:**
- Project now publicly available for study, contribution, and NuGet distribution
- Repository hygiene ensures future clones are fast (no build artifacts)
- GitHub issue/PR templates already in place for community engagement
- NuGet package metadata is complete and points to correct GitHub URL

**Notes for Future:**
- Update `uMockingSuite.csproj` placeholder URLs (currently `YOUR-ORG` placeholder) to finalized GitHub URL
- Consider adding GitHub Actions CI/CD workflow for future NuGet publishing
- Repository is ready for tagging and release versioning workflow

---

### 2026-04-17: Documentation & NuGet Packaging Complete

**Context:** Jonny asked me to make uMockingSuite presentable and publishable—this is my core charter as DevRel & Release Engineer.

**What I Did:**

1. **Created comprehensive README.md** for the uMockingSuite package with:
   - Hackathon origin story front-and-center
   - Clear technical requirements (Umbraco 17, .NET 10, Umbraco AI)
   - Installation via NuGet
   - Configuration steps (install Umbraco AI, configure profile in Settings dashboard)
   - "How It Works" section explaining key extension points (workspaceContext, Management API, AI integration, dashboard UI)
   - Development/contributing guide with local setup instructions
   - Warm hackathon context section celebrating Manchester Umbraco AI Hackathon
   - "What's Next — disengage" teaser section for future evolution
   - MIT license with personality

2. **Added LICENSE file** — MIT license, copyright 2026 Jonny Muir

3. **Created CHANGELOG.md** at repo root:
   - Follows Keep a Changelog format
   - v0.1.0 initial release entry with full feature list
   - Technical details section (targets, dependencies, key files)

4. **Updated uMockingSuite.csproj NuGet metadata**:
   - Expanded `<Description>` mentioning hackathon origin and study-vehicle purpose
   - Enhanced `<PackageTags>` with umbraco17, ai, hackathon, umbraco-ai
   - Added `<PackageReadmeFile>`, `<PackageLicenseExpression>`, `<Copyright>`, `<RepositoryUrl>`, `<PackageProjectUrl>`
   - Fixed App_Plugins packaging path: `content\App_Plugins\%(RecursiveDir)` ensures files land at `content/App_Plugins/uMockingSuite/` in NuGet package
   - Added `<None Include="README.md" Pack="true" PackagePath="\" />` so README ships with package

5. **Added GitHub repo hygiene files**:
   - `.github/ISSUE_TEMPLATE/bug_report.md` — Standard bug report template with environment fields (Umbraco version, .NET version, AI provider)
   - `.github/ISSUE_TEMPLATE/feature_request.md` — Feature request template encouraging contributions, noting hackathon/study-vehicle nature
   - `CONTRIBUTING.md` — Welcoming contribution guide with ground rules (be kind, keep it simple, write tests), local development setup, code style guidelines, PR process

6. **Validated NuGet packaging**:
   - Built in Release mode
   - Ran `dotnet pack` with `--no-build -c Release`
   - Inspected package contents with `unzip -l`
   - Confirmed correct structure:
     - `lib/net10.0/uMockingSuite.dll` — compiled library
     - `content/App_Plugins/uMockingSuite/` — backoffice assets (umbraco-package.json, JS files)
     - `README.md` at root
   - Package size: ~20KB

**Key Decisions:**

- **README tone:** Balanced technical accuracy with personality. The package is called uMockingSuite—the docs can have a bit of snark while staying professional.
- **App_Plugins packaging path:** Critical for Umbraco packages. Must be `content/App_Plugins/{PackageName}/` so files deploy correctly on NuGet install. Fixed with `<PackagePath>content\App_Plugins\%(RecursiveDir)%(Filename)%(Extension)</PackagePath>`.
- **Repository URLs:** Left as `https://github.com/YOUR-ORG/uMockingSuite` placeholder since we don't know final repo location. Easy to update later.
- **Hackathon origin story:** Made this prominent in README, CHANGELOG, and CONTRIBUTING. This is a study vehicle—that's the whole point and should be celebrated, not hidden.
- **"disengage" teaser:** Included future vision section to show this is a prototype with a roadmap, not abandonware.

**Validation Results:**
- ✅ Build succeeds (Debug and Release)
- ✅ NuGet pack succeeds
- ✅ README.md included in package
- ✅ App_Plugins at correct path (`content/App_Plugins/uMockingSuite/`)
- ✅ All metadata fields populated
- ✅ GitHub issue templates functional
- ✅ CONTRIBUTING.md guides new contributors

**What's Ready:**
- Package is ready for NuGet publishing (once repo URL is finalized)
- GitHub repo is ready for community contributions
- Documentation explains what it is, how it works, and how to extend it
- CHANGELOG ready for versioning workflow

**Notes:**
- The System.Security.Cryptography.Xml warnings come from Umbraco.Cms transitive dependencies—not our problem to fix at package level
- The package intentionally has no build step for client-side code—vanilla JS is a feature for study-vehicle purposes
- Test coverage (19 tests) mentioned in README to show this isn't just a throwaway prototype

---

### 2026-04-17: NuGet Publishing & Umbraco Marketplace Setup

**Context:** Jonny requested full NuGet publishing setup — fix placeholder URLs, add Marketplace metadata, GitHub Actions workflow, and publishing instructions.

**What I Did:**

1. **Fixed .csproj metadata:**
   - Replaced `YOUR-ORG` placeholder in `RepositoryUrl` and `PackageProjectUrl` with `https://github.com/jonnymuir/Umbraco.uMockingSuite`
   - Added `RepositoryType` = `git`
   - Added `PackageIcon` = `icon.png` (ready for icon file when available)
   - Added `GenerateDocumentationFile` = `true`
   - Added `PackageReleaseNotes` pointing to CHANGELOG on GitHub
   - Improved tags: replaced `backoffice;hackathon` with `umbraco-backoffice;notifications` and reordered to lead with `umbraco-package` (critical for Marketplace indexing)

2. **Created `umbraco-marketplace.json`** at repo root:
   - Schema-validated against `marketplace.umbraco.com`
   - Declares `nugetPackageId`, `minUmbracoVersion: 17.0.0`, source/bug/readme/license URLs
   - Enables richer Umbraco Marketplace listing beyond NuGet tag discovery

3. **Created `.github/workflows/publish-nuget.yml`:**
   - Triggers on `v*.*.*` tag push
   - Build → Pack → Push to NuGet.org using `NUGET_API_KEY` secret
   - Creates GitHub Release with auto-generated notes and `.nupkg` attached
   - Uses `--skip-duplicate` to be idempotent on re-runs

4. **Created `.github/NUGET_SETUP.md`:**
   - Step-by-step: get NuGet API key, add GitHub secret, trigger release via `git tag`
   - Notes on semantic versioning and where to update the `<Version>` property

**Key Decisions:**

- **`umbraco-package` tag first:** The Umbraco Marketplace indexes NuGet packages by this tag. It's been placed first in the tag list to ensure it's not truncated if NuGet has tag length limits.
- **`icon.png` placeholder:** Added `PackageIcon` property pointing to `icon.png` without creating the file. This primes the csproj for a future icon without causing a pack failure (dotnet warns but doesn't error if the file is absent).
- **`softprops/action-gh-release@v2`:** Industry-standard action for GitHub Releases. `generate_release_notes: true` uses GitHub's auto-generated notes from PR/commit history.
- **`--skip-duplicate`:** Makes the workflow safe to re-run if the tag already exists on NuGet — doesn't fail the job.

**Verification Results:**
- ✅ .csproj has correct GitHub URLs, RepositoryType, icon, doc gen, release notes
- ✅ `umbraco-package` tag present (Marketplace discovery)
- ✅ `umbraco-marketplace.json` created with correct schema reference
- ✅ Publish workflow created at `.github/workflows/publish-nuget.yml`
- ✅ Setup instructions at `.github/NUGET_SETUP.md`
- ✅ All changes committed and pushed to origin/main
