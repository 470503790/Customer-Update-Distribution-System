# 客户分发更新系统｜开发计划与 CI/CD 自动化交付流水线 V1（面向 Codex/Copilot 可执行）

> 基于：《客户分发更新系统-程序设计文档V2（基于Admin.NET）》

---

## 0. 目标与交付物

### 0.1 目标
- 将系统拆解为**可执行的开发计划（可直接转 GitHub Issues）**。
- 提供**可自动化交付的 CI/CD 流水线**（GitHub Actions 为默认），包含：
  - 后端（Admin.NET）构建/测试/产物
  - 前端（Admin.NET Web）构建/测试/产物
  - Windows 客户端（Agent/Tray）构建/测试/打包/发布
  - 可选：Docker 镜像发布与环境部署

### 0.2 交付物清单
- 代码仓库结构（monorepo）
- 工程化脚本（scripts/）
- GitHub Actions workflows（.github/workflows/）
- Dockerfile/compose（可选，但建议）
- 版本与发布规范（tag + 产物命名）
- 环境配置规范（dev/test/prod）

---

## 1. 仓库结构（Monorepo 建议）

```
repo-root/
  backend/                    # Admin.NET 后端（Entry + Application 模块）
    Admin.NET.Web.Entry/
    Admin.NET.Core/
    Admin.NET.Application/
    Rca7.Update.Application/  # 新增业务应用层（建议独立）
    tests/
  frontend/                   # Admin.NET 前端（Vue3）
    web/
  client/                     # Windows 客户端
    Rca7.UpdateAgent.Service/
    Rca7.UpdateTray.WinForms/
    Rca7.UpdateClient.Shared/
    tests/
  docker/                      # 容器化（可选）
    Dockerfile.backend
    Dockerfile.frontend        # 可选（或由 backend 静态托管）
    docker-compose.yml
  scripts/
    build.ps1
    build.sh
    test.ps1
    package-client.ps1
    publish-cos.ps1            # 可选：上传产物到 COS
    db-init.sql                # 可选：初始化脚本
  .github/workflows/
    ci.yml
    release.yml
    deploy-dev.yml
    deploy-prod.yml
    client-release.yml
  docs/
    design/
    runbooks/
```

> 说明：如果你直接 fork 官方 Admin.NET 项目，请按实际路径对齐；本结构是可执行落地的推荐。

---

## 2. 分支策略与版本规则

### 2.1 分支策略
- `main`：可发布分支（受保护，PR 合并）。
- `develop`：日常集成分支（可选；若团队小可直接 main）。
- `feature/*`：功能分支。
- `hotfix/*`：线上修复。

### 2.2 版本策略
- **平台版本**（本系统）：`vX.Y.Z`（Git tag）
- **客户端版本**（Agent/Tray）：跟随平台 tag 输出产物名，但不与 RCA7 业务包版本混用。
- **RCA7 业务包版本**：由实施在管理端上传（如 `1.25.12.13(12521)`），不走代码发布。

### 2.3 产物命名
- 后端发布包：`update-server_{tag}_{commit}.zip`
- 前端发布包：`admin-web_{tag}_{commit}.zip`
- 客户端：
  - `update-agent_{tag}_{runtime}.zip`
  - `update-tray_{tag}_{runtime}.zip`
- Docker 镜像：`ghcr.io/<org>/update-server:{tag}`

---

## 3. 可执行开发计划（可直接转 Issues）

> 约定：
> - P0 = MVP 必做，P1 = 下一迭代
> - 每个任务都必须产出：代码 + 单测/集成测（若适用）+ 文档（如接口/配置）

### Epic 0：工程化基座（P0）
1. [P0] 初始化 monorepo 结构并接入 Admin.NET
   - DoD：后端/前端能本地启动；CI 能跑通 build
2. [P0] 统一配置与环境管理
   - DoD：dev/test/prod 配置隔离；敏感配置走 secrets
3. [P0] 基础脚本（scripts/）
   - DoD：一键 build/test/package 可用

### Epic 1：数据模型与基础管理（P0）
1. [P0] 业务表与 SqlSugar 实体（rca_*）
2. [P0] 客户/分店/节点 CRUD + 节点 Token 生成（仅展示一次）
3. [P0] 环境枚举 DEV/TEST/UAT/PROD 固定校验
4. [P0] 节点配置只读索引（NodeConfigIndex）

### Epic 2：云存储配置与 COS 能力（P0）
1. [P0] StorageConfig：全局 + 客户覆盖
2. [P0] StorageConfig 选择策略（按 Node→Customer）
3. [P0] COS Client 封装：
   - 生成下载/上传签名 URL
   - Copy Object（全局桶 → 客户桶副本）
