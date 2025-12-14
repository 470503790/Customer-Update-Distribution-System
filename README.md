# Customer-Update-Distribution-System

客户现场的 RCA7 更新包（server.zip/client.zip/SQL 脚本）分发与回滚系统，包含基于 Admin.NET 的后端、Vue 管理端以及 Windows Agent/Tray 客户端。

## 代码结构
- `backend/`：Admin.NET 后端扩展模块与领域服务，包含发布单编排、包上传、COS 存储适配等。
- `frontend/`：Admin.NET 前端（Vue3 + Element-Plus）扩展页面的占位目录。
- `client/`：Windows Agent/Tray 客户端相关内容。
- `docs/`：需求、设计与流水线文档（中文）。
- `docker/`、`scripts/`：部署与构建脚本（脚本内含 TODO，可在落地时完善）。

## 开发与测试
1. 安装 .NET 7 SDK 及 Node.js，对应版本可通过仓库变量 `DOTNET_VERSION`、`NODE_VERSION` 统一管理。
2. 进入后端目录执行还原与测试：
   - `dotnet restore backend/Rca7.Update.Application/Rca7.Update.Application.csproj`
   - `dotnet test backend/tests/Rca7.Update.Tests.csproj`
3. 管理端前端与客户端的构建脚本位于 `scripts/`，可根据实际部署方式补充 `build.sh`/`build.ps1`/`test.ps1` 内容。

## CI/CD 配置清单
下述 GitHub Actions workflows 依赖的仓库级 Secrets/Variables，需要在运行前配置：

- Variables：`DOTNET_VERSION`、`NODE_VERSION`
- COS Secrets：`COS_SECRET_ID`、`COS_SECRET_KEY`、`COS_REGION`、`COS_BUCKET`、`COS_PREFIX`
- 部署 Secrets（Dev）：`DEPLOY_SSH_HOST_DEV`、`DEPLOY_SSH_USER_DEV`、`DEPLOY_SSH_KEY_DEV`、`DEPLOY_PATH_DEV`
- 部署 Secrets（Prod）：`DEPLOY_SSH_HOST_PROD`、`DEPLOY_SSH_USER_PROD`、`DEPLOY_SSH_KEY_PROD`、`DEPLOY_PATH_PROD`
- 客户端签名 Secrets（可选）：`SIGN_PFX_BASE64`、`SIGN_PFX_PASSWORD`

上述名称已在 `.github/workflows/ci.yml`、`release.yml`、`deploy-dev.yml`、`deploy-prod.yml`、`client-release.yml` 中引用，后续补充构建、测试、部署命令时可直接使用。
