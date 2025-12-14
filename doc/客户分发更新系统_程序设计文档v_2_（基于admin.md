# 客户分发更新系统（RCA7 更新分发）｜程序设计文档 V2（基于 Admin.NET）

> 范围：**概要设计 + 关键详细设计骨架**（在 Admin.NET 基础框架上实现接口与界面）

---

## 1. 文档信息
- 需求依据：需求文档 V2.0（固定目录 + 备份三件套 + 原路回滚）
- 基础框架：Admin.NET（后端 Furion + SqlSugar；前端 Vue3 + Element-Plus + Vite）
- DB：SQL Server
- OSS：腾讯云 COS（签名 URL）

---

## 2. 总体架构（概要设计）

### 2.1 组件视图
1) **管理端（Admin.NET 自带前端）**
- 在 Admin.NET 前端工程中新增菜单与页面：客户树/节点、云存储配置、包管理、发布单、看板、节点配置只读、审计。

2) **服务端（Admin.NET 后端）**
- 复用 Admin.NET：
  - 登录/用户/角色/菜单/操作日志/数据字典等通用模块
  - 统一异常、审计、鉴权、Swagger、动态 API 机制
- 新增业务模块：**RCA7 更新分发模块**（本系统核心）

3) **客户端**
- Windows Agent（Service）：执行更新闭环
- WinForms Tray：节点配置与可选更新交互

4) **对象存储（COS）**
- packages：版本包
- node-configs：节点配置文件
- tasks-files：任务日志/诊断文件

5) **平台数据库（SQL Server）**
- Admin.NET 系统表（用户/权限/日志）
- 本系统业务表（Customer/Store/Node/Package/Release/Task/ConfigIndex/Replica 等）

### 2.2 关键设计原则
- **在 Admin.NET 框架内扩展，不改主框架核心**：业务独立模块/应用层工程引用 Admin.NET.Core（便于跟随主仓库升级）。
- **服务端不编辑节点配置**：节点配置由 Tray 本地维护并上云，服务端只索引与展示。
- **执行前强制备份三件套**：不论版本包内容，先备份再覆盖/SQL。
- **失败原路回滚**：覆盖失败/SQL 失败/启服失败 → 自动回滚文件+DB。
- **大文件上 COS，小摘要入库**：日志/诊断不入 DB。
- **客户 COS 覆盖全局**：客户配置 COS 后，该客户节点的下载/上传均优先使用客户 COS。

---

## 3. 代码工程结构（建议落地方式）

### 3.1 方案 A（推荐）：独立业务应用层工程
> 按 Admin.NET 建议：每个应用系统单独创建应用层工程，Web 层引用该应用层即可。

- `Admin.NET.Web.Entry`（启动入口，不建议改动，仅引用业务应用层）
- `Admin.NET.Core`（框架核心）
- `Admin.NET.Application`（框架示例应用层，可尽量不改）
- **新增：`Rca7.Update.Application`（本系统应用层）**
  - `Services/`（AppService：动态 API 或 Controller）
  - `Entities/`（SqlSugar 实体）
  - `Dtos/`
  - `Managers/`（领域/编排：发布、匹配、签名 URL、复制副本）
  - `Jobs/`（可选：异步复制副本、清理任务）
  - `Constants/`（错误码、COS key 规范）

### 3.2 前端工程（Admin.NET 自带 Web）
- 在 `Web/`（Admin.NET 前端）内新增：
  - `views/rca7/`：客户树、节点、包、发布单、看板等
  - `api/rca7/`：对接后端接口
  - `router/`：新增路由
  - `stores/`：可选
  - `menu`：通过 Admin.NET 菜单管理配置生成

---

## 4. 服务端设计（基于 Admin.NET）

### 4.1 分层与职责（落在业务应用层）
- **AppService/Controller 层**：
  - 对外暴露 Admin API（管理端）与 Agent API（节点端）
  - 参数校验、鉴权、返回 DTO
- **Manager/Domain 层（业务编排）**：
  - 包上传解析、版本唯一
  - 发布单范围展开（客户/分店/节点 + env）
  - UpdateTask 状态机写入
  - COS 签名 URL、对象复制（全局桶→客户桶）
- **Repository/ORM（SqlSugar）**：
  - CRUD + 关键唯一约束
