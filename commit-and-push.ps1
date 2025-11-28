# Commit and Push Your Changes
# Run: .\commit-and-push.ps1

Write-Host "=== Step 1: Check Current Status ===" -ForegroundColor Cyan
cd C:\UstadProjects

$currentBranch = git branch --show-current
Write-Host "Current branch: $currentBranch" -ForegroundColor Yellow

Write-Host "`nChecking for uncommitted changes..." -ForegroundColor Cyan
git status

Write-Host "`n=== Step 2: Stage All Changes ===" -ForegroundColor Cyan
git add .

Write-Host "`nStaged changes:" -ForegroundColor Cyan
git status --short

Write-Host "`n=== Step 3: Commit Changes ===" -ForegroundColor Cyan
$commitMessage = @"
refactor: organize documentation, update versions to 1.5.0, improve AuthController

- Moved all .md documentation files to docs/ folder for better organization
- Updated version numbers to 1.5.0:
  - Ustad.API.csproj (Assembly version)
  - Mobile shell package.json and app.json
  - Next.js package.json
- Improved AuthController with code refactoring:
  - Simplified password upgrade logic (removed duplication)
  - Extracted SQL queries to constants for better maintainability
  - Added ParseDbTypeId helper method
  - Improved error handling with logging
  - Removed duplicate XML comments and attributes
- Added comprehensive logging for authentication flows
"@

git commit -m $commitMessage

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n✅ Commit successful!" -ForegroundColor Green
    Write-Host "`nLatest commit:" -ForegroundColor Cyan
    git log --oneline -1
    
    Write-Host "`n=== Step 4: Push to Remote ===" -ForegroundColor Cyan
    
    # Check if remote is HTTPS or SSH
    $remoteUrl = git remote get-url origin
    Write-Host "Current remote URL: $remoteUrl" -ForegroundColor Yellow
    
    # Switch to HTTPS if needed
    if ($remoteUrl -like "*git@*") {
        Write-Host "`nSwitching remote to HTTPS to avoid SSH issues..." -ForegroundColor Yellow
        git remote set-url origin https://github.com/ustadyazilim/yesiLdefterV3.git
    }
    
    Write-Host "`nPushing to origin..." -ForegroundColor Cyan
    git push -u origin feature/auth-refactor-unified
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "`n✅ Push successful!" -ForegroundColor Green
        Write-Host "`nYour commits are now on GitHub!" -ForegroundColor Green
        Write-Host "Check: https://github.com/ustadyazilim/yesiLdefterV3/commits/feature/auth-refactor-unified" -ForegroundColor Cyan
    } else {
        Write-Host "`n❌ Push failed. You may need to:" -ForegroundColor Red
        Write-Host "1. Use a Personal Access Token (not password)" -ForegroundColor Yellow
        Write-Host "2. Or set up SSH keys properly" -ForegroundColor Yellow
    }
} else {
    Write-Host "`n⚠️ Commit failed or nothing to commit" -ForegroundColor Yellow
    Write-Host "Checking what needs to be committed..." -ForegroundColor Cyan
    git status
}

Write-Host "`n=== Done ===" -ForegroundColor Green

