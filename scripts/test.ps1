<#+
.SYNOPSIS
测试执行脚本，用于后端、前端和客户端项目
Test execution script for backend, frontend, and client projects.
.DESCRIPTION
默认运行后端测试，可在可用时包括前端/客户端测试套件
Runs backend tests by default and can include frontend/client test suites when available.
.PARAMETER Configuration
dotnet test 的构建配置（默认：Release） Build configuration for dotnet test (Default: Release).
.PARAMETER TestFilter
运行特定后端测试的可选过滤器 Optional filter to run specific backend tests.
.PARAMETER BackendTestProject
后端测试项目文件路径 Path to backend test project file.
.PARAMETER FrontendDir
前端项目目录路径 Path to frontend project directory.
.PARAMETER ClientTestSolution
客户端测试解决方案或项目文件路径 Path to client test solution or project file.
.PARAMETER SkipBackend
跳过后端测试执行 Skip backend test execution.
.PARAMETER SkipFrontend
跳过前端测试执行 Skip frontend test execution.
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
