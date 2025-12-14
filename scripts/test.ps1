<#+
.SYNOPSIS
Test execution script for backend, frontend, and client projects.
.DESCRIPTION
Runs backend tests by default and can include frontend/client test suites when available.
.PARAMETER Configuration
Build configuration for dotnet test (Default: Release).
.PARAMETER TestFilter
Optional filter to run specific backend tests.
.PARAMETER BackendTestProject
Path to backend test project file.
.PARAMETER FrontendDir
Path to frontend project directory.
.PARAMETER ClientTestSolution
Path to client test solution or project file.
.PARAMETER SkipBackend
Skip backend test execution.
.PARAMETER SkipFrontend
Skip frontend test execution.
.PARAMETER SkipClient
Skip client test execution.
#>
[CmdletBinding()]
param(
    [string]$Configuration = "Release",
    [string]$TestFilter = "",
    [string]$BackendTestProject = "backend/tests/Rca7.Update.Tests.csproj",
    [string]$FrontendDir = "frontend",
    [string]$ClientTestSolution = "",
    [switch]$SkipBackend,
    [switch]$SkipFrontend,
    [switch]$SkipClient
)

$ErrorActionPreference = "Stop"
$script:RepoRoot = (Resolve-Path "$PSScriptRoot/..\").Path

function Write-Info($Message) { Write-Host "[INFO] $Message" }
function Write-WarnLine($Message) { Write-Warning $Message }
function Write-ErrorLine($Message) { Write-Host "[ERROR] $Message" -ForegroundColor Red }

function Resolve-PathRelative([string]$Path) {
    if ([System.IO.Path]::IsPathRooted($Path)) { return $Path }
    return (Join-Path $RepoRoot $Path)
}

function Invoke-BackendTests {
    if ($SkipBackend) { Write-Info "Backend tests skipped by flag."; return }
    $resolvedTestProject = Resolve-PathRelative $BackendTestProject
    if (-not (Test-Path $resolvedTestProject)) {
        Write-WarnLine "Backend test project not found at $resolvedTestProject (skipping)."
        return
    }

    $args = @("test", $resolvedTestProject, "-c", $Configuration)
    if ($TestFilter) {
        $args += @("--filter", $TestFilter)
    }

    Write-Info "Running backend tests: dotnet $($args -join ' ')"
    dotnet @args
}

function Invoke-FrontendTests {
    if ($SkipFrontend) { Write-Info "Frontend tests skipped by flag."; return }
    $resolvedFrontend = Resolve-PathRelative $FrontendDir
    $packageJson = Join-Path $resolvedFrontend "package.json"
    if (-not (Test-Path $packageJson)) {
        Write-WarnLine "No package.json found in $resolvedFrontend (skipping frontend tests)."
        return
    }

    Push-Location $resolvedFrontend
    try {
        Write-Info "Running frontend tests (if defined)"
        npm run test --if-present -- --watch=false
    }
    finally {
        Pop-Location
    }
}

function Invoke-ClientTests {
    if ($SkipClient) { Write-Info "Client tests skipped by flag."; return }
    if ([string]::IsNullOrWhiteSpace($ClientTestSolution)) {
        Write-WarnLine "Client test solution/project not provided (skipping)."
        return
    }

    $resolvedClient = Resolve-PathRelative $ClientTestSolution
    if (-not (Test-Path $resolvedClient)) {
        Write-WarnLine "Client test solution not found at $resolvedClient (skipping)."
        return
    }

    Write-Info "Running client tests: dotnet test $([IO.Path]::GetRelativePath($RepoRoot, $resolvedClient))"
    dotnet test $resolvedClient -c $Configuration
}

try {
    Invoke-BackendTests
    Invoke-FrontendTests
    Invoke-ClientTests
    Write-Info "Test execution completed."
    exit 0
}
catch {
    Write-ErrorLine $_
    exit 1
}
