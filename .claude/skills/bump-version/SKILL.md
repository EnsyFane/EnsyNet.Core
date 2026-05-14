# Metadata
name: Bump Version
description: Bumps the NuGet version of the packages

# Overview
This skill bumps the NuGet package version in `Directory.Packages.props` (located at `src/Core/Directory.Packages.props`).

## Steps

### 1. Determine the bump type

**If the user specified a bump type** (major / minor / patch), use that.

**If no bump type was specified**, determine it automatically:

1. Run `git tag --sort=-version:refname | head -1` to find the latest tag.
2. Run `git log <latest-tag>..HEAD --oneline` to list all commits since that tag.
3. Apply these rules in priority order (highest wins):
   - Any commit whose message contains `[BREAKING]` (case-insensitive) → **major**
   - Any commit whose message contains `[FEAT]` or `[FEATURE]` (case-insensitive) → **minor**
   - Any commit whose message contains `[FIX]` (case-insensitive) → **patch**
   - Anything else (e.g. `[CHORE]`, `[DOCS]`, plain messages) → **patch**
4. Tell the user which bump type was chosen and why (quote the relevant commits).

### 2. Read the current version

Read `src/Core/Directory.Packages.props` and extract the value from `<Version>X.Y.Z</Version>`.

### 3. Compute the new version

Apply the bump type to the semantic version:
- **major**: increment X, reset Y and Z to 0 → `(X+1).0.0`
- **minor**: increment Y, reset Z to 0 → `X.(Y+1).0`
- **patch**: increment Z → `X.Y.(Z+1)`

### 4. Update the file

In `src/Core/Directory.Packages.props`, update **both** of these tags to the new version:
- `<Version>...</Version>`
- `<AssemblyVersion>...</AssemblyVersion>`

Use the Edit tool to make the two replacements.

### 5. Confirm

Report: "Bumped version from `OLD` to `NEW` (bump-type)."
