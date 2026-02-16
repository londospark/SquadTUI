# Decision: Git Flow and GitHub Repository Setup

**Date:** 2026-02-17  
**By:** Danny  
**Status:** Implemented

## What

Established Git Flow branching strategy for SquadTUI and prepared project for public GitHub release:

1. **Git Flow Setup:**
   - Created `develop` branch from `main` as integration branch
   - Updated CI workflow (`.github/workflows/ci.yml`) to trigger on `develop` branch pushes and PRs
   - Release workflow already configured for `release/*` branches
   - Created `CONTRIBUTING.md` documenting full Git Flow workflow

2. **Branch Structure:**
   - `main` — Production-ready code (only merges from `release/*` and `hotfix/*`)
   - `develop` — Integration branch for all feature development
   - `feature/*` — Feature branches (branch from `develop`, merge to `develop`)
   - `release/*` — Release preparation (branch from `develop`, merge to `main` and `develop`)
   - `hotfix/*` — Critical production fixes (branch from `main`, merge to `main` and `develop`)

3. **Public Repository:**
   - Created comprehensive `README.md` with project description, features, installation, usage, tech stack
   - Added Ocean's Eleven themed credits section with all 11 squad members
   - Created `LICENSE` file (MIT, copyright 2026 LondoSpark)
   - Configured git remote: `https://github.com/londospark/SquadTUI.git`
   - **Note:** GitHub token lacks repository creation permissions — repository must be created manually via GitHub UI before pushing

## Why

**Git Flow Rationale:**
- **Stability:** `main` branch remains stable and production-ready at all times
- **Parallel Development:** Multiple features can be developed simultaneously in isolation
- **Release Control:** `release/*` branches provide controlled release preparation and testing
- **Hotfix Capability:** Critical production bugs can be fixed and deployed without disrupting feature development
- **CI/CD Integration:** Workflows already configured for automated builds/tests (CI) and releases

**Public Repository Rationale:**
- **Visibility:** SquadTUI aims to integrate with csharpfritz/SquadUI — public repo enables collaboration
- **Documentation:** README provides clear installation/usage guide for external users
- **Contribution:** CONTRIBUTING.md establishes clear workflow for external contributors
- **Credibility:** MIT license and proper documentation signal professional, maintainable project

## Consequences

**Immediate:**
- Development work happens on `develop` branch, keeping `main` clean
- Features merge to `develop` via PR, ensuring code review
- Releases follow structured workflow: `develop` → `release/vX.Y.Z` → `main` → tag → GitHub release
- CI runs on all PRs and branch pushes, catching issues early

**Next Steps:**
1. User must manually create `londospark/SquadTUI` repository via GitHub UI (token lacks `public_repo` scope)
2. After repo creation: `git push -u origin main develop` to push both branches
3. Configure branch protection rules in GitHub (require PR reviews, CI passing)
4. Team should adopt PR workflow: all changes via feature branches + PRs to `develop`

**Team Impact:**
- **All agents:** Use `develop` as base branch for new features (not `main`)
- **Danny:** Reviews PRs before merge; manages release branches
- **Toolsmith:** May need to add branch protection automation or GitHub repo setup script
- **Scribe:** Needs to merge this decision into main `.ai-team/decisions.md`

## References

- Git Flow model: https://nvie.com/posts/a-successful-git-branching-model/
- CONTRIBUTING.md: Documents complete workflow with examples
- CI workflow: `.github/workflows/ci.yml`
- Release workflow: `.github/workflows/release.yml`
