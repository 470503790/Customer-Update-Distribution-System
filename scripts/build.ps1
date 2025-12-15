<#+
.SYNOPSIS
构建自动化脚本，用于后端、前端和客户端组件
Build automation for backend, frontend, and client components.
.DESCRIPTION
还原并构建后端（应用和入口）、可选的前端 npm 项目和客户端解决方案
Restores and builds backend (application and entry), optional frontend npm project, and client solution.
.PARAMETER Configuration
构建配置（默认：Release） Build configuration (Default: Release).
.PARAMETER BackendProject
后端应用项目文件路径 Path to backend application project file.
.PARAMETER BackendEntry
后端入口项目文件路径 Path to backend entry project file.
.PARAMETER ClientSolution
客户端解决方案或项目文件路径 Path to client solution or project file.
.PARAMETER FrontendDir
前端项目目录路径 Path to frontend project directory.
.PARAMETER SkipBackend
跳过后端还原/构建 Skip backend restore/build.
.PARAMETER SkipFrontend
跳过前端构建 Skip frontend build.
.PARAMETER SkipClient
跳过客户端构建 Skip client build.
#>
[CmdletBinding()]
param(
    [string]$Configuration = "Release",
    [string]$BackendProject = "backend/Rca7.Update.Application/Rca7.Update.Application.csproj",
    [string]$BackendEntry = "backend/Rca7.Update.Web.Entry/Rca7.Update.Web.Entry.csproj",
    [string]$ClientSolution = "client/Rca7.UpdateClient.sln",
    [string]$FrontendDir = "frontend",
    [switch]$SkipBackend,
    [switch]$SkipFrontend,
    [switch]$SkipClient
)

$ErrorActionPreference = "Stop"
$script:RepoRoot = (Resolve-Path "$PSScriptRoot/..\").Path
$artifactsDir = Join-Path $RepoRoot "artifacts"

function Write-Info($Message) { Write-Host "[INFO] $Message" }
function Write-Warn($Message) { Write-Warning $Message }
function Write-ErrorLine($Message) { Write-Host "[ERROR] $Message" -ForegroundColor Red }

function Resolve-PathRelative([string]$Path) {
    if ([System.IO.Path]::IsPathRooted($Path)) { return $Path }
    return (Join-Path $RepoRoot $Path)
}

function Ensure-Directory([string]$Path) {
    if (-not (Test-Path $Path)) {
        New-Item -ItemType Directory -Path $Path | Out-Null
    }
}

function Restore-AndBuild([string]$ProjectPath, [string]$OutputPath) {
    if (-not (Test-Path $ProjectPath)) {
        Write-Warn "Project not found: $ProjectPath (skipping)"
        return
    }

    Write-Info "Restoring $([IO.Path]::GetRelativePath($RepoRoot, $ProjectPath))"
    dotnet restore $ProjectPath
    Write-Info "Building $([IO.Path]::GetRelativePath($RepoRoot, $ProjectPath)) with configuration '$Configuration'"
    dotnet build $ProjectPath -c $Configuration -o $OutputPath
    Write-Info "Artifacts available at $OutputPath"
}

function Build-Backend {
    if ($SkipBackend) { Write-Info "Backend build skipped by flag."; return }

    $backendOutput = Join-Path $artifactsDir "backend"
    Ensure-Directory $backendOutput

    Restore-AndBuild (Resolve-PathRelative $BackendProject) (Join-Path $backendOutput "application")
    Restore-AndBuild (Resolve-PathRelative $BackendEntry) (Join-Path $backendOutput "web-entry")
}

function Build-Client {
    if ($SkipClient) { Write-Info "Client build skipped by flag."; return }
    $resolvedClient = Resolve-PathRelative $ClientSolution
    if (-not (Test-Path $resolvedClient)) {
        Write-Warn "Client solution not found at $resolvedClient (skipping)."
        return
    }

    $clientOutput = Join-Path $artifactsDir "client"
    Ensure-Directory $clientOutput

    Write-Info "Restoring $([IO.Path]::GetRelativePath($RepoRoot, $resolvedClient))"
    dotnet restore $resolvedClient
    Write-Info "Building $([IO.Path]::GetRelativePath($RepoRoot, $resolvedClient)) with configuration '$Configuration'"
    dotnet build $resolvedClient -c $Configuration -o $clientOutput
    Write-Info "Artifacts available at $clientOutput"
}

function Install-FrontendDependencies([string]$Path) {
    $packageLock = Join-Path $Path "package-lock.json"
    if (Test-Path $packageLock) {
        npm ci
    }
    else {
        npm install
    }
}

function Build-Frontend {
    if ($SkipFrontend) { Write-Info "Frontend build skipped by flag."; return }
    $resolvedFrontend = Resolve-PathRelative $FrontendDir
    if (-not (Test-Path $resolvedFrontend)) {
        Write-Warn "Frontend directory not found at $resolvedFrontend (skipping)."
        return
    }

    $packageJson = Join-Path $resolvedFrontend "package.json"
    if (-not (Test-Path $packageJson)) {
        Write-Warn "No package.json found in $resolvedFrontend (skipping frontend build)."
        return
    }

    Push-Location $resolvedFrontend
    try {
        Write-Info "Installing frontend dependencies in $([IO.Path]::GetRelativePath($RepoRoot, $resolvedFrontend))"
        Install-FrontendDependencies $resolvedFrontend
        Write-Info "Building frontend project"
        npm run build
        Write-Info "Frontend build completed. Check project build output directory (e.g., dist or build)."
    }
    finally {
        Pop-Location
    }
}

try {
    Ensure-Directory $artifactsDir
    Build-Backend
    Build-Frontend
    Build-Client
    Write-Info "Build pipeline completed."
    exit 0
}
catch {
    Write-ErrorLine $_
    exit 1
}