4. [P0] 包副本表 PackageReplica（package_id, customer_id, status）

### Epic 3：包管理与发布单（P0）
1. [P0] 包上传：版本唯一校验 + sha256 + manifest 解析
2. [P0] 发布单：范围（客户/分店/节点+env）+ 策略（Force/Optional+deadline）
3. [P0] 发布时展开节点集合并生成 UpdateTask（Pending）
4. [P0] 发布时确保客户桶副本（同步复制，失败降级懒复制）

### Epic 4：节点端 API（P0）
1. [P0] Agent/Tray bind/login（静态 token → access token）
2. [P0] updates/latest（返回 downloadUrl/manifest/deadline）
3. [P0] updates/report（幂等 + 状态机跃迁）
4. [P0] files/upload-url + files/commit（CONFIG/LOG/DIAG）

### Epic 5：管理端页面（Admin.NET 前端）
1. [P0] 客户树管理页面（客户/分店/节点）
2. [P0] 云存储配置页面（全局 + 客户覆盖）
3. [P0] 包管理页面（上传 + 列表 manifest）
4. [P0] 发布单页面（创建 + 发布）
5. [P0] 看板页面（统计 + 节点列表 + 文件链接）
6. [P0] 节点配置只读页面（摘要 + 下载配置）

### Epic 6：Windows 客户端（P0）
1. [P0] Agent：状态机 + 断点恢复（agent-state.json）
2. [P0] Agent：固定目录默认值 + Tray 覆盖（读取 node-config.json）
3. [P0] Agent：停服/备份三件套/覆盖/SQL/启服
4. [P0] Agent：失败诊断文件生成 + COS 上传
5. [P0] Tray（WinForms）：绑定向导 + 配置页 + 上报配置
6. [P0] Tray：可选更新交互（立即/延后）+ 手动更新入口
7. [P0] IPC：Named Pipe（Tray ↔ Agent）

### Epic 7：测试与验收（P0）
1. [P0] 服务端单测：版本唯一、范围展开、存储选择、签名 URL
2. [P0] 服务端集成测：SQL Server 容器 + API 跑通
3. [P0] 客户端集成测：备份/覆盖/回滚（用临时目录模拟）
4. [P0] E2E：发布单→节点拉取→report→看板可见（可用模拟 agent）

---

## 4. 自动化交付流水线（GitHub Actions 默认）

### 4.1 Secrets 与变量清单（必须先建）

#### 4.1.1 通用
- `DOTNET_VERSION`: `8.0.x`
- `NODE_VERSION`: `20.x`

#### 4.1.2 COS（全局，流水线用于上传构建产物/诊断测试用）
- `COS_SECRET_ID`
- `COS_SECRET_KEY`
- `COS_REGION`
- `COS_BUCKET`
- `COS_PREFIX`（可选）

#### 4.1.3 容器发布（可选）
- `GHCR_TOKEN`（或使用 `GITHUB_TOKEN`）

#### 4.1.4 部署（可选）
- `DEPLOY_SSH_HOST_DEV` / `DEPLOY_SSH_HOST_PROD`
- `DEPLOY_SSH_USER_*`
- `DEPLOY_SSH_KEY_*`
- `DEPLOY_PATH_*`

#### 4.1.5 代码签名（可选）
- `SIGN_PFX_BASE64`
- `SIGN_PFX_PASSWORD`

---

## 5. Workflows（可直接落地的骨架）

> 将以下文件创建在 `.github/workflows/`。

### 5.1 PR CI：ci.yml（后端+前端+客户端 全量构建测试）
```yaml
name: CI
on:
  pull_request:
  push:
    branches: [ main, develop ]

jobs:
  backend:
    runs-on: ubuntu-latest
    defaults:
      run:
        working-directory: backend
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 8.0.x
      - name: Restore
        run: dotnet restore
      - name: Build
        run: dotnet build -c Release --no-restore
      - name: Test
        run: dotnet test -c Release --no-build --collect:"XPlat Code Coverage"
      - name: Upload Coverage
        uses: actions/upload-artifact@v4
        with:
          name: backend-coverage
          path: backend/**/TestResults/**

  frontend:
    runs-on: ubuntu-latest
    defaults:
      run:
        working-directory: frontend/web
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with:
          node-version: 20
          cache: 'pnpm'
      - name: Enable corepack
        run: corepack enable
      - name: Install
        run: pnpm i --frozen-lockfile
      - name: Lint
        run: pnpm lint
      - name: Build
        run: pnpm build
      - uses: actions/upload-artifact@v4
        with:
          name: admin-web-dist
          path: frontend/web/dist

  client:
    runs-on: windows-latest
    defaults:
      run:
        working-directory: client
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 8.0.x
      - name: Restore
        run: dotnet restore
      - name: Build
        run: dotnet build -c Release --no-restore
      - name: Test
        run: dotnet test -c Release --no-build
```

