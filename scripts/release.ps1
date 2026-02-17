<#
.SYNOPSIS
Automates the Git Flow release process with .ai-team/ protection.

.DESCRIPTION
This script implements a complete Git Flow release workflow:
1. Creates a release branch from develop
2. Strips .ai-team/ from the release branch (for main guard compatibility)
3. Merges release into main with --no-ff
4. Tags the release
5. Merges release back into develop
6. Restores .ai-team/ on develop (from pre-merge state)
7. Commits the restoration
8. Pushes all branches and tags

The .ai-team/ directory is runtime team state that must not exist on main/preview branches.
This script protects it by removing it during the release flow and restoring it after merging back to develop.

.PARAMETER Version
The version to release (e.g., "0.4.0" or "v0.4.0"). Required.

.EXAMPLE
.\scripts\release.ps1 -Version "0.4.0"
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$Version
)

$ErrorActionPreference = "Stop"

# Normalize version (remove 'v' prefix if present)
$Version = $Version -replace '^v', ''
$Tag = "v$Version"

Write-Host "Starting Git Flow release process for $Tag" -ForegroundColor Cyan

try {
    # Ensure we're in the repo root
    if (-not (Test-Path ".git")) {
        throw "Not in a git repository root"
    }

    # Ensure working directory is clean
    $status = git status --porcelain
    if ($status) {
        throw "Working directory is not clean. Commit or stash changes first."
    }

    # Configure git user if not already set
    $userName = git config user.name
    $userEmail = git config user.email
    if (-not $userName -or -not $userEmail) {
        Write-Host "Configuring git user..." -ForegroundColor Yellow
        git config user.name "github-actions[bot]"
        git config user.email "github-actions[bot]@users.noreply.github.com"
    }

    # Fetch latest from remote
    Write-Host "Fetching latest from remote..." -ForegroundColor Yellow
    git fetch origin

    # Ensure develop branch is up to date
    Write-Host "Checking out develop branch..." -ForegroundColor Yellow
    git checkout develop
    git pull origin develop

    # Create release branch
    $releaseBranch = "release/$Version"
    Write-Host "Creating release branch: $releaseBranch" -ForegroundColor Yellow
    git checkout -b $releaseBranch

    # Remove .ai-team/ from tracking (keeps local copies for develop later)
    if (Test-Path ".ai-team") {
        Write-Host "Removing .ai-team/ from git tracking..." -ForegroundColor Yellow
        git rm --cached -r .ai-team
        git commit -m "chore: remove .ai-team from release branch (guarded in main)"
    }

    # Push release branch
    Write-Host "Pushing release branch..." -ForegroundColor Yellow
    git push origin $releaseBranch

    # Merge release into main
    Write-Host "Merging release into main..." -ForegroundColor Yellow
    git checkout main
    git pull origin main
    git merge --no-ff $releaseBranch -m "Merge $releaseBranch into main"
    git push origin main

    # Create tag
    Write-Host "Creating release tag: $Tag" -ForegroundColor Yellow
    git tag -a $Tag -m "Release $Tag"
    git push origin $Tag

    # Merge release back into develop (this is where the .ai-team issue happens)
    Write-Host "Merging release back into develop..." -ForegroundColor Yellow
    git checkout develop
    git pull origin develop
    
    # Store parent commit (pre-merge state with .ai-team)
    $parentCommit = git rev-parse HEAD
    
    git merge --no-ff $releaseBranch -m "Merge $releaseBranch back into develop"

    # Restore .ai-team/ from the parent commit (pre-merge state)
    Write-Host "Restoring .ai-team/ from pre-merge state..." -ForegroundColor Yellow
    git checkout $parentCommit -- .ai-team/
    git add .ai-team/
    git commit -m "chore: restore .ai-team after release backmerge"

    # Push develop with the restoration
    Write-Host "Pushing develop with .ai-team/ restored..." -ForegroundColor Yellow
    git push origin develop

    # Delete release branch locally and remotely
    Write-Host "Cleaning up release branch..." -ForegroundColor Yellow
    git branch -d $releaseBranch
    git push origin --delete $releaseBranch

    Write-Host "`nRelease $Tag completed successfully!" -ForegroundColor Green
    Write-Host "Summary:" -ForegroundColor Cyan
    Write-Host "   - Release branch: $releaseBranch (deleted)" -ForegroundColor Gray
    Write-Host "   - Main branch: merged with tag $Tag" -ForegroundColor Gray
    Write-Host "   - Develop branch: merged and .ai-team/ restored" -ForegroundColor Gray

}
catch {
    Write-Host "`nRelease process failed: $_" -ForegroundColor Red
    Write-Host "Please resolve the error and try again." -ForegroundColor Yellow
    exit 1
}
