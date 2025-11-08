# PowerShell script to serve MkDocs documentation locally

Write-Host "License Management API - Documentation Server" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
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

# Check if pip is installed
$pipCheck = Get-Command pip -ErrorAction SilentlyContinue
if ($pipCheck) {
    $pipVersion = & pip --version 2>&1
    Write-Host "✓ pip found: $pipVersion" -ForegroundColor Green
}
else {
    Write-Host "✗ pip not found. Please install pip." -ForegroundColor Red
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

# Serve the documentation
Write-Host ""
Write-Host "Starting documentation server..." -ForegroundColor Cyan
Write-Host "Documentation will be available at: http://127.0.0.1:8000" -ForegroundColor Green
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Yellow
Write-Host ""

mkdocs serve