### 5.2 Release：release.yml（打 tag 自动出发布包 + GitHub Release）
```yaml
name: Release
on:
  push:
    tags:
      - 'v*.*.*'

jobs:
  build-backend:
    runs-on: ubuntu-latest
    defaults:
      run:
        working-directory: backend
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 8.0.x
      - name: Publish backend
        run: |
          dotnet publish Admin.NET.Web.Entry/Admin.NET.Web.Entry.csproj -c Release -o ../_publish/backend
      - name: Zip backend
        run: |
          cd ../_publish && zip -r update-server_${{ github.ref_name }}_${{ github.sha }}.zip backend
      - uses: actions/upload-artifact@v4
        with:
          name: backend-release
          path: backend/_publish/*.zip

  build-frontend:
    runs-on: ubuntu-latest
    defaults:
      run:
        working-directory: frontend/web
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with:
          node-version: 20
      - run: corepack enable
      - run: pnpm i --frozen-lockfile
      - run: pnpm build
      - name: Zip frontend
        run: |
          cd dist && zip -r ../../admin-web_${{ github.ref_name }}_${{ github.sha }}.zip .
      - uses: actions/upload-artifact@v4
        with:
          name: frontend-release
          path: frontend/admin-web_*.zip

  build-client:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 8.0.x
      - name: Publish agent
        run: |
          dotnet publish client/Rca7.UpdateAgent.Service/Rca7.UpdateAgent.Service.csproj -c Release -o client/_publish/agent
      - name: Publish tray
        run: |
          dotnet publish client/Rca7.UpdateTray.WinForms/Rca7.UpdateTray.WinForms.csproj -c Release -o client/_publish/tray
      - name: Package client
        shell: pwsh
        run: |
          Compress-Archive -Path client/_publish/agent/* -DestinationPath update-agent_${{ github.ref_name }}_win-x64.zip -Force
          Compress-Archive -Path client/_publish/tray/*  -DestinationPath update-tray_${{ github.ref_name }}_win-x64.zip -Force
      - uses: actions/upload-artifact@v4
        with:
          name: client-release
          path: |
            update-agent_*.zip
            update-tray_*.zip

  github-release:
    runs-on: ubuntu-latest
    needs: [build-backend, build-frontend, build-client]
    steps:
      - uses: actions/download-artifact@v4
        with:
          path: ./artifacts
      - name: Create GitHub Release
        uses: softprops/action-gh-release@v2
        with:
          files: |
            artifacts/**/**/*.zip
```

### 5.3 Deploy（可选）：deploy-dev.yml / deploy-prod.yml
> 默认给出 **Docker 部署**方案（最自动化）。如你们必须 IIS 部署，可把“zip 后端”产物通过 SSH 拷贝到 IIS 目录并执行 app offline/重启脚本。

#### 5.3.1 Dockerfile（docker/Dockerfile.backend）
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY backend/ ./backend/
RUN dotnet publish backend/Admin.NET.Web.Entry/Admin.NET.Web.Entry.csproj -c Release -o /out

FROM base AS final
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "Admin.NET.Web.Entry.dll"]
```

#### 5.3.2 deploy-dev.yml（示例：推镜像到 GHCR + SSH 到服务器 docker compose up）
```yaml
name: Deploy-Dev
on:
  push:
    branches: [ develop ]

jobs:
  docker:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Login GHCR
        run: echo ${{ secrets.GITHUB_TOKEN }} | docker login ghcr.io -u ${{ github.actor }} --password-stdin
      - name: Build & Push
        run: |
          docker build -f docker/Dockerfile.backend -t ghcr.io/${{ github.repository }}/update-server:dev-${{ github.sha }} .
          docker push ghcr.io/${{ github.repository }}/update-server:dev-${{ github.sha }}
      - name: Deploy via SSH
        uses: appleboy/ssh-action@v1.0.3
        with:
          host: ${{ secrets.DEPLOY_SSH_HOST_DEV }}
          username: ${{ secrets.DEPLOY_SSH_USER_DEV }}
          key: ${{ secrets.DEPLOY_SSH_KEY_DEV }}
          script: |
            cd ${{ secrets.DEPLOY_PATH_DEV }}
            docker pull ghcr.io/${{ github.repository }}/update-server:dev-${{ github.sha }}
            sed -i "s|IMAGE_TAG=.*|IMAGE_TAG=dev-${{ github.sha }}|" .env
            docker compose up -d
