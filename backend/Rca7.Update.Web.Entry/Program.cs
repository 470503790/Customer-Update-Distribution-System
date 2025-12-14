using System.IO;
using Rca7.Update.Application.Repositories;
using Rca7.Update.Application.Services;
using Rca7.Update.Web.Entry.Infrastructure;
using SqlSugar;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? builder.Configuration["DATABASE_CONNECTION"]
    ?? builder.Configuration["ConnectionStrings__Default"];

if (string.IsNullOrWhiteSpace(connectionString))
{
    var dbPath = Path.Combine(AppContext.BaseDirectory, "customer-update.db");
    connectionString = $"Data Source={dbPath}";
}

builder.Services.AddSingleton<ISqlSugarClient>(_ => new SqlSugarClient(new ConnectionConfig
{
    ConnectionString = connectionString,
    DbType = DbType.Sqlite,
    InitKeyType = InitKeyType.Attribute,
    IsAutoCloseConnection = true
}));

builder.Services.AddScoped<ICustomerTreeRepository, DatabaseCustomerTreeRepository>();
builder.Services.AddScoped<CustomerTreeService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSetup");
    var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
    var migrationsPath = Path.Combine(AppContext.BaseDirectory, "migrations");
    DatabaseInitializer.ApplyMigrations(db, logger, migrationsPath);
}

app.MapControllers();

app.Run();
