# Customer-Update-Distribution-System

## CI/CD 配置清单
下述 GitHub Actions workflows 依赖的仓库级 Secrets/Variables，需要在运行前配置：

- Variables：`DOTNET_VERSION`、`NODE_VERSION`
- COS Secrets：`COS_SECRET_ID`、`COS_SECRET_KEY`、`COS_REGION`、`COS_BUCKET`、`COS_PREFIX`
- 部署 Secrets（Dev）：`DEPLOY_SSH_HOST_DEV`、`DEPLOY_SSH_USER_DEV`、`DEPLOY_SSH_KEY_DEV`、`DEPLOY_PATH_DEV`
- 部署 Secrets（Prod）：`DEPLOY_SSH_HOST_PROD`、`DEPLOY_SSH_USER_PROD`、`DEPLOY_SSH_KEY_PROD`、`DEPLOY_PATH_PROD`
- 客户端签名 Secrets（可选）：`SIGN_PFX_BASE64`、`SIGN_PFX_PASSWORD`

上述名称已在 `.github/workflows/ci.yml`、`release.yml`、`deploy-dev.yml`、`deploy-prod.yml`、`client-release.yml` 中引用，后续补充构建、测试、部署命令时可直接使用。
