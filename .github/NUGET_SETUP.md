# NuGet Publishing Setup

This repo is configured to auto-publish to NuGet.org when a version tag is pushed.

## One-time Setup

### 1. Get a NuGet.org API Key

1. Create an account at [nuget.org](https://www.nuget.org/) if you don't have one
2. Go to your account → **API Keys**
3. Click **Create**
4. Set a name (e.g., `uMockingSuite-publish`), select **Push new packages and package versions**
5. Scope it to the `uMockingSuite` package ID (or glob `*` for all packages)
6. Copy the generated key — you won't see it again

### 2. Add the Secret to GitHub

1. Go to your repo on GitHub: https://github.com/jonnymuir/Umbraco.uMockingSuite
2. **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Name: `NUGET_API_KEY`
5. Value: paste the API key from step 1
6. Click **Add secret**

## Publishing a Release

```bash
# Tag the commit you want to release
git tag v0.1.0

# Push the tag — this triggers the workflow
git push origin v0.1.0
```

The GitHub Actions workflow will:
- Build and pack the project in Release mode
- Push the `.nupkg` to NuGet.org
- Create a GitHub Release with auto-generated notes and the `.nupkg` attached

## Version Management

The version is set in `uMockingSuite/uMockingSuite.csproj` under `<Version>`. Update it before tagging:

```xml
<Version>0.2.0</Version>
```

Follow [Semantic Versioning](https://semver.org/): `MAJOR.MINOR.PATCH`
