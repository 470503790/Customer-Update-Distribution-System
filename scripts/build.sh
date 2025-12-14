#!/usr/bin/env bash
# 描述：后端、前端和客户端组件的构建自动化脚本
# Description: Build automation for backend, frontend, and client components.
# 用法 Usage: build.sh [options]
#   -c|--configuration <Configuration>   构建配置（默认：Release） Build configuration (Default: Release)
#   --backend-project <Path>             后端应用项目文件路径 Path to backend application project file
#   --backend-entry <Path>               后端入口项目文件路径 Path to backend entry project file
#   --client-solution <Path>             客户端解决方案或项目文件路径 Path to client solution or project file
#   --frontend-dir <Path>                前端项目目录路径 Path to frontend project directory
#   --skip-backend                       跳过后端还原/构建 Skip backend restore/build
#   --skip-frontend                      跳过前端构建 Skip frontend build
#   --skip-client                        跳过客户端构建 Skip client build
#   -h|--help                            显示帮助 Show help

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

CONFIGURATION="Release"
BACKEND_PROJECT="$REPO_ROOT/backend/Rca7.Update.Application/Rca7.Update.Application.csproj"
BACKEND_ENTRY="$REPO_ROOT/backend/Rca7.Update.Web.Entry/Rca7.Update.Web.Entry.csproj"
CLIENT_SOLUTION="$REPO_ROOT/client/Rca7.UpdateClient.sln"
FRONTEND_DIR="$REPO_ROOT/frontend"
ARTIFACTS_DIR="$REPO_ROOT/artifacts"

SKIP_BACKEND=false
SKIP_FRONTEND=false
SKIP_CLIENT=false

usage() {
  sed -n '2,14p' "$0"
}

log_info() {
  echo "[INFO] $*"
}

log_warn() {
  echo "[WARN] $*" >&2
}

log_error() {
  echo "[ERROR] $*" >&2
}

parse_args() {
  while [[ $# -gt 0 ]]; do
    case "$1" in
      -c|--configuration)
        CONFIGURATION="$2"; shift 2 ;;
      --backend-project)
        BACKEND_PROJECT="$2"; shift 2 ;;
      --backend-entry)
        BACKEND_ENTRY="$2"; shift 2 ;;
      --client-solution)
        CLIENT_SOLUTION="$2"; shift 2 ;;
      --frontend-dir)
        FRONTEND_DIR="$2"; shift 2 ;;
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

ensure_directory() {
  mkdir -p "$1"
}

restore_and_build() {
  local project_path="$1"
  local output_dir="$2"
  if [[ -f "$project_path" ]]; then
    log_info "Restoring ${project_path#"$REPO_ROOT/"}"
    dotnet restore "$project_path"
    log_info "Building ${project_path#"$REPO_ROOT/"} with configuration '$CONFIGURATION'"
    dotnet build "$project_path" -c "$CONFIGURATION" -o "$output_dir"
    log_info "Artifacts available at $output_dir"
  else
    log_warn "Project not found: $project_path (skipping)"
  fi
}

build_backend() {
  [[ "$SKIP_BACKEND" == true ]] && { log_info "Backend build skipped by flag."; return; }
  local backend_output="$ARTIFACTS_DIR/backend"
  ensure_directory "$backend_output"

  restore_and_build "$BACKEND_PROJECT" "$backend_output/application"
  restore_and_build "$BACKEND_ENTRY" "$backend_output/web-entry"
}

build_client() {
  [[ "$SKIP_CLIENT" == true ]] && { log_info "Client build skipped by flag."; return; }
  if [[ ! -f "$CLIENT_SOLUTION" ]]; then
    log_warn "Client solution not found at $CLIENT_SOLUTION (skipping)."
    return
  fi

  local client_output="$ARTIFACTS_DIR/client"
  ensure_directory "$client_output"

  log_info "Restoring ${CLIENT_SOLUTION#"$REPO_ROOT/"}"
  dotnet restore "$CLIENT_SOLUTION"
  log_info "Building ${CLIENT_SOLUTION#"$REPO_ROOT/"} with configuration '$CONFIGURATION'"
  dotnet build "$CLIENT_SOLUTION" -c "$CONFIGURATION" -o "$client_output"
  log_info "Artifacts available at $client_output"
}

install_frontend_dependencies() {
  local package_lock="$1/package-lock.json"
  if [[ -f "$package_lock" ]]; then
    npm ci
  else
    npm install
  fi
}

build_frontend() {
  [[ "$SKIP_FRONTEND" == true ]] && { log_info "Frontend build skipped by flag."; return; }
  if [[ ! -d "$FRONTEND_DIR" ]]; then
    log_warn "Frontend directory not found at $FRONTEND_DIR (skipping)."
    return
  fi

  local package_json="$FRONTEND_DIR/package.json"
  if [[ ! -f "$package_json" ]]; then
    log_warn "No package.json found in $FRONTEND_DIR (skipping frontend build)."
    return
  fi

  log_info "Installing frontend dependencies in ${FRONTEND_DIR#"$REPO_ROOT/"}"
  pushd "$FRONTEND_DIR" >/dev/null
  install_frontend_dependencies
  log_info "Building frontend project"
  npm run build
  log_info "Frontend build completed. Check project build output directory (e.g., dist or build)."
  popd >/dev/null
}

main() {
  parse_args "$@"
  ensure_directory "$ARTIFACTS_DIR"

  build_backend
  build_frontend
  build_client

  log_info "Build pipeline completed."
}

main "$@"