- **框架复用**：
  - RBAC/菜单/操作日志
  - 统一异常与日志
  - 配置管理、缓存（如需）

### 4.2 鉴权体系
- **管理端用户**：沿用 Admin.NET 登录与 RBAC。
- **节点端（Agent/Tray）**：
  - 静态长期 NodeToken（服务端存 hash+salt）
  - 建议登录换取短期 AccessToken（JWT 或框架内 token），减少长期 token 频繁传输
  - Agent API 与 Admin API 分组：
    - Admin API：需登录并校验权限
    - Agent API：仅需节点 token/AccessToken，不走菜单权限

### 4.3 云存储选择策略（关键）
- StorageConfig：
  - Global（唯一启用）
  - Customer Override（CustomerId 唯一；启用覆盖 Global）
- 节点调用时：根据 NodeId→CustomerId 选择 StorageConfig。

### 4.4 版本包在客户桶的“副本策略”（满足客户桶覆盖）
- 默认：版本包上传到 **全局桶**：`{prefix}/packages/{version}/{version}.zip`
- 若客户启用客户桶：该客户所有节点下载必须走客户桶。
- 实现：在发布时或首次节点拉取时，执行 COS Copy 将全局桶对象复制到客户桶对应 key。

**两种实现策略**
- 策略 1（同步复制，推荐）：发布时展开目标客户集合，逐客户复制，复制完成后才允许节点拉取到 downloadUrl。
- 策略 2（懒复制）：首次节点拉取时检测副本不存在，触发复制并返回“准备中”，下一次拉取拿到 downloadUrl。

V2 默认采用：**同步复制 + 失败降级为懒复制**（保证发布可用性）。

---

## 5. 数据库与实体设计（SqlSugar）

### 5.1 业务表清单
> Admin.NET 系统表沿用框架；以下为业务表。

1) `rca_customer`：客户
2) `rca_store`：分店
3) `rca_node`：节点（含 env、token hash）
4) `rca_storage_config`：云存储配置（global/customer override）
5) `rca_package`：更新包
6) `rca_release`：发布单
7) `rca_release_target`：发布范围（客户/分店/节点 + env）
8) `rca_update_task`：节点执行任务（状态机）
9) `rca_node_config_index`：节点配置索引（cosKey+摘要）
10) `rca_package_replica`：包副本映射（PackageId + CustomerId + CosKey + Status）

### 5.2 关键字段与唯一约束（必须落库）
- `rca_customer.code` 唯一
- `rca_store`：unique(customer_id, code)
- `rca_node`：unique(customer_id, env_code, node_code)
- `rca_package.version` 唯一
- `rca_release_target`：unique(release_id, target_type, target_id, env_code)
- `rca_update_task`：unique(release_id, node_id)
- `rca_node_config_index.node_id` PK
- `rca_storage_config`：
  - global 仅允许 1 条 enabled=true
  - customer override：unique(customer_id)
- `rca_package_replica`：unique(package_id, customer_id)

### 5.3 SqlSugar 实体骨架（示例）
> 仅提供骨架，字段与约束在实现时用 SqlSugar 特性或 Fluent 配置。

- `RcaCustomerEntity`
- `RcaStoreEntity`
- `RcaNodeEntity`
- `RcaStorageConfigEntity`
- `RcaPackageEntity`
- `RcaReleaseEntity`
- `RcaReleaseTargetEntity`
- `RcaUpdateTaskEntity`
- `RcaNodeConfigIndexEntity`
- `RcaPackageReplicaEntity`

---

## 6. 接口设计（骨架）

### 6.1 Agent/Tray API（节点端）

#### 6.1.1 绑定/登录
- `POST /api/agent/auth/bind`
  - 入参：customerCode, storeCode, envCode, nodeCode, token
  - 出参：nodeId

- `POST /api/agent/auth/login`
  - 入参：nodeCode, envCode, token
  - 出参：accessToken, expiresAt

#### 6.1.2 拉取更新
- `GET /api/agent/updates/latest`
  - Header：Authorization: Bearer {accessToken}
  - 出参：
    - releaseId, version
    - strategy（Force/Optional）
    - optionalDeadlineAt
    - downloadUrl（签名 URL）
    - zipSha256, fileSize
    - manifest：hasServerZip/hasClientZip/hasSchemaSql/hasDataSql
    - serverTime

