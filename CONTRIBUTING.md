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

```bash
# Start from develop
git checkout develop
git pull origin develop

# Create release branch
git checkout -b release/v0.3.0

# Update version in src/SquadTUI/SquadTUI.csproj
# Update CHANGELOG.md if present

# Commit version bump
git add .
git commit -m "chore: bump version to 0.3.0"

# Push release branch (triggers automated release workflow)
git push origin release/v0.3.0

# After CI passes, merge to main via PR
# Then merge back to develop via PR
```

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
4. **Tests:** Ensure all tests pass (`dotnet test`)
5. **CI:** All CI checks must pass before merge

## CI/CD Pipeline

- **CI (`ci.yml`):** Runs on every push to `develop`/`main` and all PRs
  - Matrix builds: Windows/macOS/Linux × x64/ARM64
  - Runs: restore, build, test
  
- **Release (`release.yml`):** Runs on push to `release/*` branches
  - Builds cross-platform binaries (6 platforms)
  - Creates git tags (extracted from csproj version)
  - Publishes GitHub release with binary artifacts

## Code Style

- **Language:** C# 13 with .NET 10
- **Style:** Follow existing codebase conventions
- **Records:** Prefer immutable `record` types for models
- **Nullability:** Enable nullable reference types
- **Naming:** Use `SquadTask`, `SquadTaskStatus` (avoid `Task` collision)

## Questions?

Open an issue or reach out to the team. We're here to help!

---

**Remember:** Ocean's Eleven themed — we plan the heist together. 🎰
