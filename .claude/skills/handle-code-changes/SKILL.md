---
name: handle-code-changes
description: Post-change checklist for this repo. Verifies the local NuGet package version (Directory.Packages.props) isn't already behind what's been published, and bumps it forward if so. Run after finishing any code change in this repo, before calling the change done.
---

# Handle code changes

## When to use

Run this after a code change in this repo (feature, fix, refactor) is otherwise finished — before
reporting the change as complete. It guards against one specific, real failure mode: a GitHub
release can be cut (which publishes to NuGet) without `Directory.Packages.props`'s `<Version>`
being bumped afterward in `main`. When that happens, local `Version` silently falls behind what's
already public, and the next prerelease build or manual bump computes from the stale number —
risking a version that collides with one already on NuGet.

## Steps

### 1. Find the latest released version

GitHub releases are the source of truth here, not NuGet directly: a new NuGet version is only ever
published after a GitHub release/tag is created (see
`.github/workflows/ensy-net-core-workflow.yml`'s `release: types: [created]` trigger and
`infra/scripts/get-nuget-version.ps1`, which takes the published version straight from the tag
name `vX.Y.Z`).

```
gh release list --limit 1 --exclude-drafts --exclude-pre-releases
```

If `gh` isn't authenticated or available, fall back to:

```
gh api repos/EnsyInc/EnsyNet.Core/releases/latest --jq .tag_name
```

Strip the leading `v` to get the released version, e.g. `v10.0.2` → `10.0.2`.

**Cross-check (optional, not the primary source):** the NuGet flat-container index for one of the
published packages, e.g.:

```
curl -s https://api.nuget.org/v3-flatcontainer/ensynet.core/index.json
```

Take the highest version in `versions` with no `-` in it (stable releases only — skip
`-main.*`/`-dev.*` prereleases). This should agree with the GitHub release tag; if it doesn't
(e.g. NuGet indexing lag), trust the GitHub release.

### 2. Read the local version

Read `<Version>` from `src/Core/Directory.Packages.props`.

### 3. Compare

Compare the local version against the latest released version using semver ordering.

- **Local is already greater than released:** nothing to do — stop here.
- **Local is less than or equal to released:** local is stale. Continue to step 4.

### 4. Compute the new version

Bump the **released** version's patch number by 1 — don't increment the stale local version, and
don't try to infer an intended major/minor bump here. This guarantees a version that has never
been published, without stepping on `/bump-version`'s job of deciding the real bump type for the
next deliberate release.

Example: local is `10.0.1`, latest released is `10.0.2` → new local version is `10.0.3`.

### 5. Update the file

In `src/Core/Directory.Packages.props`, update **both**:

- `<Version>...</Version>`
- `<AssemblyVersion>...</AssemblyVersion>`

to the version computed in step 4.

### 6. Report

If a bump was needed: "Local NuGet version was stale (`OLD`, but `RELEASED` is already published)
— bumped to `NEW` to avoid a version collision."

If local was already ahead of the latest release, don't mention this check at all — it's a no-op
in the common case and shouldn't add noise to the change summary.

## Relationship to `/bump-version`

This is a passive safety net, not a replacement for `/bump-version`. `/bump-version` decides
major/minor/patch from commit messages for a deliberate release bump; this skill only guards
against the local version having silently fallen behind what's already public. If both apply,
run this check first — bumping from a stale base could otherwise still land on an already-taken
version.