#### 6.1.3 状态上报
- `POST /api/agent/updates/report`
  - Header：Idempotency-Key（建议）
  - 入参：releaseId, status, progress, errorCode?, errorMessage?, logsSnippet?, backupLocalPath?
  - 行为：upsert UpdateTask

#### 6.1.4 文件上传（配置/日志/诊断）
- `POST /api/agent/files/upload-url`
  - 入参：fileType(CONFIG/LOG/DIAG), fileName, contentType, size, sha256, releaseId?
  - 出参：cosKey, uploadUrl, expireAt

- `POST /api/agent/files/commit`
  - 入参：fileType, cosKey, sha256, size, meta
  - 行为：
    - CONFIG：写 NodeConfigIndex（索引+摘要）
    - LOG/DIAG：追加到 UpdateTask.reportFilesJson

### 6.2 Admin API（管理端）

#### 6.2.1 客户树
- `GET/POST/PUT/DELETE /api/admin/rca/customers`
- `GET/POST/PUT/DELETE /api/admin/rca/stores`
- `GET/POST/PUT/DELETE /api/admin/rca/nodes`
- `POST /api/admin/rca/nodes/{id}/token`（生成 token，仅展示一次）

#### 6.2.2 云存储配置
- `POST /api/admin/rca/storage/global`
- `POST /api/admin/rca/storage/customers/{customerId}`
- `GET /api/admin/rca/storage/effective?nodeId=...`（排障用，可选）

#### 6.2.3 包管理
- `POST /api/admin/rca/packages`（上传 zip + version + packageType）
- `GET /api/admin/rca/packages`（列表/详情）

校验：server.zip/client.zip/结构.sql/data.sql 至少一个存在，否则 E_PACKAGE_INVALID。

#### 6.2.4 发布单
- `POST /api/admin/rca/releases`（创建）
- `POST /api/admin/rca/releases/{id}/publish`（发布）
- `POST /api/admin/rca/releases/{id}/pause|revoke`（P1）

#### 6.2.5 看板与节点配置只读
- `GET /api/admin/rca/releases/{id}/dashboard`
- `GET /api/admin/rca/nodes/{id}/config`（NodeConfigIndex + 文件下载签名 URL）

---

## 7. 关键业务编排（详细设计骨架）

### 7.1 包上传与解析（Admin.NET AppService）
**输入**：version、packageType、zip(file)

**步骤**
1) 校验 version 唯一（DB unique + 业务层提前查）
2) 计算 sha256、fileSize
3) 解压/读取 zip 目录（不落地或落临时）
4) 识别 manifest：server.zip/client.zip/结构.sql/data.sql
5) 写 COS（全局桶）
6) 入库 rca_package

**失败回滚**
- COS 上传失败：不入库
- 入库失败：可删除 COS 对象（最佳实践）

### 7.2 发布单范围展开与任务生成
**输入**：releaseId、targets（客户/分店/节点 + env）、策略与截止时间

**步骤**
1) 校验策略：Optional 必须有 deadline
2) 解析范围 → 展开节点集合（customer/store/node + env 过滤）
3) 对每节点 upsert `rca_update_task`（Pending）
4) 计算目标客户集合（从节点集合 distinct customerId）
5) 对每目标客户：若有客户桶，则确保 package 副本存在：
   - 查 `rca_package_replica`
   - 无则 COS Copy 全局对象 → 客户桶 key
   - 写 `rca_package_replica`（status=Ready/Pending/Failed）

### 7.3 节点拉取更新（最新可执行 release 选择）
**规则**
- 只返回该节点范围内、已发布、且该节点对应 task 处于 Pending/Failed（可重试）/WaitingUser 的最新 release
- 已 Succeeded/Rollbacked 不再下发

**downloadUrl 生成**
- 计算节点有效 StorageConfig（客户桶优先）
- 若客户桶：
  - 查 replica=Ready
  - replica 不存在或 Pending：
    - 若采用懒复制：触发复制并返回 “replicaPreparing=true” + 下发空 downloadUrl（或 202）
    - 否则（同步复制策略）：理论上不会发生；发生则返回错误码 E_REPLICA_NOT_READY
- 生成签名 URL（GET）并返回

