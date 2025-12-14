#!/usr/bin/env bash
# 描述：后端、前端和客户端项目的测试执行脚本
# Description: Test execution for backend, frontend, and client projects.
# 用法 Usage: test.sh [options]
#   -c|--configuration <Configuration>   dotnet test 的构建配置（默认：Release） Build configuration for dotnet test (Default: Release)
#   --test-filter <Filter>               运行特定后端测试的可选过滤器 Optional filter to run specific backend tests
#   --backend-test-project <Path>        后端测试项目文件路径 Path to backend test project file
#   --frontend-dir <Path>                前端项目目录路径 Path to frontend project directory
#   --client-test-solution <Path>        客户端测试解决方案或项目文件路径 Path to client test solution or project file
#   --skip-backend                       跳过后端测试执行 Skip backend test execution
#   --skip-frontend                      跳过前端测试执行 Skip frontend test execution
#   --skip-client                        跳过客户端测试执行 Skip client test execution
#   -h|--help                            显示帮助 Show help

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

CONFIGURATION="Release"
TEST_FILTER=""
BACKEND_TEST_PROJECT="$REPO_ROOT/backend/tests/Rca7.Update.Tests.csproj"
FRONTEND_DIR="$REPO_ROOT/frontend"
CLIENT_TEST_SOLUTION=""

SKIP_BACKEND=false
SKIP_FRONTEND=false
SKIP_CLIENT=false

usage() {
  sed -n '2,16p' "$0"
}

log_info() { echo "[INFO] $*"; }
log_warn() { echo "[WARN] $*" >&2; }
log_error() { echo "[ERROR] $*" >&2; }

parse_args() {
  while [[ $# -gt 0 ]]; do
    case "$1" in
      -c|--configuration)
        CONFIGURATION="$2"; shift 2 ;;
      --test-filter)
        TEST_FILTER="$2"; shift 2 ;;
      --backend-test-project)
        BACKEND_TEST_PROJECT="$2"; shift 2 ;;
      --frontend-dir)
        FRONTEND_DIR="$2"; shift 2 ;;
      --client-test-solution)
        CLIENT_TEST_SOLUTION="$2"; shift 2 ;;
      --skip-backend)
        SKIP_BACKEND=true; shift ;;
      --skip-frontend)
        SKIP_FRONTEND=true; shift ;;
      --skip-client)
        SKIP_CLIENT=true; shift ;;
      -h|--help)
        usage; exit 0 ;;
      *)
        log_error "Unknown argument: $1"; usage; exit 1 ;;
    esac
  done
}

invoke_backend_tests() {
  [[ "$SKIP_BACKEND" == true ]] && { log_info "Backend tests skipped by flag."; return; }
  if [[ ! -f "$BACKEND_TEST_PROJECT" ]]; then
    log_warn "Backend test project not found at $BACKEND_TEST_PROJECT (skipping)."
    return
  fi

  local args=("test" "$BACKEND_TEST_PROJECT" "-c" "$CONFIGURATION")
  if [[ -n "$TEST_FILTER" ]]; then
    args+=("--filter" "$TEST_FILTER")
  fi

  log_info "Running backend tests: dotnet ${args[*]}"
  dotnet "${args[@]}"
}

invoke_frontend_tests() {
  [[ "$SKIP_FRONTEND" == true ]] && { log_info "Frontend tests skipped by flag."; return; }
  local package_json="$FRONTEND_DIR/package.json"
  if [[ ! -f "$package_json" ]]; then
    log_warn "No package.json found in $FRONTEND_DIR (skipping frontend tests)."
    return
  fi

  log_info "Running frontend tests (if defined)"
  pushd "$FRONTEND_DIR" >/dev/null
  npm run test --if-present -- --watch=false
  popd >/dev/null
}

invoke_client_tests() {
  [[ "$SKIP_CLIENT" == true ]] && { log_info "Client tests skipped by flag."; return; }
  if [[ -z "$CLIENT_TEST_SOLUTION" ]]; then
    log_warn "Client test solution/project not provided (skipping)."
    return
  fi
  if [[ ! -f "$CLIENT_TEST_SOLUTION" ]]; then
    log_warn "Client test solution not found at $CLIENT_TEST_SOLUTION (skipping)."
    return
  fi

  log_info "Running client tests: dotnet test ${CLIENT_TEST_SOLUTION#"$REPO_ROOT/"}"
  dotnet test "$CLIENT_TEST_SOLUTION" -c "$CONFIGURATION"
}

main() {
  parse_args "$@"

  invoke_backend_tests
  invoke_frontend_tests
  invoke_client_tests

  log_info "Test execution completed."
}

main "$@"
