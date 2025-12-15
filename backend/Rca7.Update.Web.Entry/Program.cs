using System.IO;
using Rca7.Update.Application.Repositories;
using Rca7.Update.Application.Services;
using Rca7.Update.Web.Entry.Infrastructure;
using SqlSugar;

// 创建 Web 应用程序构建器
var builder = WebApplication.CreateBuilder(args);

// 添加控制器服务
builder.Services.AddControllers();

// 获取数据库连接字符串，优先级：Default > DATABASE_CONNECTION > ConnectionStrings__Default
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? builder.Configuration["DATABASE_CONNECTION"]
    ?? builder.Configuration["ConnectionStrings__Default"];

// 如果未配置连接字符串，使用本地 SQLite 文件
if (string.IsNullOrWhiteSpace(connectionString))
{
    var dbPath = Path.Combine(AppContext.BaseDirectory, "customer-update.db");
    connectionString = $"Data Source={dbPath}";
}

// 注册 SqlSugar ORM 客户端
builder.Services.AddSingleton<ISqlSugarClient>(_ => new SqlSugarClient(new ConnectionConfig
{
    ConnectionString = connectionString,
    DbType = DbType.Sqlite,
    InitKeyType = InitKeyType.Attribute,
    IsAutoCloseConnection = true
}));

// 注册仓储和服务
builder.Services.AddScoped<ICustomerTreeRepository, DatabaseCustomerTreeRepository>();
builder.Services.AddScoped<CustomerTreeService>();

var app = builder.Build();

// 应用数据库迁移
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSetup");
    var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
    var migrationsPath = Path.Combine(AppContext.BaseDirectory, "migrations");
    DatabaseInitializer.ApplyMigrations(db, logger, migrationsPath);
}

// 映射控制器路由
app.MapControllers();

// 启动应用程序
app.Run();
