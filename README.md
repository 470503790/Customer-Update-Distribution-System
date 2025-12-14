# Customer-Update-Distribution-System

客户现场的 RCA7 更新包（server.zip/client.zip/SQL 脚本）分发与回滚系统，包含基于 Admin.NET 的后端、Vue 管理端以及 Windows Agent/Tray 客户端。

## 当前能力概览
- 客户-分支-节点树模型：支持版本格式校验、唯一性校验以及节点 Token 一次性生成（见 `backend/Rca7.Update.Application/Services/CustomerTreeService.cs`）。
- 发布单编排：支持默认的停服 → 备份 → 部署 → 数据脚本 → 重启 → 上报流水，支持回滚触发（`ReleaseOrderService` + `ReleaseOrchestrator`）。
- 包上传与 COS 预签名：提供对象键生成、按客户/分支/节点分层的路径规则以及签名 URL 生成逻辑（`CosStorageService`）。
- 审计日志聚合：创建发布单、回滚等操作会写入审计日志存储，便于看板或追踪。

## 代码结构
- `backend/`：Admin.NET 后端扩展模块与领域服务，包含发布单编排、包上传、COS 存储适配等。
- `frontend/`：Admin.NET 前端（Vue3 + Element-Plus）扩展页面的占位目录。
- `client/`：Windows Agent/Tray 客户端相关内容。
- `docs/`：需求、设计与流水线文档（中文）。
- `docker/`、`scripts/`：部署与构建脚本（脚本内含 TODO，可在落地时完善）。

## 快速开始
### 依赖安装
- .NET 7 SDK（版本可通过仓库变量 `DOTNET_VERSION` 统一管理）。
- Node.js 16+（版本可通过仓库变量 `NODE_VERSION` 统一管理）。
- 可选：PowerShell 7（在 Windows 或 CI 中调用 `.ps1` 脚本时使用）。

### 环境变量与 Secrets 占位
本地可使用 `.env.local` 或 shell 环境变量的形式模拟 CI 所需的 Secrets/Variables，示例（请自行替换为有效值或哑元值）：

```bash
# COS 对象存储
export COS_SECRET_ID="local-dev-id"
export COS_SECRET_KEY="local-dev-key"
export COS_REGION="ap-guangzhou"
export COS_BUCKET="demo-bucket"
export COS_PREFIX="rca7-demo"

# 部署 SSH（示例为 Dev 环境，Prod 同理）
export DEPLOY_SSH_HOST_DEV="dev.example.com"
export DEPLOY_SSH_USER_DEV="deploy"
export DEPLOY_SSH_KEY_DEV="$HOME/.ssh/id_rsa"   # 可使用私钥路径或占位字符串
export DEPLOY_PATH_DEV="/opt/rca7/demo"

# 客户端签名（如需）
export SIGN_PFX_BASE64="BASE64_PFX_PLACEHOLDER"
export SIGN_PFX_PASSWORD="pfx-password"
```

### 本地运行
- 后端（入口为 `Rca7.Update.Web.Entry`，未提供连接串时会退回到本地 SQLite 文件 `customer-update.db`）：
  ```bash
  dotnet restore backend/Rca7.Update.Web.Entry/Rca7.Update.Web.Entry.csproj
  dotnet run --project backend/Rca7.Update.Web.Entry/Rca7.Update.Web.Entry.csproj --urls "http://localhost:5000"
  ```
- 前端（如果已有 package.json）：
  ```bash
  cd frontend
  npm install
  npm run dev -- --host
  ```
- 客户端（根据需求选择服务端或托盘程序）：
  ```bash
  dotnet restore client/Rca7.UpdateClient.sln
  dotnet run --project client/Rca7.UpdateAgent.Service/Rca7.UpdateAgent.Service.csproj
  # 或
  dotnet run --project client/Rca7.UpdateTray.WinForms/Rca7.UpdateTray.WinForms.csproj
  ```

## 开发与测试
1. 安装 .NET 7 SDK 及 Node.js，对应版本可通过仓库变量 `DOTNET_VERSION`、`NODE_VERSION` 统一管理。
2. 进入后端目录执行还原与测试：
   - `dotnet restore backend/Rca7.Update.Application/Rca7.Update.Application.csproj`
   - `dotnet test backend/tests/Rca7.Update.Tests.csproj`
3. 管理端前端与客户端的构建脚本位于 `scripts/`，可根据实际部署方式补充 `build.sh`/`build.ps1`/`test.ps1` 内容。

## API 速览（后端）
- 客户树：
  - `GET /api/customers`：查看客户-分支-节点树。
  - `POST /api/customers`：创建客户（唯一版本校验）。
  - `POST /api/branches`：为客户新增分支。
  - `POST /api/nodes`：为分支新增节点。
  - `POST /api/nodes/{id}/token`：生成一次性节点 Token。
- 包上传：
  - `POST /api/packages/uploads`：根据客户/分支/节点与包类型生成 COS 预签名上传/下载 URL。
- 发布单：
  - `POST /api/releaseorders`：创建发布单并自动排入编排器。
  - `GET /api/releaseorders` / `GET /api/releaseorders/{id}`：查看发布单与步骤进度。
  - `POST /api/releaseorders/{id}/rollback`：触发回滚并记录审计。

