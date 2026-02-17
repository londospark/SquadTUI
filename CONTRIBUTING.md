# Contributing to SquadTUI

Thank you for considering contributing to SquadTUI! This document outlines our Git workflow and contribution process.

## Git Flow Workflow

We follow a Git Flow branching strategy with the following structure:

### Branch Types

#### `main` Branch
- **Purpose:** Production-ready code
- **Protection:** Only receives merges from `release/*` and `hotfix/*` branches
- **Triggers:** Release workflow when `release/*` branches are pushed

#### `develop` Branch
- **Purpose:** Integration branch for features
- **Protection:** All feature development merges here first
- **Triggers:** CI builds and tests on every push

#### Feature Branches (`feature/*`)
- **Purpose:** New features and enhancements
- **Naming:** `feature/short-description` (e.g., `feature/add-theme-switcher`)
- **Branch from:** `develop`
- **Merge into:** `develop` via Pull Request

#### Release Branches (`release/*`)
- **Purpose:** Prepare a new production release
- **Naming:** `release/vX.Y.Z` (e.g., `release/v0.3.0`)
- **Branch from:** `develop`
- **Merge into:** Both `main` and `develop`
- **Triggers:** Automated release build, tag creation, and GitHub release publication

#### Hotfix Branches (`hotfix/*`)
- **Purpose:** Critical bug fixes for production
- **Naming:** `hotfix/short-description` (e.g., `hotfix/fix-crash-on-startup`)
- **Branch from:** `main`
- **Merge into:** Both `main` and `develop`

## Contribution Workflow

### 1. Feature Development

```bash
# Start from develop
git checkout develop
git pull origin develop

# Create feature branch
git checkout -b feature/my-awesome-feature

# Make changes, commit regularly
git add .
git commit -m "feat: add awesome feature"

# Push to remote
git push origin feature/my-awesome-feature

# Create Pull Request to develop
```

### 2. Creating a Release

We have two approaches for creating releases:

#### Option A: Automated Script (Recommended)

```bash
# Use the automated release script to handle the full git flow
.\scripts\release.ps1 -Version "0.4.0"
```

This script automatically:
1. Creates the release branch from develop
2. Strips `.ai-team/` from tracking (for main guard compliance)
3. Merges release into main with `--no-ff`
4. Creates and pushes the release tag
5. Merges release back into develop
6. **Restores `.ai-team/`** on develop (critical step!)
7. Cleans up the release branch

#### Option B: Manual Process

If you prefer to handle the release manually:

```bash
# Start from develop
git checkout develop
git pull origin develop

# Create release branch
git checkout -b release/v0.4.0

# Update version in src/SquadTUI/SquadTUI.csproj
# Update CHANGELOG.md if present

# Commit version bump
git add .
git commit -m "chore: bump version to 0.4.0"

# Remove .ai-team/ from tracking (required for main guard)
git rm --cached -r .ai-team/
git commit -m "chore: remove .ai-team from release branch"

# Push release branch (triggers automated release workflow)
git push origin release/v0.4.0

# After CI passes, merge to main via PR
git checkout main
git merge --no-ff release/v0.4.0
git push origin main

# Then merge back to develop via PR
git checkout develop
git merge --no-ff release/v0.4.0

# ⚠️ CRITICAL: Restore .ai-team/ immediately after merging back
git checkout HEAD~1 -- .ai-team/
git add .ai-team/
git commit -m "chore: restore .ai-team after release backmerge"
git push origin develop

# Clean up
git branch -d release/v0.4.0
git push origin --delete release/v0.4.0
```

#### ℹ️ Why `.ai-team/` Handling is Necessary

- `.ai-team/` contains runtime team state and must NOT exist on `main` or `preview` branches
- The `squad-main-guard.yml` workflow enforces this by blocking PRs that include `.ai-team/`
- During release, we use `git rm --cached` to remove tracking without deleting local files
- After merging the release branch back into develop, we must **immediately restore** `.ai-team/` from the pre-merge state
- The automated script handles this restoration automatically; the manual process requires explicit action

### 3. Hotfix Process

```bash
# Start from main
git checkout main
git pull origin main

# Create hotfix branch
git checkout -b hotfix/critical-bug-fix

# Make fixes
git add .
git commit -m "fix: resolve critical bug"

# Push to remote
git push origin hotfix/critical-bug-fix

# Create PR to main
# After merge, merge main back into develop
```

## Pull Request Guidelines

1. **Branch naming:** Follow the conventions above
2. **Commit messages:** Use conventional commits format:
   - `feat:` for new features
   - `fix:` for bug fixes
   - `docs:` for documentation changes
   - `chore:` for maintenance tasks
   - `refactor:` for code refactoring
   - `test:` for test additions/changes
3. **Description:** Clearly explain what and why
4. **Tests:** Ensure all tests pass:
   ```bash
   # Run all tests (excluding CI-only meta tests)
   dotnet test --filter "FullyQualifiedName!~DotnetBuild&FullyQualifiedName!~DotnetTest"
   ```
5. **CI:** All CI checks must pass before merge (6-platform matrix: Windows/macOS/Linux × x64/ARM64)

## CI/CD Pipeline

- **CI (`ci.yml`):** Runs on every push to `develop`/`main` and all PRs
  - Matrix builds: Windows/macOS/Linux × x64/ARM64 (6 platforms)
  - Runs: restore, build, test
  
- **Release (`release.yml`):** Runs on push to `release/*` branches
  - Builds cross-platform binaries (6 platforms)
  - Creates git tags (extracted from csproj version)
  - Publishes GitHub release with binary artifacts

## The Dark Souls Squad

| Role | Name | Specialty |
|------|------|-----------|
| 🎯 Lead | **Solaire** | Architecture, code review, big picture |
| 💻 Backend Dev | **Andre** | Services, data layer, integrations |
| 🎨 Frontend Dev | **Siegmeyer** | UI/UX, screens, theming |
| 🧪 QA Engineer | **Patches** | Testing, quality assurance |
| 📣 UX Designer | **Firekeeper** | User experience, design systems |
| 📝 Docs | **Scribe** | Documentation, knowledge management |
| 🔧 DevOps | **Ralph** | CI/CD, tooling, automation |

## Code Style

- **Language:** C# 13 with .NET 10
- **Style:** Follow existing codebase conventions
- **Records:** Prefer immutable `record` types for models
- **Nullability:** Enable nullable reference types
- **Naming:** Use `SquadTask`, `SquadTaskStatus` (avoid `Task` collision)

## Questions?

Open an issue or reach out to the team. We're here to help!

---

**Remember:** Dark Souls themed — we link the flame together. ☀️ \\[T]/
