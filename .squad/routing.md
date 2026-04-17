# Work Routing

How to decide who handles what.

## Routing Table

| Work Type | Route To | Examples |
|-----------|----------|----------|
| Package architecture, structure decisions | Boris | Project setup, .csproj, manifest layout, overall design |
| Umbraco 17 API, notification pipeline, DI | Rishi | Content notifications, composers, Umbraco extension points |
| Backoffice UI, Lit components, TypeScript | Theresa | Notification panels, toast messages, extension element manifests |
| Mocking engine, AI integration, services | Tony | Mocking logic, LLM calls, content analysis, service layer |
| Tests, QA, edge cases | Gordon | Unit tests, integration tests, coverage, QA sign-off |
| Documentation, README, NuGet packaging, Umbraco Marketplace, GitHub repo hygiene, releases | John | README, CHANGELOG, .csproj package metadata, NuGet output, GitHub Actions CI/CD |
| Security audits, vulnerability remediation, dependency pinning | Maggie | NuGet NU1903 warnings, GHSA advisories, transitive dependency pinning, CI security gates |
| Code review | Boris | Review PRs, check quality, approve or reject work |
| Testing | Gordon | Write tests, find edge cases, verify fixes |
| Scope & priorities | Boris | What to build next, trade-offs, decisions |
| Session logging | Scribe | Automatic — never needs routing |

## Issue Routing

| Label | Action | Who |
|-------|--------|-----|
| `squad` | Triage: analyze issue, assign `squad:{member}` label | Lead |
| `squad:{name}` | Pick up issue and complete the work | Named member |

### How Issue Assignment Works

1. When a GitHub issue gets the `squad` label, the **Lead** triages it — analyzing content, assigning the right `squad:{member}` label, and commenting with triage notes.
2. When a `squad:{member}` label is applied, that member picks up the issue in their next session.
3. Members can reassign by removing their label and adding another member's label.
4. The `squad` label is the "inbox" — untriaged issues waiting for Lead review.

## Rules

1. **Eager by default** — spawn all agents who could usefully start work, including anticipatory downstream work.
2. **Scribe always runs** after substantial work, always as `mode: "background"`. Never blocks.
3. **Quick facts → coordinator answers directly.** Don't spawn an agent for "what port does the server run on?"
4. **When two agents could handle it**, pick the one whose domain is the primary concern.
5. **"Team, ..." → fan-out.** Spawn all relevant agents in parallel as `mode: "background"`.
6. **Anticipate downstream work.** If a feature is being built, spawn the tester to write test cases from requirements simultaneously.
7. **Issue-labeled work** — when a `squad:{member}` label is applied to an issue, route to that member. The Lead handles all `squad` (base label) triage.