### 7.4 文件上传两段式（upload-url + commit）
- upload-url：生成 PUT 签名 URL + cosKey
- commit：
  - CONFIG：更新 `rca_node_config_index`
  - LOG/DIAG：追加 `rca_update_task.report_files_json`

### 7.5 状态机写入与幂等
- `rca_update_task` 唯一键（release_id,node_id），report 接口按状态允许跃迁：
  - 允许从任意非终态 → 更高阶段
  - 终态（Succeeded/Rollbacked/Failed）再次上报仅更新日志索引，不改变终态（除非带 forceReset，P1）
- report 支持 Idempotency-Key：重复提交不重复追加文件记录

---

## 8. 管理端页面设计（基于 Admin.NET 前端）

### 8.1 菜单与路由（建议）
- RCA7 更新分发
  - 客户树管理（客户/分店/节点）
  - 节点配置（只读）
  - 云存储配置（全局/客户）
  - 更新包管理
  - 发布单管理
  - 发布看板
  - 审计/操作日志（复用 Admin.NET）

### 8.2 页面骨架
1) 客户树管理
- 左侧树（客户→分店→节点）
- 右侧详情 CRUD
- 节点：生成 Token（弹窗仅展示一次）

2) 云存储配置
- 全局配置表单
- 客户配置列表 + 编辑（启用覆盖标识）

3) 包管理
- 上传（版本号、类型、文件）
- 列表：version、manifest、sha256、创建人

4) 发布单
- 新建：选择包、范围（树选择+环境）、策略与 deadline
- 发布后只读

5) 看板
- 概览统计（Pending/Running/Success/Fail/Rollback）
- 节点列表：阶段、耗时、错误码、backupLocalPath、文件链接

6) 节点配置只读
- 展示 NodeConfigIndex 摘要
- 下载配置文件（签名 URL）

---

## 9. Agent/Tray 对接点（与 Admin.NET API 兼容）

### 9.1 Agent 与 Tray 的 IPC（不变）
- Named Pipe：`\\.\\pipe\\Rca7UpdateIpc`
- Tray 决策可选更新：立即/延后

### 9.2 与服务端交互
- 管理端与节点端均通过 Admin.NET 后端服务提供 API
- COS 上传/下载使用服务端签名 URL

---

## 10. 部署与配置（基于 Admin.NET）

### 10.1 服务端
- 使用 Admin.NET 标准启动方式（Entry 项目）
- 配置项：
  - SQL Server 连接（按 Admin.NET 的数据库配置方式）
  - 全局 COS 配置（rca_storage_config global）
  - JWT/鉴权配置（沿用 Admin.NET）
  - CORS（为 Agent/Tray）

### 10.2 前端
- 使用 Admin.NET 前端构建与部署流程（pnpm build 等）
- 通过菜单管理发布新增页面入口

---

## 11. 测试设计（骨架）

### 11.1 服务端
- 包上传解析与版本唯一
- 发布单范围展开正确性（customer/store/node + env）
- 存储策略选择（global vs customer override）
- 包副本复制（replica Ready/Pending）
- Agent 拉取与 downloadUrl 生成

### 11.2 端到端
- 发布单 → 节点执行 → 看板可追踪 → 日志文件可下载

---

## 12. 未决项（进入详细设计/开发前必须固化）
1) 包副本复制：同步复制的超时与并发控制（每客户串行/并行阈值）
2) 多数据库备份：需求是否只允许单库（data.bak），还是允许多库（data_{db}.bak）
3) 节点配置文件加密方案：DPAPI/对称加密/国密（与 Admin.NET 能力对齐）
4) Agent 拉取频率与抖动策略（避免同时请求洪峰）

---

## 附录：错误码与状态机（对齐需求）
- 状态机：Pending/Downloading/Verifying/WaitingUser/StoppingService/BackingUp/InstallingServer/InstallingClient/RunningSql/StartingService/Succeeded/Failed/Rollbacking/Rollbacked
- 错误码：E_VERSION_DUPLICATE、E_PACKAGE_INVALID、E_HASH_MISMATCH、E_SERVICE_STOP_TIMEOUT、E_BACKUP_FAILED_*、E_SQL_FAILED_*、E_SERVICE_START_FAILED、E_ROLLBACK_FAILED、E_REPLICA_NOT_READY（新增）

