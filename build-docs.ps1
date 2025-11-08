# PowerShell script to build MkDocs documentation

Write-Host "License Management API - Documentation Builder" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

# Check if Python is installed
$pythonCheck = Get-Command python -ErrorAction SilentlyContinue
if ($pythonCheck) {
    $pythonVersion = & python --version 2>&1
    Write-Host "✓ Python found: $pythonVersion" -ForegroundColor Green
}
else {
    Write-Host "✗ Python not found. Please install Python 3.8+ from https://www.python.org/" -ForegroundColor Red
    exit 1
}

# Check if MkDocs is installed
$mkdocsInstalled = $false
$mkdocsCheck = Get-Command mkdocs -ErrorAction SilentlyContinue
if ($mkdocsCheck) {
    $mkdocsVersion = & mkdocs --version 2>&1
    Write-Host "✓ MkDocs found: $mkdocsVersion" -ForegroundColor Green
    $mkdocsInstalled = $true
}
else {
    Write-Host "✗ MkDocs not found." -ForegroundColor Yellow
}

# Install dependencies if needed
if (-not $mkdocsInstalled) {
    Write-Host ""
    Write-Host "Installing MkDocs and dependencies..." -ForegroundColor Yellow
    pip install -r requirements.txt
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Dependencies installed successfully" -ForegroundColor Green
    }
    else {
        Write-Host "✗ Failed to install dependencies" -ForegroundColor Red
        exit 1
    }
}

# Build the documentation
Write-Host ""
Write-Host "Building documentation..." -ForegroundColor Cyan

mkdocs build

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✓ Documentation built successfully!" -ForegroundColor Green
    Write-Host "Output directory: site/" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "To view the documentation:" -ForegroundColor Yellow
    Write-Host "  1. Run: .\serve-docs.ps1" -ForegroundColor White
    Write-Host "  2. Or open: site\index.html in your browser" -ForegroundColor White
}
else {
    Write-Host ""
    Write-Host "✗ Failed to build documentation" -ForegroundColor Red
    exit 1
}