```

---

## 6. 自动化“可交给 Codex/Copilot”的交付清单（执行顺序）

> 下面是建议交给自动化编码助手的逐步任务，每步都可独立 PR。

### Step 1：仓库初始化与 CI（必须先做）
- [ ] 搭建 monorepo 结构，落 `backend/ frontend/ client/ docker/ scripts/`
- [ ] 迁入/引用 Admin.NET（fork 或 submodule 或 vendor）并能启动
- [ ] 新增 `.github/workflows/ci.yml` 并确保 CI 绿

### Step 2：数据库与实体
- [ ] 新增 `Rca7.Update.Application/Entities`：rca_* 实体（SqlSugar）
- [ ] 新增初始化脚本或 CodeFirst（推荐：迁移工具/初始化 API）
- [ ] 单测覆盖：version unique、存储选择策略

### Step 3：核心 API（Agent/Tray + Admin）
- [ ] 实现 Admin CRUD：客户/分店/节点、token 生成
- [ ] 实现 StorageConfig API：global/customer override
- [ ] 实现 Package 上传：sha256 + manifest
- [ ] 实现 Release 创建/发布：范围展开 + UpdateTask
- [ ] 实现 Agent API：login/latest/report/upload-url/commit

### Step 4：前端页面
- [ ] 在 Admin.NET 前端新增 RCA7 菜单与页面
- [ ] 完成包上传、发布单、看板、配置只读

### Step 5：客户端实现与 client-release 流水线
- [ ] Agent：状态机 + 备份三件套 + 覆盖/SQL/回滚
- [ ] Tray：配置 + 上报 + 可选交互 + IPC
- [ ] 添加 `.github/workflows/client-release.yml`（或整合 release.yml）

### Step 6：部署与运维
- [ ] 容器化（Dockerfile + compose）
- [ ] deploy-dev/prod workflows（按实际环境接入）
- [ ] runbook：回滚/重启/日志定位

---

## 7. 脚本与本地开发一键命令（建议）

### 7.1 scripts/build.ps1（示例骨架）
```powershell
param([string]$Configuration = "Release")

Write-Host "Build backend..."
pushd backend
  dotnet restore
  dotnet build -c $Configuration
popd

Write-Host "Build frontend..."
pushd frontend/web
  corepack enable
  pnpm i --frozen-lockfile
  pnpm build
popd

Write-Host "Build client..."
pushd client
  dotnet restore
  dotnet build -c $Configuration
popd
```

### 7.2 scripts/package-client.ps1（示例骨架）
```powershell
param([string]$Tag = "dev")

$agentOut = "client/_publish/agent"
$trayOut  = "client/_publish/tray"

mkdir -Force $agentOut | Out-Null
mkdir -Force $trayOut  | Out-Null

dotnet publish client/Rca7.UpdateAgent.Service/Rca7.UpdateAgent.Service.csproj -c Release -o $agentOut

dotnet publish client/Rca7.UpdateTray.WinForms/Rca7.UpdateTray.WinForms.csproj -c Release -o $trayOut

Compress-Archive -Path "$agentOut/*" -DestinationPath "update-agent_$Tag_win-x64.zip" -Force
Compress-Archive -Path "$trayOut/*"  -DestinationPath "update-tray_$Tag_win-x64.zip" -Force
```

---

## 8. 交付约束与质量门禁（建议写入 PR 模板）
- CI 必须通过：backend build/test、frontend lint/build、client build/test
- 新增 API 必须：
  - Swagger 可见
  - 权限校验明确（Admin 与 Agent 分区）
  - 错误码统一
- 关键业务（发布、latest、report、upload-url/commit）必须有单测/集成测
- 生产部署必须支持回滚：
  - Docker：上一镜像 tag 回退
  - IIS：保留上一发布包并可一键还原

---

## 9. 给 Codex/Copilot 的执行提示（建议直接复制）

- 不要修改 Admin.NET 框架核心实现，业务全部放在 `Rca7.Update.Application`。
- 所有 API 都必须有 DTO，禁止直接暴露 Entity。
- 所有 COS 操作必须通过统一 `CosClient` 封装，并实现：签名 URL（GET/PUT）+ CopyObject。
- 包副本策略必须实现 `rca_package_replica`，并且在发布时尽量同步复制；拉取时如果未就绪返回明确状态/错误码。
- 节点配置文件只索引不编辑：服务端只保存 `cosKey + summary`。
- 任务状态机写入必须幂等（unique releaseId+nodeId），report 接口支持 Idempotency-Key。

---

## 10. 附：最小可运行的环境（建议用于 dev）
- 使用 Docker Compose 启动：update-server + sqlserver（用于本地开发/CI 集成测试）
- 生产对接现有 SQL Server，COS 走真实凭证

> 到此，本计划与流水线骨架即可直接交给 Codex/Copilot 开始逐步实现（按 Step 1→6）。