> 提示：`backend/Rca7.Update.Web.Entry/appsettings.json` 提供了默认的 COS 配置结构，若要使用外部数据库，可设置 `ConnectionStrings__Default` 或 `DATABASE_CONNECTION` 环境变量。

## 脚本使用说明
### `scripts/build.sh`
- 参数：`--configuration`、`--backend-project`、`--backend-entry`、`--client-solution`、`--frontend-dir`、`--skip-backend`、`--skip-frontend`、`--skip-client`。
- 本地示例：`bash scripts/build.sh --configuration Debug --skip-frontend`。
- CI 调用：在 GitHub Actions 中通过 `DOTNET_VERSION`、`NODE_VERSION` 等变量预装 SDK 后，直接运行 `bash scripts/build.sh --configuration Release`。

### `scripts/build.ps1`
- 参数：`-Configuration`、`-BackendProject`、`-BackendEntry`、`-ClientSolution`、`-FrontendDir`、`-SkipBackend`、`-SkipFrontend`、`-SkipClient`。
- 本地示例（PowerShell 7）：`pwsh -File scripts/build.ps1 -Configuration Debug -SkipClient`。
- CI 调用：在 Windows 代理或安装了 PowerShell 的环境中，执行 `pwsh -File scripts/build.ps1 -Configuration Release`。

### `scripts/test.sh`
- 参数：`--configuration`、`--test-filter`、`--backend-test-project`、`--frontend-dir`、`--client-test-solution`、`--skip-backend`、`--skip-frontend`、`--skip-client`。
- 本地示例：`bash scripts/test.sh --configuration Debug --test-filter "Category=Unit"`。
- CI 调用：可在还原依赖后直接运行 `bash scripts/test.sh --skip-frontend --skip-client`，仅覆盖后端测试。

### `scripts/test.ps1`
- 参数：`-Configuration`、`-TestFilter`、`-BackendTestProject`、`-FrontendDir`、`-ClientTestSolution`、`-SkipBackend`、`-SkipFrontend`、`-SkipClient`。
- 本地示例：`pwsh -File scripts/test.ps1 -Configuration Debug -TestFilter "Category=Integration"`。
- CI 调用：在 Windows/PowerShell 环境中执行 `pwsh -File scripts/test.ps1 -SkipFrontend -SkipClient` 聚焦后端测试。

### `scripts/package-client.ps1`
- 作用：预留的客户端打包脚本，尚未实现；可在添加签名、打包逻辑后，通过 `pwsh -File scripts/package-client.ps1 -OutputPath <artifacts>` 在本地或 CI 中调用。

## CI/CD 流程
- **CI（.github/workflows/ci.yml）**：
  - 后端：Restore → Build → Test，全量输出 TRX 结果作为构件。
  - 前端：自动检测 `frontend/package.json`，存在则 npm 安装、lint、build；不存在则跳过但保持 Job 成功。
  - 客户端：构建 Windows 解决方案，预留测试目录探测。
- **Release（.github/workflows/release.yml）**：
  - 后端：`dotnet publish` 生成发布目录并压缩为 `backend_<tag>.zip`。
  - 前端：存在 `frontend` 时打包 `dist` 目录为 `admin-web_<tag>.zip`。
  - 客户端：分别发布 Agent Service 与 Tray，再各自打包 zip。
  - COS 上传：若配置 `COS_BUCKET` 等 Secrets，则在 `upload-to-cos` Job 中替换为实际上传逻辑；默认会跳过。
  - GitHub Release：汇总所有 zip 构件发布到 tag 对应的 Release。

## CI/CD 配置清单
下述 GitHub Actions workflows 依赖的仓库级 Secrets/Variables，需要在运行前配置：

- Variables：`DOTNET_VERSION`、`NODE_VERSION`
- COS Secrets：`COS_SECRET_ID`、`COS_SECRET_KEY`、`COS_REGION`、`COS_BUCKET`、`COS_PREFIX`
- 部署 Secrets（Dev）：`DEPLOY_SSH_HOST_DEV`、`DEPLOY_SSH_USER_DEV`、`DEPLOY_SSH_KEY_DEV`、`DEPLOY_PATH_DEV`
- 部署 Secrets（Prod）：`DEPLOY_SSH_HOST_PROD`、`DEPLOY_SSH_USER_PROD`、`DEPLOY_SSH_KEY_PROD`、`DEPLOY_PATH_PROD`
- 客户端签名 Secrets（可选）：`SIGN_PFX_BASE64`、`SIGN_PFX_PASSWORD`

上述名称已在 `.github/workflows/ci.yml`、`release.yml`、`deploy-dev.yml`、`deploy-prod.yml`、`client-release.yml` 中引用，后续补充构建、测试、部署命令时可直接使用。

## 设计与规划文档
- 开发计划与 CI/CD 流水线：参见 [docs/客户分发更新系统_开发计划与ci_cd流水线v_1_（admin.md](<docs/客户分发更新系统_开发计划与ci_cd流水线v_1_（admin.md>)。
- 程序设计文档：参见 [docs/客户分发更新系统_程序设计文档v_2_（基于admin.md](<docs/客户分发更新系统_程序设计文档v_2_（基于admin.md>)。
