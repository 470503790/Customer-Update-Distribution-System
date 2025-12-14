#!/usr/bin/env bash
# Description: Test execution for backend, frontend, and client projects.
# Usage: test.sh [options]
#   -c|--configuration <Configuration>   Build configuration for dotnet test (Default: Release)
#   --test-filter <Filter>               Optional filter to run specific backend tests
#   --backend-test-project <Path>        Path to backend test project file
#   --frontend-dir <Path>                Path to frontend project directory
#   --client-test-solution <Path>        Path to client test solution or project file
#   --skip-backend                       Skip backend test execution
#   --skip-frontend                      Skip frontend test execution
#   --skip-client                        Skip client test execution
#   -h|--help                            Show help

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
